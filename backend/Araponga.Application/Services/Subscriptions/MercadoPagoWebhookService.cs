using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Subscriptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para processar webhooks do Mercado Pago relacionados a assinaturas.
/// </summary>
public sealed class MercadoPagoWebhookService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ISubscriptionPaymentRepository _paymentRepository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MercadoPagoWebhookService> _logger;

    public MercadoPagoWebhookService(
        ISubscriptionRepository subscriptionRepository,
        ISubscriptionPaymentRepository paymentRepository,
        ISubscriptionPlanRepository planRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<MercadoPagoWebhookService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
        _planRepository = planRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Processa um evento do Mercado Pago.
    /// </summary>
    public async Task<OperationResult> ProcessEventAsync(
        string eventType,
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing Mercado Pago event: {EventType}", eventType);

            return eventType switch
            {
                "subscription.created" => await HandleSubscriptionCreatedAsync(eventData, cancellationToken),
                "subscription.updated" => await HandleSubscriptionUpdatedAsync(eventData, cancellationToken),
                "subscription.cancelled" => await HandleSubscriptionCancelledAsync(eventData, cancellationToken),
                "payment.created" => await HandlePaymentCreatedAsync(eventData, cancellationToken),
                "payment.approved" => await HandlePaymentApprovedAsync(eventData, cancellationToken),
                "payment.rejected" => await HandlePaymentRejectedAsync(eventData, cancellationToken),
                _ => OperationResult.Success() // Ignora eventos desconhecidos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Mercado Pago event: {EventType}", eventType);
            return OperationResult.Failure($"Error processing event: {ex.Message}");
        }
    }

    private async Task<OperationResult> HandleSubscriptionCreatedAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscriptionObj = eventData;
            var mpSubscriptionId = subscriptionObj.TryGetProperty("id", out var idProp) 
                ? idProp.GetString() 
                : null;
            var mpCustomerId = subscriptionObj.TryGetProperty("payer_id", out var payerProp)
                ? payerProp.GetString()
                : null;
            var status = subscriptionObj.TryGetProperty("status", out var statusProp)
                ? statusProp.GetString()
                : null;

            if (string.IsNullOrEmpty(mpSubscriptionId))
            {
                return OperationResult.Failure("Subscription ID is required");
            }

            // Buscar assinatura local pelo Mercado Pago Subscription ID
            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                mpSubscriptionId, // Reutiliza o campo para armazenar ID do Mercado Pago
                cancellationToken);

            if (subscription != null)
            {
                // Atualizar assinatura existente
                subscription.UpdateStripeIds(mpSubscriptionId, mpCustomerId);
                subscription.UpdateStatus(MapMercadoPagoStatus(status ?? "active"));
                
                await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
            }
            else
            {
                _logger.LogWarning(
                    "Subscription created event received but local subscription not found: {MercadoPagoSubscriptionId}",
                    mpSubscriptionId);
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
            var subscriptionObj = eventData;
            var mpSubscriptionId = subscriptionObj.TryGetProperty("id", out var idProp)
                ? idProp.GetString()
                : null;
            var status = subscriptionObj.TryGetProperty("status", out var statusProp)
                ? statusProp.GetString()
                : null;

            if (string.IsNullOrEmpty(mpSubscriptionId))
            {
                return OperationResult.Failure("Subscription ID is required");
            }

            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                mpSubscriptionId,
                cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Subscription updated event received but local subscription not found: {MercadoPagoSubscriptionId}",
                    mpSubscriptionId);
                return OperationResult.Failure("Subscription not found");
            }

            subscription.UpdateStatus(MapMercadoPagoStatus(status ?? "active"));
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

    private async Task<OperationResult> HandleSubscriptionCancelledAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscriptionObj = eventData;
            var mpSubscriptionId = subscriptionObj.TryGetProperty("id", out var idProp)
                ? idProp.GetString()
                : null;

            if (string.IsNullOrEmpty(mpSubscriptionId))
            {
                return OperationResult.Failure("Subscription ID is required");
            }

            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                mpSubscriptionId,
                cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Subscription cancelled event received but local subscription not found: {MercadoPagoSubscriptionId}",
                    mpSubscriptionId);
                return OperationResult.Success(); // Já foi cancelado ou não existe
            }

            subscription.Cancel(cancelAtPeriodEnd: false);
            subscription.UpdateStatus(SubscriptionStatus.CANCELED);

            await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription.cancelled event");
            return OperationResult.Failure($"Error handling subscription.cancelled: {ex.Message}");
        }
    }

    private Task<OperationResult> HandlePaymentCreatedAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        // Mercado Pago envia payment.created antes de payment.approved
        // Por enquanto, apenas loga
        _logger.LogInformation("Payment created event received from Mercado Pago");
        return Task.FromResult(OperationResult.Success());
    }

    private async Task<OperationResult> HandlePaymentApprovedAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var paymentObj = eventData;
            var mpPaymentId = paymentObj.TryGetProperty("id", out var idProp)
                ? idProp.GetInt64().ToString()
                : null;
            var mpSubscriptionId = paymentObj.TryGetProperty("subscription_id", out var subProp)
                ? subProp.GetString()
                : null;

            if (string.IsNullOrEmpty(mpSubscriptionId))
            {
                _logger.LogWarning("Payment approved event without subscription ID");
                return OperationResult.Success(); // Pode ser um pagamento único
            }

            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                mpSubscriptionId,
                cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Payment approved event received but subscription not found: {MercadoPagoSubscriptionId}",
                    mpSubscriptionId);
                return OperationResult.Failure("Subscription not found");
            }

            // Buscar ou criar registro de pagamento
            var existingPayment = await _paymentRepository.GetByStripeInvoiceIdAsync(
                mpPaymentId ?? string.Empty,
                cancellationToken);

            if (existingPayment == null)
            {
                var amount = paymentObj.TryGetProperty("transaction_amount", out var amountProp)
                    ? amountProp.GetDecimal()
                    : 0m;

                var currency = paymentObj.TryGetProperty("currency_id", out var currencyProp)
                    ? currencyProp.GetString() ?? "BRL"
                    : "BRL";

                var paymentDate = paymentObj.TryGetProperty("date_created", out var dateProp)
                    ? DateTime.Parse(dateProp.GetString() ?? DateTime.UtcNow.ToString())
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
                    mpPaymentId);

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
            _logger.LogError(ex, "Error handling payment.approved event");
            return OperationResult.Failure($"Error handling payment.approved: {ex.Message}");
        }
    }

    private async Task<OperationResult> HandlePaymentRejectedAsync(
        JsonElement eventData,
        CancellationToken cancellationToken)
    {
        try
        {
            var paymentObj = eventData;
            var mpPaymentId = paymentObj.TryGetProperty("id", out var idProp)
                ? idProp.GetInt64().ToString()
                : null;
            var mpSubscriptionId = paymentObj.TryGetProperty("subscription_id", out var subProp)
                ? subProp.GetString()
                : null;

            if (string.IsNullOrEmpty(mpSubscriptionId))
            {
                return OperationResult.Success(); // Pode ser um pagamento único
            }

            var subscription = await _subscriptionRepository.GetByStripeSubscriptionIdAsync(
                mpSubscriptionId,
                cancellationToken);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Payment rejected event received but subscription not found: {MercadoPagoSubscriptionId}",
                    mpSubscriptionId);
                return OperationResult.Failure("Subscription not found");
            }

            // Buscar ou criar registro de pagamento
            var existingPayment = await _paymentRepository.GetByStripeInvoiceIdAsync(
                mpPaymentId ?? string.Empty,
                cancellationToken);

            var failureReason = paymentObj.TryGetProperty("status_detail", out var detailProp)
                ? detailProp.GetString() ?? "Payment rejected"
                : "Payment rejected";

            if (existingPayment == null)
            {
                var amount = paymentObj.TryGetProperty("transaction_amount", out var amountProp)
                    ? amountProp.GetDecimal()
                    : 0m;

                var currency = paymentObj.TryGetProperty("currency_id", out var currencyProp)
                    ? currencyProp.GetString() ?? "BRL"
                    : "BRL";

                var paymentDate = paymentObj.TryGetProperty("date_created", out var dateProp)
                    ? DateTime.Parse(dateProp.GetString() ?? DateTime.UtcNow.ToString())
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
                    mpPaymentId,
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
            _logger.LogError(ex, "Error handling payment.rejected event");
            return OperationResult.Failure($"Error handling payment.rejected: {ex.Message}");
        }
    }

    private static SubscriptionStatus MapMercadoPagoStatus(string mpStatus)
    {
        return mpStatus.ToLowerInvariant() switch
        {
            "authorized" or "active" => SubscriptionStatus.ACTIVE,
            "cancelled" or "canceled" => SubscriptionStatus.CANCELED,
            "paused" => SubscriptionStatus.PAST_DUE,
            "pending" => SubscriptionStatus.UNPAID,
            _ => SubscriptionStatus.ACTIVE
        };
    }
}
