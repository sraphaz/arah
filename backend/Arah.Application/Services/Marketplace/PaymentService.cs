using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Modules.Marketplace.Application.Interfaces;
using Arah.Modules.Marketplace.Domain;

namespace Arah.Application.Services;

public sealed record PaymentInitiateResult(
    Guid TransactionId,
    CheckoutStatus Status,
    string GatewayPaymentId,
    PaymentMethod Method,
    string? PixCopyPasteCode,
    DateTimeOffset InitiatedAtUtc);

public sealed record PaymentConfirmResult(
    Guid TransactionId,
    CheckoutStatus Status,
    string GatewayPaymentId,
    DateTime PaidAtUtc,
    bool AlreadyPaid);

/// <summary>
/// Transição de pagamento Checkout → Paid (DOD-05). Após confirmar, delega ledger a
/// <see cref="SellerPayoutService.ProcessPaidCheckoutAsync"/>.
/// </summary>
public sealed class PaymentService
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly SellerPayoutService _sellerPayoutService;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(
        ICheckoutRepository checkoutRepository,
        IPaymentGateway paymentGateway,
        SellerPayoutService sellerPayoutService,
        IUnitOfWork unitOfWork)
    {
        _checkoutRepository = checkoutRepository;
        _paymentGateway = paymentGateway;
        _sellerPayoutService = sellerPayoutService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaymentInitiateResult>> InitiatePaymentAsync(
        Guid transactionId,
        PaymentMethod method,
        CancellationToken cancellationToken)
    {
        var checkout = await _checkoutRepository.GetByIdAsync(transactionId, cancellationToken);
        if (checkout is null)
        {
            return Result<PaymentInitiateResult>.Failure("Transaction not found.");
        }

        if (checkout.Status == CheckoutStatus.Paid)
        {
            return Result<PaymentInitiateResult>.Failure("Transaction is already paid.");
        }

        if (checkout.Status == CheckoutStatus.Refunded || checkout.Status == CheckoutStatus.Canceled)
        {
            return Result<PaymentInitiateResult>.Failure($"Cannot pay transaction in status {checkout.Status}.");
        }

        if (checkout.TotalAmount is null)
        {
            return Result<PaymentInitiateResult>.Failure("Checkout amounts are not finalized.");
        }

        var atUtc = DateTimeOffset.UtcNow;

        if (checkout.Status == CheckoutStatus.Created)
        {
            checkout.MarkAsAwaitingPayment(atUtc.UtcDateTime);
        }

        var amountInCents = (long)(checkout.TotalAmount.Value * 100);
        var gatewayResult = await _paymentGateway.CreatePaymentIntentAsync(
            checkout.Id,
            amountInCents,
            checkout.Currency,
            method,
            new Dictionary<string, string> { { "checkoutId", checkout.Id.ToString() } },
            cancellationToken);

        if (gatewayResult.IsFailure)
        {
            return Result<PaymentInitiateResult>.Failure(gatewayResult.Error ?? "Payment initiation failed.");
        }

        await _checkoutRepository.UpdateAsync(checkout, cancellationToken);
        // Persiste a transição para AwaitingPayment (necessário no Postgres, que só grava no commit).
        await _unitOfWork.CommitAsync(cancellationToken);

        var intent = gatewayResult.Value!;
        return Result<PaymentInitiateResult>.Success(new PaymentInitiateResult(
            checkout.Id,
            checkout.Status,
            intent.GatewayPaymentId,
            intent.Method,
            intent.PixCopyPasteCode,
            atUtc));
    }

    public async Task<Result<PaymentConfirmResult>> ConfirmPaymentAsync(
        Guid transactionId,
        string gatewayPaymentId,
        CancellationToken cancellationToken)
    {
        var checkout = await _checkoutRepository.GetByIdAsync(transactionId, cancellationToken);
        if (checkout is null)
        {
            return Result<PaymentConfirmResult>.Failure("Transaction not found.");
        }

        if (checkout.Status == CheckoutStatus.Paid)
        {
            // Idempotente e auto-recuperável: garante que o ledger existe mesmo que um
            // processamento anterior tenha falhado após marcar o checkout como Paid.
            var healResult = await _sellerPayoutService.ProcessPaidCheckoutAsync(checkout.Id, cancellationToken);
            if (healResult.IsFailure)
            {
                return Result<PaymentConfirmResult>.Failure(healResult.Error ?? "Failed to process paid checkout.");
            }

            return Result<PaymentConfirmResult>.Success(new PaymentConfirmResult(
                checkout.Id,
                checkout.Status,
                gatewayPaymentId,
                checkout.UpdatedAtUtc,
                AlreadyPaid: true));
        }

        if (checkout.Status is not (CheckoutStatus.Created or CheckoutStatus.AwaitingPayment))
        {
            return Result<PaymentConfirmResult>.Failure(
                $"Cannot confirm payment for transaction in status {checkout.Status}.");
        }

        var statusResult = await _paymentGateway.GetPaymentStatusAsync(gatewayPaymentId, cancellationToken);
        if (statusResult.IsFailure)
        {
            return Result<PaymentConfirmResult>.Failure(statusResult.Error ?? "Payment status unavailable.");
        }

        // Impede que um pagamento aprovado de outra transação confirme este checkout.
        if (statusResult.Value!.CheckoutId is { } paymentCheckoutId && paymentCheckoutId != transactionId)
        {
            return Result<PaymentConfirmResult>.Failure(
                "Payment does not belong to this transaction.");
        }

        if (statusResult.Value.Status != PaymentIntentStatus.Approved)
        {
            return Result<PaymentConfirmResult>.Failure(
                $"Payment not approved. Gateway status: {statusResult.Value.Status}.");
        }

        var paidAt = statusResult.Value.ApprovedAtUtc ?? DateTime.UtcNow;
        checkout.MarkAsPaid(paidAt);
        await _checkoutRepository.UpdateAsync(checkout, cancellationToken);

        var payoutResult = await _sellerPayoutService.ProcessPaidCheckoutAsync(checkout.Id, cancellationToken);
        if (payoutResult.IsFailure)
        {
            return Result<PaymentConfirmResult>.Failure(payoutResult.Error ?? "Failed to process paid checkout.");
        }

        return Result<PaymentConfirmResult>.Success(new PaymentConfirmResult(
            checkout.Id,
            checkout.Status,
            gatewayPaymentId,
            paidAt,
            AlreadyPaid: false));
    }
}
