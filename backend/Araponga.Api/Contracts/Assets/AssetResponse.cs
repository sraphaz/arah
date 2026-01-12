namespace Araponga.Api.Contracts.Assets;

public sealed record AssetResponse(
    Guid Id,
    Guid TerritoryId,
    string Type,
    string Name,
    string? Description,
    string Status,
    IReadOnlyCollection<AssetGeoAnchorResponse> GeoAnchors,
    int ValidationsCount,
    int EligibleResidentsCount,
    double ValidationPct,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    DateTime? ArchivedAtUtc,
    string? ArchiveReason);
