namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class PlatformRevenueTransactionRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid CheckoutId { get; set; }
    public long FeeAmountInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public Guid? FinancialTransactionId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
