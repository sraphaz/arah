using Araponga.Domain.Subscriptions;

namespace Araponga.Application.Interfaces;

public interface ISubscriptionPlanRepository
{
    Task<SubscriptionPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<SubscriptionPlan>> GetGlobalPlansAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<SubscriptionPlan>> GetTerritoryPlansAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<SubscriptionPlan>> GetPlansForTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<SubscriptionPlan?> GetDefaultPlanAsync(CancellationToken cancellationToken);
    Task<SubscriptionPlan?> GetDefaultPlanForTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
    Task AddAsync(SubscriptionPlan plan, CancellationToken cancellationToken);
    Task UpdateAsync(SubscriptionPlan plan, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
