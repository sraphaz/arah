namespace Araponga.Api.Contracts.Subscriptions;

public sealed class CreateSubscriptionRequest
{
    public Guid PlanId { get; set; }
    public Guid? TerritoryId { get; set; }
    public string? CouponCode { get; set; }
}
