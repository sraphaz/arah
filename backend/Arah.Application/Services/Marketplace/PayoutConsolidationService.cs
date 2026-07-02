using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Modules.Marketplace.Application.Interfaces;
using Arah.Modules.Marketplace.Domain;

namespace Arah.Application.Services;

public sealed record ConsolidatedPayoutLine(string Recipient, decimal Amount);

public sealed record ConsolidatedPayoutResult(
    Guid TerritoryId,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc,
    string Currency,
    int TransactionCount,
    decimal TotalGrossAmount,
    decimal TotalFeeAmount,
    IReadOnlyList<ConsolidatedPayoutLine> Lines);

/// <summary>
/// Payout consolidado por período (AC-55-5). Agrega o split das transações pagas
/// (não estornadas) de um território no intervalo, por destinatário. Read-model:
/// deriva de checkouts pagos (fonte de verdade), sem mutar estado.
/// </summary>
public sealed class PayoutConsolidationService
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly IFeeSplitRuleRepository _feeSplitRules;

    public PayoutConsolidationService(
        ICheckoutRepository checkoutRepository,
        IFeeSplitRuleRepository feeSplitRules)
    {
        _checkoutRepository = checkoutRepository;
        _feeSplitRules = feeSplitRules;
    }

    public async Task<Result<ConsolidatedPayoutResult>> GetConsolidatedAsync(
        Guid territoryId,
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken cancellationToken)
    {
        if (fromUtc > toUtc)
        {
            return Result<ConsolidatedPayoutResult>.Failure("fromUtc must be earlier than or equal to toUtc.");
        }

        var all = await _checkoutRepository.ListAllAsync(cancellationToken);
        var paid = all
            .Where(c =>
                c.TerritoryId == territoryId &&
                c.Status == CheckoutStatus.Paid &&
                c.PlatformFeeAmount is not null &&
                c.ItemsSubtotalAmount is not null &&
                c.UpdatedAtUtc >= fromUtc.UtcDateTime &&
                c.UpdatedAtUtc <= toUtc.UtcDateTime)
            .ToList();

        var accumulator = new Dictionary<string, decimal>(StringComparer.Ordinal);
        decimal totalGross = 0m;
        decimal totalFee = 0m;
        string currency = "BRL";

        foreach (var checkout in paid)
        {
            var rule = await _feeSplitRules.GetActiveAsync(
                territoryId,
                TransactionQuoteService.DefaultRevenueType,
                new DateTimeOffset(checkout.UpdatedAtUtc, TimeSpan.Zero),
                cancellationToken);

            if (rule is null)
            {
                continue;
            }

            currency = checkout.Currency;
            var fee = checkout.PlatformFeeAmount!.Value;
            totalGross += checkout.ItemsSubtotalAmount!.Value;
            totalFee += fee;

            foreach (var line in FeeSplitCalculator.Build(fee, rule))
            {
                accumulator[line.Recipient] = accumulator.GetValueOrDefault(line.Recipient) + line.Amount;
            }
        }

        var lines = accumulator
            .Select(kv => new ConsolidatedPayoutLine(kv.Key, kv.Value))
            .OrderBy(l => l.Recipient)
            .ToList();

        return Result<ConsolidatedPayoutResult>.Success(new ConsolidatedPayoutResult(
            territoryId,
            fromUtc,
            toUtc,
            currency,
            paid.Count,
            totalGross,
            totalFee,
            lines));
    }
}
