namespace SAMUDataFlow.Domain.Entidades;

/// <summary>Preferências operacionais compartilhadas do painel.</summary>
public sealed class ConfiguracaoSistema
{
    public const int IdUnico = 1;
    public const int PeriodoHistoricoPadrao = 30;

    public int Id { get; private set; }
    public string NomeServico { get; private set; } = "SAMU DataFlow";
    public string Municipio { get; private set; } = "Recife";
    public string UnidadeFederativa { get; private set; } = "PE";
    public int DiasPadraoHistorico { get; private set; } = PeriodoHistoricoPadrao;
    public DateTime AtualizadoEmUtc { get; private set; }
    public string AtualizadoPor { get; private set; } = "Sistema";

    private ConfiguracaoSistema() { }

    public static ConfiguracaoSistema CriarPadrao() => new()
    {
        Id = IdUnico,
        AtualizadoEmUtc = DateTime.UtcNow
    };

    public void Atualizar(string? nomeServico, string? municipio, string? unidadeFederativa,
        int diasPadraoHistorico, string atualizadoPor)
    {
        var nome = nomeServico?.Trim() ?? "";
        var cidade = municipio?.Trim() ?? "";
        var uf = unidadeFederativa?.Trim().ToUpperInvariant() ?? "";
        if (nome.Length is 0 or > 100) throw new ArgumentException("O nome do serviço deve ter de 1 a 100 caracteres.");
        if (cidade.Length is 0 or > 100) throw new ArgumentException("O município deve ter de 1 a 100 caracteres.");
        if (uf.Length != 2 || uf.Any(c => !char.IsAsciiLetter(c))) throw new ArgumentException("Informe a sigla da UF com duas letras.");
        if (diasPadraoHistorico is not (30 or 90 or 180 or 365)) throw new ArgumentException("O período histórico deve ser 30, 90, 180 ou 365 dias.");
        var responsavel = atualizadoPor?.Trim() ?? "";
        if (responsavel.Length is 0 or > 100) throw new ArgumentException("Responsável inválido.");

        NomeServico = nome;
        Municipio = cidade;
        UnidadeFederativa = uf;
        DiasPadraoHistorico = diasPadraoHistorico;
        AtualizadoPor = responsavel;
        AtualizadoEmUtc = DateTime.UtcNow;
    }
}
