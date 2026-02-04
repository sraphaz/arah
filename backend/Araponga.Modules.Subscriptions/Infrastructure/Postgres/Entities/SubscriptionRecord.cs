using Araponga.Domain.Subscriptions;

namespace Araponga.Modules.Subscriptions.Infrastructure.Postgres.Entities;

public sealed class SubscriptionRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TerritoryId { get; set; }
    public Guid PlanId { get; set; }
    public int Status { get; set; } // SubscriptionStatus enum
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime? TrialStart { get; set; }
    public DateTime? TrialEnd { get; set; }
    public DateTime? CanceledAt { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string? StripeCustomerId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
