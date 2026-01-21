namespace Araponga.Application.Models;

public sealed class SearchFilters
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Currency { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public double? MinRating { get; set; }
    public SearchSortOrder SortOrder { get; set; } = SearchSortOrder.Relevance;
}

public enum SearchSortOrder
{
    Relevance,
    PriceAscending,
    PriceDescending,
    DateDescending,
    RatingDescending
}
