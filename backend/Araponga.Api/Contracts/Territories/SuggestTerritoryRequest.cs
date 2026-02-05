namespace Araponga.Api.Contracts.Territories;

public sealed record SuggestTerritoryRequest(
    string Name,
    string? Description,
    string City,
    string State,
    double Latitude,
    double Longitude,
    double? RadiusKm = null
);
