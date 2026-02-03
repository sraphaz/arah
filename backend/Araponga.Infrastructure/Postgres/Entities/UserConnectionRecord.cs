namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class UserConnectionRecord
{
    public Guid Id { get; set; }
    public Guid RequesterUserId { get; set; }
    public Guid TargetUserId { get; set; }
    public int Status { get; set; } // ConnectionStatus enum
    public Guid? TerritoryId { get; set; }
    public DateTime RequestedAtUtc { get; set; }
    public DateTime? RespondedAtUtc { get; set; }
    public DateTime? RemovedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
