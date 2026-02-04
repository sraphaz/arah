namespace Araponga.Modules.Feed.Infrastructure.Postgres.Entities;

public sealed class PostLikeRecord
{
    public Guid PostId { get; set; }
    public string ActorId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
