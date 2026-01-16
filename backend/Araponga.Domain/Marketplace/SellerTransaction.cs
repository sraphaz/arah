using Araponga.Domain.Territories;
using Araponga.Domain.Users;

namespace Araponga.Domain.Marketplace;

/// <summary>
/// Transação de vendedor (valor devido ao vendedor após checkout pago).
/// </summary>
public sealed class SellerTransaction
{
    public Guid Id { get; private set; }
    public Guid TerritoryId { get; private set; }
    public Guid StoreId { get; private set; }
    public Guid CheckoutId { get; private set; }
    public Guid SellerUserId { get; private set; }
    
    /// <summary>
    /// Valor bruto do checkout em centavos.
    /// </summary>
    public long GrossAmountInCents { get; private set; }
    
    /// <summary>
    /// Valor das fees da plataforma em centavos.
    /// </summary>
    public long PlatformFeeInCents { get; private set; }
    
    /// <summary>
    /// Valor líquido devido ao vendedor em centavos (GrossAmount - PlatformFee).
    /// </summary>
    public long NetAmountInCents { get; private set; }
    
    /// <summary>
    /// Moeda da transação (ex: "BRL", "USD").
    /// </summary>
    public string Currency { get; private set; }
    
    public SellerTransactionStatus Status { get; private set; }
    
    /// <summary>
    /// ID do payout no gateway (se já processado).
    /// </summary>
    public string? PayoutId { get; private set; }
    
    /// <summary>
    /// Data em que ficou pronto para payout (após período de retenção).
    /// </summary>
    public DateTime? ReadyForPayoutAtUtc { get; private set; }
    
    /// <summary>
    /// Data em que o payout foi processado.
    /// </summary>
    public DateTime? PaidAtUtc { get; private set; }
    
    /// <summary>
    /// ID da transação financeira relacionada.
    /// </summary>
    public Guid? FinancialTransactionId { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Navigation properties
    public Territory Territory { get; private set; } = null!;
    public Store Store { get; private set; } = null!;
    public Checkout Checkout { get; private set; } = null!;
    public User SellerUser { get; private set; } = null!;
    
    private SellerTransaction() 
    {
        // Inicializar propriedades para EF Core
        Currency = string.Empty;
    } // EF Core
    
    public SellerTransaction(
        Guid id,
        Guid territoryId,
        Guid storeId,
        Guid checkoutId,
        Guid sellerUserId,
        long grossAmountInCents,
        long platformFeeInCents,
        string currency)
    {
        Id = id;
        TerritoryId = territoryId;
        StoreId = storeId;
        CheckoutId = checkoutId;
        SellerUserId = sellerUserId;
        GrossAmountInCents = grossAmountInCents;
        PlatformFeeInCents = platformFeeInCents;
        NetAmountInCents = grossAmountInCents - platformFeeInCents;
        Currency = currency;
        Status = SellerTransactionStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void MarkAsReadyForPayout()
    {
        if (Status != SellerTransactionStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot mark as ready for payout. Current status: {Status}");
        }
        
        Status = SellerTransactionStatus.ReadyForPayout;
        ReadyForPayoutAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void StartPayout(string payoutId)
    {
        if (Status != SellerTransactionStatus.ReadyForPayout)
        {
            throw new InvalidOperationException($"Cannot start payout. Current status: {Status}");
        }
        
        Status = SellerTransactionStatus.ProcessingPayout;
        PayoutId = payoutId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void CompletePayout()
    {
        if (Status != SellerTransactionStatus.ProcessingPayout)
        {
            throw new InvalidOperationException($"Cannot complete payout. Current status: {Status}");
        }
        
        Status = SellerTransactionStatus.Paid;
        PaidAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void FailPayout()
    {
        if (Status != SellerTransactionStatus.ProcessingPayout)
        {
            throw new InvalidOperationException($"Cannot fail payout. Current status: {Status}");
        }
        
        Status = SellerTransactionStatus.Failed;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void Cancel()
    {
        if (Status == SellerTransactionStatus.Paid)
        {
            throw new InvalidOperationException("Cannot cancel a paid transaction");
        }
        
        Status = SellerTransactionStatus.Canceled;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void SetFinancialTransactionId(Guid financialTransactionId)
    {
        FinancialTransactionId = financialTransactionId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
