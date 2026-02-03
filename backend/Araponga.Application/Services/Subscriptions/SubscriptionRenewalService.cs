using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Microsoft.Extensions.Logging;
using OperationResult = Araponga.Application.Common.OperationResult;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para processar renovações automáticas de assinaturas.
/// </summary>
public sealed class SubscriptionRenewalService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionPaymentRepository _paymentRepository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly ISubscriptionGatewayFactory _gatewayFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubscriptionRenewalService> _logger;

    public SubscriptionRenewalService(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionPaymentRepository paymentRepository,
        ISubscriptionPlanRepository planRepository,
        ISubscriptionGatewayFactory gatewayFactory,
        IUnitOfWork unitOfWork,
        ILogger<SubscriptionRenewalService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
        _planRepository = planRepository;
        _gatewayFactory = gatewayFactory;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Processa renovações de assinaturas que estão próximas do fim do período.
    /// </summary>
    public async Task<OperationResult> ProcessRenewalsAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Buscar assinaturas que expiram nos próximos 3 dias e estão ativas
            var expiringSoon = DateTime.UtcNow.AddDays(3);
            var activeSubscriptions = await _subscriptionRepository.ListAsync(
                userId: null,
                territoryId: null,
                status: SubscriptionStatus.ACTIVE,
                cancellationToken);

            var subscriptionsToRenew = activeSubscriptions
                .Where(s => s.CurrentPeriodEnd <= expiringSoon && 
                           s.CurrentPeriodEnd > DateTime.UtcNow &&
                           !string.IsNullOrEmpty(s.StripeSubscriptionId))
                .ToList();

            _logger.LogInformation(
                "Found {Count} subscriptions to renew",
                subscriptionsToRenew.Count);

            var successCount = 0;
            var failureCount = 0;

            foreach (var subscription in subscriptionsToRenew)
            {
                try
                {
                    var result = await RenewSubscriptionAsync(subscription, cancellationToken);
                    if (result.IsSuccess)
                    {
                        successCount++;
                    }
                    else
                    {
                        failureCount++;
                        _logger.LogWarning(
                            "Failed to renew subscription {SubscriptionId}: {Error}",
                            subscription.Id,
                            result.Error);
                    }
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger.LogError(
                        ex,
                        "Error renewing subscription {SubscriptionId}",
                        subscription.Id);
                }
            }

            _logger.LogInformation(
                "Renewal processing completed: {SuccessCount} succeeded, {FailureCount} failed",
                successCount,
                failureCount);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing subscription renewals");
            return OperationResult.Failure($"Error processing renewals: {ex.Message}");
        }
    }

    /// <summary>
    /// Renova uma assinatura específica.
    /// </summary>
    private async Task<OperationResult> RenewSubscriptionAsync(
        Subscription subscription,
        CancellationToken cancellationToken)
    {
        try
        {
            // Buscar plano
            var plan = await _planRepository.GetByIdAsync(subscription.PlanId, cancellationToken);
            if (plan == null)
            {
                return OperationResult.Failure("Plan not found");
            }

            // Se for plano FREE, não precisa renovar
            if (plan.Tier == SubscriptionPlanTier.FREE)
            {
                return OperationResult.Success();
            }

            // Verificar se já existe pagamento para este período
            var existingPayment = await _paymentRepository.GetBySubscriptionAndPeriodAsync(
                subscription.Id,
                subscription.CurrentPeriodStart,
                subscription.CurrentPeriodEnd,
                cancellationToken);

            if (existingPayment != null && existingPayment.Status == SubscriptionPaymentStatus.Succeeded)
            {
                // Já foi pago, apenas atualizar período se necessário
                _logger.LogInformation(
                    "Subscription {SubscriptionId} already has payment for current period",
                    subscription.Id);
                return OperationResult.Success();
            }

            // O gateway deve processar a renovação automaticamente via webhook
            // Este método apenas verifica e atualiza o período se necessário
            var gateway = _gatewayFactory.GetGateway();
            if (gateway != null && !string.IsNullOrEmpty(subscription.StripeSubscriptionId))
            {
                var gatewayInfo = await gateway.GetSubscriptionAsync(
                    subscription.StripeSubscriptionId,
                    cancellationToken);

                if (gatewayInfo != null)
                {
                    // Atualizar período baseado no gateway
                    subscription.UpdatePeriod(
                        gatewayInfo.CurrentPeriodStart,
                        gatewayInfo.CurrentPeriodEnd);
                    subscription.UpdateStatus(gatewayInfo.Status);

                    await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
                    await _unitOfWork.CommitAsync(cancellationToken);
                }
            }

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error renewing subscription {SubscriptionId}", subscription.Id);
            return OperationResult.Failure($"Error renewing subscription: {ex.Message}");
        }
    }
}
