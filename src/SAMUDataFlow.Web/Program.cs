using SAMUDataFlow.Infrastructure;
using SAMUDataFlow.Web.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using SAMUDataFlow.Domain.Entidades;
using SAMUDataFlow.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Antiforgery;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/acesso-negado";
    options.Cookie.Name = "SamuDataFlow.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Events.OnValidatePrincipal = async context =>
    {
        var principal = context.Principal;
        var id = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var papel = principal?.FindFirst(ClaimTypes.Role)?.Value;
        if (!int.TryParse(id, out var usuarioId) || string.IsNullOrEmpty(papel))
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return;
        }

        var fabrica = context.HttpContext.RequestServices.GetRequiredService<IDbContextFactory<SamuDbContext>>();
        await using var db = await fabrica.CreateDbContextAsync();
        var papelAtual = await db.Usuarios.AsNoTracking().Where(x => x.Id == usuarioId).Select(x => x.Papel).SingleOrDefaultAsync();
        if (!string.Equals(papelAtual, papel, StringComparison.Ordinal))
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    };
});
builder.Services.AddAuthorization(options => options.FallbackPolicy = options.DefaultPolicy);
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// Senha do PostgreSQL vem de user-secrets (Banco:Senha); o restante, do appsettings.json.
builder.Services.AddInfraestrutura(ConexaoBanco.Montar(
    builder.Configuration.GetConnectionString("SamuDb"),
    builder.Configuration["Banco:Senha"]));

var app = builder.Build();

// Números, datas e moeda em pt-BR (ex.: 238.764 e 12,4%)
const string culturaPtBr = "pt-BR";
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture(culturaPtBr)
    .AddSupportedCultures(culturaPtBr)
    .AddSupportedUICultures(culturaPtBr));

if (app.Environment.IsDevelopment())
{
    // Aplica migrations. A base fica vazia até a importação dos dados reais.
    await app.Services.InicializarBancoAsync();
    app.Logger.LogInformation("Banco pronto. Nenhum dado de demonstração foi inserido.");
}

else
{
    app.UseHsts();
}

// Em produção, aplique as migrations antes de iniciar a aplicação. A conta é criada
// somente quando as variáveis de bootstrap estão definidas e nunca promove contas antigas.
await CriarContaInicialAsync(app.Services, builder.Configuration);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/login", (HttpContext http) =>
{
    if (http.User.Identity?.IsAuthenticated == true) return Results.Redirect("/");
    var token = http.RequestServices.GetRequiredService<IAntiforgery>().GetAndStoreTokens(http).RequestToken;
    var tokenSeguro = HtmlEncoder.Default.Encode(token ?? "");
    var mensagem = http.Request.Query.ContainsKey("erro") ? "<p role='alert'>Login ou senha inválidos.</p>" : "";
    var html = $"<!doctype html><html lang='pt-BR'><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'><title>Entrar — SAMU DataFlow</title><body style='font:16px system-ui;max-width:360px;margin:10vh auto;padding:24px'><h1>Entrar no SAMU DataFlow</h1>{mensagem}<form method='post' action='/login'><input type='hidden' name='__RequestVerificationToken' value='{tokenSeguro}'><label>Login<br><input name='login' autocomplete='username' required maxlength='100' style='width:100%;padding:10px;margin:8px 0 16px'></label><label>Senha<br><input type='password' name='senha' autocomplete='current-password' required style='width:100%;padding:10px;margin:8px 0 16px'></label><button style='padding:10px 18px'>Entrar</button></form></body></html>";
    return Results.Content(html, "text/html; charset=utf-8");
}).AllowAnonymous();
app.MapPost("/login", async (HttpContext http, IDbContextFactory<SamuDbContext> factory, IPasswordHasher<Usuario> hasher) =>
{
    try { await http.RequestServices.GetRequiredService<IAntiforgery>().ValidateRequestAsync(http); }
    catch (AntiforgeryValidationException) { return Results.BadRequest("Formulário expirado. Atualize a página e tente novamente."); }
    var form = await http.Request.ReadFormAsync();
    var login = form["login"].ToString().Trim();
    var senha = form["senha"].ToString();
    await using var db = await factory.CreateDbContextAsync();
    var usuario = await db.Usuarios.SingleOrDefaultAsync(x => x.Login == login);
    if (usuario is null || usuario.SenhaHash is null || hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha) == PasswordVerificationResult.Failed)
        return Results.Redirect("/login?erro=1");
    var claims = new[] { new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), new Claim(ClaimTypes.Name, usuario.Nome), new Claim("setor", usuario.Setor), new Claim(ClaimTypes.Role, usuario.Papel) };
    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)), new AuthenticationProperties { IsPersistent = false });
    return Results.Redirect("/");
}).AllowAnonymous();
app.MapPost("/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).RequireAuthorization().DisableAntiforgery();
app.MapGet("/acesso-negado", () => Results.Content("<h1>Acesso negado</h1><p>Seu perfil não permite esta operação.</p><a href='/'>Voltar</a>", "text/html; charset=utf-8")).RequireAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task CriarContaInicialAsync(IServiceProvider servicos, IConfiguration config)
{
    var login = config["SamuAdmin:Login"];
    var senha = config["SamuAdmin:Senha"];
    if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha)) return;
    if (senha.Length < 12)
        throw new InvalidOperationException("A senha de bootstrap SamuAdmin:Senha precisa ter pelo menos 12 caracteres.");

    await using var escopo = servicos.CreateAsyncScope();
    var fabrica = escopo.ServiceProvider.GetRequiredService<IDbContextFactory<SamuDbContext>>();
    var hasher = escopo.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
    await using var db = await fabrica.CreateDbContextAsync();
    if (await db.Usuarios.AnyAsync(x => x.Login == login.Trim())) return;

    var nome = config["SamuAdmin:Nome"] ?? "Administrador do sistema";
    var setor = config["SamuAdmin:Setor"] ?? "Administração";
    var usuario = Usuario.CriarConta(nome, setor, login, "bootstrap", Usuario.AdministradorCompleto);
    var hash = hasher.HashPassword(usuario, senha);
    usuario = Usuario.CriarConta(nome, setor, login, hash, Usuario.AdministradorCompleto);
    db.Usuarios.Add(usuario);
    await db.SaveChangesAsync();
}
