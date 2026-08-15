namespace Arah.Api.Contracts.Assets;

public sealed record CreateNaturalAssetRequest(
    string Type,
    string Name,
    string? Description,
    double Latitude,
    double Longitude,
    string? WaterType = null,
    string? PotabilityNotes = null,
    DateTime? LastTestedAtUtc = null);

public sealed record NaturalAssetResponse(
    Guid Id,
    Guid TerritoryId,
    string Type,
    string Name,
    string? Description,
    string Status,
    double Latitude,
    double Longitude,
    string? WaterType,
    string? PotabilityNotes,
    DateTime? LastTestedAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
