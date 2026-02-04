using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Araponga.Modules.Subscriptions.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Subscriptions.Infrastructure.Postgres;

public sealed class PostgresSubscriptionRepository : ISubscriptionRepository
{
    private readonly SubscriptionsDbContext _dbContext;

    public PostgresSubscriptionRepository(SubscriptionsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<Subscription?> GetByUserIdAsync(Guid userId, Guid? territoryId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.UserId == userId);

        if (territoryId.HasValue)
        {
            query = query.Where(s => s.TerritoryId == territoryId);
        }
        else
        {
            query = query.Where(s => s.TerritoryId == null);
        }

        var record = await query.FirstOrDefaultAsync(cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<Subscription>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Subscription>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.TerritoryId == territoryId)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Subscription>> ListAsync(
        Guid? userId,
        Guid? territoryId,
        SubscriptionStatus? status,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Subscriptions.AsNoTracking().AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(s => s.UserId == userId.Value);
        }

        if (territoryId.HasValue)
        {
            query = query.Where(s => s.TerritoryId == territoryId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == (int)status.Value);
        }

        var records = await query.ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Subscription>> GetActiveSubscriptionsAsync(CancellationToken cancellationToken)
    {
        var records = await _dbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.Status == (int)SubscriptionStatus.ACTIVE)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Subscription>> GetSubscriptionsExpiringSoonAsync(DateTime beforeDate, CancellationToken cancellationToken)
    {
        var records = await _dbContext.Subscriptions
            .AsNoTracking()
            .Where(s => s.CurrentPeriodEnd <= beforeDate && s.Status == (int)SubscriptionStatus.ACTIVE)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<Subscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeSubscriptionId, cancellationToken);
        return record?.ToDomain();
    }

    public Task AddAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        _dbContext.Subscriptions.Add(subscription.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == subscription.Id, cancellationToken);

        if (record == null)
        {
            throw new InvalidOperationException($"Subscription {subscription.Id} not found.");
        }

        record.UserId = subscription.UserId;
        record.TerritoryId = subscription.TerritoryId;
        record.PlanId = subscription.PlanId;
        record.Status = (int)subscription.Status;
        record.CurrentPeriodStart = subscription.CurrentPeriodStart;
        record.CurrentPeriodEnd = subscription.CurrentPeriodEnd;
        record.TrialStart = subscription.TrialStart;
        record.TrialEnd = subscription.TrialEnd;
        record.CanceledAt = subscription.CanceledAt;
        record.CancelAtPeriodEnd = subscription.CancelAtPeriodEnd;
        record.StripeSubscriptionId = subscription.StripeSubscriptionId;
        record.StripeCustomerId = subscription.StripeCustomerId;
        record.UpdatedAtUtc = subscription.UpdatedAtUtc;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Subscriptions
            .AnyAsync(s => s.Id == id, cancellationToken);
    }
}
