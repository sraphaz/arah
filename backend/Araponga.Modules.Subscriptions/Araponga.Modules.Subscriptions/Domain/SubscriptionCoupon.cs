namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Relacionamento entre assinatura e cupom.
/// </summary>
public sealed class SubscriptionCoupon
{
    public Guid Id { get; private set; }
    public Guid SubscriptionId { get; private set; }
    public Guid CouponId { get; private set; }
    
    /// <summary>
    /// Data de aplicação do cupom.
    /// </summary>
    public DateTime AppliedAtUtc { get; private set; }
    
    private SubscriptionCoupon()
    {
    }
    
    public SubscriptionCoupon(
        Guid id,
        Guid subscriptionId,
        Guid couponId)
    {
        if (subscriptionId == Guid.Empty)
        {
            throw new ArgumentException("Subscription ID is required.", nameof(subscriptionId));
        }
        
        if (couponId == Guid.Empty)
        {
            throw new ArgumentException("Coupon ID is required.", nameof(couponId));
        }
        
        Id = id;
        SubscriptionId = subscriptionId;
        CouponId = couponId;
        AppliedAtUtc = DateTime.UtcNow;
    }
}
