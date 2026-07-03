using Arah.Application.Common;
using Arah.Application.Interfaces;

namespace Arah.Infrastructure.Payments;

/// <summary>
/// Gateway mock de cobrança para desenvolvimento e testes (DOD-05).
/// Em produção, substituir por Stripe PaymentIntent ou Mercado Pago PIX.
/// </summary>
public sealed class MockPaymentGateway : IPaymentGateway
{
    private readonly Dictionary<string, PaymentIntentStatus> _payments = new();
    private int _counter = 1;

    public Task<OperationResult<PaymentIntentResult>> CreatePaymentIntentAsync(
        Guid checkoutId,
        long amountInCents,
        string currency,
        PaymentMethod method,
        Dictionary<string, string>? metadata,
        CancellationToken cancellationToken)
    {
        var paymentId = $"pi_mock_{_counter++}_{checkoutId:N}";
        _payments[paymentId] = PaymentIntentStatus.Pending;

        var pixCode = method == PaymentMethod.Pix
            ? $"00020126580014BR.GOV.BCB.PIX0136{paymentId}"
            : null;

        var result = new PaymentIntentResult(
            paymentId,
            PaymentIntentStatus.Pending,
            method,
            pixCode,
            DateTime.UtcNow);

        return Task.FromResult(OperationResult<PaymentIntentResult>.Success(result));
    }

    public Task<OperationResult<PaymentStatusResult>> GetPaymentStatusAsync(
        string gatewayPaymentId,
        CancellationToken cancellationToken)
    {
        if (!_payments.TryGetValue(gatewayPaymentId, out var status))
        {
            return Task.FromResult(OperationResult<PaymentStatusResult>.Failure("Payment not found."));
        }

        var approvedAt = status == PaymentIntentStatus.Approved ? DateTime.UtcNow : (DateTime?)null;
        return Task.FromResult(OperationResult<PaymentStatusResult>.Success(
            new PaymentStatusResult(gatewayPaymentId, status, null, approvedAt)));
    }

    /// <summary>Simula aprovação de pagamento (testes e webhook mock).</summary>
    public void SimulatePaymentApproved(string gatewayPaymentId)
    {
        _payments[gatewayPaymentId] = PaymentIntentStatus.Approved;
    }
}
