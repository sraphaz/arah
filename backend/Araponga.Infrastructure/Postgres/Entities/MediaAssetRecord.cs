using Araponga.Domain.Media;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class MediaAssetRecord
{
    public Guid Id { get; set; }
    public Guid UploadedByUserId { get; set; }
    public MediaType MediaType { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int? WidthPx { get; set; }
    public int? HeightPx { get; set; }
    public string Checksum { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public Guid? DeletedByUserId { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}