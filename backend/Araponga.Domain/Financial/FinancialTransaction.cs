using Araponga.Domain.Marketplace;
using Araponga.Domain.Territories;

namespace Araponga.Domain.Financial;

/// <summary>
/// Transação financeira central para rastreabilidade completa.
/// Cada centavo deve ser rastreado através desta entidade.
/// </summary>
public sealed class FinancialTransaction
{
    public Guid Id { get; private set; }
    public Guid TerritoryId { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    
    /// <summary>
    /// Valor da transação em centavos.
    /// </summary>
    public long AmountInCents { get; private set; }
    
    /// <summary>
    /// Moeda da transação (ex: "BRL", "USD").
    /// </summary>
    public string Currency { get; private set; }
    
    /// <summary>
    /// Descrição da transação.
    /// </summary>
    public string Description { get; private set; }
    
    /// <summary>
    /// ID da entidade relacionada (ex: CheckoutId, SellerTransactionId).
    /// </summary>
    public Guid? RelatedEntityId { get; private set; }
    
    /// <summary>
    /// Tipo da entidade relacionada (ex: "Checkout", "SellerTransaction").
    /// </summary>
    public string? RelatedEntityType { get; private set; }
    
    /// <summary>
    /// Transações relacionadas (ex: Payment relacionado a Checkout, Payout relacionado a SellerTransaction).
    /// </summary>
    public List<Guid> RelatedTransactionIds { get; private set; }
    
    /// <summary>
    /// Metadados adicionais (JSON).
    /// </summary>
    public Dictionary<string, string>? Metadata { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Navigation properties
    public Territory Territory { get; private set; } = null!;
    public Checkout? RelatedCheckout { get; private set; }
    
    private FinancialTransaction() 
    {
        // Inicializar propriedades para EF Core
        Currency = string.Empty;
        Description = string.Empty;
        RelatedTransactionIds = new List<Guid>();
    } // EF Core
    
    public FinancialTransaction(
        Guid id,
        Guid territoryId,
        TransactionType type,
        long amountInCents,
        string currency,
        string description,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        Dictionary<string, string>? metadata = null)
    {
        Id = id;
        TerritoryId = territoryId;
        Type = type;
        Status = TransactionStatus.Pending;
        AmountInCents = amountInCents;
        Currency = currency;
        Description = description;
        RelatedEntityId = relatedEntityId;
        RelatedEntityType = relatedEntityType;
        RelatedTransactionIds = new List<Guid>();
        Metadata = metadata;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateStatus(TransactionStatus newStatus)
    {
        if (Status == newStatus)
        {
            return;
        }
        
        Status = newStatus;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void AddRelatedTransaction(Guid relatedTransactionId)
    {
        if (!RelatedTransactionIds.Contains(relatedTransactionId))
        {
            RelatedTransactionIds.Add(relatedTransactionId);
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
    
    public void UpdateMetadata(Dictionary<string, string> metadata)
    {
        Metadata = metadata;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
