namespace Arah.Modules.Assets.Domain;

/// <summary>
/// Tipos canônicos NATURAL_ASSET (UPPERCASE). Slice WA-N1: apenas tipos de ponto.
/// RIVER/STREAM exigem WATERCOURSE_DETAILS (24.0b).
/// </summary>
public static class NaturalAssetType
{
    public const string Spring = "SPRING";
    public const string Waterfall = "WATERFALL";
    public const string PotableWater = "POTABLE_WATER";

    /// <summary>Tipos hídricos de ponto permitidos neste slice.</summary>
    public static readonly IReadOnlySet<string> PointTypes = new HashSet<string>(StringComparer.Ordinal)
    {
        Spring,
        Waterfall,
        PotableWater
    };

    /// <summary>Tipos de curso d'água (ainda não suportados neste slice).</summary>
    public static readonly IReadOnlySet<string> WatercourseTypes = new HashSet<string>(StringComparer.Ordinal)
    {
        "RIVER",
        "STREAM"
    };

    public static string? Normalize(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return null;
        }

        return type.Trim().ToUpperInvariant();
    }

    public static bool TryValidatePointType(string? type, out string? normalized, out string? errorMessage)
    {
        normalized = Normalize(type);
        errorMessage = null;
        if (normalized is null)
        {
            errorMessage = "Type is required.";
            return false;
        }

        if (WatercourseTypes.Contains(normalized))
        {
            errorMessage = "RIVER and STREAM require WATERCOURSE_DETAILS (not available in this slice).";
            return false;
        }

        if (!PointTypes.Contains(normalized))
        {
            errorMessage = "Invalid natural asset type. Allowed point types: SPRING, WATERFALL, POTABLE_WATER.";
            return false;
        }

        return true;
    }
}
