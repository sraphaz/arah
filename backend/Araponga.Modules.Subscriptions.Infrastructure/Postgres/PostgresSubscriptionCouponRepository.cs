using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Araponga.Modules.Subscriptions.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Subscriptions.Infrastructure.Postgres;

public sealed class PostgresSubscriptionCouponRepository : ISubscriptionCouponRepository
{
    private readonly SubscriptionsDbContext _dbContext;

    public PostgresSubscriptionCouponRepository(SubscriptionsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubscriptionCoupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionCoupons
            .AsNoTracking()
            .FirstOrDefaultAsync(sc => sc.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<SubscriptionCoupon?> GetBySubscriptionIdAsync(Guid subscriptionId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionCoupons
            .AsNoTracking()
            .FirstOrDefaultAsync(sc => sc.SubscriptionId == subscriptionId, cancellationToken);
        return record?.ToDomain();
    }

    public Task AddAsync(SubscriptionCoupon subscriptionCoupon, CancellationToken cancellationToken)
    {
        _dbContext.SubscriptionCoupons.Add(subscriptionCoupon.ToRecord());
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(SubscriptionCoupon subscriptionCoupon, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionCoupons
            .FirstOrDefaultAsync(sc => sc.Id == subscriptionCoupon.Id, cancellationToken);

        if (record != null)
        {
            _dbContext.SubscriptionCoupons.Remove(record);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.SubscriptionCoupons
            .AnyAsync(sc => sc.Id == id, cancellationToken);
    }
}
