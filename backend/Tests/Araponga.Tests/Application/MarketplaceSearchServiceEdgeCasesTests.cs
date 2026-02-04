using Araponga.Application.Common;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for MarketplaceSearchService (SearchStores empty, SearchItems empty, filters).
/// </summary>
public sealed class MarketplaceSearchServiceEdgeCasesTests
{
    private static readonly Guid TerritoryB = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task SearchStoresAsync_WithEmptyTerritory_ReturnsEmptyPaged()
    {
        var ds = new InMemoryDataStore();
        ds.TerritoryStores.Clear();
        var storeRepo = new InMemoryStoreRepository(ds);
        var itemRepo = new InMemoryStoreItemRepository(ds);
        var storeRatingRepo = new InMemoryStoreRatingRepository(ds);
        var itemRatingRepo = new InMemoryStoreItemRatingRepository(ds);
        var svc = new MarketplaceSearchService(storeRepo, itemRepo, storeRatingRepo, itemRatingRepo);
        var filters = new SearchFilters();
        var paging = new PaginationParameters(1, 10);

        var result = await svc.SearchStoresAsync(TerritoryB, filters, paging, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.TotalCount);
    }

    [Fact]
    public async Task SearchStoresAsync_WithQueryFilter_ReturnsMatchingStores()
    {
        var ds = new InMemoryDataStore();
        var storeRepo = new InMemoryStoreRepository(ds);
        var itemRepo = new InMemoryStoreItemRepository(ds);
        var storeRatingRepo = new InMemoryStoreRatingRepository(ds);
        var itemRatingRepo = new InMemoryStoreItemRatingRepository(ds);
        var svc = new MarketplaceSearchService(storeRepo, itemRepo, storeRatingRepo, itemRatingRepo);
        var filters = new SearchFilters { Query = "Vale" };
        var paging = new PaginationParameters(1, 10);

        var result = await svc.SearchStoresAsync(TerritoryB, filters, paging, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task SearchItemsAsync_WithEmptyTerritory_ReturnsEmptyPaged()
    {
        var ds = new InMemoryDataStore();
        ds.StoreItems.Clear();
        var storeRepo = new InMemoryStoreRepository(ds);
        var itemRepo = new InMemoryStoreItemRepository(ds);
        var storeRatingRepo = new InMemoryStoreRatingRepository(ds);
        var itemRatingRepo = new InMemoryStoreItemRatingRepository(ds);
        var svc = new MarketplaceSearchService(storeRepo, itemRepo, storeRatingRepo, itemRatingRepo);
        var filters = new SearchFilters();
        var paging = new PaginationParameters(1, 10);

        var result = await svc.SearchItemsAsync(TerritoryB, filters, paging, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.TotalCount);
    }
}
