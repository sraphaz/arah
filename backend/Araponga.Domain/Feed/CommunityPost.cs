namespace Araponga.Domain.Feed;

public sealed class CommunityPost
{
    public CommunityPost(
        Guid id,
        Guid territoryId,
        Guid authorUserId,
        string title,
        string content,
        PostType type,
        PostVisibility visibility,
        PostStatus status,
        Guid? mapEntityId,
        DateTime createdAtUtc,
        string? referenceType = null,
        Guid? referenceId = null)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content is required.", nameof(content));
        }

        if (authorUserId == Guid.Empty)
        {
            throw new ArgumentException("Author user ID is required.", nameof(authorUserId));
        }

        Id = id;
        TerritoryId = territoryId;
        AuthorUserId = authorUserId;
        Title = title.Trim();
        Content = content.Trim();
        Type = type;
        Visibility = visibility;
        Status = status;
        MapEntityId = mapEntityId;
        ReferenceType = string.IsNullOrWhiteSpace(referenceType) ? null : referenceType.Trim();
        ReferenceId = referenceId;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid AuthorUserId { get; }
    public string Title { get; }
    public string Content { get; }
    public PostType Type { get; }
    public PostVisibility Visibility { get; }
    public PostStatus Status { get; }
    public Guid? MapEntityId { get; }
    public string? ReferenceType { get; }
    public Guid? ReferenceId { get; }
    public DateTime CreatedAtUtc { get; }
}
