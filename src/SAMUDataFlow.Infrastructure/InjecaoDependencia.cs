using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SAMUDataFlow.Infrastructure.Persistencia;

namespace SAMUDataFlow.Infrastructure;

public static class InjecaoDependencia
{
    public static IServiceCollection AddInfraestrutura(this IServiceCollection servicos, string stringConexao)
    {
        // Blazor Server: cada operação cria o seu próprio DbContext pela fábrica.
        servicos.AddDbContextFactory<SamuDbContext>(opcoes => opcoes.UseNpgsql(stringConexao));

        // Contexto com escopo, necessário para as ferramentas do EF (dotnet ef) e para endpoints da API.
        servicos.AddScoped(p => p.GetRequiredService<IDbContextFactory<SamuDbContext>>().CreateDbContext());

        return servicos;
    }

    /// <summary>Aplica as migrations sem criar registros de demonstração.</summary>
    public static async Task InicializarBancoAsync(this IServiceProvider provedor, CancellationToken ct = default)
    {
        await using var escopo = provedor.CreateAsyncScope();
        var fabrica = escopo.ServiceProvider.GetRequiredService<IDbContextFactory<SamuDbContext>>();

        await using var db = await fabrica.CreateDbContextAsync(ct);
        await db.Database.MigrateAsync(ct);
    }
}
