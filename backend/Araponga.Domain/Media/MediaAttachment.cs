namespace Araponga.Domain.Media;

/// <summary>
/// Representa a associação de uma mídia (MediaAsset) a uma entidade do sistema (User, Post, Event, StoreItem, ChatMessage).
/// Uma mídia pode estar associada a múltiplas entidades, e uma entidade pode ter múltiplas mídias associadas.
/// </summary>
public sealed class MediaAttachment
{
    public MediaAttachment(
        Guid id,
        Guid mediaAssetId,
        MediaOwnerType ownerType,
        Guid ownerId,
        int displayOrder,
        DateTime createdAtUtc)
    {
        if (mediaAssetId == Guid.Empty)
        {
            throw new ArgumentException("Media asset ID is required.", nameof(mediaAssetId));
        }

        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Owner ID is required.", nameof(ownerId));
        }

        if (displayOrder < 0)
        {
            throw new ArgumentException("Display order must be non-negative.", nameof(displayOrder));
        }

        Id = id;
        MediaAssetId = mediaAssetId;
        OwnerType = ownerType;
        OwnerId = ownerId;
        DisplayOrder = displayOrder;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }

    /// <summary>
    /// ID da mídia associada (MediaAsset).
    /// </summary>
    public Guid MediaAssetId { get; }

    /// <summary>
    /// Tipo da entidade proprietária (User, Post, Event, StoreItem, ChatMessage).
    /// </summary>
    public MediaOwnerType OwnerType { get; }

    /// <summary>
    /// ID da entidade proprietária.
    /// </summary>
    public Guid OwnerId { get; }

    /// <summary>
    /// Ordem de exibição (quando múltiplas mídias estão associadas à mesma entidade).
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Data/hora UTC de criação da associação.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Atualiza a ordem de exibição.
    /// </summary>
    public void UpdateDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentException("Display order must be non-negative.", nameof(displayOrder));
        }

        DisplayOrder = displayOrder;
    }
}