using Araponga.Domain.Territories;

namespace Araponga.Domain.Marketplace;

/// <summary>
/// Configuração de payout por território.
/// </summary>
public sealed class TerritoryPayoutConfig
{
    public Guid Id { get; private set; }
    public Guid TerritoryId { get; private set; }
    
    /// <summary>
    /// Período de retenção em dias (quantos dias aguardar antes de liberar para payout).
    /// </summary>
    public int RetentionPeriodDays { get; private set; }
    
    /// <summary>
    /// Valor mínimo em centavos para processar payout (acumula até atingir).
    /// </summary>
    public long MinimumPayoutAmountInCents { get; private set; }
    
    /// <summary>
    /// Valor máximo em centavos por payout (se exceder, divide em múltiplos payouts).
    /// </summary>
    public long? MaximumPayoutAmountInCents { get; private set; }
    
    /// <summary>
    /// Frequência de payout (Daily, Weekly, Monthly, Manual).
    /// </summary>
    public PayoutFrequency Frequency { get; private set; }
    
    /// <summary>
    /// Se payout automático está habilitado.
    /// </summary>
    public bool AutoPayoutEnabled { get; private set; }
    
    /// <summary>
    /// Se requer aprovação manual antes de processar payout.
    /// </summary>
    public bool RequiresApproval { get; private set; }
    
    /// <summary>
    /// Moeda da configuração (ex: "BRL", "USD").
    /// </summary>
    public string Currency { get; private set; }
    
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    
    // Navigation property
    public Territory Territory { get; private set; } = null!;
    
    private TerritoryPayoutConfig() 
    {
        // Inicializar propriedades para EF Core
        Currency = string.Empty;
    } // EF Core
    
    public TerritoryPayoutConfig(
        Guid id,
        Guid territoryId,
        int retentionPeriodDays,
        long minimumPayoutAmountInCents,
        long? maximumPayoutAmountInCents,
        PayoutFrequency frequency,
        bool autoPayoutEnabled,
        bool requiresApproval,
        string currency)
    {
        Id = id;
        TerritoryId = territoryId;
        RetentionPeriodDays = retentionPeriodDays;
        MinimumPayoutAmountInCents = minimumPayoutAmountInCents;
        MaximumPayoutAmountInCents = maximumPayoutAmountInCents;
        Frequency = frequency;
        AutoPayoutEnabled = autoPayoutEnabled;
        RequiresApproval = requiresApproval;
        Currency = currency;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void Update(
        int retentionPeriodDays,
        long minimumPayoutAmountInCents,
        long? maximumPayoutAmountInCents,
        PayoutFrequency frequency,
        bool autoPayoutEnabled,
        bool requiresApproval)
    {
        RetentionPeriodDays = retentionPeriodDays;
        MinimumPayoutAmountInCents = minimumPayoutAmountInCents;
        MaximumPayoutAmountInCents = maximumPayoutAmountInCents;
        Frequency = frequency;
        AutoPayoutEnabled = autoPayoutEnabled;
        RequiresApproval = requiresApproval;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    
    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

/// <summary>
/// Frequência de payout.
/// </summary>
public enum PayoutFrequency
{
    /// <summary>
    /// Diário (processa payouts diariamente).
    /// </summary>
    Daily = 1,
    
    /// <summary>
    /// Semanal (processa payouts semanalmente).
    /// </summary>
    Weekly = 2,
    
    /// <summary>
    /// Mensal (processa payouts mensalmente).
    /// </summary>
    Monthly = 3,
    
    /// <summary>
    /// Manual (apenas quando solicitado).
    /// </summary>
    Manual = 4
}
