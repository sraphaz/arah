namespace Araponga.Domain.Marketplace;

public sealed class StoreRatingResponse
{
    public StoreRatingResponse(
        Guid id,
        Guid ratingId,
        Guid storeId,
        string responseText,
        DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (ratingId == Guid.Empty)
        {
            throw new ArgumentException("Rating ID is required.", nameof(ratingId));
        }

        if (storeId == Guid.Empty)
        {
            throw new ArgumentException("Store ID is required.", nameof(storeId));
        }

        if (string.IsNullOrWhiteSpace(responseText))
        {
            throw new ArgumentException("Response text is required.", nameof(responseText));
        }

        if (responseText.Length > 2000)
        {
            throw new ArgumentException("Response text must not exceed 2000 characters.", nameof(responseText));
        }

        Id = id;
        RatingId = ratingId;
        StoreId = storeId;
        ResponseText = responseText.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid RatingId { get; }
    public Guid StoreId { get; }
    public string ResponseText { get; }
    public DateTime CreatedAtUtc { get; }
}
