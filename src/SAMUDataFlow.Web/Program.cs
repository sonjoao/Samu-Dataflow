using SAMUDataFlow.Infrastructure;
using SAMUDataFlow.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
