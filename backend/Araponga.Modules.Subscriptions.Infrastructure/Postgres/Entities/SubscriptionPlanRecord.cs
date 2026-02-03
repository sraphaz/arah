using Araponga.Domain.Subscriptions;

namespace Araponga.Modules.Subscriptions.Infrastructure.Postgres.Entities;

public sealed class SubscriptionPlanRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Tier { get; set; } // SubscriptionPlanTier enum
    public int Scope { get; set; } // PlanScope enum
    public Guid? TerritoryId { get; set; }
    public decimal? PricePerCycle { get; set; }
    public int? BillingCycle { get; set; } // SubscriptionBillingCycle enum (nullable)
    public string CapabilitiesJson { get; set; } = "[]"; // JSON array of FeatureCapability
    public string? LimitsJson { get; set; } // JSON object
    public bool IsDefault { get; set; }
    public int? TrialDays { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string? StripePriceId { get; set; }
    public string? StripeProductId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
