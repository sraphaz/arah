using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryPostAssetRepository : IPostAssetRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryPostAssetRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(IReadOnlyCollection<PostAsset> postAssets, CancellationToken cancellationToken)
    {
        _dataStore.PostAssets.AddRange(postAssets);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Guid>> ListPostIdsByAssetIdAsync(Guid assetId, CancellationToken cancellationToken)
    {
        var postIds = _dataStore.PostAssets
            .Where(postAsset => postAsset.AssetId == assetId)
            .Select(postAsset => postAsset.PostId)
            .Distinct()
            .ToList();
        return Task.FromResult<IReadOnlyList<Guid>>(postIds);
    }
}
