using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySubscriptionRepository : ISubscriptionRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySubscriptionRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var subscription = _dataStore.Subscriptions.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(subscription);
    }

    public Task<Subscription?> GetByUserIdAsync(Guid userId, Guid? territoryId, CancellationToken cancellationToken)
    {
        var subscription = _dataStore.Subscriptions.FirstOrDefault(s =>
            s.UserId == userId &&
            s.TerritoryId == territoryId &&
            (s.Status == SubscriptionStatus.ACTIVE || s.Status == SubscriptionStatus.TRIALING));
        return Task.FromResult(subscription);
    }

    public Task<IReadOnlyList<Subscription>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var subscriptions = _dataStore.Subscriptions
            .Where(s => s.UserId == userId)
            .ToList();
        return Task.FromResult<IReadOnlyList<Subscription>>(subscriptions);
    }

    public Task<IReadOnlyList<Subscription>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var subscriptions = _dataStore.Subscriptions
            .Where(s => s.TerritoryId == territoryId)
            .ToList();
        return Task.FromResult<IReadOnlyList<Subscription>>(subscriptions);
    }

    public Task<IReadOnlyList<Subscription>> ListAsync(
        Guid? userId,
        Guid? territoryId,
        SubscriptionStatus? status,
        CancellationToken cancellationToken)
    {
        var subscriptions = _dataStore.Subscriptions.AsEnumerable();

        if (userId.HasValue)
        {
            subscriptions = subscriptions.Where(s => s.UserId == userId.Value);
        }

        if (territoryId.HasValue)
        {
            subscriptions = subscriptions.Where(s => s.TerritoryId == territoryId.Value);
        }

        if (status.HasValue)
        {
            subscriptions = subscriptions.Where(s => s.Status == status.Value);
        }

        return Task.FromResult<IReadOnlyList<Subscription>>(subscriptions.ToList());
    }

    public Task<IReadOnlyList<Subscription>> GetActiveSubscriptionsAsync(CancellationToken cancellationToken)
    {
        var subscriptions = _dataStore.Subscriptions
            .Where(s => s.Status == SubscriptionStatus.ACTIVE || s.Status == SubscriptionStatus.TRIALING)
            .ToList();
        return Task.FromResult<IReadOnlyList<Subscription>>(subscriptions);
    }

    public Task<IReadOnlyList<Subscription>> GetSubscriptionsExpiringSoonAsync(DateTime beforeDate, CancellationToken cancellationToken)
    {
        var subscriptions = _dataStore.Subscriptions
            .Where(s => s.CurrentPeriodEnd <= beforeDate &&
                       (s.Status == SubscriptionStatus.ACTIVE || s.Status == SubscriptionStatus.TRIALING))
            .ToList();
        return Task.FromResult<IReadOnlyList<Subscription>>(subscriptions);
    }

    public Task<Subscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId, CancellationToken cancellationToken)
    {
        var subscription = _dataStore.Subscriptions.FirstOrDefault(s => s.StripeSubscriptionId == stripeSubscriptionId);
        return Task.FromResult(subscription);
    }

    public Task AddAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        _dataStore.Subscriptions.Add(subscription);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        // In-memory: a referência já está na lista, então não precisa fazer nada
        // Mas vamos garantir que existe
        var existing = _dataStore.Subscriptions.FirstOrDefault(s => s.Id == subscription.Id);
        if (existing is null)
        {
            _dataStore.Subscriptions.Add(subscription);
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var exists = _dataStore.Subscriptions.Any(s => s.Id == id);
        return Task.FromResult(exists);
    }
}
