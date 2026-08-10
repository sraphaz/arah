using Arah.Modules.Assets.Domain;

namespace Arah.Modules.Assets.Application.Interfaces;

public interface INaturalAssetRepository
{
    Task<NaturalAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<NaturalAsset>> ListAsync(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<NaturalAsset>> ListPagedAsync(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types,
        int skip,
        int take,
        CancellationToken cancellationToken);

    Task<int> CountAsync(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types,
        CancellationToken cancellationToken);

    Task AddAsync(NaturalAsset asset, CancellationToken cancellationToken);

    /// <summary>Returns false when the asset id is not present in persistence.</summary>
    Task<bool> UpdateAsync(NaturalAsset asset, CancellationToken cancellationToken);
}
