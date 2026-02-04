namespace Araponga.Domain.Feed;

public sealed class PostComment
{
    public PostComment(
        Guid id,
        Guid postId,
        Guid userId,
        string content,
        DateTime createdAtUtc)
    {
        if (postId == Guid.Empty)
        {
            throw new ArgumentException("Post ID is required.", nameof(postId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content is required.", nameof(content));
        }

        Id = id;
        PostId = postId;
        UserId = userId;
        Content = content.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid PostId { get; }
    public Guid UserId { get; }
    public string Content { get; }
    public DateTime CreatedAtUtc { get; }
}
