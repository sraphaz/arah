namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Histórico de mudanças em planos de assinatura (auditoria).
/// </summary>
public sealed class SubscriptionPlanHistory
{
    public Guid Id { get; private set; }
    public Guid PlanId { get; private set; }
    public Guid ChangedByUserId { get; private set; }
    public SubscriptionPlanHistoryChangeType ChangeType { get; private set; }
    
    /// <summary>
    /// Estado anterior do plano (JSON).
    /// </summary>
    public Dictionary<string, object>? PreviousState { get; private set; }
    
    /// <summary>
    /// Novo estado do plano (JSON).
    /// </summary>
    public Dictionary<string, object>? NewState { get; private set; }
    
    /// <summary>
    /// Motivo da mudança (nullable).
    /// </summary>
    public string? ChangeReason { get; private set; }
    
    public DateTime ChangedAtUtc { get; private set; }
    
    private SubscriptionPlanHistory()
    {
    }
    
    public SubscriptionPlanHistory(
        Guid id,
        Guid planId,
        Guid changedByUserId,
        SubscriptionPlanHistoryChangeType changeType,
        Dictionary<string, object>? previousState = null,
        Dictionary<string, object>? newState = null,
        string? changeReason = null)
    {
        if (planId == Guid.Empty)
        {
            throw new ArgumentException("Plan ID is required.", nameof(planId));
        }
        
        if (changedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Changed by user ID is required.", nameof(changedByUserId));
        }
        
        Id = id;
        PlanId = planId;
        ChangedByUserId = changedByUserId;
        ChangeType = changeType;
        PreviousState = previousState;
        NewState = newState;
        ChangeReason = changeReason;
        ChangedAtUtc = DateTime.UtcNow;
    }
}
