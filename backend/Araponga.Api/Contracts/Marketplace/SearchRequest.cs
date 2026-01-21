namespace Araponga.Api.Contracts.Marketplace;

public sealed record SearchRequest(
    string? Query,
    string? Category,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? Currency,
    double? Latitude,
    double? Longitude,
    double? RadiusKm,
    double? MinRating,
    string? SortOrder);
