namespace Araponga.Domain.Events;

public sealed class EventParticipation
{
    public EventParticipation(
        Guid eventId,
        Guid userId,
        EventParticipationStatus status,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException("Event ID is required.", nameof(eventId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        EventId = eventId;
        UserId = userId;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid EventId { get; }
    public Guid UserId { get; }
    public EventParticipationStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void UpdateStatus(EventParticipationStatus status, DateTime updatedAtUtc)
    {
        Status = status;
        UpdatedAtUtc = updatedAtUtc;
    }
}
