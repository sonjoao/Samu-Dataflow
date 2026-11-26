namespace SAMUDataFlow.Domain.Entidades;

/// <summary>Registro de auditoria sem valores clínicos ou dados pessoais da ocorrência.</summary>
public sealed class EventoHistorico
{
    public int Id { get; private set; }
    public string TipoRegistro { get; private set; } = "";
    public int? RegistroId { get; private set; }
    public string? CodigoOcorrencia { get; private set; }
    public string Acao { get; private set; } = "";
    public string Responsavel { get; private set; } = "";
    public string Detalhes { get; private set; } = "";
    public DateTime CriadoEmUtc { get; private set; }

    private EventoHistorico() { }

    public static EventoHistorico Criar(string tipoRegistro, int? registroId, string? codigoOcorrencia,
        string acao, string responsavel, string detalhes, DateTime? criadoEmUtc = null)
    {
        if (string.IsNullOrWhiteSpace(tipoRegistro) || tipoRegistro.Trim().Length > 40)
            throw new ArgumentException("Tipo de registro inválido.", nameof(tipoRegistro));
        if (string.IsNullOrWhiteSpace(acao) || acao.Trim().Length > 60)
            throw new ArgumentException("Ação inválida.", nameof(acao));
        if (string.IsNullOrWhiteSpace(responsavel) || responsavel.Trim().Length > 100)
            throw new ArgumentException("Responsável inválido.", nameof(responsavel));
        if (string.IsNullOrWhiteSpace(detalhes) || detalhes.Trim().Length > 500)
            throw new ArgumentException("Detalhes inválidos.", nameof(detalhes));

        return new EventoHistorico
        {
            TipoRegistro = tipoRegistro.Trim(),
            RegistroId = registroId,
            CodigoOcorrencia = string.IsNullOrWhiteSpace(codigoOcorrencia) ? null : codigoOcorrencia.Trim(),
            Acao = acao.Trim(),
            Responsavel = responsavel.Trim(),
            Detalhes = detalhes.Trim(),
            CriadoEmUtc = DateTime.SpecifyKind(criadoEmUtc ?? DateTime.UtcNow, DateTimeKind.Utc)
        };
    }
}
