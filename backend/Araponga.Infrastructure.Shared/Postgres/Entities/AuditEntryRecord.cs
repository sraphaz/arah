namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class AuditEntryRecord
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid ActorUserId { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid TargetId { get; set; }
    public DateTime TimestampUtc { get; set; }
}
