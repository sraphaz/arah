using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Microsoft.Extensions.Logging;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para calcular métricas e analytics de assinaturas.
/// </summary>
public sealed class SubscriptionAnalyticsService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionPaymentRepository _paymentRepository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly ILogger<SubscriptionAnalyticsService> _logger;

    public SubscriptionAnalyticsService(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionPaymentRepository paymentRepository,
        ISubscriptionPlanRepository planRepository,
        ILogger<SubscriptionAnalyticsService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
        _planRepository = planRepository;
        _logger = logger;
    }

    /// <summary>
    /// Calcula o MRR (Monthly Recurring Revenue).
    /// </summary>
    public async Task<decimal> GetMRRAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;

        var activeSubscriptions = await _subscriptionRepository.ListAsync(
            userId: null,
            territoryId: null,
            status: SubscriptionStatus.ACTIVE,
            cancellationToken);

        var mrr = 0m;

        // Filtrar subscriptions que estavam ativas durante o período solicitado
        foreach (var subscription in activeSubscriptions)
        {
            // Considerar apenas subscriptions que estavam ativas no período
            // Uma subscription está ativa no período se:
            // - Foi criada antes ou durante o período E
            // - Não foi cancelada antes do início do período OU foi cancelada mas cancelAtPeriodEnd é true
            // E estava ativa durante pelo menos parte do período (CurrentPeriodStart <= end && CurrentPeriodEnd >= start)
            // Para MRR, consideramos subscriptions que estavam ativas em qualquer momento do período
            var wasActiveInPeriod = subscription.CreatedAtUtc <= end &&
                (subscription.CanceledAt == null || 
                 subscription.CanceledAt >= start || 
                 (subscription.CancelAtPeriodEnd && subscription.CurrentPeriodEnd >= start)) &&
                subscription.CurrentPeriodStart <= end &&
                subscription.CurrentPeriodEnd >= start;
            
            if (!wasActiveInPeriod)
            {
                continue;
            }

            var plan = await _planRepository.GetByIdAsync(subscription.PlanId, cancellationToken);
            if (plan == null || plan.Tier == SubscriptionPlanTier.FREE || !plan.PricePerCycle.HasValue)
            {
                continue;
            }

            var monthlyValue = plan.BillingCycle switch
            {
                SubscriptionBillingCycle.MONTHLY => plan.PricePerCycle.Value,
                SubscriptionBillingCycle.QUARTERLY => plan.PricePerCycle.Value / 3m,
                SubscriptionBillingCycle.YEARLY => plan.PricePerCycle.Value / 12m,
                _ => 0m
            };

            mrr += monthlyValue;
        }

        return mrr;
    }

    /// <summary>
    /// Calcula a taxa de churn.
    /// </summary>
    public async Task<decimal> GetChurnRateAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;

        // Assinaturas ativas no início do período
        var activeAtStart = await _subscriptionRepository.ListAsync(
            userId: null,
            territoryId: null,
            status: SubscriptionStatus.ACTIVE,
            cancellationToken);

        // Contar assinaturas que estavam ativas no início do período
        // Uma assinatura estava ativa no início se foi criada antes ou durante o período
        var activeCount = activeAtStart.Count(s => s.CurrentPeriodStart <= start);

        if (activeCount == 0)
        {
            return 0m;
        }

        // Assinaturas canceladas no período
        var canceledSubscriptions = await _subscriptionRepository.ListAsync(
            userId: null,
            territoryId: null,
            status: SubscriptionStatus.CANCELED,
            cancellationToken);

        var canceledCount = canceledSubscriptions.Count(s =>
            s.CanceledAt.HasValue &&
            s.CanceledAt.Value >= start &&
            s.CanceledAt.Value <= end);

        return (canceledCount / (decimal)activeCount) * 100m;
    }

    /// <summary>
    /// Obtém o número de assinaturas ativas.
    /// </summary>
    public async Task<int> GetActiveSubscriptionsCountAsync(CancellationToken cancellationToken)
    {
        var activeSubscriptions = await _subscriptionRepository.ListAsync(
            userId: null,
            territoryId: null,
            status: SubscriptionStatus.ACTIVE,
            cancellationToken);

        return activeSubscriptions.Count;
    }

    /// <summary>
    /// Obtém o número de novas assinaturas no período.
    /// </summary>
    public async Task<int> GetNewSubscriptionsCountAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;

        var allSubscriptions = await _subscriptionRepository.ListAsync(
            userId: null,
            territoryId: null,
            status: null,
            cancellationToken);

        return allSubscriptions.Count(s =>
            s.CurrentPeriodStart >= start &&
            s.CurrentPeriodStart <= end);
    }

    /// <summary>
    /// Obtém o número de assinaturas canceladas no período.
    /// </summary>
    public async Task<int> GetCanceledSubscriptionsCountAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;

        var canceledSubscriptions = await _subscriptionRepository.ListAsync(
            userId: null,
            territoryId: null,
            status: SubscriptionStatus.CANCELED,
            cancellationToken);

        return canceledSubscriptions.Count(s =>
            s.CanceledAt.HasValue &&
            s.CanceledAt.Value >= start &&
            s.CanceledAt.Value <= end);
    }

    /// <summary>
    /// Obtém receita por plano no período.
    /// </summary>
    public async Task<Dictionary<Guid, decimal>> GetRevenueByPlanAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;

        // Buscar todos os pagamentos no período especificado
        var allPayments = await _paymentRepository.GetByDateRangeAsync(start, end, cancellationToken);
        var successfulPayments = allPayments
            .Where(p => p.Status == SubscriptionPaymentStatus.Succeeded)
            .ToList();

        var revenueByPlan = new Dictionary<Guid, decimal>();

        foreach (var payment in successfulPayments)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(payment.SubscriptionId, cancellationToken);
            if (subscription != null)
            {
                if (!revenueByPlan.ContainsKey(subscription.PlanId))
                {
                    revenueByPlan[subscription.PlanId] = 0m;
                }

                revenueByPlan[subscription.PlanId] += payment.Amount;
            }
        }

        return revenueByPlan;
    }
}
