using Araponga.Application.Interfaces;
using Araponga.Domain.Assets;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryAssetGeoAnchorRepository : IAssetGeoAnchorRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryAssetGeoAnchorRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<AssetGeoAnchor>> ListByAssetIdsAsync(
        IReadOnlyCollection<Guid> assetIds,
        CancellationToken cancellationToken)
    {
        var anchors = _dataStore.AssetGeoAnchors
            .Where(anchor => assetIds.Contains(anchor.AssetId))
            .ToList();

        return Task.FromResult<IReadOnlyList<AssetGeoAnchor>>(anchors);
    }

    public Task AddAsync(IReadOnlyCollection<AssetGeoAnchor> anchors, CancellationToken cancellationToken)
    {
        _dataStore.AssetGeoAnchors.AddRange(anchors);
        return Task.CompletedTask;
    }

    public Task ReplaceForAssetAsync(
        Guid assetId,
        IReadOnlyCollection<AssetGeoAnchor> anchors,
        CancellationToken cancellationToken)
    {
        _dataStore.AssetGeoAnchors.RemoveAll(anchor => anchor.AssetId == assetId);
        _dataStore.AssetGeoAnchors.AddRange(anchors);
        return Task.CompletedTask;
    }
}
