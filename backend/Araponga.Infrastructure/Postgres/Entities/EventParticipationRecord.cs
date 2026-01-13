using Araponga.Domain.Events;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class EventParticipationRecord
{
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public EventParticipationStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
