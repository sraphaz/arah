using Arah.Application.Interfaces;
using Arah.Domain.Financial;
using Arah.Infrastructure.InMemory;

namespace Arah.Infrastructure.InMemory;

public sealed class InMemoryFeeSplitRuleRepository : IFeeSplitRuleRepository
{
    private readonly InMemoryDataStore _store;

    public InMemoryFeeSplitRuleRepository(InMemoryDataStore store)
    {
        _store = store;
    }

    public Task<FeeSplitRule?> GetActiveAsync(
        Guid territoryId,
        string revenueType,
        DateTimeOffset atUtc,
        CancellationToken cancellationToken)
    {
        var match = _store.FeeSplitRules
            .Where(r =>
                r.TerritoryId == territoryId &&
                string.Equals(r.RevenueType, revenueType, StringComparison.OrdinalIgnoreCase) &&
                r.IsActiveAt(atUtc))
            .OrderByDescending(r => r.EffectiveFromUtc)
            .FirstOrDefault();

        return Task.FromResult(match);
    }

    public Task AddAsync(FeeSplitRule rule, CancellationToken cancellationToken)
    {
        _store.FeeSplitRules.Add(rule);
        return Task.CompletedTask;
    }

    public Task SupersedeAsync(Guid ruleId, DateTimeOffset supersededAtUtc, CancellationToken cancellationToken)
    {
        var rule = _store.FeeSplitRules.FirstOrDefault(r => r.Id == ruleId);
        if (rule is null)
        {
            throw new InvalidOperationException($"FeeSplitRule {ruleId} not found.");
        }

        rule.Supersede(supersededAtUtc);
        return Task.CompletedTask;
    }
}
