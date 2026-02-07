namespace Arah.Bff.Contracts.Journeys.Events;

public sealed record EventParticipationRequest(
    Guid EventId,
    string Status);
