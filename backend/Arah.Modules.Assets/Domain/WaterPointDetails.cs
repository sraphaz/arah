using Arah.Domain.Geo;

namespace Arah.Modules.Assets.Domain;

/// <summary>
/// Detalhes de ponto hídrico (nascente, cachoeira, água potável/poço).
/// </summary>
public sealed class WaterPointDetails
{
    public WaterPointDetails(
        double latitude,
        double longitude,
        string? waterType = null,
        string? potabilityNotes = null,
        DateTime? lastTestedAtUtc = null)
    {
        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude/longitude is invalid.");
        }

        if (!WaterPointWaterType.TryValidate(waterType, out var normalizedWaterType, out var waterTypeError))
        {
            throw new ArgumentException(waterTypeError ?? "Invalid water_type.", nameof(waterType));
        }

        Latitude = latitude;
        Longitude = longitude;
        WaterType = normalizedWaterType;
        PotabilityNotes = string.IsNullOrWhiteSpace(potabilityNotes) ? null : potabilityNotes.Trim();
        LastTestedAtUtc = lastTestedAtUtc;
    }

    public double Latitude { get; }
    public double Longitude { get; }
    public string? WaterType { get; }
    public string? PotabilityNotes { get; }
    public DateTime? LastTestedAtUtc { get; }
}
