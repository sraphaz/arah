using Arah.Modules.Assets.Application.Interfaces;
using Arah.Modules.Assets.Domain;

namespace Arah.Infrastructure.InMemory;

public sealed class InMemoryAssetRepository : ITerritoryAssetRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryAssetRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<TerritoryAsset>> ListAsync(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        CancellationToken cancellationToken,
        IReadOnlyCollection<string>? subtypes = null)
    {
        var query = ApplyFilters(
            _dataStore.TerritoryAssets.Where(asset => asset.TerritoryId == territoryId),
            assetId,
            types,
            status,
            search,
            subtypes);

        return Task.FromResult<IReadOnlyList<TerritoryAsset>>(query.ToList());
    }

    public Task<IReadOnlyList<TerritoryAsset>> ListByIdsAsync(
        IReadOnlyCollection<Guid> assetIds,
        CancellationToken cancellationToken)
    {
        var assets = _dataStore.TerritoryAssets
            .Where(asset => assetIds.Contains(asset.Id))
            .ToList();

        return Task.FromResult<IReadOnlyList<TerritoryAsset>>(assets);
    }

    public Task<TerritoryAsset?> GetByIdAsync(Guid assetId, CancellationToken cancellationToken)
    {
        var asset = _dataStore.TerritoryAssets.FirstOrDefault(a => a.Id == assetId);
        return Task.FromResult(asset);
    }

    public Task AddAsync(TerritoryAsset asset, CancellationToken cancellationToken)
    {
        _dataStore.TerritoryAssets.Add(asset);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryAsset asset, CancellationToken cancellationToken)
    {
        var existing = _dataStore.TerritoryAssets.FirstOrDefault(a => a.Id == asset.Id);
        if (existing is null)
        {
            return Task.CompletedTask;
        }

        _dataStore.TerritoryAssets.Remove(existing);
        _dataStore.TerritoryAssets.Add(asset);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TerritoryAsset>> ListPagedAsync(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        int skip,
        int take,
        CancellationToken cancellationToken,
        IReadOnlyCollection<string>? subtypes = null)
    {
        var query = ApplyFilters(
            _dataStore.TerritoryAssets.Where(asset => asset.TerritoryId == territoryId),
            assetId,
            types,
            status,
            search,
            subtypes);

        var result = query
            .OrderByDescending(asset => asset.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<TerritoryAsset>>(result);
    }

    public Task<int> CountAsync(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        CancellationToken cancellationToken,
        IReadOnlyCollection<string>? subtypes = null)
    {
        var query = ApplyFilters(
            _dataStore.TerritoryAssets.Where(asset => asset.TerritoryId == territoryId),
            assetId,
            types,
            status,
            search,
            subtypes);

        const int maxInt32 = int.MaxValue;
        var count = query.Count();
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }

    private static IEnumerable<TerritoryAsset> ApplyFilters(
        IEnumerable<TerritoryAsset> query,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        IReadOnlyCollection<string>? subtypes)
    {
        var normalizedTypes = TerritoryAssetTypeMatch.NormalizeFilter(types);
        var normalizedSubtypes = TerritoryAssetTypeMatch.NormalizeFilter(subtypes);

        if (assetId is not null)
        {
            query = query.Where(asset => asset.Id == assetId.Value);
        }

        if (normalizedTypes is not null || normalizedSubtypes is not null)
        {
            query = query.Where(asset =>
                TerritoryAssetTypeMatch.Matches(asset.Type, asset.Subtype, normalizedTypes, normalizedSubtypes));
        }

        if (status is not null)
        {
            query = query.Where(asset => asset.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(asset =>
                asset.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (asset.Description is not null && asset.Description.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        return query;
    }
}
