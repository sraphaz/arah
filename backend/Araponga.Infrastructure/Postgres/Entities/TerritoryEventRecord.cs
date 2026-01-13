using Araponga.Domain.Events;
using Araponga.Domain.Social;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class TerritoryEventRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime? EndsAtUtc { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? LocationLabel { get; set; }
    public Guid CreatedByUserId { get; set; }
    public MembershipRole CreatedByMembership { get; set; }
    public EventStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
