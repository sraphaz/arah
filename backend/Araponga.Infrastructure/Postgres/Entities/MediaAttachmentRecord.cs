using Araponga.Domain.Media;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class MediaAttachmentRecord
{
    public Guid Id { get; set; }
    public Guid MediaAssetId { get; set; }
    public MediaOwnerType OwnerType { get; set; }
    public Guid OwnerId { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}