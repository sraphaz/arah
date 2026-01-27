using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Interfaces;

public interface ISubscriptionCouponRepository
{
    Task<SubscriptionCoupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<SubscriptionCoupon?> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken);
    Task AddAsync(SubscriptionCoupon subscriptionCoupon, CancellationToken cancellationToken);
    Task RemoveAsync(SubscriptionCoupon subscriptionCoupon, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
