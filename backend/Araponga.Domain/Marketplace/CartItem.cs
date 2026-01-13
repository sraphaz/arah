namespace Araponga.Domain.Marketplace;

public sealed class CartItem
{
    public CartItem(
        Guid id,
        Guid cartId,
        Guid listingId,
        int quantity,
        string? notes,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (cartId == Guid.Empty)
        {
            throw new ArgumentException("Cart ID is required.", nameof(cartId));
        }

        if (listingId == Guid.Empty)
        {
            throw new ArgumentException("Listing ID is required.", nameof(listingId));
        }

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be at least 1.");
        }

        Id = id;
        CartId = cartId;
        ListingId = listingId;
        Quantity = quantity;
        Notes = notes;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid CartId { get; }
    public Guid ListingId { get; }
    public int Quantity { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Update(int quantity, string? notes, DateTime updatedAtUtc)
    {
        Quantity = quantity;
        Notes = notes;
        UpdatedAtUtc = updatedAtUtc;
    }
}
