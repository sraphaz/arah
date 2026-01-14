using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryPlatformFeeConfigRepository : IPlatformFeeConfigRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryPlatformFeeConfigRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<PlatformFeeConfig?> GetActiveAsync(Guid territoryId, ItemType itemType, CancellationToken cancellationToken)
    {
        var config = _dataStore.PlatformFeeConfigs.FirstOrDefault(c =>
            c.TerritoryId == territoryId &&
            c.ItemType == itemType &&
            c.IsActive);

        return Task.FromResult(config);
    }

    public Task<IReadOnlyList<PlatformFeeConfig>> ListActiveAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var configs = _dataStore.PlatformFeeConfigs.Where(c => c.TerritoryId == territoryId && c.IsActive).ToList();
        return Task.FromResult<IReadOnlyList<PlatformFeeConfig>>(configs);
    }

    public Task<PlatformFeeConfig?> GetByIdAsync(Guid configId, CancellationToken cancellationToken)
    {
        var config = _dataStore.PlatformFeeConfigs.FirstOrDefault(c => c.Id == configId);
        return Task.FromResult(config);
    }

    public Task AddAsync(PlatformFeeConfig config, CancellationToken cancellationToken)
    {
        _dataStore.PlatformFeeConfigs.Add(config);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PlatformFeeConfig config, CancellationToken cancellationToken)
    {
        var index = _dataStore.PlatformFeeConfigs.FindIndex(c => c.Id == config.Id);
        if (index >= 0)
        {
            _dataStore.PlatformFeeConfigs[index] = config;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<PlatformFeeConfig>> ListActivePagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var configs = _dataStore.PlatformFeeConfigs
            .Where(c => c.TerritoryId == territoryId && c.IsActive)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<PlatformFeeConfig>>(configs);
    }

    public Task<int> CountActiveAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var count = _dataStore.PlatformFeeConfigs.Count(c => c.TerritoryId == territoryId && c.IsActive);
        return Task.FromResult(count);
    }
}
