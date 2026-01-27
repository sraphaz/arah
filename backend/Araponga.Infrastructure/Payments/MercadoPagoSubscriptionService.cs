using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Payments;

/// <summary>
/// Serviço de integração com Mercado Pago Subscriptions.
/// Por enquanto, implementação mock para desenvolvimento.
/// Em produção, implementar com SDK do Mercado Pago.
/// </summary>
public sealed class MercadoPagoSubscriptionService : ISubscriptionGateway
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MercadoPagoSubscriptionService> _logger;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IUserRepository _userRepository;

    public string GatewayName => "MercadoPago";

    public MercadoPagoSubscriptionService(
        IConfiguration configuration,
        ILogger<MercadoPagoSubscriptionService> logger,
        ISubscriptionPlanRepository planRepository,
        IUserRepository userRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _planRepository = planRepository;
        _userRepository = userRepository;
    }

    public async Task<OperationResult<SubscriptionGatewayResult>> CreateSubscriptionAsync(
        Guid userId,
        Guid planId,
        string? couponCode,
        CancellationToken cancellationToken)
    {
        // Buscar plano
        var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);
        if (plan == null)
        {
            return OperationResult<SubscriptionGatewayResult>.Failure("Plan not found.");
        }

        // Se for plano FREE, não precisa criar no Mercado Pago
        if (plan.Tier == SubscriptionPlanTier.FREE)
        {
            return OperationResult<SubscriptionGatewayResult>.Failure("FREE plan does not require Mercado Pago subscription.");
        }

        // Buscar usuário
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return OperationResult<SubscriptionGatewayResult>.Failure("User not found.");
        }

        // TODO: Implementar criação real no Mercado Pago usando SDK
        // Por enquanto, retorna mock
        _logger.LogWarning("MercadoPagoSubscriptionService: Using mock implementation. Real Mercado Pago integration not yet implemented.");

        var mockSubscriptionId = $"mp_sub_{Guid.NewGuid():N}";
        var mockCustomerId = $"mp_cus_{Guid.NewGuid():N}";
        var now = DateTime.UtcNow;
        var periodEnd = CalculatePeriodEnd(now, plan.BillingCycle ?? SubscriptionBillingCycle.MONTHLY);

        var result = new SubscriptionGatewayResult
        {
            GatewaySubscriptionId = mockSubscriptionId,
            GatewayCustomerId = mockCustomerId,
            CurrentPeriodStart = now,
            CurrentPeriodEnd = periodEnd,
            Status = SubscriptionStatus.ACTIVE
        };

        return OperationResult<SubscriptionGatewayResult>.Success(result);
    }

    public async Task<OperationResult<SubscriptionGatewayResult>> UpdateSubscriptionAsync(
        Guid subscriptionId,
        Guid newPlanId,
        CancellationToken cancellationToken)
    {
        // TODO: Implementar atualização real no Mercado Pago
        _logger.LogWarning("MercadoPagoSubscriptionService: Using mock implementation for update.");

        var plan = await _planRepository.GetByIdAsync(newPlanId, cancellationToken);
        if (plan == null)
        {
            return OperationResult<SubscriptionGatewayResult>.Failure("Plan not found.");
        }

        var now = DateTime.UtcNow;
        var periodEnd = CalculatePeriodEnd(now, plan.BillingCycle ?? SubscriptionBillingCycle.MONTHLY);

        var result = new SubscriptionGatewayResult
        {
            GatewaySubscriptionId = $"mp_sub_{subscriptionId}",
            GatewayCustomerId = $"mp_cus_{subscriptionId}",
            CurrentPeriodStart = now,
            CurrentPeriodEnd = periodEnd,
            Status = SubscriptionStatus.ACTIVE
        };

        return OperationResult<SubscriptionGatewayResult>.Success(result);
    }

    public Task<OperationResult> CancelSubscriptionAsync(
        Guid subscriptionId,
        bool cancelAtPeriodEnd,
        CancellationToken cancellationToken)
    {
        // TODO: Implementar cancelamento real no Mercado Pago
        _logger.LogWarning("MercadoPagoSubscriptionService: Using mock implementation for cancel.");
        return Task.FromResult(OperationResult.Success());
    }

    public Task<OperationResult> ReactivateSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken)
    {
        // TODO: Implementar reativação real no Mercado Pago
        _logger.LogWarning("MercadoPagoSubscriptionService: Using mock implementation for reactivate.");
        return Task.FromResult(OperationResult.Success());
    }

    public Task<SubscriptionGatewayInfo?> GetSubscriptionAsync(
        string gatewaySubscriptionId,
        CancellationToken cancellationToken)
    {
        // TODO: Implementar busca real no Mercado Pago
        _logger.LogWarning("MercadoPagoSubscriptionService: Using mock implementation for get subscription.");
        
        // Retorna null para indicar que não encontrou (mock)
        return Task.FromResult<SubscriptionGatewayInfo?>(null);
    }

    private static DateTime CalculatePeriodEnd(DateTime start, SubscriptionBillingCycle cycle)
    {
        return cycle switch
        {
            SubscriptionBillingCycle.MONTHLY => start.AddMonths(1),
            SubscriptionBillingCycle.QUARTERLY => start.AddMonths(3),
            SubscriptionBillingCycle.YEARLY => start.AddYears(1),
            _ => start.AddMonths(1)
        };
    }
}
