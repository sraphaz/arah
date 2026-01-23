namespace Araponga.Api.Contracts.Territories;

public sealed record TerritoryResponse(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    string City,
    string State,
    double Latitude,
    double Longitude,
    DateTime CreatedAtUtc,
    IReadOnlyList<string> Tags
);
