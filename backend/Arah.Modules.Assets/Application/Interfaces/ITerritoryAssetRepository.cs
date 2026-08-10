using Arah.Modules.Assets.Domain;

namespace Arah.Modules.Assets.Application.Interfaces;

public interface ITerritoryAssetRepository
{
    Task<IReadOnlyList<TerritoryAsset>> ListAsync(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        CancellationToken cancellationToken,
        IReadOnlyCollection<string>? subtypes = null,
        IReadOnlyCollection<string>? typesOrSubtypes = null);

    Task<IReadOnlyList<TerritoryAsset>> ListByIdsAsync(IReadOnlyCollection<Guid> assetIds, CancellationToken cancellationToken);
    Task<TerritoryAsset?> GetByIdAsync(Guid assetId, CancellationToken cancellationToken);
    Task AddAsync(TerritoryAsset asset, CancellationToken cancellationToken);
    Task UpdateAsync(TerritoryAsset asset, CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists assets with pagination.
    /// </summary>
    Task<IReadOnlyList<TerritoryAsset>> ListPagedAsync(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        int skip,
        int take,
        CancellationToken cancellationToken,
        IReadOnlyCollection<string>? subtypes = null,
        IReadOnlyCollection<string>? typesOrSubtypes = null);
    
    /// <summary>
    /// Counts assets matching criteria.
    /// </summary>
    Task<int> CountAsync(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        CancellationToken cancellationToken,
        IReadOnlyCollection<string>? subtypes = null,
        IReadOnlyCollection<string>? typesOrSubtypes = null);
}
