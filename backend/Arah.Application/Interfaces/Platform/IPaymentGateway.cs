using Arah.Application.Common;

namespace Arah.Application.Interfaces;

/// <summary>
/// Gateway de cobrança (entrada) — PIX, cartão, etc. Espelha <see cref="IPayoutGateway"/> para saída.
/// </summary>
public interface IPaymentGateway
{
    Task<OperationResult<PaymentIntentResult>> CreatePaymentIntentAsync(
        Guid checkoutId,
        long amountInCents,
        string currency,
        PaymentMethod method,
        Dictionary<string, string>? metadata,
        CancellationToken cancellationToken);

    Task<OperationResult<PaymentStatusResult>> GetPaymentStatusAsync(
        string gatewayPaymentId,
        CancellationToken cancellationToken);
}

/// <summary>Método de pagamento suportado pelo gateway.</summary>
public enum PaymentMethod
{
    Pix = 1,
    Stripe = 2
}

/// <summary>Resultado da criação de intenção de pagamento.</summary>
public record PaymentIntentResult(
    string GatewayPaymentId,
    PaymentIntentStatus Status,
    PaymentMethod Method,
    string? PixCopyPasteCode,
    DateTime CreatedAtUtc);

/// <summary>Status da intenção de pagamento no gateway.</summary>
public enum PaymentIntentStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Canceled = 4
}

/// <summary>Resultado da consulta de status de pagamento.</summary>
/// <param name="CheckoutId">
/// Checkout (transação) ao qual esta intenção de pagamento pertence, quando o gateway
/// consegue resolvê-lo. Usado para impedir que um pagamento aprovado de outra transação
/// confirme um checkout diferente. <c>null</c> quando o gateway não expõe o vínculo.
/// </param>
public record PaymentStatusResult(
    string GatewayPaymentId,
    PaymentIntentStatus Status,
    string? FailureReason,
    DateTime? ApprovedAtUtc,
    Guid? CheckoutId = null);
