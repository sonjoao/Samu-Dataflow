using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SAMUDataFlow.Domain.Comum;
using SAMUDataFlow.Infrastructure.Persistencia;

namespace SAMUDataFlow.Web.Endpoints;

public static class RecursosSamuEndpoints
{
    public static IEndpointRouteBuilder MapRecursosSamu(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/tema", SalvarTemaAsync).RequireAuthorization();
        endpoints.MapGet("/relatorios/exportar", ExportarRelatorioAsync).RequireAuthorization();
        return endpoints;
    }

    private static async Task<IResult> SalvarTemaAsync(HttpContext http, IAntiforgery antiforgery)
    {
        try { await antiforgery.ValidateRequestAsync(http); }
        catch (AntiforgeryValidationException) { return Results.BadRequest("Formulário expirado. Atualize a página e tente novamente."); }

        var form = await http.Request.ReadFormAsync();
        var tema = http.Request.Cookies["SamuDataFlow.Tema"] == "claro" ? "escuro" : "claro";

        http.Response.Cookies.Append("SamuDataFlow.Tema", tema, new CookieOptions
        {
            Path = "/",
            MaxAge = TimeSpan.FromDays(365),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            Secure = http.Request.IsHttps
        });

        var retorno = form["returnUrl"].ToString();
        var destino = retorno.StartsWith("/", StringComparison.Ordinal)
            && !retorno.StartsWith("//", StringComparison.Ordinal)
            && !retorno.Contains('\\') ? retorno : "/";
        return Results.LocalRedirect(destino);
    }

    private static async Task<IResult> ExportarRelatorioAsync(
        DateOnly? de, DateOnly? ate, IDbContextFactory<SamuDbContext> fabrica, CancellationToken cancellationToken)
    {
        if (de is null || ate is null || de > ate)
            return Results.BadRequest("Informe um intervalo de datas válido.");

        var inicioUtc = DateTime.SpecifyKind(de.Value.ToDateTime(TimeOnly.MinValue).AddHours(3), DateTimeKind.Utc);
        var fimUtc = DateTime.SpecifyKind(ate.Value.AddDays(1).ToDateTime(TimeOnly.MinValue).AddHours(3), DateTimeKind.Utc);
        await using var db = await fabrica.CreateDbContextAsync(cancellationToken);
        var ocorrencias = await db.Ocorrencias.AsNoTracking().Where(x => !x.EstaArquivada)
            .Where(x => x.DataChamado >= inicioUtc && x.DataChamado < fimUtc)
            .OrderBy(x => x.DataChamado).ToListAsync(cancellationToken);

        var csv = new StringBuilder();
        csv.AppendLine("Código;Data e hora;Distrito;Bairro;Tipo de agravo;Gravidade;Status;Tempo de resposta (min);Desfecho;Hospital de destino");
        foreach (var x in ocorrencias)
        {
            var campos = new[]
            {
                Csv(x.CodigoExterno),
                Csv(x.DataChamado.AddHours(-3).ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"))),
                Csv(x.Distrito), Csv(x.Bairro), Csv(x.TipoAgravo.Rotulo()), Csv(x.Gravidade.Rotulo()),
                Csv(x.Status.Rotulo()), Csv(x.TempoRespostaMinutos?.ToString("N1", CultureInfo.GetCultureInfo("pt-BR"))),
                Csv(x.Desfecho.HasValue ? x.Desfecho.Value.Rotulo() : null), Csv(x.HospitalDestino)
            };
            csv.AppendLine(string.Join(';', campos));
        }

        var bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(csv.ToString());
        var nomeArquivo = $"relatorio-samu-{de.Value:yyyyMMdd}-{ate.Value:yyyyMMdd}.csv";
        return Results.File(bytes, "text/csv; charset=utf-8", nomeArquivo);
    }

    private static string Csv(string? valor) => $"\"{(valor ?? string.Empty).Replace("\"", "\"\"")}\"";
}
