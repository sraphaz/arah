namespace Araponga.Application.Models;

public sealed record CombinedSearchResult(
    IReadOnlyList<StoreSearchResult> Stores,
    IReadOnlyList<ItemSearchResult> Items,
    int TotalCount);
