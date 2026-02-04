using Araponga.Modules.Assets.Domain;

namespace Araponga.Modules.Assets.Application.Interfaces;

public interface IAssetValidationRepository
{
    Task<bool> ExistsAsync(Guid assetId, Guid userId, CancellationToken cancellationToken);
    Task AddAsync(AssetValidation validation, CancellationToken cancellationToken);
    Task<int> CountByAssetIdAsync(Guid assetId, CancellationToken cancellationToken);
    Task<IReadOnlyDictionary<Guid, int>> CountByAssetIdsAsync(IReadOnlyCollection<Guid> assetIds, CancellationToken cancellationToken);
}
