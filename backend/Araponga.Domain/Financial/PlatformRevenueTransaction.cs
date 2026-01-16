using Araponga.Domain.Marketplace;
using Araponga.Domain.Territories;

namespace Araponga.Domain.Financial;

/// <summary>
/// Transação de receita da plataforma (fee coletada).
/// </summary>
public sealed class PlatformRevenueTransaction
{
    public Guid Id { get; private set; }
    public Guid TerritoryId { get; private set; }
    public Guid CheckoutId { get; private set; }
    
    /// <summary>
    /// Valor da fee em centavos.
    /// </summary>
    public long FeeAmountInCents { get; private set; }
    
    /// <summary>
    /// Moeda da transação (ex: "BRL", "USD").
    /// </summary>
    public string Currency { get; private set; }
    
    /// <summary>
    /// ID da transação financeira relacionada.
    /// </summary>
    public Guid? FinancialTransactionId { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    
    // Navigation properties
    public Territory Territory { get; private set; } = null!;
    public Checkout Checkout { get; private set; } = null!;
    
    private PlatformRevenueTransaction() 
    {
        // Inicializar propriedades para EF Core
        Currency = string.Empty;
    } // EF Core
    
    public PlatformRevenueTransaction(
        Guid id,
        Guid territoryId,
        Guid checkoutId,
        long feeAmountInCents,
        string currency)
    {
        Id = id;
        TerritoryId = territoryId;
        CheckoutId = checkoutId;
        FeeAmountInCents = feeAmountInCents;
        Currency = currency;
        CreatedAtUtc = DateTime.UtcNow;
    }
    
    public void SetFinancialTransactionId(Guid financialTransactionId)
    {
        FinancialTransactionId = financialTransactionId;
    }
}
