namespace Araponga.Domain.Marketplace;

public sealed class ListingInquiry
{
    public ListingInquiry(
        Guid id,
        Guid territoryId,
        Guid listingId,
        Guid storeId,
        Guid fromUserId,
        string? message,
        InquiryStatus status,
        Guid? batchId,
        DateTime createdAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (listingId == Guid.Empty)
        {
            throw new ArgumentException("Listing ID is required.", nameof(listingId));
        }

        if (storeId == Guid.Empty)
        {
            throw new ArgumentException("Store ID is required.", nameof(storeId));
        }

        if (fromUserId == Guid.Empty)
        {
            throw new ArgumentException("From user ID is required.", nameof(fromUserId));
        }

        Id = id;
        TerritoryId = territoryId;
        ListingId = listingId;
        StoreId = storeId;
        FromUserId = fromUserId;
        Message = message;
        Status = status;
        BatchId = batchId;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid ListingId { get; }
    public Guid StoreId { get; }
    public Guid FromUserId { get; }
    public string? Message { get; }
    public InquiryStatus Status { get; private set; }
    public Guid? BatchId { get; }
    public DateTime CreatedAtUtc { get; }

    public void Close()
    {
        Status = InquiryStatus.Closed;
    }
}
