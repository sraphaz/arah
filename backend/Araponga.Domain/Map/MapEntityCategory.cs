using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Araponga.Domain.Map;

public static class MapEntityCategory
{
    public static readonly IReadOnlyList<string> Allowed = new[]
    {
        "estabelecimento",
        "órgão do governo",
        "espaço público",
        "espaço natural"
    };

    private static readonly Dictionary<string, string> NormalizedLookup = Allowed.ToDictionary(
        NormalizeKey,
        value => value);

    public static string AllowedList => string.Join(", ", Allowed);

    public static bool TryNormalize(string? value, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var key = NormalizeKey(value);
        if (NormalizedLookup.TryGetValue(key, out var match))
        {
            normalized = match;
            return true;
        }

        return false;
    }

    private static string NormalizeKey(string value)
    {
        var trimmed = value
            .Trim()
            .Replace('_', ' ')
            .Replace('-', ' ')
            .ToLowerInvariant();
        trimmed = Regex.Replace(trimmed, "\\s+", " ");

        var normalized = trimmed.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
