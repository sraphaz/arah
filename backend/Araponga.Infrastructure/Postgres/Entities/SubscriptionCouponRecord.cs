namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class SubscriptionCouponRecord
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid CouponId { get; set; }
    public DateTime AppliedAtUtc { get; set; }
}
