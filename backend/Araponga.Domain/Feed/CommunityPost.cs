namespace Araponga.Domain.Feed;

public sealed class CommunityPost
{
    public CommunityPost(
        Guid id,
        Guid territoryId,
        string title,
        string content,
        PostVisibility visibility,
        DateTime createdAtUtc)
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

        Id = id;
        TerritoryId = territoryId;
        Title = title.Trim();
        Content = content.Trim();
        Visibility = visibility;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public string Title { get; }
    public string Content { get; }
    public PostVisibility Visibility { get; }
    public DateTime CreatedAtUtc { get; }
}
