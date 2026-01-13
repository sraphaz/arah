namespace Araponga.Application.Models;

public sealed record EventParticipationCounts(
    Guid EventId,
    int InterestedCount,
    int ConfirmedCount);
