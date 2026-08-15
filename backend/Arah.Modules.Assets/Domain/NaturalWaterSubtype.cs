namespace Arah.Modules.Assets.Domain;

/// <summary>
/// Allowlist de subtypes hídricos para TerritoryAsset com Type = natural (ponte WA-E1).
/// WELL no MER alvo é water_type de ponto; na ponte usa subtype "well".
/// </summary>
public static class NaturalWaterSubtype
{
    public const string NaturalType = "natural";

    public static readonly IReadOnlySet<string> Allowed = new HashSet<string>(StringComparer.Ordinal)
    {
        "river",
        "stream",
        "spring",
        "waterfall",
        "well",
        "potable_water"
    };

    /// <summary>
    /// Normaliza subtype (trim + lowercase). Null/whitespace → null.
    /// </summary>
    public static string? Normalize(string? subtype)
    {
        if (string.IsNullOrWhiteSpace(subtype))
        {
            return null;
        }

        return subtype.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Valida combinação type + subtype. Em falha, errorMessage descreve o motivo.
    /// </summary>
    public static bool TryValidate(string normalizedType, string? normalizedSubtype, out string? errorMessage)
    {
        errorMessage = null;
        if (normalizedSubtype is null)
        {
            return true;
        }

        if (!string.Equals(normalizedType, NaturalType, StringComparison.Ordinal))
        {
            errorMessage = "Subtype is only allowed when type is natural.";
            return false;
        }

        if (!Allowed.Contains(normalizedSubtype))
        {
            errorMessage = "Invalid natural water subtype. Allowed: river, stream, spring, waterfall, well, potable_water.";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Mapeamento futuro para NATURAL_ASSET.type (MER). Não persiste NaturalAsset.
    /// </summary>
    public static string? ToNaturalAssetType(string? normalizedSubtype) => normalizedSubtype switch
    {
        "river" => "RIVER",
        "stream" => "STREAM",
        "spring" => "SPRING",
        "waterfall" => "WATERFALL",
        "well" => "POTABLE_WATER",
        "potable_water" => "POTABLE_WATER",
        _ => null
    };
}
