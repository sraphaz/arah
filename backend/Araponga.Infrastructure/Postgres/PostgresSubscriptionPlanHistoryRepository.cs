using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresSubscriptionPlanHistoryRepository : ISubscriptionPlanHistoryRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresSubscriptionPlanHistoryRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubscriptionPlanHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SubscriptionPlanHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<SubscriptionPlanHistory>> GetByPlanIdAsync(Guid planId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SubscriptionPlanHistories
            .AsNoTracking()
            .Where(h => h.PlanId == planId)
            .OrderByDescending(h => h.ChangedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(SubscriptionPlanHistory history, CancellationToken cancellationToken)
    {
        _dbContext.SubscriptionPlanHistories.Add(history.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.SubscriptionPlanHistories
            .AnyAsync(h => h.Id == id, cancellationToken);
    }
}
