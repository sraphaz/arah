namespace Arah.Api.Contracts.Transactions;

public sealed record ConsumptionMeterResponse(
    Guid Id,
    Guid SubscriptionId,
    string Metric,
    decimal Usage,
    decimal Quota,
    decimal OverageRate);

public sealed record WalletResponse(
    Guid Id,
    string OwnerType,
    Guid OwnerId,
    Guid TerritoryId,
    decimal Balance,
    string Currency,
    string? PayoutMethod,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);
