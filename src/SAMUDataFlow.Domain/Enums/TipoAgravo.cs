using SAMUDataFlow.Domain.Comum;

namespace SAMUDataFlow.Domain.Enums;

public enum TipoAgravo
{
    [Rotulo("Trauma")] Trauma = 1,
    [Rotulo("Clínico", Apelidos = "Clinica")] Clinico = 2,
    [Rotulo("Cardiológico", Apelidos = "Cardiologica,Cardiaco")] Cardiologico = 3,
    [Rotulo("Respiratório", Apelidos = "Respiratoria")] Respiratorio = 4,
    [Rotulo("Psiquiátrico", Apelidos = "Psiquiatrica")] Psiquiatrico = 5,
    [Rotulo("Outros", Apelidos = "Outro")] Outros = 6
}
