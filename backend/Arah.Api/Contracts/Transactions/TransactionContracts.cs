namespace Arah.Api.Contracts.Transactions;

public sealed record FeeSplitLineResponse(string Recipient, decimal Amount);

public sealed record TransactionQuoteResponse(
    Guid TransactionId,
    Guid TerritoryId,
    Guid StoreId,
    string Currency,
    decimal GrossAmount,
    decimal FeeAmount,
    decimal NetToSeller,
    IReadOnlyList<FeeSplitLineResponse> Split,
    Guid FeeSplitRuleId,
    DateTimeOffset QuotedAtUtc);

public sealed record TransactionReceiptResponse(
    Guid TransactionId,
    string Status,
    string Currency,
    decimal GrossAmount,
    decimal FeeAmount,
    decimal NetToSeller,
    IReadOnlyList<FeeSplitLineResponse> Split,
    Guid FeeSplitRuleId,
    DateTimeOffset QuotedAtUtc,
    DateTime? PaidAtUtc);

public sealed record TransactionRefundResponse(
    Guid TransactionId,
    string Status,
    string Currency,
    decimal ReversedGrossAmount,
    decimal ReversedFeeAmount,
    decimal ReversedNetToSeller,
    IReadOnlyList<FeeSplitLineResponse> ReversedSplit,
    Guid FeeSplitRuleId,
    DateTimeOffset RefundedAtUtc);

public sealed record ConsolidatedPayoutLineResponse(string Recipient, decimal Amount);

public sealed record ConsolidatedPayoutResponse(
    Guid TerritoryId,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc,
    string Currency,
    int TransactionCount,
    decimal TotalGrossAmount,
    decimal TotalFeeAmount,
    IReadOnlyList<ConsolidatedPayoutLineResponse> Lines);

public sealed record CommercialPlanResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? BillingCycle,
    IReadOnlyList<string> Capabilities);
