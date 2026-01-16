using Araponga.Domain.Marketplace;
using Araponga.Domain.Territories;

namespace Araponga.Domain.Financial;

/// <summary>
/// Transação de despesa da plataforma (payout processado).
/// </summary>
public sealed class PlatformExpenseTransaction
{
    public Guid Id { get; private set; }
    public Guid TerritoryId { get; private set; }
    public Guid SellerTransactionId { get; private set; }
    
    /// <summary>
    /// Valor do payout em centavos.
    /// </summary>
    public long PayoutAmountInCents { get; private set; }
    
    /// <summary>
    /// Moeda da transação (ex: "BRL", "USD").
    /// </summary>
    public string Currency { get; private set; }
    
    /// <summary>
    /// ID do payout no gateway.
    /// </summary>
    public string? PayoutId { get; private set; }
    
    /// <summary>
    /// ID da transação financeira relacionada.
    /// </summary>
    public Guid? FinancialTransactionId { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    
    // Navigation properties
    public Territory Territory { get; private set; } = null!;
    public SellerTransaction SellerTransaction { get; private set; } = null!;
    
    private PlatformExpenseTransaction() 
    {
        // Inicializar propriedades para EF Core
        Currency = string.Empty;
    } // EF Core
    
    public PlatformExpenseTransaction(
        Guid id,
        Guid territoryId,
        Guid sellerTransactionId,
        long payoutAmountInCents,
        string currency,
        string? payoutId = null)
    {
        Id = id;
        TerritoryId = territoryId;
        SellerTransactionId = sellerTransactionId;
        PayoutAmountInCents = payoutAmountInCents;
        Currency = currency;
        PayoutId = payoutId;
        CreatedAtUtc = DateTime.UtcNow;
    }
    
    public void SetFinancialTransactionId(Guid financialTransactionId)
    {
        FinancialTransactionId = financialTransactionId;
    }
}
