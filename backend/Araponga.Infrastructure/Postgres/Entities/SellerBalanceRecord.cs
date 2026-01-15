namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class SellerBalanceRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid SellerUserId { get; set; }
    public long PendingAmountInCents { get; set; }
    public long ReadyForPayoutAmountInCents { get; set; }
    public long PaidAmountInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
