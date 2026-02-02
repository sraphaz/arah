namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class PlatformFinancialBalanceRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public long TotalRevenueInCents { get; set; }
    public long TotalExpensesInCents { get; set; }
    public long NetBalanceInCents { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
