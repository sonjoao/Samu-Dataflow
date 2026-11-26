using SAMUDataFlow.Domain.Excecoes;

namespace SAMUDataFlow.Domain.Entidades;

/// <summary>Usuário do sistema.</summary>
public sealed class Usuario
{
    public const string AdministradorSomenteLeitura = "Administrador 1";
    public const string AdministradorCompleto = "Administrador 2";
    public int Id { get; private set; }
    public string Nome { get; private set; } = "";
    public string Setor { get; private set; } = "";
    public string? Login { get; private set; }
    public string? SenhaHash { get; private set; }
    public string Papel { get; private set; } = AdministradorSomenteLeitura;

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

    public static Usuario CriarConta(string nome, string setor, string login, string senhaHash, string papel)
    {
        var usuario = Criar(nome, setor);
        if (string.IsNullOrWhiteSpace(login) || login.Trim().Length > 100)
            throw new DominioExcecaoUsuario("O login deve ter de 1 a 100 caracteres.");
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new DominioExcecaoUsuario("A senha precisa ser definida.");
        if (!PapeisValidos.Contains(papel))
            throw new DominioExcecaoUsuario("Papel de usuário inválido.");
        usuario.Login = login.Trim();
        usuario.SenhaHash = senhaHash;
        usuario.Papel = papel;
        return usuario;
    }

    public static bool PapelValido(string papel) => PapeisValidos.Contains(papel);

    public void AlterarPapel(string papel)
    {
        if (!PapelValido(papel)) throw new DominioExcecaoUsuario("Papel de usuário inválido.");
        Papel = papel;
    }

    public void AtivarConta(string login, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(login) || login.Trim().Length > 100)
            throw new DominioExcecaoUsuario("O login deve ter de 1 a 100 caracteres.");
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new DominioExcecaoUsuario("A senha precisa ser definida.");
        Login = login.Trim();
        SenhaHash = senhaHash;
    }

    private static readonly HashSet<string> PapeisValidos =
        [AdministradorSomenteLeitura, AdministradorCompleto];

    private sealed class DominioExcecaoUsuario(string mensagem) : DominioException(mensagem);
}
