using Araponga.Domain.Social.JoinRequests;

namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class TerritoryJoinRequestRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid RequesterUserId { get; set; }
    public string? Message { get; set; }
    public TerritoryJoinRequestStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
    public Guid? DecidedByUserId { get; set; }
}
