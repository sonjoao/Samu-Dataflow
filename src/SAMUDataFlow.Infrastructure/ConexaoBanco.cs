using Npgsql;

namespace SAMUDataFlow.Infrastructure;

public static class ConexaoBanco
{
    /// <summary>Junta a string de conexão do appsettings com a senha guardada em user-secrets.</summary>
    public static string Montar(string? stringBase, string? senha)
    {
        if (string.IsNullOrWhiteSpace(stringBase))
            throw new InvalidOperationException("Defina ConnectionStrings:SamuDb no appsettings.json.");

        var construtor = new NpgsqlConnectionStringBuilder(stringBase);

        if (!string.IsNullOrEmpty(senha))
            construtor.Password = senha;

        return construtor.ConnectionString;
    }
}
