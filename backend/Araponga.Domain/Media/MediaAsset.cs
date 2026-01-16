namespace Araponga.Domain.Media;

/// <summary>
/// Representa um arquivo de mídia (imagem, vídeo, áudio, documento) armazenado no sistema.
/// MediaAssets são criados através de upload e podem ser associados a diferentes entidades via MediaAttachment.
/// </summary>
public sealed class MediaAsset
{
    public MediaAsset(
        Guid id,
        Guid uploadedByUserId,
        MediaType mediaType,
        string mimeType,
        string storageKey,
        long sizeBytes,
        int? widthPx,
        int? heightPx,
        string checksum,
        DateTime createdAtUtc,
        Guid? deletedByUserId,
        DateTime? deletedAtUtc)
    {
        if (uploadedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Uploaded by user ID is required.", nameof(uploadedByUserId));
        }

        if (string.IsNullOrWhiteSpace(mimeType))
        {
            throw new ArgumentException("MIME type is required.", nameof(mimeType));
        }

        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key is required.", nameof(storageKey));
        }

        if (sizeBytes < 0)
        {
            throw new ArgumentException("Size bytes must be non-negative.", nameof(sizeBytes));
        }

        if (widthPx.HasValue && widthPx.Value < 0)
        {
            throw new ArgumentException("Width pixels must be non-negative.", nameof(widthPx));
        }

        if (heightPx.HasValue && heightPx.Value < 0)
        {
            throw new ArgumentException("Height pixels must be non-negative.", nameof(heightPx));
        }

        if (string.IsNullOrWhiteSpace(checksum))
        {
            throw new ArgumentException("Checksum is required.", nameof(checksum));
        }

        Id = id;
        UploadedByUserId = uploadedByUserId;
        MediaType = mediaType;
        MimeType = mimeType;
        StorageKey = storageKey;
        SizeBytes = sizeBytes;
        WidthPx = widthPx;
        HeightPx = heightPx;
        Checksum = checksum;
        CreatedAtUtc = createdAtUtc;
        DeletedByUserId = deletedByUserId;
        DeletedAtUtc = deletedAtUtc;
    }

    public Guid Id { get; }

    /// <summary>
    /// ID do usuário que fez o upload da mídia.
    /// </summary>
    public Guid UploadedByUserId { get; }

    /// <summary>
    /// Tipo de mídia (Image, Video, Audio, Document).
    /// </summary>
    public MediaType MediaType { get; }

    /// <summary>
    /// Tipo MIME do arquivo (ex: "image/jpeg", "video/mp4").
    /// </summary>
    public string MimeType { get; }

    /// <summary>
    /// Chave única no sistema de armazenamento (caminho/URI do arquivo).
    /// </summary>
    public string StorageKey { get; }

    /// <summary>
    /// Tamanho do arquivo em bytes.
    /// </summary>
    public long SizeBytes { get; }

    /// <summary>
    /// Largura em pixels (apenas para imagens e vídeos).
    /// </summary>
    public int? WidthPx { get; }

    /// <summary>
    /// Altura em pixels (apenas para imagens e vídeos).
    /// </summary>
    public int? HeightPx { get; }

    /// <summary>
    /// Checksum do arquivo (SHA-256) para verificação de integridade.
    /// </summary>
    public string Checksum { get; }

    /// <summary>
    /// Data/hora UTC de criação do registro.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// ID do usuário que deletou a mídia (soft delete).
    /// </summary>
    public Guid? DeletedByUserId { get; private set; }

    /// <summary>
    /// Data/hora UTC de exclusão (soft delete).
    /// </summary>
    public DateTime? DeletedAtUtc { get; private set; }

    /// <summary>
    /// Indica se a mídia foi deletada (soft delete).
    /// </summary>
    public bool IsDeleted => DeletedAtUtc.HasValue;

    /// <summary>
    /// Realiza soft delete da mídia.
    /// </summary>
    public void Delete(Guid deletedByUserId, DateTime deletedAtUtc)
    {
        if (deletedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Deleted by user ID is required.", nameof(deletedByUserId));
        }

        if (IsDeleted)
        {
            throw new InvalidOperationException("Media asset is already deleted.");
        }

        DeletedByUserId = deletedByUserId;
        DeletedAtUtc = deletedAtUtc;
    }

    /// <summary>
    /// Restaura uma mídia deletada (soft delete revert).
    /// </summary>
    public void Restore()
    {
        if (!IsDeleted)
        {
            throw new InvalidOperationException("Media asset is not deleted.");
        }

        DeletedByUserId = null;
        DeletedAtUtc = null;
    }
}