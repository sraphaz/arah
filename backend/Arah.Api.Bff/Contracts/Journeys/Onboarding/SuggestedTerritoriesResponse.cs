namespace Arah.Bff.Contracts.Journeys.Onboarding;

public sealed record SuggestedTerritoriesResponse(
    IReadOnlyList<TerritorySuggestionDto> Territories);

public sealed record TerritorySuggestionDto(
    Guid Id,
    string Name,
    string? Description,
    double DistanceKm,
    double Latitude,
    double Longitude);
