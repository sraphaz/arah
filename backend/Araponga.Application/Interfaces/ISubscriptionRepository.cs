using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Subscription?> GetByUserIdAsync(Guid userId, Guid? territoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Subscription>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Subscription>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Subscription>> ListAsync(
        Guid? userId,
        Guid? territoryId,
        SubscriptionStatus? status,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Subscription>> GetActiveSubscriptionsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Subscription>> GetSubscriptionsExpiringSoonAsync(DateTime beforeDate, CancellationToken cancellationToken);
    Task<Subscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId, CancellationToken cancellationToken);
    Task AddAsync(Subscription subscription, CancellationToken cancellationToken);
    Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
