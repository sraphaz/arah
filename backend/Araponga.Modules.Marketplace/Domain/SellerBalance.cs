using Araponga.Domain.Territories;
using Araponga.Domain.Users;

namespace Araponga.Modules.Marketplace.Domain;

/// <summary>
/// Saldo de vendedor por território.
/// </summary>
public sealed class SellerBalance
{
    public Guid Id { get; private set; }
    public Guid TerritoryId { get; private set; }
    public Guid SellerUserId { get; private set; }
    
    /// <summary>
    /// Valor pendente (aguardando período de retenção ou valor mínimo) em centavos.
    /// </summary>
    public long PendingAmountInCents { get; private set; }
    
    /// <summary>
    /// Valor pronto para payout em centavos.
    /// </summary>
    public long ReadyForPayoutAmountInCents { get; private set; }
    
    /// <summary>
    /// Valor total pago (já transferido) em centavos.
    /// </summary>
    public long PaidAmountInCents { get; private set; }
    
    /// <summary>
    /// Moeda do saldo (ex: "BRL", "USD").
    /// </summary>
    public string Currency { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Navigation properties
    public Territory Territory { get; private set; } = null!;
    public User SellerUser { get; private set; } = null!;
    
    private SellerBalance() 
    {
        // Inicializar propriedades para EF Core
        Currency = string.Empty;
    } // EF Core
    
    public SellerBalance(
        Guid id,
        Guid territoryId,
        Guid sellerUserId,
        string currency)
    {
        Id = id;
        TerritoryId = territoryId;
        SellerUserId = sellerUserId;
        Currency = currency;
        PendingAmountInCents = 0;
        ReadyForPayoutAmountInCents = 0;
        PaidAmountInCents = 0;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void AddPendingAmount(long amountInCents)
    {
        if (amountInCents < 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amountInCents));
        }
        
        PendingAmountInCents += amountInCents;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void MoveToReadyForPayout(long amountInCents)
    {
        if (amountInCents < 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amountInCents));
        }
        
        if (amountInCents > PendingAmountInCents)
        {
            throw new InvalidOperationException($"Cannot move {amountInCents} to ready for payout. Pending amount: {PendingAmountInCents}");
        }
        
        PendingAmountInCents -= amountInCents;
        ReadyForPayoutAmountInCents += amountInCents;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void MarkAsPaid(long amountInCents)
    {
        if (amountInCents < 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amountInCents));
        }
        
        if (amountInCents > ReadyForPayoutAmountInCents)
        {
            throw new InvalidOperationException($"Cannot mark {amountInCents} as paid. Ready for payout amount: {ReadyForPayoutAmountInCents}");
        }
        
        ReadyForPayoutAmountInCents -= amountInCents;
        PaidAmountInCents += amountInCents;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void CancelPendingAmount(long amountInCents)
    {
        if (amountInCents < 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amountInCents));
        }
        
        if (amountInCents > PendingAmountInCents)
        {
            throw new InvalidOperationException($"Cannot cancel {amountInCents}. Pending amount: {PendingAmountInCents}");
        }
        
        PendingAmountInCents -= amountInCents;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
