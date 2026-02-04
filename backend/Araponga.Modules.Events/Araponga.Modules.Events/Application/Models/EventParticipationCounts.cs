namespace Araponga.Application.Models;

public sealed record EventParticipationCounts
{
    public Guid EventId { get; init; }

    private int _interestedCount;
    private int _confirmedCount;

    public int InterestedCount
    {
        get => _interestedCount;
        init => _interestedCount = value > int.MaxValue ? int.MaxValue : value;
    }

    public int ConfirmedCount
    {
        get => _confirmedCount;
        init => _confirmedCount = value > int.MaxValue ? int.MaxValue : value;
    }

    public EventParticipationCounts(Guid eventId, int interestedCount, int confirmedCount)
    {
        EventId = eventId;
        InterestedCount = interestedCount;
        ConfirmedCount = confirmedCount;
    }
}
