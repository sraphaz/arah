using Arah.Domain.Territories;

namespace Arah.Application.Interfaces.Geo;

public interface IMunicipalityTerritoryProvisioningService
{
    Task<MunicipalityTerritoryProvisioningResult?> SuggestOrCreateAsync(
        double latitude,
        double longitude,
        string? city,
        string? stateCode,
        CancellationToken cancellationToken = default);
}

public sealed record MunicipalityTerritoryProvisioningResult(
    Guid TerritoryId,
    string Name,
    string City,
    string StateCode,
    string? Description,
    double Latitude,
    double Longitude,
    double DistanceKm,
    int? IbgeCode,
    bool Created,
    IReadOnlyList<TerritoryBoundaryPoint>? BoundaryPolygon)
{
    public static MunicipalityTerritoryProvisioningResult FromTerritory(
        Territory territory,
        double distanceKm,
        bool created,
        int? ibgeCode) =>
        new(
            territory.Id,
            territory.Name,
            territory.City,
            territory.State,
            territory.Description,
            territory.Latitude,
            territory.Longitude,
            Math.Round(distanceKm, 2),
            ibgeCode,
            created,
            territory.BoundaryPolygon);
}
