using Araponga.Domain.Governance;

namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class TerritoryModerationRuleRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid? CreatedByVotingId { get; set; }
    public RuleType RuleType { get; set; }
    public string RuleJson { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
