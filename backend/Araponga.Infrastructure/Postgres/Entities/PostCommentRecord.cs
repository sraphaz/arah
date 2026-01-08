namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class PostCommentRecord
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
