using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySubscriptionCouponRepository : ISubscriptionCouponRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySubscriptionCouponRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<SubscriptionCoupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var subscriptionCoupon = _dataStore.SubscriptionCoupons.FirstOrDefault(sc => sc.Id == id);
        return Task.FromResult(subscriptionCoupon);
    }

    public Task<SubscriptionCoupon?> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var subscriptionCoupon = _dataStore.SubscriptionCoupons.FirstOrDefault(sc => sc.SubscriptionId == subscriptionId);
        return Task.FromResult(subscriptionCoupon);
    }

    public Task AddAsync(SubscriptionCoupon subscriptionCoupon, CancellationToken cancellationToken)
    {
        _dataStore.SubscriptionCoupons.Add(subscriptionCoupon);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(SubscriptionCoupon subscriptionCoupon, CancellationToken cancellationToken)
    {
        _dataStore.SubscriptionCoupons.Remove(subscriptionCoupon);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var exists = _dataStore.SubscriptionCoupons.Any(sc => sc.Id == id);
        return Task.FromResult(exists);
    }
}
