namespace Araponga.Api.Contracts.Subscriptions;

public sealed class SubscriptionPlanResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Tier { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public Guid? TerritoryId { get; set; }
    public decimal? PricePerCycle { get; set; }
    public string? BillingCycle { get; set; }
    public List<string> Capabilities { get; set; } = new();
    public Dictionary<string, object> Limits { get; set; } = new();
    public bool IsDefault { get; set; }
    public int? TrialDays { get; set; }
    public bool IsActive { get; set; }
}
