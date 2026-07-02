using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Modules.Marketplace.Application.Interfaces;
using Arah.Modules.Marketplace.Domain;

namespace Arah.Application.Services;

public sealed record FeeSplitLine(string Recipient, decimal Amount);

public sealed record TransactionQuoteResult(
    Guid TransactionId,
    Guid TerritoryId,
    Guid StoreId,
    string Currency,
    decimal GrossAmount,
    decimal FeeAmount,
    decimal NetToSeller,
    IReadOnlyList<FeeSplitLine> Split,
    Guid FeeSplitRuleId,
    DateTimeOffset QuotedAtUtc);

public sealed record TransactionReceiptResult(
    Guid TransactionId,
    CheckoutStatus Status,
    string Currency,
    decimal GrossAmount,
    decimal FeeAmount,
    decimal NetToSeller,
    IReadOnlyList<FeeSplitLine> Split,
    Guid FeeSplitRuleId,
    DateTimeOffset QuotedAtUtc,
    DateTime? PaidAtUtc);

/// <summary>
/// Quote e receipt FASE55 — checkout.Id como transactionId na v1.
/// </summary>
public sealed class TransactionQuoteService
{
    public const string DefaultRevenueType = "marketplace";

    private readonly ICheckoutRepository _checkoutRepository;
    private readonly IFeeSplitRuleRepository _feeSplitRules;

    public TransactionQuoteService(
        ICheckoutRepository checkoutRepository,
        IFeeSplitRuleRepository feeSplitRules)
    {
        _checkoutRepository = checkoutRepository;
        _feeSplitRules = feeSplitRules;
    }

    public async Task<Result<TransactionQuoteResult>> QuoteAsync(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        var checkout = await _checkoutRepository.GetByIdAsync(transactionId, cancellationToken);
        if (checkout is null)
        {
            return Result<TransactionQuoteResult>.Failure("Transaction not found.");
        }

        if (checkout.ItemsSubtotalAmount is null || checkout.PlatformFeeAmount is null || checkout.TotalAmount is null)
        {
            return Result<TransactionQuoteResult>.Failure("Checkout amounts are not finalized.");
        }

        var atUtc = DateTimeOffset.UtcNow;
        var rule = await _feeSplitRules.GetActiveAsync(
            checkout.TerritoryId,
            DefaultRevenueType,
            atUtc,
            cancellationToken);

        if (rule is null)
        {
            return Result<TransactionQuoteResult>.Failure("No active fee split rule for territory.");
        }

        var gross = checkout.ItemsSubtotalAmount.Value;
        var fee = checkout.PlatformFeeAmount.Value;
        var net = FeeSplitCalculator.RoundBankers(gross - fee);
        var split = FeeSplitCalculator.Build(fee, rule);

        return Result<TransactionQuoteResult>.Success(new TransactionQuoteResult(
            checkout.Id,
            checkout.TerritoryId,
            checkout.StoreId,
            checkout.Currency,
            gross,
            fee,
            net,
            split,
            rule.Id,
            atUtc));
    }

    public async Task<Result<TransactionReceiptResult>> GetReceiptAsync(
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        var quoteResult = await QuoteAsync(transactionId, cancellationToken);
        if (quoteResult.IsFailure)
        {
            return Result<TransactionReceiptResult>.Failure(quoteResult.Error ?? "Quote failed.");
        }

        var checkout = await _checkoutRepository.GetByIdAsync(transactionId, cancellationToken);
        if (checkout is null)
        {
            return Result<TransactionReceiptResult>.Failure("Transaction not found.");
        }

        if (checkout.Status != CheckoutStatus.Paid)
        {
            return Result<TransactionReceiptResult>.Failure("Receipt is available only for paid transactions.");
        }

        var quote = quoteResult.Value!;
        return Result<TransactionReceiptResult>.Success(new TransactionReceiptResult(
            quote.TransactionId,
            checkout.Status,
            quote.Currency,
            quote.GrossAmount,
            quote.FeeAmount,
            quote.NetToSeller,
            quote.Split,
            quote.FeeSplitRuleId,
            quote.QuotedAtUtc,
            checkout.UpdatedAtUtc));
    }

}
