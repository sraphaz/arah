namespace Araponga.Domain.Marketplace;

public sealed class Checkout
{
    public Checkout(
        Guid id,
        Guid territoryId,
        Guid buyerUserId,
        Guid storeId,
        CheckoutStatus status,
        string currency,
        decimal? itemsSubtotalAmount,
        decimal? platformFeeAmount,
        decimal? totalAmount,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (buyerUserId == Guid.Empty)
        {
            throw new ArgumentException("Buyer user ID is required.", nameof(buyerUserId));
        }

        if (storeId == Guid.Empty)
        {
            throw new ArgumentException("Store ID is required.", nameof(storeId));
        }

        Id = id;
        TerritoryId = territoryId;
        BuyerUserId = buyerUserId;
        StoreId = storeId;
        Status = status;
        Currency = currency;
        ItemsSubtotalAmount = itemsSubtotalAmount;
        PlatformFeeAmount = platformFeeAmount;
        TotalAmount = totalAmount;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid BuyerUserId { get; }
    public Guid StoreId { get; }
    public CheckoutStatus Status { get; private set; }
    public string Currency { get; private set; }
    public decimal? ItemsSubtotalAmount { get; private set; }
    public decimal? PlatformFeeAmount { get; private set; }
    public decimal? TotalAmount { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void SetTotals(decimal itemsSubtotal, decimal platformFee, decimal totalAmount, DateTime updatedAtUtc)
    {
        ItemsSubtotalAmount = itemsSubtotal;
        PlatformFeeAmount = platformFee;
        TotalAmount = totalAmount;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void SetStatus(CheckoutStatus status, DateTime updatedAtUtc)
    {
        Status = status;
        UpdatedAtUtc = updatedAtUtc;
    }
}
