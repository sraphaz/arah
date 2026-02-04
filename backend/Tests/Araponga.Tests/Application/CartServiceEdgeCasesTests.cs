using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for CartService (GetCartById not found, UpdateItem quantity &lt; 1).
/// </summary>
public sealed class CartServiceEdgeCasesTests
{
    private static readonly Guid TerritoryB = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static CartService CreateService(InMemoryDataStore ds)
    {
        var featureFlags = new InMemoryFeatureFlagService();
        featureFlags.SetEnabledFlags(TerritoryB, new List<FeatureFlag> { FeatureFlag.MarketplaceEnabled });
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var featureFlagCache = new FeatureFlagCacheService(featureFlags, cache);
        var guard = new TerritoryFeatureFlagGuard(featureFlagCache);
        return new CartService(
            new InMemoryCartRepository(ds),
            new InMemoryCartItemRepository(ds),
            new InMemoryStoreItemRepository(ds),
            new InMemoryStoreRepository(ds),
            new InMemoryCheckoutRepository(ds),
            new InMemoryCheckoutItemRepository(ds),
            new InMemoryInquiryRepository(ds),
            new InMemoryPlatformFeeConfigRepository(ds),
            guard,
            new InMemoryUnitOfWork(),
            null);
    }

    [Fact]
    public async Task GetCartByIdAsync_WhenCartNotFound_ReturnsSuccessNull()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var userId = Guid.NewGuid();

        var result = await svc.GetCartByIdAsync(Guid.NewGuid(), userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task UpdateItemAsync_WhenQuantityLessThanOne_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var userId = Guid.NewGuid();

        var result = await svc.UpdateItemAsync(
            Guid.NewGuid(),
            userId,
            0,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Quantity", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }
}
