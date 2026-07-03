using Arah.Application.Common;
using Arah.Application.Interfaces;

namespace Arah.Infrastructure.Payments;

/// <summary>
/// Gateway de cobrança no-op para ambientes sem PSP real configurado (ex.: produção antes de
/// integrar Stripe/Mercado Pago). Nunca cria intenções de pagamento falsas: retorna falha
/// explícita para que os endpoints <c>/pay</c> e <c>/confirm-payment</c> não iniciem cobranças
/// que não podem ser aprovadas. O <see cref="MockPaymentGateway"/> só é usado fora de produção.
/// </summary>
public sealed class UnavailablePaymentGateway : IPaymentGateway
{
    private const string NotConfigured =
        "Payment gateway not configured for this environment.";

    public Task<OperationResult<PaymentIntentResult>> CreatePaymentIntentAsync(
        Guid checkoutId,
        long amountInCents,
        string currency,
        PaymentMethod method,
        Dictionary<string, string>? metadata,
        CancellationToken cancellationToken) =>
        Task.FromResult(OperationResult<PaymentIntentResult>.Failure(NotConfigured));

    public Task<OperationResult<PaymentStatusResult>> GetPaymentStatusAsync(
        string gatewayPaymentId,
        CancellationToken cancellationToken) =>
        Task.FromResult(OperationResult<PaymentStatusResult>.Failure(NotConfigured));
}
