using Araponga.Modules.Assets.Domain;

namespace Araponga.Modules.Assets.Application.Interfaces;

public interface IAssetGeoAnchorRepository
{
    Task<IReadOnlyList<AssetGeoAnchor>> ListByAssetIdsAsync(IReadOnlyCollection<Guid> assetIds, CancellationToken cancellationToken);
    Task AddAsync(IReadOnlyCollection<AssetGeoAnchor> anchors, CancellationToken cancellationToken);
    Task ReplaceForAssetAsync(Guid assetId, IReadOnlyCollection<AssetGeoAnchor> anchors, CancellationToken cancellationToken);
}
