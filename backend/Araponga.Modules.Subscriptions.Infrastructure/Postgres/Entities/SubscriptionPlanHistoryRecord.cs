using Araponga.Domain.Subscriptions;

namespace Araponga.Modules.Subscriptions.Infrastructure.Postgres.Entities;

public sealed class SubscriptionPlanHistoryRecord
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public Guid ChangedByUserId { get; set; }
    public int ChangeType { get; set; } // SubscriptionPlanHistoryChangeType enum
    public string? PreviousStateJson { get; set; } // JSON object
    public string? NewStateJson { get; set; } // JSON object
    public string? ChangeReason { get; set; }
    public DateTime ChangedAtUtc { get; set; }
}
