namespace Arah.Api.Contracts.Subscriptions;

public sealed class CreateMerchantSubscriptionRequest
{
    public Guid PlanId { get; set; }
    public string? CouponCode { get; set; }
}
