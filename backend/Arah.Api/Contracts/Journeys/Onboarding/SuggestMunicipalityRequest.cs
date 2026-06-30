namespace Arah.Api.Contracts.Journeys.Onboarding;

public sealed record SuggestMunicipalityRequest(
    double Latitude,
    double Longitude,
    string? City = null,
    string? State = null);

public sealed record SuggestMunicipalityResponse(
    Guid TerritoryId,
    string Name,
    string City,
    string State,
    string? Description,
    double DistanceKm,
    double Latitude,
    double Longitude,
    int? IbgeCode,
    bool Created);
