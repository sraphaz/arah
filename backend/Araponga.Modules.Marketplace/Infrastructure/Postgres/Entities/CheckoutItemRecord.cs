using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class CheckoutItemRecord
{
    public Guid Id { get; set; }
    public Guid CheckoutId { get; set; }
    public Guid ItemId { get; set; }
    public ItemType ItemType { get; set; }
    public string TitleSnapshot { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineSubtotal { get; set; }
    public decimal? PlatformFeeLine { get; set; }
    public decimal? LineTotal { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
