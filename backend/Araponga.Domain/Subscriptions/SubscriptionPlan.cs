namespace Araponga.Domain.Subscriptions;

/// <summary>
/// Plano de assinatura (Global ou Territorial).
/// </summary>
public sealed class SubscriptionPlan
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public SubscriptionPlanTier Tier { get; private set; }
    public PlanScope Scope { get; private set; }
    
    /// <summary>
    /// ID do território (nullable, apenas se Scope = Territory).
    /// </summary>
    public Guid? TerritoryId { get; private set; }
    
    /// <summary>
    /// Preço por ciclo (nullable para FREE).
    /// </summary>
    public decimal? PricePerCycle { get; private set; }
    
    /// <summary>
    /// Ciclo de cobrança (nullable para FREE).
    /// </summary>
    public SubscriptionBillingCycle? BillingCycle { get; private set; }
    
    /// <summary>
    /// Funcionalidades liberadas pelo plano.
    /// </summary>
    public List<FeatureCapability> Capabilities { get; private set; }
    
    /// <summary>
    /// Limites específicos (JSON): maxPosts, maxEvents, maxStorage, etc.
    /// </summary>
    public Dictionary<string, object>? Limits { get; private set; }
    
    /// <summary>
    /// Se é o plano padrão (FREE sempre é default).
    /// </summary>
    public bool IsDefault { get; private set; }
    
    /// <summary>
    /// Dias de trial (nullable, apenas para planos pagos).
    /// </summary>
    public int? TrialDays { get; private set; }
    
    /// <summary>
    /// Se o plano está ativo (pode ser desativado mas não deletado).
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// ID do usuário que criou o plano.
    /// </summary>
    public Guid CreatedByUserId { get; private set; }
    
    /// <summary>
    /// ID do preço no Stripe (nullable, apenas para planos pagos).
    /// </summary>
    public string? StripePriceId { get; private set; }
    
    /// <summary>
    /// ID do produto no Stripe (nullable, apenas para planos pagos).
    /// </summary>
    public string? StripeProductId { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    private SubscriptionPlan()
    {
        Name = string.Empty;
        Capabilities = new List<FeatureCapability>();
    }
    
    public SubscriptionPlan(
        Guid id,
        string name,
        string? description,
        SubscriptionPlanTier tier,
        PlanScope scope,
        Guid? territoryId,
        decimal? pricePerCycle,
        SubscriptionBillingCycle? billingCycle,
        List<FeatureCapability> capabilities,
        Dictionary<string, object>? limits,
        bool isDefault,
        int? trialDays,
        Guid createdByUserId,
        string? stripePriceId = null,
        string? stripeProductId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }
        
        if (tier == SubscriptionPlanTier.FREE && pricePerCycle.HasValue && pricePerCycle.Value != 0)
        {
            throw new ArgumentException("FREE plan must have price = 0.", nameof(pricePerCycle));
        }
        
        if (tier != SubscriptionPlanTier.FREE && (!pricePerCycle.HasValue || pricePerCycle.Value <= 0))
        {
            throw new ArgumentException("Paid plans must have price > 0.", nameof(pricePerCycle));
        }
        
        if (scope == PlanScope.Territory && !territoryId.HasValue)
        {
            throw new ArgumentException("Territory ID is required for territorial plans.", nameof(territoryId));
        }
        
        if (scope == PlanScope.Global && territoryId.HasValue)
        {
            throw new ArgumentException("Global plans cannot have territory ID.", nameof(territoryId));
        }
        
        Id = id;
        Name = name.Trim();
        Description = description?.Trim();
        Tier = tier;
        Scope = scope;
        TerritoryId = territoryId;
        PricePerCycle = pricePerCycle;
        BillingCycle = billingCycle;
        Capabilities = capabilities ?? new List<FeatureCapability>();
        Limits = limits;
        IsDefault = isDefault || tier == SubscriptionPlanTier.FREE; // FREE sempre é default
        TrialDays = trialDays;
        IsActive = true;
        CreatedByUserId = createdByUserId;
        StripePriceId = stripePriceId;
        StripeProductId = stripeProductId;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }
        
        Name = name.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdatePrice(decimal? pricePerCycle, SubscriptionBillingCycle? billingCycle)
    {
        if (Tier == SubscriptionPlanTier.FREE && pricePerCycle.HasValue && pricePerCycle.Value != 0)
        {
            throw new InvalidOperationException("FREE plan must have price = 0.");
        }
        
        if (Tier != SubscriptionPlanTier.FREE && (!pricePerCycle.HasValue || pricePerCycle.Value <= 0))
        {
            throw new InvalidOperationException("Paid plans must have price > 0.");
        }
        
        PricePerCycle = pricePerCycle;
        BillingCycle = billingCycle;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateCapabilities(List<FeatureCapability> capabilities)
    {
        Capabilities = capabilities ?? new List<FeatureCapability>();
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateLimits(Dictionary<string, object>? limits)
    {
        Limits = limits;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        if (Tier == SubscriptionPlanTier.FREE && Scope == PlanScope.Global)
        {
            throw new InvalidOperationException("Global FREE plan cannot be deactivated.");
        }
        
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void UpdateStripeIds(string? stripePriceId, string? stripeProductId)
    {
        StripePriceId = stripePriceId;
        StripeProductId = stripeProductId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
