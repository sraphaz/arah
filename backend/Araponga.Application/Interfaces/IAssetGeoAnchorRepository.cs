using Araponga.Domain.Assets;

namespace Araponga.Application.Interfaces;

public interface IAssetGeoAnchorRepository
{
    Task<IReadOnlyList<AssetGeoAnchor>> ListByAssetIdsAsync(IReadOnlyCollection<Guid> assetIds, CancellationToken cancellationToken);
    Task AddAsync(IReadOnlyCollection<AssetGeoAnchor> anchors, CancellationToken cancellationToken);
    Task ReplaceForAssetAsync(Guid assetId, IReadOnlyCollection<AssetGeoAnchor> anchors, CancellationToken cancellationToken);
}
