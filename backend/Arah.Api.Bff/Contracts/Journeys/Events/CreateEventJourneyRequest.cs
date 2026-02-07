namespace Arah.Bff.Contracts.Journeys.Events;

public sealed record CreateEventJourneyRequest(
    Guid TerritoryId,
    string Title,
    string? Description,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    double? Latitude,
    double? Longitude,
    string? LocationLabel,
    Guid? CoverMediaId,
    IReadOnlyList<Guid>? AdditionalMediaIds);
