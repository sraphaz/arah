using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class CheckoutRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid BuyerUserId { get; set; }
    public Guid StoreId { get; set; }
    public CheckoutStatus Status { get; set; }
    public string Currency { get; set; } = "BRL";
    public decimal? ItemsSubtotalAmount { get; set; }
    public decimal? PlatformFeeAmount { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
