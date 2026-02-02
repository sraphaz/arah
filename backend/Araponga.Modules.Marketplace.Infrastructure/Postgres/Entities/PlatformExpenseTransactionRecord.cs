namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class PlatformExpenseTransactionRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid SellerTransactionId { get; set; }
    public long PayoutAmountInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? PayoutId { get; set; }
    public Guid? FinancialTransactionId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
