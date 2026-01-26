namespace Araponga.Api.Contracts.Subscriptions;

public sealed class SubscriptionResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TerritoryId { get; set; }
    public Guid PlanId { get; set; }
    public string Tier { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime? TrialStart { get; set; }
    public DateTime? TrialEnd { get; set; }
    public DateTime? CanceledAt { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
}
