using Araponga.Modules.Moderation.Domain.Evidence;

namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class DocumentEvidenceRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TerritoryId { get; set; }
    public DocumentEvidenceKind Kind { get; set; }
    public StorageProvider StorageProvider { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Sha256 { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
