using Araponga.Domain.Media;

namespace Araponga.Application.Interfaces.Media;

/// <summary>
/// Repositório para operações com MediaAttachment.
/// </summary>
public interface IMediaAttachmentRepository
{
    /// <summary>
    /// Cria um novo MediaAttachment.
    /// </summary>
    Task AddAsync(MediaAttachment attachment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um MediaAttachment por ID.
    /// </summary>
    Task<MediaAttachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista MediaAttachments por entidade proprietária (OwnerType + OwnerId).
    /// </summary>
    Task<IReadOnlyList<MediaAttachment>> ListByOwnerAsync(MediaOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista MediaAttachments por MediaAssetId.
    /// </summary>
    Task<IReadOnlyList<MediaAttachment>> ListByMediaAssetIdAsync(Guid mediaAssetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista MediaAttachments por múltiplos proprietários.
    /// </summary>
    Task<IReadOnlyList<MediaAttachment>> ListByOwnersAsync(MediaOwnerType ownerType, IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um MediaAttachment existente.
    /// </summary>
    Task UpdateAsync(MediaAttachment attachment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta um MediaAttachment.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta todos os MediaAttachments de uma entidade proprietária.
    /// </summary>
    Task DeleteByOwnerAsync(MediaOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta todos os MediaAttachments de um MediaAsset.
    /// </summary>
    Task DeleteByMediaAssetIdAsync(Guid mediaAssetId, CancellationToken cancellationToken = default);
}