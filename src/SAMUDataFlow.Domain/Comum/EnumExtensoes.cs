using System.Reflection;

namespace SAMUDataFlow.Domain.Comum;

internal static class MapaRotulos<T> where T : struct, Enum
{
    public static readonly Dictionary<T, string> Rotulos = new();
    public static readonly Dictionary<string, T> PorChave = new();

    static MapaRotulos()
    {
        foreach (var valor in Enum.GetValues<T>())
        {
            var nome = valor.ToString();
            var atributo = typeof(T).GetField(nome)?.GetCustomAttribute<RotuloAttribute>();

            Rotulos[valor] = atributo?.Texto ?? nome;
            PorChave[Texto.Chave(nome)] = valor;

            if (atributo is null) continue;

            PorChave[Texto.Chave(atributo.Texto)] = valor;

            var apelidos = (atributo.Apelidos ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var apelido in apelidos)
                PorChave[Texto.Chave(apelido)] = valor;
        }
    }
}

public static class EnumExtensoes
{
    /// <summary>Texto para exibir na tela (ex.: Psiquiatrico → "Psiquiátrico").</summary>
    public static string Rotulo<T>(this T valor) where T : struct, Enum =>
        MapaRotulos<T>.Rotulos.TryGetValue(valor, out var rotulo) ? rotulo : valor.ToString();

    /// <summary>Converte texto livre (sem acento, qualquer caixa, apelidos) em enum.</summary>
    public static bool TentarConverter<T>(string? texto, out T valor) where T : struct, Enum
    {
        valor = default;
        var chave = Texto.Chave(texto);
        return chave.Length > 0 && MapaRotulos<T>.PorChave.TryGetValue(chave, out valor);
    }
}
