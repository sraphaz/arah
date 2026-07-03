using System.Collections.Concurrent;
using Arah.Application.Common;
using Arah.Application.Interfaces;

namespace Arah.Infrastructure.Payments;

/// <summary>
/// Gateway mock de cobrança para desenvolvimento e testes (DOD-05).
/// Em produção, substituir por Stripe PaymentIntent ou Mercado Pago PIX.
/// Registrado como singleton: o estado das intenções precisa sobreviver entre requisições
/// (criar intenção e confirmar/consultar acontecem em requisições distintas).
/// </summary>
public sealed class MockPaymentGateway : IPaymentGateway
{
    private readonly ConcurrentDictionary<string, PaymentRecord> _payments = new();
    private int _counter;

    private sealed record PaymentRecord(Guid CheckoutId, PaymentIntentStatus Status);

    public Task<OperationResult<PaymentIntentResult>> CreatePaymentIntentAsync(
        Guid checkoutId,
        long amountInCents,
        string currency,
        PaymentMethod method,
        Dictionary<string, string>? metadata,
        CancellationToken cancellationToken)
    {
        var sequence = Interlocked.Increment(ref _counter);
        var paymentId = $"pi_mock_{sequence}_{checkoutId:N}";
        _payments[paymentId] = new PaymentRecord(checkoutId, PaymentIntentStatus.Pending);

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
        if (!_payments.TryGetValue(gatewayPaymentId, out var record))
        {
            return Task.FromResult(OperationResult<PaymentStatusResult>.Failure("Payment not found."));
        }

        var approvedAt = record.Status == PaymentIntentStatus.Approved ? DateTime.UtcNow : (DateTime?)null;
        return Task.FromResult(OperationResult<PaymentStatusResult>.Success(
            new PaymentStatusResult(gatewayPaymentId, record.Status, null, approvedAt, record.CheckoutId)));
    }

    /// <summary>Simula aprovação de pagamento (testes e webhook mock).</summary>
    public void SimulatePaymentApproved(string gatewayPaymentId)
    {
        _payments.AddOrUpdate(
            gatewayPaymentId,
            _ => new PaymentRecord(Guid.Empty, PaymentIntentStatus.Approved),
            (_, existing) => existing with { Status = PaymentIntentStatus.Approved });
    }
}
