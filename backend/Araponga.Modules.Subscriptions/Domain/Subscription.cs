namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Assinatura de um usuário em um plano.
/// </summary>
public sealed class Subscription
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// ID do território (nullable, para assinaturas territoriais).
    /// </summary>
    public Guid? TerritoryId { get; private set; }
    
    /// <summary>
    /// ID do plano de assinatura.
    /// </summary>
    public Guid PlanId { get; private set; }
    
    public SubscriptionStatus Status { get; private set; }
    
    /// <summary>
    /// Início do período atual.
    /// </summary>
    public DateTime CurrentPeriodStart { get; private set; }
    
    /// <summary>
    /// Fim do período atual.
    /// </summary>
    public DateTime CurrentPeriodEnd { get; private set; }
    
    /// <summary>
    /// Início do trial (nullable).
    /// </summary>
    public DateTime? TrialStart { get; private set; }
    
    /// <summary>
    /// Fim do trial (nullable).
    /// </summary>
    public DateTime? TrialEnd { get; private set; }
    
    /// <summary>
    /// Data de cancelamento (nullable).
    /// </summary>
    public DateTime? CanceledAt { get; private set; }
    
    /// <summary>
    /// Se deve cancelar ao fim do período.
    /// </summary>
    public bool CancelAtPeriodEnd { get; private set; }
    
    /// <summary>
    /// ID da assinatura no Stripe (nullable, apenas para planos pagos).
    /// </summary>
    public string? StripeSubscriptionId { get; private set; }
    
    /// <summary>
    /// ID do cliente no Stripe (nullable, apenas para planos pagos).
    /// </summary>
    public string? StripeCustomerId { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    private Subscription()
    {
    }
    
    public Subscription(
        Guid id,
        Guid userId,
        Guid? territoryId,
        Guid planId,
        SubscriptionStatus status,
        DateTime currentPeriodStart,
        DateTime currentPeriodEnd,
        DateTime? trialStart = null,
        DateTime? trialEnd = null,
        string? stripeSubscriptionId = null,
        string? stripeCustomerId = null)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }
        
        if (planId == Guid.Empty)
        {
            throw new ArgumentException("Plan ID is required.", nameof(planId));
        }
        
        Id = id;
        UserId = userId;
        TerritoryId = territoryId;
        PlanId = planId;
        Status = status;
        CurrentPeriodStart = currentPeriodStart;
        CurrentPeriodEnd = currentPeriodEnd;
        TrialStart = trialStart;
        TrialEnd = trialEnd;
        CanceledAt = null;
        CancelAtPeriodEnd = false;
        StripeSubscriptionId = stripeSubscriptionId;
        StripeCustomerId = stripeCustomerId;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateStatus(SubscriptionStatus status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdatePeriod(DateTime periodStart, DateTime periodEnd)
    {
        CurrentPeriodStart = periodStart;
        CurrentPeriodEnd = periodEnd;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdatePlan(Guid planId)
    {
        PlanId = planId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void StartTrial(DateTime trialStart, DateTime trialEnd)
    {
        TrialStart = trialStart;
        TrialEnd = trialEnd;
        Status = SubscriptionStatus.TRIALING;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void EndTrial()
    {
        if (TrialEnd.HasValue && DateTime.UtcNow >= TrialEnd.Value)
        {
            Status = SubscriptionStatus.ACTIVE;
            TrialStart = null;
            TrialEnd = null;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
    
    public void Cancel(bool cancelAtPeriodEnd)
    {
        CanceledAt = DateTime.UtcNow;
        CancelAtPeriodEnd = cancelAtPeriodEnd;
        
        if (!cancelAtPeriodEnd)
        {
            Status = SubscriptionStatus.CANCELED;
        }
        
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void Reactivate()
    {
        CanceledAt = null;
        CancelAtPeriodEnd = false;
        Status = SubscriptionStatus.ACTIVE;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateStripeIds(string? stripeSubscriptionId, string? stripeCustomerId)
    {
        StripeSubscriptionId = stripeSubscriptionId;
        StripeCustomerId = stripeCustomerId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
