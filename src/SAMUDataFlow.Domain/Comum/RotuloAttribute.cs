namespace SAMUDataFlow.Domain.Comum;

/// <summary>Texto de exibição de um valor de enum, com apelidos aceitos na importação (separados por vírgula).</summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class RotuloAttribute(string texto) : Attribute
{
    public string Texto { get; } = texto;
    public string? Apelidos { get; set; }
}
