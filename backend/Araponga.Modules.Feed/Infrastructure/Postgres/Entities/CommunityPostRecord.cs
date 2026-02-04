using Araponga.Domain.Feed;

namespace Araponga.Modules.Feed.Infrastructure.Postgres.Entities;

public sealed class CommunityPostRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid AuthorUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public PostType Type { get; set; }
    public PostVisibility Visibility { get; set; }
    public PostStatus Status { get; set; }
    public Guid? MapEntityId { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? EditedAtUtc { get; set; }
    public int EditCount { get; set; }
    public string? TagsJson { get; set; } // JSON array de tags
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
