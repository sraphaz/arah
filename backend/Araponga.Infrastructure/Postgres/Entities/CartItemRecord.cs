namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class CartItemRecord
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
