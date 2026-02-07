namespace Arah.Bff.Contracts.Journeys.Onboarding;

public sealed record TerritorySummary(
    Guid Id,
    string Name,
    string? Description,
    bool Active);
