namespace Araponga.Api.Contracts.Subscriptions;

/// <summary>
/// Resposta com métricas gerais de assinaturas.
/// </summary>
public sealed class SubscriptionAnalyticsResponse
{
    public decimal MRR { get; set; }
    public decimal ChurnRate { get; set; }
    public int ActiveSubscriptionsCount { get; set; }
    public int NewSubscriptionsCount { get; set; }
    public int CanceledSubscriptionsCount { get; set; }
    public Dictionary<Guid, decimal> RevenueByPlan { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// Resposta com MRR.
/// </summary>
public sealed class MRRResponse
{
    public decimal MRR { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// Resposta com taxa de churn.
/// </summary>
public sealed class ChurnRateResponse
{
    public decimal ChurnRate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

/// <summary>
/// Resposta com receita por plano.
/// </summary>
public sealed class RevenueByPlanResponse
{
    public Dictionary<Guid, decimal> RevenueByPlan { get; set; } = new();
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}
