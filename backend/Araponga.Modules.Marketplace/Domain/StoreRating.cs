namespace Araponga.Modules.Marketplace.Domain;

public sealed class StoreRating
{
    public StoreRating(
        Guid id,
        Guid storeId,
        Guid userId,
        int rating,
        string? comment,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (storeId == Guid.Empty)
        {
            throw new ArgumentException("Store ID is required.", nameof(storeId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (rating < 1 || rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));
        }

        if (comment is not null && comment.Length > 2000)
        {
            throw new ArgumentException("Comment must not exceed 2000 characters.", nameof(comment));
        }

        Id = id;
        StoreId = storeId;
        UserId = userId;
        Rating = rating;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid StoreId { get; }
    public Guid UserId { get; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Update(int rating, string? comment, DateTime updatedAtUtc)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));
        }

        if (comment is not null && comment.Length > 2000)
        {
            throw new ArgumentException("Comment must not exceed 2000 characters.", nameof(comment));
        }

        Rating = rating;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        UpdatedAtUtc = updatedAtUtc;
    }
}
