using SAMUDataFlow.Domain.Excecoes;

namespace SAMUDataFlow.Domain.Entidades;

/// <summary>Usuário do sistema.</summary>
public sealed class Usuario
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = "";
    public string Setor { get; private set; } = "";

    public string Iniciais => string.Concat(
        Nome.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(p => p.Length > 2)
            .Take(2)
            .Select(p => char.ToUpperInvariant(p[0])));

    private Usuario() { }

    public static Usuario Criar(string nome, string setor)
    {
        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(setor))
            throw new DominioExcecaoUsuario("Nome e setor do usuário são obrigatórios.");

        return new Usuario { Nome = nome.Trim(), Setor = setor.Trim() };
    }

    private sealed class DominioExcecaoUsuario(string mensagem) : DominioException(mensagem);
}
