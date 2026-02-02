using Araponga.Domain.Marketplace;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class StoreItemRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid StoreId { get; set; }
    public ItemType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public ItemPricingType PricingType { get; set; }
    public decimal? PriceAmount { get; set; }
    public string? Currency { get; set; }
    public string? Unit { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public ItemStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
