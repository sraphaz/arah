using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryTerritoryModerationRuleRepository : ITerritoryModerationRuleRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTerritoryModerationRuleRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(TerritoryModerationRule rule, CancellationToken cancellationToken)
    {
        _dataStore.TerritoryModerationRules.Add(rule);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryModerationRule rule, CancellationToken cancellationToken)
    {
        var index = _dataStore.TerritoryModerationRules.FindIndex(r => r.Id == rule.Id);
        if (index >= 0)
        {
            _dataStore.TerritoryModerationRules[index] = rule;
        }
        else
        {
            _dataStore.TerritoryModerationRules.Add(rule);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TerritoryModerationRule>> ListByTerritoryAsync(
        Guid territoryId,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        var rules = _dataStore.TerritoryModerationRules
            .Where(r => r.TerritoryId == territoryId);

        if (isActive.HasValue)
        {
            rules = rules.Where(r => r.IsActive == isActive.Value);
        }

        var result = rules
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<TerritoryModerationRule>>(result);
    }

    public Task<TerritoryModerationRule?> GetByIdAsync(Guid ruleId, CancellationToken cancellationToken)
    {
        var rule = _dataStore.TerritoryModerationRules.FirstOrDefault(r => r.Id == ruleId);
        return Task.FromResult(rule);
    }
}
