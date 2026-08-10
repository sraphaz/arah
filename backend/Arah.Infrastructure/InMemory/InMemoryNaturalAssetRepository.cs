using Arah.Modules.Assets.Application.Interfaces;
using Arah.Modules.Assets.Domain;

namespace Arah.Infrastructure.InMemory;

public sealed class InMemoryNaturalAssetRepository : INaturalAssetRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryNaturalAssetRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<NaturalAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var asset = _dataStore.NaturalAssets.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(asset);
    }

    public Task<IReadOnlyList<NaturalAsset>> ListAsync(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types,
        CancellationToken cancellationToken)
    {
        var list = ApplyFilters(_dataStore.NaturalAssets.Where(a => a.TerritoryId == territoryId), status, types)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<NaturalAsset>>(list);
    }

    public Task<IReadOnlyList<NaturalAsset>> ListPagedAsync(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var list = ApplyFilters(_dataStore.NaturalAssets.Where(a => a.TerritoryId == territoryId), status, types)
            .OrderByDescending(a => a.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();
        return Task.FromResult<IReadOnlyList<NaturalAsset>>(list);
    }

    public Task<int> CountAsync(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types,
        CancellationToken cancellationToken)
    {
        var count = ApplyFilters(_dataStore.NaturalAssets.Where(a => a.TerritoryId == territoryId), status, types)
            .Count();
        return Task.FromResult(count);
    }

    public Task AddAsync(NaturalAsset asset, CancellationToken cancellationToken)
    {
        _dataStore.NaturalAssets.Add(asset);
        return Task.CompletedTask;
    }

    public Task<bool> UpdateAsync(NaturalAsset asset, CancellationToken cancellationToken)
    {
        var existing = _dataStore.NaturalAssets.FirstOrDefault(a => a.Id == asset.Id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        if (!ReferenceEquals(existing, asset))
        {
            _dataStore.NaturalAssets.Remove(existing);
            _dataStore.NaturalAssets.Add(asset);
        }

        return Task.FromResult(true);
    }

    private static IEnumerable<NaturalAsset> ApplyFilters(
        IEnumerable<NaturalAsset> query,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types)
    {
        if (status is not null)
        {
            query = query.Where(a => a.Status == status);
        }

        if (types is { Count: > 0 })
        {
            var normalized = types
                .Select(NaturalAssetType.Normalize)
                .Where(t => t is not null)
                .Cast<string>()
                .ToHashSet(StringComparer.Ordinal);
            if (normalized.Count > 0)
            {
                query = query.Where(a => normalized.Contains(a.Type));
            }
        }

        return query;
    }
}
