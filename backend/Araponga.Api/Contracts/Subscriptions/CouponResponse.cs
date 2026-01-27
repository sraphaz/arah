namespace Araponga.Api.Contracts.Subscriptions;

public sealed class CouponResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public int? MaxUses { get; set; }
    public int UsedCount { get; set; }
    public bool IsActive { get; set; }
}
