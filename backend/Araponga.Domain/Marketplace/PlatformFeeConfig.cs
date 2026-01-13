namespace Araponga.Domain.Marketplace;

public sealed class PlatformFeeConfig
{
    public PlatformFeeConfig(
        Guid id,
        Guid territoryId,
        ListingType listingType,
        PlatformFeeMode feeMode,
        decimal feeValue,
        string? currency,
        bool isActive,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (feeValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(feeValue), "Fee value must be non-negative.");
        }

        Id = id;
        TerritoryId = territoryId;
        ListingType = listingType;
        FeeMode = feeMode;
        FeeValue = feeValue;
        Currency = currency;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public ListingType ListingType { get; }
    public PlatformFeeMode FeeMode { get; private set; }
    public decimal FeeValue { get; private set; }
    public string? Currency { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Update(PlatformFeeMode feeMode, decimal feeValue, string? currency, bool isActive, DateTime updatedAtUtc)
    {
        FeeMode = feeMode;
        FeeValue = feeValue;
        Currency = currency;
        IsActive = isActive;
        UpdatedAtUtc = updatedAtUtc;
    }
}
