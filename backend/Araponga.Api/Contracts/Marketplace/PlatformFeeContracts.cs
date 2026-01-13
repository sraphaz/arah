namespace Araponga.Api.Contracts.Marketplace;

public sealed record PlatformFeeRequest(
    Guid TerritoryId,
    string ListingType,
    string FeeMode,
    decimal FeeValue,
    string? Currency,
    bool IsActive);

public sealed record PlatformFeeResponse(
    Guid Id,
    Guid TerritoryId,
    string ListingType,
    string FeeMode,
    decimal FeeValue,
    string? Currency,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
