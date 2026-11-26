using SAMUDataFlow.Domain.Comum;
using SAMUDataFlow.Domain.Enums;
using SAMUDataFlow.Domain.Excecoes;

namespace SAMUDataFlow.Domain.Entidades;

/// <summary>
/// Ocorrência atendida pelo SAMU. Só nasce válida (método <see cref="Criar"/>) e só muda
/// por <see cref="Atualizar"/>, que repete as mesmas validações.
/// </summary>
public sealed class Ocorrencia
{
    public const int TamanhoMaximoTexto = 100;

    public int Id { get; private set; }
    public string CodigoExterno { get; private set; } = "";
    public DateTime DataChamado { get; private set; }
    public DateTime? DataChegada { get; private set; }
    public string Distrito { get; private set; } = "";
    public string Bairro { get; private set; } = "";
    public TipoAgravo TipoAgravo { get; private set; }
    public Gravidade Gravidade { get; private set; }
    public StatusOcorrencia Status { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public double? TempoRespostaMinutos { get; private set; }
    public string? AnotacoesParamedicos { get; private set; }
    public DesfechoOcorrencia? Desfecho { get; private set; }
    public string? HospitalDestino { get; private set; }
    public string? ObservacoesExtras { get; private set; }
    public string? Acompanhante { get; private set; }
    public DateTime RegistradoEm { get; private set; }

    private Ocorrencia() { } // exigido pelo EF Core

    public static Ocorrencia Criar(
        string? codigoExterno,
        DateTime dataChamado,
        DateTime? dataChegada,
        string? distrito,
        string? bairro,
        TipoAgravo tipoAgravo,
        Gravidade gravidade,
        StatusOcorrencia status,
        double? latitude = null,
        double? longitude = null,
        DateTime? registradoEm = null,
        string? anotacoesParamedicos = null,
        DesfechoOcorrencia? desfecho = null,
        string? hospitalDestino = null,
        string? observacoesExtras = null,
        string? acompanhante = null)
    {
        var codigo = Texto.Normalizar(codigoExterno);

        if (codigo.Length is 0 or > TamanhoMaximoTexto)
            throw new OcorrenciaInvalidaException("O código da ocorrência deve ter de 1 a 100 caracteres.");

        var ocorrencia = new Ocorrencia
        {
            CodigoExterno = codigo,
            RegistradoEm = ParaUtc(registradoEm ?? DateTime.UtcNow)
        };

        ocorrencia.Aplicar(dataChamado, dataChegada, distrito, bairro, tipoAgravo, gravidade, status, latitude, longitude,
            anotacoesParamedicos, desfecho, hospitalDestino, observacoesExtras, acompanhante);
        return ocorrencia;
    }

    /// <summary>Altera os dados da ocorrência. O código externo nunca muda.</summary>
    public void Atualizar(
        DateTime dataChamado,
        DateTime? dataChegada,
        string? distrito,
        string? bairro,
        TipoAgravo tipoAgravo,
        Gravidade gravidade,
        StatusOcorrencia status,
        double? latitude = null,
        double? longitude = null,
        string? anotacoesParamedicos = null,
        DesfechoOcorrencia? desfecho = null,
        string? hospitalDestino = null,
        string? observacoesExtras = null,
        string? acompanhante = null) =>
        Aplicar(dataChamado, dataChegada, distrito, bairro, tipoAgravo, gravidade, status, latitude, longitude,
            anotacoesParamedicos, desfecho, hospitalDestino, observacoesExtras, acompanhante);

    private void Aplicar(
        DateTime dataChamado,
        DateTime? dataChegada,
        string? distrito,
        string? bairro,
        TipoAgravo tipoAgravo,
        Gravidade gravidade,
        StatusOcorrencia status,
        double? latitude,
        double? longitude,
        string? anotacoesParamedicos,
        DesfechoOcorrencia? desfecho,
        string? hospitalDestino,
        string? observacoesExtras,
        string? acompanhante)
    {
        var distritoOk = Texto.Titulo(distrito);
        var bairroOk = Texto.Titulo(bairro);

        if (distritoOk.Length is 0 or > TamanhoMaximoTexto)
            throw new OcorrenciaInvalidaException("O distrito deve ter de 1 a 100 caracteres.");

        if (bairroOk.Length is 0 or > TamanhoMaximoTexto)
            throw new OcorrenciaInvalidaException("O bairro deve ter de 1 a 100 caracteres.");

        if (!Enum.IsDefined(tipoAgravo))
            throw new OcorrenciaInvalidaException("Tipo de agravo inválido.");

        if (!Enum.IsDefined(gravidade))
            throw new OcorrenciaInvalidaException("Gravidade inválida.");

        if (!Enum.IsDefined(status))
            throw new OcorrenciaInvalidaException("Status inválido.");

        if (desfecho.HasValue && !Enum.IsDefined(desfecho.Value))
            throw new OcorrenciaInvalidaException("Desfecho inválido.");

        ValidarTextoOpcional(anotacoesParamedicos, 4000, "As anotações dos paramédicos");
        ValidarTextoOpcional(hospitalDestino, 200, "O hospital de destino");
        ValidarTextoOpcional(observacoesExtras, 4000, "As observações extras");
        ValidarTextoOpcional(acompanhante, 200, "A informação sobre acompanhante");

        if (dataChamado == default)
            throw new OcorrenciaInvalidaException("Data do chamado inválida.");

        var chamado = ParaUtc(dataChamado);
        DateTime? chegada = dataChegada.HasValue ? ParaUtc(dataChegada.Value) : null;

        if (chegada.HasValue && chegada.Value < chamado)
            throw new OcorrenciaInvalidaException("A chegada não pode ser anterior ao chamado.");

        if ((latitude is null) != (longitude is null))
            throw new OcorrenciaInvalidaException("Informe latitude e longitude juntas.");

        if (latitude is < -90 or > 90)
            throw new OcorrenciaInvalidaException("Latitude deve estar entre -90 e 90.");

        if (longitude is < -180 or > 180)
            throw new OcorrenciaInvalidaException("Longitude deve estar entre -180 e 180.");

        DataChamado = chamado;
        DataChegada = chegada;
        Distrito = distritoOk;
        Bairro = bairroOk;
        TipoAgravo = tipoAgravo;
        Gravidade = gravidade;
        Status = status;
        Latitude = latitude;
        Longitude = longitude;
        AnotacoesParamedicos = TextoOpcional(anotacoesParamedicos);
        Desfecho = desfecho;
        HospitalDestino = TextoOpcional(hospitalDestino);
        ObservacoesExtras = TextoOpcional(observacoesExtras);
        Acompanhante = TextoOpcional(acompanhante);
        TempoRespostaMinutos = chegada.HasValue
            ? Math.Round((chegada.Value - chamado).TotalMinutes, 2)
            : null;
    }

    private static void ValidarTextoOpcional(string? valor, int maximo, string campo)
    {
        if (valor?.Trim().Length > maximo)
            throw new OcorrenciaInvalidaException($"{campo} deve ter no máximo {maximo} caracteres.");
    }

    private static string? TextoOpcional(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static DateTime ParaUtc(DateTime valor) => valor.Kind switch
    {
        DateTimeKind.Utc => valor,
        DateTimeKind.Local => valor.ToUniversalTime(),
        _ => DateTime.SpecifyKind(valor, DateTimeKind.Utc)
    };
}
