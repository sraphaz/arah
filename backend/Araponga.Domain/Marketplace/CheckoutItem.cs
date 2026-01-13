namespace Araponga.Domain.Marketplace;

public sealed class CheckoutItem
{
    public CheckoutItem(
        Guid id,
        Guid checkoutId,
        Guid listingId,
        ListingType listingType,
        string titleSnapshot,
        int quantity,
        decimal? unitPrice,
        decimal? lineSubtotal,
        decimal? platformFeeLine,
        decimal? lineTotal,
        DateTime createdAtUtc)
    {
        if (checkoutId == Guid.Empty)
        {
            throw new ArgumentException("Checkout ID is required.", nameof(checkoutId));
        }

        if (listingId == Guid.Empty)
        {
            throw new ArgumentException("Listing ID is required.", nameof(listingId));
        }

        if (string.IsNullOrWhiteSpace(titleSnapshot))
        {
            throw new ArgumentException("Title snapshot is required.", nameof(titleSnapshot));
        }

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be at least 1.");
        }

        Id = id;
        CheckoutId = checkoutId;
        ListingId = listingId;
        ListingType = listingType;
        TitleSnapshot = titleSnapshot.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineSubtotal = lineSubtotal;
        PlatformFeeLine = platformFeeLine;
        LineTotal = lineTotal;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid CheckoutId { get; }
    public Guid ListingId { get; }
    public ListingType ListingType { get; }
    public string TitleSnapshot { get; }
    public int Quantity { get; }
    public decimal? UnitPrice { get; }
    public decimal? LineSubtotal { get; }
    public decimal? PlatformFeeLine { get; }
    public decimal? LineTotal { get; }
    public DateTime CreatedAtUtc { get; }
}
