namespace Arah.Bff.Contracts.Journeys.Events;

public sealed record CreateEventJourneyResponse(
    EventDetailJourneyDto Event,
    IReadOnlyList<string> MediaUrls);
