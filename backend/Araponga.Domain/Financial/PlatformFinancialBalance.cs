using Araponga.Domain.Territories;

namespace Araponga.Domain.Financial;

/// <summary>
/// Saldo financeiro da plataforma por território.
/// </summary>
public sealed class PlatformFinancialBalance
{
    public Guid Id { get; private set; }
    public Guid TerritoryId { get; private set; }
    
    /// <summary>
    /// Total de receitas (fees coletadas) em centavos.
    /// </summary>
    public long TotalRevenueInCents { get; private set; }
    
    /// <summary>
    /// Total de despesas (payouts processados) em centavos.
    /// </summary>
    public long TotalExpensesInCents { get; private set; }
    
    /// <summary>
    /// Saldo líquido (receitas - despesas) em centavos.
    /// </summary>
    public long NetBalanceInCents { get; private set; }
    
    /// <summary>
    /// Moeda do saldo (ex: "BRL", "USD").
    /// </summary>
    public string Currency { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Navigation property
    public Territory Territory { get; private set; } = null!;
    
    private PlatformFinancialBalance() 
    {
        // Inicializar propriedades para EF Core
        Currency = string.Empty;
    } // EF Core
    
    public PlatformFinancialBalance(
        Guid id,
        Guid territoryId,
        string currency)
    {
        Id = id;
        TerritoryId = territoryId;
        Currency = currency;
        TotalRevenueInCents = 0;
        TotalExpensesInCents = 0;
        NetBalanceInCents = 0;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void AddRevenue(long amountInCents)
    {
        if (amountInCents < 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amountInCents));
        }
        
        TotalRevenueInCents += amountInCents;
        RecalculateNetBalance();
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void AddExpense(long amountInCents)
    {
        if (amountInCents < 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amountInCents));
        }
        
        TotalExpensesInCents += amountInCents;
        RecalculateNetBalance();
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    private void RecalculateNetBalance()
    {
        NetBalanceInCents = TotalRevenueInCents - TotalExpensesInCents;
    }
}
