namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Pagamento de uma assinatura.
/// </summary>
public sealed class SubscriptionPayment
{
    public Guid Id { get; private set; }
    public Guid SubscriptionId { get; private set; }
    
    /// <summary>
    /// Valor do pagamento.
    /// </summary>
    public decimal Amount { get; private set; }
    
    /// <summary>
    /// Moeda do pagamento (padrão: BRL).
    /// </summary>
    public string Currency { get; private set; }
    
    public SubscriptionPaymentStatus Status { get; private set; }
    
    /// <summary>
    /// Data do pagamento.
    /// </summary>
    public DateTime PaymentDate { get; private set; }
    
    /// <summary>
    /// Início do período coberto.
    /// </summary>
    public DateTime PeriodStart { get; private set; }
    
    /// <summary>
    /// Fim do período coberto.
    /// </summary>
    public DateTime PeriodEnd { get; private set; }
    
    /// <summary>
    /// ID da invoice no Stripe (nullable).
    /// </summary>
    public string? StripeInvoiceId { get; private set; }
    
    /// <summary>
    /// ID do payment intent no Stripe (nullable).
    /// </summary>
    public string? StripePaymentIntentId { get; private set; }
    
    /// <summary>
    /// Motivo da falha (nullable).
    /// </summary>
    public string? FailureReason { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    private SubscriptionPayment()
    {
        Currency = "BRL";
    }
    
    public SubscriptionPayment(
        Guid id,
        Guid subscriptionId,
        decimal amount,
        string currency,
        SubscriptionPaymentStatus status,
        DateTime paymentDate,
        DateTime periodStart,
        DateTime periodEnd,
        string? stripeInvoiceId = null,
        string? stripePaymentIntentId = null,
        string? failureReason = null)
    {
        if (subscriptionId == Guid.Empty)
        {
            throw new ArgumentException("Subscription ID is required.", nameof(subscriptionId));
        }
        
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }
        
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }
        
        Id = id;
        SubscriptionId = subscriptionId;
        Amount = amount;
        Currency = currency;
        Status = status;
        PaymentDate = paymentDate;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        StripeInvoiceId = stripeInvoiceId;
        StripePaymentIntentId = stripePaymentIntentId;
        FailureReason = failureReason;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateStatus(SubscriptionPaymentStatus status, string? failureReason = null)
    {
        Status = status;
        FailureReason = failureReason;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateStripeIds(string? stripeInvoiceId, string? stripePaymentIntentId)
    {
        StripeInvoiceId = stripeInvoiceId;
        StripePaymentIntentId = stripePaymentIntentId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
