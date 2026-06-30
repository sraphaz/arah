namespace Arah.Api.Contracts.Journeys.Onboarding;

public sealed record ProposeTerritoryBoundaryPoint(double Latitude, double Longitude);

public sealed record ProposeTerritoryRequest(
    string City,
    string State,
    double Latitude,
    double Longitude,
    string? Name = null,
    double? RadiusKm = null,
    IReadOnlyList<ProposeTerritoryBoundaryPoint>? BoundaryPolygon = null,
    Guid? ParentTerritoryId = null);

public sealed record ProposeTerritoryResponse(
    Guid TerritoryId,
    string Name,
    string City,
    string State,
    string? Description,
    double DistanceKm,
    double Latitude,
    double Longitude,
    string Status,
    bool IsPending,
    bool Created);

public sealed record PendingTerritoryResponse(
    Guid Id,
    string Name,
    string City,
    string State,
    string? Description,
    double Latitude,
    double Longitude,
    DateTime CreatedAtUtc,
    Guid? ParentTerritoryId);

public sealed record ReviewTerritoryProposalRequest(bool Approve);
