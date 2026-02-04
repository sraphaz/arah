using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class SellerTransactionRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid StoreId { get; set; }
    public Guid CheckoutId { get; set; }
    public Guid SellerUserId { get; set; }
    public long GrossAmountInCents { get; set; }
    public long PlatformFeeInCents { get; set; }
    public long NetAmountInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Status { get; set; } // SellerTransactionStatus enum
    public string? PayoutId { get; set; }
    public DateTime? ReadyForPayoutAtUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public Guid? FinancialTransactionId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
