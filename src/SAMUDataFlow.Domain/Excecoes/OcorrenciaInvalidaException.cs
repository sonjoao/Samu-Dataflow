namespace SAMUDataFlow.Domain.Excecoes;

public sealed class OcorrenciaInvalidaException(string mensagem) : DominioException(mensagem);
