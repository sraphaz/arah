using Araponga.Domain.Feed;

namespace Araponga.Application.Interfaces;

public interface IPostAssetRepository
{
    Task AddAsync(IReadOnlyCollection<PostAsset> postAssets, CancellationToken cancellationToken);
    Task<IReadOnlyList<Guid>> ListPostIdsByAssetIdAsync(Guid assetId, CancellationToken cancellationToken);
}
