namespace Araponga.Api.Contracts.Territories;

/// <summary>
/// Request para sugerir/cadastrar um território.
/// Pode informar centro (Latitude, Longitude) + RadiusKm ou um polígono (BoundaryPolygon com pelo menos 3 pontos).
/// Se BoundaryPolygon for informado, Latitude e Longitude podem ser o centroide ou o primeiro ponto.
/// </summary>
public sealed record SuggestTerritoryRequest(
    string Name,
    string? Description,
    string City,
    string State,
    double Latitude,
    double Longitude,
    double? RadiusKm = null,
    IReadOnlyList<GeoPointDto>? BoundaryPolygon = null
);
