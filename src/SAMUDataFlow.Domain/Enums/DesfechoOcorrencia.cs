using SAMUDataFlow.Domain.Comum;

namespace SAMUDataFlow.Domain.Enums;

/// <summary>Resultado clínico/operacional registrado ao encerrar um atendimento.</summary>
public enum DesfechoOcorrencia
{
    [Rotulo("Encaminhado para hospital")] EncaminhadoParaHospital = 1,
    [Rotulo("Atendido no local")] AtendidoNoLocal = 2,
    [Rotulo("Recusa de atendimento ou transporte")] Recusa = 3,
    [Rotulo("Óbito")] Obito = 4,
    [Rotulo("Não localizado")] NaoLocalizado = 5,
    [Rotulo("Cancelada")] Cancelada = 6,
    [Rotulo("Outro")] Outro = 7
}
