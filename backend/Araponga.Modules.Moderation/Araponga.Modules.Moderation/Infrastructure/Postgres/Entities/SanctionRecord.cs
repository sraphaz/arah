using Araponga.Modules.Moderation.Domain.Moderation;

namespace Araponga.Modules.Moderation.Infrastructure.Postgres.Entities;

public sealed class SanctionRecord
{
    public Guid Id { get; set; }
    public Guid? TerritoryId { get; set; }
    public SanctionScope Scope { get; set; }
    public SanctionTargetType TargetType { get; set; }
    public Guid TargetId { get; set; }
    public SanctionType Type { get; set; }
    public string Reason { get; set; } = string.Empty;
    public SanctionStatus Status { get; set; }
    public DateTime StartAtUtc { get; set; }
    public DateTime? EndAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
