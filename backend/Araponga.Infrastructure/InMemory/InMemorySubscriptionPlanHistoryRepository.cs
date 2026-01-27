using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySubscriptionPlanHistoryRepository : ISubscriptionPlanHistoryRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySubscriptionPlanHistoryRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<SubscriptionPlanHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var history = _dataStore.SubscriptionPlanHistories.FirstOrDefault(h => h.Id == id);
        return Task.FromResult(history);
    }

    public Task<IReadOnlyList<SubscriptionPlanHistory>> GetByPlanIdAsync(Guid planId, CancellationToken cancellationToken)
    {
        var histories = _dataStore.SubscriptionPlanHistories
            .Where(h => h.PlanId == planId)
            .OrderByDescending(h => h.ChangedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<SubscriptionPlanHistory>>(histories);
    }

    public Task AddAsync(SubscriptionPlanHistory history, CancellationToken cancellationToken)
    {
        _dataStore.SubscriptionPlanHistories.Add(history);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var exists = _dataStore.SubscriptionPlanHistories.Any(h => h.Id == id);
        return Task.FromResult(exists);
    }
}
