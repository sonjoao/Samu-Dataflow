using SAMUDataFlow.Domain.Comum;

namespace SAMUDataFlow.Domain.Enums;

public enum StatusOcorrencia
{
    [Rotulo("Em andamento")] EmAndamento = 1,
    [Rotulo("Concluída", Apelidos = "Concluido,Finalizada,Finalizado")] Concluida = 2
}
