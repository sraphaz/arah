using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Email;
using Araponga.Domain.Subscriptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para processar webhooks do Stripe relacionados a assinaturas.
/// </summary>
public sealed class StripeWebhookService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionPaymentRepository _paymentRepository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutbox _outbox;
    private readonly ILogger<StripeWebhookService> _logger;

    public StripeWebhookService(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionPaymentRepository paymentRepository,
        ISubscriptionPlanRepository planRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IOutbox outbox,
        ILogger<StripeWebhookService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
        _planRepository = planRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _logger = logger;
    }

    /// <summary>
    /// Processa um evento do Stripe.
    /// </summary>
    public async Task<OperationResult> ProcessEventAsync(
        string eventType,
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing Stripe event: {EventType}", eventType);

            return eventType switch
            {
                "customer.subscription.created" => await HandleSubscriptionCreatedAsync(eventData, cancellationToken),
                "customer.subscription.updated" => await HandleSubscriptionUpdatedAsync(eventData, cancellationToken),
                "customer.subscription.deleted" => await HandleSubscriptionDeletedAsync(eventData, cancellationToken),
                "invoice.payment_succeeded" => await HandlePaymentSucceededAsync(eventData, cancellationToken),
                "invoice.payment_failed" => await HandlePaymentFailedAsync(eventData, cancellationToken),
                "customer.subscription.trial_will_end" => await HandleTrialWillEndAsync(eventData, cancellationToken),
                _ => OperationResult.Success() // Ignora eventos desconhecidos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Stripe event: {EventType}", eventType);
            return OperationResult.Failure($"Error processing event: {ex.Message}");
        }
    }

    private async Task<OperationResult> HandleSubscriptionCreatedAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscriptionObj = eventData.GetProperty("object");
            var stripeSubscriptionId = subscriptionObj.GetProperty("id").GetString();
            var stripeCustomerId = subscriptionObj.GetProperty("customer").GetString();
            var status = subscriptionObj.GetProperty("status").GetString();
            var currentPeriodStart = DateTimeOffset.FromUnixTimeSeconds(
                subscriptionObj.GetProperty("current_period_start").GetInt64()).DateTime;
            var currentPeriodEnd = DateTimeOffset.FromUnixTimeSeconds(
                subscriptionObj.GetProperty("current_period_end").GetInt64()).DateTime;

            // Buscar assinatura local pelo StripeSubscriptionId
            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                stripeSubscriptionId!,
                cancellationToken);

            if (subscription != null)
            {
                // Atualizar assinatura existente
                subscription.UpdateStripeIds(stripeSubscriptionId, stripeCustomerId);
                subscription.UpdatePeriod(currentPeriodStart, currentPeriodEnd);
                subscription.UpdateStatus(MapStripeStatus(status!));
                
                await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
            }
            else
            {
                _logger.LogWarning(
                    "Subscription created event received but local subscription not found: {StripeSubscriptionId}",
                    stripeSubscriptionId);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription.created event");
            return OperationResult.Failure($"Error handling subscription.created: {ex.Message}");
        }
    }

    private async Task<OperationResult> HandleSubscriptionUpdatedAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscriptionObj = eventData.GetProperty("object");
            var stripeSubscriptionId = subscriptionObj.GetProperty("id").GetString();
            var status = subscriptionObj.GetProperty("status").GetString();
            var currentPeriodStart = DateTimeOffset.FromUnixTimeSeconds(
                subscriptionObj.GetProperty("current_period_start").GetInt64()).DateTime;
            var currentPeriodEnd = DateTimeOffset.FromUnixTimeSeconds(
                subscriptionObj.GetProperty("current_period_end").GetInt64()).DateTime;
            var cancelAtPeriodEnd = subscriptionObj.TryGetProperty("cancel_at_period_end", out var cancelProp) &&
                                     cancelProp.GetBoolean();

            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                stripeSubscriptionId!,
                cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Subscription updated event received but local subscription not found: {StripeSubscriptionId}",
                    stripeSubscriptionId);
                return OperationResult.Failure("Subscription not found");
            }

            subscription.UpdatePeriod(currentPeriodStart, currentPeriodEnd);
            subscription.UpdateStatus(MapStripeStatus(status!));

            if (cancelAtPeriodEnd && subscription.Status != SubscriptionStatus.CANCELED)
            {
                subscription.Cancel(cancelAtPeriodEnd: true);
            }

            await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription.updated event");
            return OperationResult.Failure($"Error handling subscription.updated: {ex.Message}");
        }
    }

    private async Task<OperationResult> HandleSubscriptionDeletedAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscriptionObj = eventData.GetProperty("object");
            var stripeSubscriptionId = subscriptionObj.GetProperty("id").GetString();

            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                stripeSubscriptionId!,
                cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Subscription deleted event received but local subscription not found: {StripeSubscriptionId}",
                    stripeSubscriptionId);
                return OperationResult.Success(); // Já foi deletado ou não existe
            }

            // Cancelar assinatura (volta para FREE será tratado pelo SubscriptionService)
            subscription.Cancel(cancelAtPeriodEnd: false);
            subscription.UpdateStatus(SubscriptionStatus.CANCELED);

            await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription.deleted event");
            return OperationResult.Failure($"Error handling subscription.deleted: {ex.Message}");
        }
    }

    private async Task<OperationResult> HandlePaymentSucceededAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var invoiceObj = eventData.GetProperty("object");
            var stripeInvoiceId = invoiceObj.GetProperty("id").GetString();
            var stripeSubscriptionId = invoiceObj.TryGetProperty("subscription", out var subProp)
                ? subProp.GetString()
                : null;

            if (string.IsNullOrEmpty(stripeSubscriptionId))
            {
                _logger.LogWarning("Payment succeeded event without subscription ID");
                return OperationResult.Success(); // Pode ser um pagamento único, não uma assinatura
            }

            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                stripeSubscriptionId,
                cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Payment succeeded event received but subscription not found: {StripeSubscriptionId}",
                    stripeSubscriptionId);
                return OperationResult.Failure("Subscription not found");
            }

            // Buscar ou criar registro de pagamento
            var existingPayment = await _paymentRepository.GetByStripeInvoiceIdAsync(
                stripeInvoiceId!,
                cancellationToken);

            if (existingPayment == null)
            {
                var amount = invoiceObj.TryGetProperty("amount_paid", out var amountProp)
                    ? amountProp.GetDecimal() / 100m // Stripe usa centavos
                    : 0m;

                var currency = invoiceObj.TryGetProperty("currency", out var currencyProp)
                    ? currencyProp.GetString() ?? "BRL"
                    : "BRL";

                var paymentDate = invoiceObj.TryGetProperty("created", out var createdProp)
                    ? DateTimeOffset.FromUnixTimeSeconds(createdProp.GetInt64()).DateTime
                    : DateTime.UtcNow;

                var periodStart = subscription.CurrentPeriodStart;
                var periodEnd = subscription.CurrentPeriodEnd;

                var payment = new SubscriptionPayment(
                    Guid.NewGuid(),
                    subscription.Id,
                    amount,
                    currency,
                    SubscriptionPaymentStatus.Succeeded,
                    paymentDate,
                    periodStart,
                    periodEnd,
                    stripeInvoiceId);

                await _paymentRepository.AddAsync(payment, cancellationToken);
            }
            else
            {
                existingPayment.UpdateStatus(SubscriptionPaymentStatus.Succeeded);
                await _paymentRepository.UpdateAsync(existingPayment, cancellationToken);
            }

            // Atualizar status da assinatura para ACTIVE se estava PAST_DUE
            if (subscription.Status == SubscriptionStatus.PAST_DUE)
            {
                subscription.UpdateStatus(SubscriptionStatus.ACTIVE);
                await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling payment.succeeded event");
            return OperationResult.Failure($"Error handling payment.succeeded: {ex.Message}");
        }
    }

    private async Task<OperationResult> HandlePaymentFailedAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var invoiceObj = eventData.GetProperty("object");
            var stripeInvoiceId = invoiceObj.GetProperty("id").GetString();
            var stripeSubscriptionId = invoiceObj.TryGetProperty("subscription", out var subProp)
                ? subProp.GetString()
                : null;

            if (string.IsNullOrEmpty(stripeSubscriptionId))
            {
                return OperationResult.Success(); // Pode ser um pagamento único
            }

            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                stripeSubscriptionId,
                cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Payment failed event received but subscription not found: {StripeSubscriptionId}",
                    stripeSubscriptionId);
                return OperationResult.Failure("Subscription not found");
            }

            // Buscar ou criar registro de pagamento
            var existingPayment = await _paymentRepository.GetByStripeInvoiceIdAsync(
                stripeInvoiceId!,
                cancellationToken);

            var failureReason = invoiceObj.TryGetProperty("last_payment_error", out var errorProp)
                ? errorProp.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : "Payment failed"
                : "Payment failed";

            if (existingPayment == null)
            {
                var amount = invoiceObj.TryGetProperty("amount_due", out var amountProp)
                    ? amountProp.GetDecimal() / 100m
                    : 0m;

                var currency = invoiceObj.TryGetProperty("currency", out var currencyProp)
                    ? currencyProp.GetString() ?? "BRL"
                    : "BRL";

                var paymentDate = invoiceObj.TryGetProperty("created", out var createdProp)
                    ? DateTimeOffset.FromUnixTimeSeconds(createdProp.GetInt64()).DateTime
                    : DateTime.UtcNow;

                var periodStart = subscription.CurrentPeriodStart;
                var periodEnd = subscription.CurrentPeriodEnd;

                var payment = new SubscriptionPayment(
                    Guid.NewGuid(),
                    subscription.Id,
                    amount,
                    currency,
                    SubscriptionPaymentStatus.Failed,
                    paymentDate,
                    periodStart,
                    periodEnd,
                    stripeInvoiceId,
                    failureReason: failureReason);

                await _paymentRepository.AddAsync(payment, cancellationToken);
            }
            else
            {
                existingPayment.UpdateStatus(SubscriptionPaymentStatus.Failed, failureReason);
                await _paymentRepository.UpdateAsync(existingPayment, cancellationToken);
            }

            // Atualizar status da assinatura para PAST_DUE
            subscription.UpdateStatus(SubscriptionStatus.PAST_DUE);
            await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling payment.failed event");
            return OperationResult.Failure($"Error handling payment.failed: {ex.Message}");
        }
    }

    private async Task<OperationResult> HandleTrialWillEndAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscriptionObj = eventData.GetProperty("object");
            var stripeSubscriptionId = subscriptionObj.GetProperty("id").GetString();

            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                stripeSubscriptionId!,
                cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Trial will end event received but subscription not found: {StripeSubscriptionId}",
                    stripeSubscriptionId);
                return OperationResult.Success();
            }

            // Enviar notificação ao usuário sobre fim do trial
            await SendTrialWillEndNotificationAsync(subscription, cancellationToken);

            _logger.LogInformation(
                "Trial will end soon for subscription {SubscriptionId}, user {UserId}",
                subscription.Id,
                subscription.UserId);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling trial_will_end event");
            return OperationResult.Failure($"Error handling trial_will_end: {ex.Message}");
        }
    }

    /// <summary>
    /// Envia notificação ao usuário sobre fim do trial.
    /// </summary>
    private async Task SendTrialWillEndNotificationAsync(
        Subscription subscription,
        CancellationToken cancellationToken)
    {
        try
        {
            var plan = await _planRepository.GetByIdAsync(subscription.PlanId, cancellationToken);
            var planName = plan?.Name ?? "seu plano";

            var daysRemaining = subscription.TrialEnd.HasValue
                ? (int)(subscription.TrialEnd.Value - DateTime.UtcNow).TotalDays
                : 0;

            var payload = new NotificationDispatchPayload(
                "subscription.trial_will_end",
                new[] { subscription.UserId },
                "Seu período de trial está terminando",
                $"Seu período de trial do plano {planName} termina em {daysRemaining} dia(s). " +
                "Para continuar usando os recursos premium, certifique-se de que seu método de pagamento está atualizado.",
                new Dictionary<string, string>
                {
                    ["subscriptionId"] = subscription.Id.ToString(),
                    ["planId"] = subscription.PlanId.ToString(),
                    ["planName"] = planName,
                    ["daysRemaining"] = daysRemaining.ToString(),
                    ["trialEndDate"] = subscription.TrialEnd?.ToString("O") ?? ""
                });

            var message = new OutboxMessage(
                Guid.NewGuid(),
                "notification.dispatch",
                JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                DateTime.UtcNow);

            await _outbox.EnqueueAsync(message, cancellationToken);

            _logger.LogInformation(
                "Trial will end notification queued for subscription {SubscriptionId}, user {UserId}",
                subscription.Id,
                subscription.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending trial will end notification for subscription {SubscriptionId}", subscription.Id);
            // Não falha o webhook se a notificação falhar
        }
    }

    private static SubscriptionStatus MapStripeStatus(string stripeStatus)
    {
        return stripeStatus switch
        {
            "active" => SubscriptionStatus.ACTIVE,
            "canceled" => SubscriptionStatus.CANCELED,
            "past_due" => SubscriptionStatus.PAST_DUE,
            "unpaid" => SubscriptionStatus.UNPAID,
            "trialing" => SubscriptionStatus.TRIALING,
            "incomplete" => SubscriptionStatus.UNPAID,
            "incomplete_expired" => SubscriptionStatus.EXPIRED,
            _ => SubscriptionStatus.ACTIVE
        };
    }
}
