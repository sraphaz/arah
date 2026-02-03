using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Interfaces;

public interface ISubscriptionPlanHistoryRepository
{
    Task<SubscriptionPlanHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<SubscriptionPlanHistory>> GetByPlanIdAsync(Guid planId, CancellationToken cancellationToken);
    Task AddAsync(SubscriptionPlanHistory history, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
