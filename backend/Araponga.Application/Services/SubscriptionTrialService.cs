using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Email;
using Araponga.Domain.Subscriptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using OperationResult = Araponga.Application.Common.OperationResult;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar períodos de trial de assinaturas.
/// </summary>
public sealed class SubscriptionTrialService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutbox _outbox;
    private readonly ILogger<SubscriptionTrialService> _logger;

    public SubscriptionTrialService(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionPlanRepository planRepository,
        IUnitOfWork unitOfWork,
        IOutbox outbox,
        ILogger<SubscriptionTrialService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _planRepository = planRepository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    /// <summary>
    /// Processa conversões de trials que expiraram.
    /// </summary>
    public async Task<OperationResult> ProcessExpiredTrialsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var activeSubscriptions = await _subscriptionRepository.ListAsync(
                userId: null,
                territoryId: null,
                status: SubscriptionStatus.TRIALING,
                cancellationToken);

            var expiredTrials = activeSubscriptions
                .Where(s => s.TrialEnd.HasValue && s.TrialEnd.Value <= DateTime.UtcNow)
                .ToList();

            _logger.LogInformation(
                "Found {Count} expired trials to process",
                expiredTrials.Count);

            var successCount = 0;
            var failureCount = 0;

            foreach (var subscription in expiredTrials)
            {
                try
                {
                    var result = await ConvertTrialToActiveAsync(subscription, cancellationToken);
                    if (result.IsSuccess)
                    {
                        successCount++;
                    }
                    else
                    {
                        failureCount++;
                        _logger.LogWarning(
                            "Failed to convert trial for subscription {SubscriptionId}: {Error}",
                            subscription.Id,
                            result.Error);
                    }
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger.LogError(
                        ex,
                        "Error converting trial for subscription {SubscriptionId}",
                        subscription.Id);
                }
            }

            _logger.LogInformation(
                "Trial conversion completed: {SuccessCount} succeeded, {FailureCount} failed",
                successCount,
                failureCount);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing expired trials");
            return OperationResult.Failure($"Error processing expired trials: {ex.Message}");
        }
    }

    /// <summary>
    /// Converte um trial expirado para assinatura ativa.
    /// </summary>
    private async Task<OperationResult> ConvertTrialToActiveAsync(
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

            // Se for plano FREE, não precisa de conversão
            if (plan.Tier == SubscriptionPlanTier.FREE)
            {
                subscription.EndTrial();
                await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                return OperationResult.Success();
            }

            // Para planos pagos, o gateway deve processar a cobrança
            // Por enquanto, apenas finaliza o trial e marca como ativa
            // O gateway processará a primeira cobrança via webhook
            // EndTrial() atualiza o status para ACTIVE se TrialEnd <= DateTime.UtcNow
            subscription.EndTrial();
            // Garantir que está ativa (EndTrial() pode não atualizar se TrialEnd não foi definido corretamente)
            subscription.UpdateStatus(SubscriptionStatus.ACTIVE);

            await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Trial converted to active for subscription {SubscriptionId}",
                subscription.Id);

            // Enviar notificação ao usuário sobre fim do trial
            await SendTrialEndedNotificationAsync(subscription, cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting trial for subscription {SubscriptionId}", subscription.Id);
            return OperationResult.Failure($"Error converting trial: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém assinaturas com trial que expira em breve (para notificações).
    /// </summary>
    public async Task<IReadOnlyList<Subscription>> GetTrialsExpiringSoonAsync(
        int daysBeforeExpiration,
        CancellationToken cancellationToken)
    {
        var activeSubscriptions = await _subscriptionRepository.ListAsync(
            userId: null,
            territoryId: null,
            status: SubscriptionStatus.TRIALING,
            cancellationToken);

        var expirationDate = DateTime.UtcNow.AddDays(daysBeforeExpiration);

        return activeSubscriptions
            .Where(s => s.TrialEnd.HasValue &&
                      s.TrialEnd.Value <= expirationDate &&
                      s.TrialEnd.Value > DateTime.UtcNow)
            .ToList();
    }

    /// <summary>
    /// Envia notificação ao usuário sobre fim do trial.
    /// </summary>
    private async Task SendTrialEndedNotificationAsync(
        Subscription subscription,
        CancellationToken cancellationToken)
    {
        try
        {
            var plan = await _planRepository.GetByIdAsync(subscription.PlanId, cancellationToken);
            var planName = plan?.Name ?? "seu plano";

            var payload = new NotificationDispatchPayload(
                "subscription.trial_ended",
                new[] { subscription.UserId },
                "Seu período de trial terminou",
                $"Seu período de trial do plano {planName} terminou. " +
                "Sua assinatura está agora ativa e as cobranças serão processadas automaticamente.",
                new Dictionary<string, string>
                {
                    ["subscriptionId"] = subscription.Id.ToString(),
                    ["planId"] = subscription.PlanId.ToString(),
                    ["planName"] = planName
                });

            var message = new OutboxMessage(
                Guid.NewGuid(),
                "notification.dispatch",
                JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                DateTime.UtcNow);

            await _outbox.EnqueueAsync(message, cancellationToken);

            _logger.LogInformation(
                "Trial ended notification queued for subscription {SubscriptionId}, user {UserId}",
                subscription.Id,
                subscription.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending trial ended notification for subscription {SubscriptionId}", subscription.Id);
            // Não falha a conversão se a notificação falhar
        }
    }
}
