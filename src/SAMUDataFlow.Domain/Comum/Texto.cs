using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SAMUDataFlow.Domain.Comum;

/// <summary>Normalização de textos usada por validação, filtros e importação.</summary>
public static partial class Texto
{
    private static readonly HashSet<string> Particulas =
        new(StringComparer.Ordinal) { "de", "da", "do", "das", "dos", "e" };

    /// <summary>Remove espaços nas pontas e reduz espaços repetidos a um só.</summary>
    public static string Limpar(string? valor) =>
        EspacosMultiplos().Replace((valor ?? "").Trim(), " ");

    /// <summary>Limpa e coloca em maiúsculas (usado nos códigos de ocorrência).</summary>
    public static string Normalizar(string? valor) => Limpar(valor).ToUpperInvariant();

    /// <summary>Chave de comparação: sem acentos, sem diferença de caixa ("Clínico" = "CLINICO").</summary>
    public static string Chave(string? valor)
    {
        var decomposto = Limpar(valor).Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposto.Length);

        foreach (var c in decomposto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }

    /// <summary>"alto josé do pinho" vira "Alto José do Pinho"; "DISTRITO vi" vira "Distrito VI".</summary>
    public static string Titulo(string? valor)
    {
        var limpo = Limpar(valor);
        if (limpo.Length == 0) return limpo;

        var cultura = CultureInfo.GetCultureInfo("pt-BR");
        var palavras = limpo.Split(' ');

        for (var i = 0; i < palavras.Length; i++)
        {
            var minuscula = palavras[i].ToLower(cultura);
            var maiuscula = palavras[i].ToUpper(cultura);

            if (i > 0 && Particulas.Contains(minuscula))
                palavras[i] = minuscula;
            else if (NumeroRomano().IsMatch(maiuscula))
                palavras[i] = maiuscula;
            else
                palavras[i] = char.ToUpper(minuscula[0], cultura) + minuscula[1..];
        }

        return string.Join(' ', palavras);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex EspacosMultiplos();

    [GeneratedRegex(@"^(?:X{0,3})(?:IX|IV|V?I{0,3})$")]
    private static partial Regex NumeroRomano();
}
