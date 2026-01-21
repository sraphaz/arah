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
        Guid? referenceId = null,
        DateTime? editedAtUtc = null,
        int editCount = 0)
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
        EditedAtUtc = editedAtUtc;
        // Proteção contra valores que excedem int.MaxValue (pode vir do banco de dados)
        EditCount = editCount > int.MaxValue ? int.MaxValue : editCount;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid AuthorUserId { get; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public PostType Type { get; }
    public PostVisibility Visibility { get; }
    public PostStatus Status { get; }
    public Guid? MapEntityId { get; }
    public string? ReferenceType { get; }
    public Guid? ReferenceId { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? EditedAtUtc { get; private set; }
    public int EditCount { get; private set; }

    /// <summary>
    /// Edita o post, atualizando título, conteúdo e incrementando contador de edições.
    /// </summary>
    public void Edit(string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content is required.", nameof(content));
        }

        Title = title.Trim();
        Content = content.Trim();
        EditedAtUtc = DateTime.UtcNow;
        if (EditCount < int.MaxValue)
        {
            EditCount++;
        }
    }
}
