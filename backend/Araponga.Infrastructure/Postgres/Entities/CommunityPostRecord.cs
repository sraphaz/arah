using Araponga.Domain.Feed;

namespace Araponga.Infrastructure.Postgres.Entities;

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
    public DateTime CreatedAtUtc { get; set; }
}
