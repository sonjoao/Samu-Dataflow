namespace SAMUDataFlow.Domain.Excecoes;

/// <summary>Violação de uma regra de negócio do domínio.</summary>
public abstract class DominioException(string mensagem) : Exception(mensagem);
