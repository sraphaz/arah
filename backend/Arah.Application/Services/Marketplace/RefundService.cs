using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Modules.Marketplace.Application.Interfaces;
using Arah.Modules.Marketplace.Domain;

namespace Arah.Application.Services;

public sealed record TransactionRefundResult(
    Guid TransactionId,
    CheckoutStatus Status,
    string Currency,
    decimal ReversedGrossAmount,
    decimal ReversedFeeAmount,
    decimal ReversedNetToSeller,
    IReadOnlyList<FeeSplitLine> ReversedSplit,
    Guid FeeSplitRuleId,
    DateTimeOffset RefundedAtUtc);

/// <summary>
/// Estorno FASE55 (AC-55-6) — reverte fee e splits proporcionalmente no ledger (DOD-06).
/// Idempotente: um checkout só pode ser estornado uma vez.
/// </summary>
public sealed class RefundService
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly IFeeSplitRuleRepository _feeSplitRules;
    private readonly SellerPayoutService _sellerPayoutService;

    public RefundService(
        ICheckoutRepository checkoutRepository,
        IFeeSplitRuleRepository feeSplitRules,
        SellerPayoutService sellerPayoutService)
    {
        _checkoutRepository = checkoutRepository;
        _feeSplitRules = feeSplitRules;
        _sellerPayoutService = sellerPayoutService;
    }

    public async Task<Result<TransactionRefundResult>> RefundAsync(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        var checkout = await _checkoutRepository.GetByIdAsync(transactionId, cancellationToken);
        if (checkout is null)
        {
            return Result<TransactionRefundResult>.Failure("Transaction not found.");
        }

        if (checkout.Status == CheckoutStatus.Refunded)
        {
            return Result<TransactionRefundResult>.Failure("Transaction is already refunded.");
        }

        if (checkout.Status != CheckoutStatus.Paid)
        {
            return Result<TransactionRefundResult>.Failure("Only paid transactions can be refunded.");
        }

        if (checkout.ItemsSubtotalAmount is null || checkout.PlatformFeeAmount is null)
        {
            return Result<TransactionRefundResult>.Failure("Checkout amounts are not finalized.");
        }

        var atUtc = DateTimeOffset.UtcNow;
        var rule = await _feeSplitRules.GetActiveAsync(
            checkout.TerritoryId,
            TransactionQuoteService.DefaultRevenueType,
            atUtc,
            cancellationToken);

        if (rule is null)
        {
            return Result<TransactionRefundResult>.Failure("No active fee split rule for territory.");
        }

        var ledgerResult = await _sellerPayoutService.ReversePaidCheckoutAsync(transactionId, cancellationToken);
        if (ledgerResult.IsFailure)
        {
            return Result<TransactionRefundResult>.Failure(ledgerResult.Error ?? "Ledger reversal failed.");
        }

        var gross = checkout.ItemsSubtotalAmount.Value;
        var fee = checkout.PlatformFeeAmount.Value;
        var net = FeeSplitCalculator.RoundBankers(gross - fee);
        var reversedSplit = FeeSplitCalculator.Reverse(FeeSplitCalculator.Build(fee, rule));

        checkout.SetStatus(CheckoutStatus.Refunded, atUtc.UtcDateTime);
        await _checkoutRepository.UpdateAsync(checkout, cancellationToken);

        return Result<TransactionRefundResult>.Success(new TransactionRefundResult(
            checkout.Id,
            checkout.Status,
            checkout.Currency,
            -gross,
            -fee,
            -net,
            reversedSplit,
            rule.Id,
            atUtc));
    }
}
