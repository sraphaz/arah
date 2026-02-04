namespace Araponga.Modules.Moderation.Domain.Evidence;

/// <summary>
/// Evidência documental (metadados) armazenada em um provedor de storage.
/// O conteúdo bruto do arquivo não é armazenado no domínio.
/// </summary>
public sealed class DocumentEvidence
{
    public const int MaxContentTypeLength = 200;
    public const int MaxOriginalFileNameLength = 300;
    public const int MaxStorageKeyLength = 500;
    public const int MaxSha256Length = 64;

    public DocumentEvidence(
        Guid id,
        Guid userId,
        Guid? territoryId,
        DocumentEvidenceKind kind,
        StorageProvider storageProvider,
        string storageKey,
        string contentType,
        long sizeBytes,
        string sha256,
        string? originalFileName,
        DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID is required.", nameof(id));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("StorageKey is required.", nameof(storageKey));
        var normalizedStorageKey = storageKey.Trim();
        if (normalizedStorageKey.Length > MaxStorageKeyLength)
            throw new ArgumentException($"StorageKey must not exceed {MaxStorageKeyLength} characters.", nameof(storageKey));
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("ContentType is required.", nameof(contentType));
        var normalizedContentType = contentType.Trim();
        if (normalizedContentType.Length > MaxContentTypeLength)
            throw new ArgumentException($"ContentType must not exceed {MaxContentTypeLength} characters.", nameof(contentType));
        if (sizeBytes <= 0)
            throw new ArgumentException("SizeBytes must be > 0.", nameof(sizeBytes));
        if (string.IsNullOrWhiteSpace(sha256))
            throw new ArgumentException("Sha256 is required.", nameof(sha256));
        var normalizedSha = sha256.Trim().ToLowerInvariant();
        if (normalizedSha.Length != MaxSha256Length)
            throw new ArgumentException("Sha256 must be a 64-hex string.", nameof(sha256));
        var normalizedFileName = string.IsNullOrWhiteSpace(originalFileName) ? null : originalFileName.Trim();
        if (normalizedFileName is not null && normalizedFileName.Length > MaxOriginalFileNameLength)
            throw new ArgumentException($"OriginalFileName must not exceed {MaxOriginalFileNameLength} characters.", nameof(originalFileName));
        if (kind == DocumentEvidenceKind.Residency && territoryId is null)
            throw new ArgumentException("TerritoryId is required for residency evidence.", nameof(territoryId));

        Id = id;
        UserId = userId;
        TerritoryId = territoryId;
        Kind = kind;
        StorageProvider = storageProvider;
        StorageKey = normalizedStorageKey;
        ContentType = normalizedContentType;
        SizeBytes = sizeBytes;
        Sha256 = normalizedSha;
        OriginalFileName = normalizedFileName;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid? TerritoryId { get; }
    public DocumentEvidenceKind Kind { get; }
    public StorageProvider StorageProvider { get; }
    public string StorageKey { get; }
    public string ContentType { get; }
    public long SizeBytes { get; }
    public string Sha256 { get; }
    public string? OriginalFileName { get; }
    public DateTime CreatedAtUtc { get; }
}
