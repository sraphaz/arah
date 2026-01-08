using Araponga.Domain.Feed;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class CommunityPostRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public PostType Type { get; set; }
    public PostVisibility Visibility { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
