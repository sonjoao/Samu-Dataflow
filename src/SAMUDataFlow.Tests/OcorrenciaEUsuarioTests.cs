using SAMUDataFlow.Domain.Entidades;
using SAMUDataFlow.Domain.Enums;
using SAMUDataFlow.Domain.Excecoes;
using Xunit;

namespace SAMUDataFlow.Tests;

public sealed class OcorrenciaEUsuarioTests
{
    [Fact]
    public void CriarOcorrenciaArmazenaDetalhesOpcionaisNormalizados()
    {
        var ocorrencia = NovaOcorrencia(
            anotacoes: "  paciente consciente  ",
            desfecho: DesfechoOcorrencia.EncaminhadoParaHospital,
            hospital: "  Hospital Central  ",
            observacoes: "  Acompanhado pela família  ",
            acompanhante: "  Maria  ");

        Assert.Equal("paciente consciente", ocorrencia.AnotacoesParamedicos);
        Assert.Equal(DesfechoOcorrencia.EncaminhadoParaHospital, ocorrencia.Desfecho);
        Assert.Equal("Hospital Central", ocorrencia.HospitalDestino);
        Assert.Equal("Acompanhado pela família", ocorrencia.ObservacoesExtras);
        Assert.Equal("Maria", ocorrencia.Acompanhante);
    }

    [Fact]
    public void CriarOcorrenciaMantemDetalhesNulosParaCompatibilidadeLegada()
    {
        var ocorrencia = NovaOcorrencia();

        Assert.Null(ocorrencia.AnotacoesParamedicos);
        Assert.Null(ocorrencia.Desfecho);
        Assert.Null(ocorrencia.HospitalDestino);
        Assert.Null(ocorrencia.ObservacoesExtras);
        Assert.Null(ocorrencia.Acompanhante);
    }

    [Fact]
    public void CriarOcorrenciaRejeitaAnotacaoAcimaDoLimite()
    {
        var excecao = Assert.Throws<OcorrenciaInvalidaException>(() =>
            NovaOcorrencia(anotacoes: new string('x', 4001)));

        Assert.Contains("4000", excecao.Message);
    }

    [Fact]
    public void UsuarioLegadoComecaComPapelSomenteLeitura()
    {
        var usuario = Usuario.Criar("João da Silva", "Operação");

        Assert.Equal(Usuario.AdministradorSomenteLeitura, usuario.Papel);
        Assert.Null(usuario.Login);
        Assert.Null(usuario.SenhaHash);
    }

    [Fact]
    public void UsuarioAceitaSomenteOsDoisPapeisConhecidos()
    {
        Assert.True(Usuario.PapelValido(Usuario.AdministradorSomenteLeitura));
        Assert.True(Usuario.PapelValido(Usuario.AdministradorCompleto));
        Assert.False(Usuario.PapelValido("Administrador desconhecido"));
        Assert.ThrowsAny<DominioException>(() => Usuario.CriarConta("Ana Silva", "Central", "ana", "hash", "Root"));
    }

    [Fact]
    public void ArquivarOcorrenciaPreservaRegistroParaConsultaHistorica()
    {
        var ocorrencia = NovaOcorrencia();

        ocorrencia.Arquivar();

        Assert.True(ocorrencia.EstaArquivada);
        Assert.Equal("SAMU-1", ocorrencia.CodigoExterno);
        Assert.Equal("Boa Viagem", ocorrencia.Bairro);
    }

    [Fact]
    public void ConfiguracaoSistemaValidaPeriodoERegistraAtualizacao()
    {
        var configuracao = ConfiguracaoSistema.CriarPadrao();

        configuracao.Atualizar("SAMU Recife", "Recife", "pe", 90, "Ana Silva");

        Assert.Equal("SAMU Recife", configuracao.NomeServico);
        Assert.Equal("PE", configuracao.UnidadeFederativa);
        Assert.Equal(90, configuracao.DiasPadraoHistorico);
        Assert.Equal("Ana Silva", configuracao.AtualizadoPor);
        Assert.Throws<ArgumentException>(() => configuracao.Atualizar("SAMU", "Recife", "PE", 45, "Ana Silva"));
    }

    [Fact]
    public void EventoHistoricoNaoAceitaDetalhesSemResponsavel()
    {
        Assert.Throws<ArgumentException>(() => EventoHistorico.Criar("Ocorrência", 1, "SAMU-1", "Criada", "", "Registro criado."));
    }

    private static Ocorrencia NovaOcorrencia(
        string? anotacoes = null,
        DesfechoOcorrencia? desfecho = null,
        string? hospital = null,
        string? observacoes = null,
        string? acompanhante = null) =>
        Ocorrencia.Criar(
            "SAMU-1", DateTime.SpecifyKind(new DateTime(2026, 1, 1, 12, 0, 0), DateTimeKind.Utc), null,
            "Recife", "Boa Viagem", TipoAgravo.Trauma, Gravidade.Media, StatusOcorrencia.Concluida,
            anotacoesParamedicos: anotacoes, desfecho: desfecho, hospitalDestino: hospital,
            observacoesExtras: observacoes, acompanhante: acompanhante);
}
