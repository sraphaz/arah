namespace Araponga.Modules.Feed.Infrastructure.Postgres.Entities;

public sealed class PostShareRecord
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
