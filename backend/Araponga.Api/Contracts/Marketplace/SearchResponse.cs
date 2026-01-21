using Araponga.Api.Contracts.Common;

namespace Araponga.Api.Contracts.Marketplace;

public sealed record SearchResponse(
    IReadOnlyList<StoreSearchResultResponse> Stores,
    IReadOnlyList<ItemSearchResultResponse> Items,
    int TotalCount);

public sealed record StoreSearchResultResponse(
    Guid Id,
    Guid TerritoryId,
    Guid OwnerUserId,
    string DisplayName,
    string? Description,
    string Status,
    bool PaymentsEnabled,
    double AverageRating,
    DateTime CreatedAtUtc);

public sealed record ItemSearchResultResponse(
    Guid Id,
    Guid TerritoryId,
    Guid StoreId,
    string Type,
    string Title,
    string? Description,
    string? Category,
    string? Tags,
    string PricingType,
    decimal? PriceAmount,
    string? Currency,
    string? Unit,
    double? Latitude,
    double? Longitude,
    string Status,
    double AverageRating,
    DateTime CreatedAtUtc);
