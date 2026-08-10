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

    Task UpdateAsync(NaturalAsset asset, CancellationToken cancellationToken);
}
