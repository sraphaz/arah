using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySubscriptionPlanRepository : ISubscriptionPlanRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySubscriptionPlanRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<SubscriptionPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var plan = _dataStore.SubscriptionPlans.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(plan);
    }

    public Task<IReadOnlyList<SubscriptionPlan>> GetGlobalPlansAsync(CancellationToken cancellationToken)
    {
        var plans = _dataStore.SubscriptionPlans
            .Where(p => p.Scope == PlanScope.Global && p.IsActive)
            .ToList();
        return Task.FromResult<IReadOnlyList<SubscriptionPlan>>(plans);
    }

    public Task<IReadOnlyList<SubscriptionPlan>> GetTerritoryPlansAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var plans = _dataStore.SubscriptionPlans
            .Where(p => p.Scope == PlanScope.Territory && p.TerritoryId == territoryId && p.IsActive)
            .ToList();
        return Task.FromResult<IReadOnlyList<SubscriptionPlan>>(plans);
    }

    public Task<IReadOnlyList<SubscriptionPlan>> GetPlansForTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        // Retorna planos globais + planos específicos do território
        var plans = _dataStore.SubscriptionPlans
            .Where(p => p.IsActive && (p.Scope == PlanScope.Global || (p.Scope == PlanScope.Territory && p.TerritoryId == territoryId)))
            .ToList();
        return Task.FromResult<IReadOnlyList<SubscriptionPlan>>(plans);
    }

    public Task<SubscriptionPlan?> GetDefaultPlanAsync(CancellationToken cancellationToken)
    {
        var plan = _dataStore.SubscriptionPlans.FirstOrDefault(p => p.IsDefault && p.Scope == PlanScope.Global && p.IsActive);
        return Task.FromResult(plan);
    }

    public Task<SubscriptionPlan?> GetDefaultPlanForTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        // Primeiro tenta encontrar um plano padrão específico do território
        var territoryPlan = _dataStore.SubscriptionPlans.FirstOrDefault(p => p.IsDefault && p.Scope == PlanScope.Territory && p.TerritoryId == territoryId && p.IsActive);
        if (territoryPlan != null)
        {
            return Task.FromResult<SubscriptionPlan?>(territoryPlan);
        }

        // Se não encontrar, retorna o plano padrão global
        return GetDefaultPlanAsync(cancellationToken);
    }

    public Task AddAsync(SubscriptionPlan plan, CancellationToken cancellationToken)
    {
        _dataStore.SubscriptionPlans.Add(plan);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(SubscriptionPlan plan, CancellationToken cancellationToken)
    {
        // In-memory: a referência já está na lista, então não precisa fazer nada
        // Mas vamos garantir que existe
        var existing = _dataStore.SubscriptionPlans.FirstOrDefault(p => p.Id == plan.Id);
        if (existing is null)
        {
            _dataStore.SubscriptionPlans.Add(plan);
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var exists = _dataStore.SubscriptionPlans.Any(p => p.Id == id);
        return Task.FromResult(exists);
    }
}
