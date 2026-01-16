using Araponga.Domain.Media;

namespace Araponga.Application.Interfaces.Media;

/// <summary>
/// Repositório para operações com MediaAsset.
/// </summary>
public interface IMediaAssetRepository
{
    /// <summary>
    /// Cria um novo MediaAsset.
    /// </summary>
    Task AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um MediaAsset por ID.
    /// </summary>
    Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista MediaAssets por ID do usuário que fez upload.
    /// </summary>
    Task<IReadOnlyList<MediaAsset>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista MediaAssets por IDs.
    /// </summary>
    Task<IReadOnlyList<MediaAsset>> ListByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um MediaAsset existente.
    /// </summary>
    Task UpdateAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista MediaAssets deletados (soft delete).
    /// </summary>
    Task<IReadOnlyList<MediaAsset>> ListDeletedAsync(CancellationToken cancellationToken = default);
}