namespace Arah.Application.Interfaces.Geo;

/// <summary>
/// Reverse geocode (lat/lng → município/UF). Implementação padrão: Nominatim/OSM.
/// </summary>
public interface IReverseGeocodingService
{
    Task<ReverseGeocodeResult?> ReverseAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default);
}

public sealed record ReverseGeocodeResult(string City, string StateCode);
