namespace Araponga.Api.Contracts.Marketplace;

public sealed record CreateListingRequest(
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
    string? Status);

public sealed record UpdateListingRequest(
    string? Type,
    string? Title,
    string? Description,
    string? Category,
    string? Tags,
    string? PricingType,
    decimal? PriceAmount,
    string? Currency,
    string? Unit,
    double? Latitude,
    double? Longitude,
    string? Status);

public sealed record ListingResponse(
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
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
