using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryTerritoryPayoutConfigRepository : ITerritoryPayoutConfigRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTerritoryPayoutConfigRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<TerritoryPayoutConfig?> GetActiveAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var config = _dataStore.TerritoryPayoutConfigs
            .FirstOrDefault(c => c.TerritoryId == territoryId && c.IsActive);
        return Task.FromResult(config);
    }

    public Task<TerritoryPayoutConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var config = _dataStore.TerritoryPayoutConfigs.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(config);
    }

    public Task<List<TerritoryPayoutConfig>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var configs = _dataStore.TerritoryPayoutConfigs
            .Where(c => c.TerritoryId == territoryId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToList();
        return Task.FromResult(configs);
    }

    public Task AddAsync(TerritoryPayoutConfig config, CancellationToken cancellationToken)
    {
        _dataStore.TerritoryPayoutConfigs.Add(config);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryPayoutConfig config, CancellationToken cancellationToken)
    {
        var index = _dataStore.TerritoryPayoutConfigs.FindIndex(c => c.Id == config.Id);
        if (index >= 0)
        {
            _dataStore.TerritoryPayoutConfigs[index] = config;
        }
        return Task.CompletedTask;
    }
}
