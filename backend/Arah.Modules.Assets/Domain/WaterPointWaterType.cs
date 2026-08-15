namespace Arah.Modules.Assets.Domain;

/// <summary>
/// water_type de WATER_POINT_DETAILS. WELL não é tipo top-level de NaturalAsset.
/// </summary>
public static class WaterPointWaterType
{
    public const string Well = "WELL";
    public const string Tap = "TAP";

    public static readonly IReadOnlySet<string> Allowed = new HashSet<string>(StringComparer.Ordinal)
    {
        Well,
        Tap
    };

    public static string? Normalize(string? waterType)
    {
        if (string.IsNullOrWhiteSpace(waterType))
        {
            return null;
        }

        return waterType.Trim().ToUpperInvariant();
    }

    public static bool TryValidate(string? waterType, out string? normalized, out string? errorMessage)
    {
        normalized = Normalize(waterType);
        errorMessage = null;
        if (normalized is null)
        {
            return true;
        }

        if (!Allowed.Contains(normalized))
        {
            errorMessage = "Invalid water_type. Allowed: WELL, TAP.";
            return false;
        }

        return true;
    }
}
