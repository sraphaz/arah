using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Microsoft.Extensions.Logging;
using OperationResult = Araponga.Application.Common.OperationResult;

namespace Araponga.Application.Services;

public sealed class SubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly ISubscriptionCouponRepository _subscriptionCouponRepository;
    private readonly ISubscriptionGatewayFactory _gatewayFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubscriptionService>? _logger;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionPlanRepository planRepository,
        ICouponRepository couponRepository,
        ISubscriptionCouponRepository subscriptionCouponRepository,
        IUnitOfWork unitOfWork,
        ISubscriptionGatewayFactory gatewayFactory,
        ILogger<SubscriptionService>? logger = null)
    {
        _subscriptionRepository = subscriptionRepository;
        _planRepository = planRepository;
        _couponRepository = couponRepository;
        _subscriptionCouponRepository = subscriptionCouponRepository;
        _unitOfWork = unitOfWork;
        _gatewayFactory = gatewayFactory;
        _logger = logger;
    }

    /// <summary>
    /// Obtém ou cria assinatura FREE padrão para o usuário.
    /// </summary>
    public async Task<Subscription> GetOrCreateUserSubscriptionAsync(
        Guid userId,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        // Tenta obter assinatura existente
        var existing = await _subscriptionRepository.GetByUserIdAsync(userId, territoryId, cancellationToken);
        if (existing != null)
        {
            return existing;
        }

        // Se não existe, busca plano FREE padrão
        SubscriptionPlan? defaultPlan;
        if (territoryId.HasValue)
        {
            defaultPlan = await _planRepository.GetDefaultPlanForTerritoryAsync(territoryId.Value, cancellationToken);
        }
        else
        {
            defaultPlan = await _planRepository.GetDefaultPlanAsync(cancellationToken);
        }

        if (defaultPlan == null)
        {
            throw new InvalidOperationException("Default FREE plan not found. System must have a default plan.");
        }

        // Cria assinatura FREE (não precisa de Stripe)
        var now = DateTime.UtcNow;
        var subscription = new Subscription(
            Guid.NewGuid(),
            userId,
            territoryId,
            defaultPlan.Id,
            SubscriptionStatus.ACTIVE,
            now,
            now.AddYears(100), // FREE nunca expira
            null,
            null,
            null,
            null);

        await _subscriptionRepository.AddAsync(subscription, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return subscription;
    }

    /// <summary>
    /// Obtém planos disponíveis para um território (territoriais + globais).
    /// </summary>
    public async Task<IReadOnlyList<SubscriptionPlan>> GetAvailablePlansForTerritoryAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        return await _planRepository.GetPlansForTerritoryAsync(territoryId, cancellationToken);
    }

    /// <summary>
    /// Obtém plano por ID (acesso público).
    /// </summary>
    public async Task<SubscriptionPlan?> GetPlanByIdAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        return await _planRepository.GetByIdAsync(planId, cancellationToken);
    }

    /// <summary>
    /// Obtém planos globais (acesso público).
    /// </summary>
    public async Task<IReadOnlyList<SubscriptionPlan>> GetGlobalPlansAsync(
        CancellationToken cancellationToken)
    {
        return await _planRepository.GetGlobalPlansAsync(cancellationToken);
    }

    /// <summary>
    /// Cria uma assinatura paga para o usuário.
    /// </summary>
    public async Task<Result<Subscription>> CreateSubscriptionAsync(
        Guid userId,
        Guid? territoryId,
        Guid planId,
        string? couponCode,
        CancellationToken cancellationToken)
    {
        // Verificar se já existe assinatura ativa
        var existing = await _subscriptionRepository.GetByUserIdAsync(userId, territoryId, cancellationToken);
        if (existing != null && existing.Status == SubscriptionStatus.ACTIVE)
        {
            return Result<Subscription>.Failure("User already has an active subscription.");
        }

        // Buscar plano
        var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);
        if (plan == null)
        {
            return Result<Subscription>.Failure($"Plan {planId} not found.");
        }

        if (!plan.IsActive)
        {
            return Result<Subscription>.Failure("Plan is not active.");
        }

        // Validar cupom se fornecido
        Coupon? coupon = null;
        if (!string.IsNullOrWhiteSpace(couponCode))
        {
            coupon = await _couponRepository.GetByCodeAsync(couponCode, cancellationToken);
            if (coupon == null)
            {
                return Result<Subscription>.Failure("Coupon not found.");
            }

            if (!coupon.IsValid(DateTime.UtcNow))
            {
                return Result<Subscription>.Failure("Coupon is not valid.");
            }
        }

        // Calcular período
        var now = DateTime.UtcNow;
        var periodEnd = CalculatePeriodEnd(now, plan.BillingCycle ?? SubscriptionBillingCycle.MONTHLY);

        // Criar assinatura
        var subscription = new Subscription(
            Guid.NewGuid(),
            userId,
            territoryId,
            planId,
            SubscriptionStatus.ACTIVE,
            now,
            periodEnd);

        // Aplicar trial se o plano tiver
        if (plan.TrialDays.HasValue && plan.TrialDays.Value > 0)
        {
            subscription.StartTrial(now, now.AddDays(plan.TrialDays.Value));
        }

        // Se for plano pago, criar assinatura no gateway
        if (plan.Tier != SubscriptionPlanTier.FREE)
        {
            var gateway = _gatewayFactory.GetGateway();
            if (gateway != null)
            {
                var gatewayResult = await gateway.CreateSubscriptionAsync(
                    userId,
                    planId,
                    couponCode,
                    cancellationToken);

                if (gatewayResult.IsFailure)
                {
                    return Result<Subscription>.Failure($"Failed to create subscription in {gateway.GatewayName}: {gatewayResult.Error}");
                }

                subscription.UpdateStripeIds(
                    gatewayResult.Value!.GatewaySubscriptionId,
                    gatewayResult.Value.GatewayCustomerId);
                subscription.UpdatePeriod(
                    gatewayResult.Value.CurrentPeriodStart,
                    gatewayResult.Value.CurrentPeriodEnd);
                subscription.UpdateStatus(gatewayResult.Value.Status);
            }
        }

        await _subscriptionRepository.AddAsync(subscription, cancellationToken);

        // Aplicar cupom se fornecido
        if (coupon != null)
        {
            var subscriptionCoupon = new SubscriptionCoupon(
                Guid.NewGuid(),
                subscription.Id,
                coupon.Id);
            await _subscriptionCouponRepository.AddAsync(subscriptionCoupon, cancellationToken);
            coupon.Use();
            await _couponRepository.UpdateAsync(coupon, cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Subscription>.Success(subscription);
    }

    /// <summary>
    /// Atualiza assinatura (upgrade/downgrade).
    /// </summary>
    public async Task<Result<Subscription>> UpdateSubscriptionAsync(
        Guid subscriptionId,
        Guid newPlanId,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken);
        if (subscription == null)
        {
            return Result<Subscription>.Failure($"Subscription {subscriptionId} not found.");
        }

        var newPlan = await _planRepository.GetByIdAsync(newPlanId, cancellationToken);
        if (newPlan == null)
        {
            return Result<Subscription>.Failure($"Plan {newPlanId} not found.");
        }

        if (!newPlan.IsActive)
        {
            return Result<Subscription>.Failure("New plan is not active.");
        }

        // Se for plano pago, atualizar no gateway
        if (newPlan.Tier != SubscriptionPlanTier.FREE && !string.IsNullOrEmpty(subscription.StripeSubscriptionId))
        {
            var gateway = _gatewayFactory.GetGateway();
            if (gateway != null)
            {
                var gatewayResult = await gateway.UpdateSubscriptionAsync(
                    subscription.Id,
                    newPlanId,
                    cancellationToken);

                if (gatewayResult.IsFailure)
                {
                    return Result<Subscription>.Failure($"Failed to update subscription in {gateway.GatewayName}: {gatewayResult.Error}");
                }

                subscription.UpdatePlan(newPlanId);
                subscription.UpdateStripeIds(
                    gatewayResult.Value!.GatewaySubscriptionId,
                    gatewayResult.Value.GatewayCustomerId);
                subscription.UpdatePeriod(
                    gatewayResult.Value.CurrentPeriodStart,
                    gatewayResult.Value.CurrentPeriodEnd);
                subscription.UpdateStatus(gatewayResult.Value.Status);
            }
        }
        else
        {
            // Atualizar localmente apenas
            subscription.UpdatePlan(newPlanId);
            subscription.UpdateStatus(SubscriptionStatus.ACTIVE);
            var periodEnd = CalculatePeriodEnd(subscription.CurrentPeriodStart, newPlan.BillingCycle ?? SubscriptionBillingCycle.MONTHLY);
            subscription.UpdatePeriod(subscription.CurrentPeriodStart, periodEnd);
        }

        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Subscription>.Success(subscription);
    }

    /// <summary>
    /// Cancela assinatura (volta para FREE).
    /// </summary>
    public async Task<OperationResult> CancelSubscriptionAsync(
        Guid subscriptionId,
        bool cancelAtPeriodEnd,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken);
        if (subscription == null)
        {
            return OperationResult.Failure($"Subscription {subscriptionId} not found.");
        }

        subscription.Cancel(cancelAtPeriodEnd);

        // Cancelar no gateway se for plano pago
        if (!string.IsNullOrEmpty(subscription.StripeSubscriptionId))
        {
            var gateway = _gatewayFactory.GetGateway();
            if (gateway != null)
            {
                var result = await gateway.CancelSubscriptionAsync(
                    subscription.Id,
                    cancelAtPeriodEnd,
                    cancellationToken);

                if (result.IsFailure)
                {
                    _logger?.LogWarning("Failed to cancel subscription in {Gateway}: {Error}", 
                        gateway.GatewayName, result.Error);
                }
            }
        }

        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    /// <summary>
    /// Reativa assinatura cancelada.
    /// </summary>
    public async Task<Result<Subscription>> ReactivateSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken);
        if (subscription == null)
        {
            return Result<Subscription>.Failure($"Subscription {subscriptionId} not found.");
        }

        subscription.Reactivate();

        // Se for plano pago, reativar no gateway
        if (!string.IsNullOrEmpty(subscription.StripeSubscriptionId))
        {
            var gateway = _gatewayFactory.GetGateway();
            if (gateway != null)
            {
                var gatewayResult = await gateway.ReactivateSubscriptionAsync(
                    subscription.Id,
                    cancellationToken);

                if (gatewayResult.IsFailure)
                {
                    return Result<Subscription>.Failure($"Failed to reactivate subscription in {gateway.GatewayName}: {gatewayResult.Error}");
                }
            }
        }

        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Subscription>.Success(subscription);
    }

    /// <summary>
    /// Obtém assinatura do usuário (retorna FREE se não tiver pago).
    /// </summary>
    public async Task<Subscription> GetUserSubscriptionAsync(
        Guid userId,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        return await GetOrCreateUserSubscriptionAsync(userId, territoryId, cancellationToken);
    }

    /// <summary>
    /// Obtém assinatura por ID.
    /// </summary>
    public async Task<Subscription?> GetSubscriptionAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken)
    {
        return await _subscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken);
    }

    /// <summary>
    /// Lista assinaturas com filtros.
    /// </summary>
    public async Task<IReadOnlyList<Subscription>> ListSubscriptionsAsync(
        Guid? userId,
        Guid? territoryId,
        SubscriptionStatus? status,
        CancellationToken cancellationToken)
    {
        return await _subscriptionRepository.ListAsync(userId, territoryId, status, cancellationToken);
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
