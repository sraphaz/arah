namespace Araponga.Application.Models;

public sealed record EventParticipationCounts(
    Guid EventId,
    int InterestedCount,
    int ConfirmedCount)
{
    public EventParticipationCounts(Guid eventId, int interestedCount, int confirmedCount)
        : this(
            eventId,
            interestedCount > int.MaxValue ? int.MaxValue : interestedCount,
            confirmedCount > int.MaxValue ? int.MaxValue : confirmedCount)
    {
    }
}
