using Araponga.Application.Services;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for InquiryService (item not found, ListMy empty, ListReceived empty).
/// </summary>
public sealed class InquiryServiceEdgeCasesTests
{
    private static InquiryService CreateService(InMemoryDataStore ds)
    {
        var featureFlags = new InMemoryFeatureFlagService();
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var featureFlagCache = new FeatureFlagCacheService(featureFlags, cache);
        var guard = new TerritoryFeatureFlagGuard(featureFlagCache);
        return new InquiryService(
            new InMemoryInquiryRepository(ds),
            new InMemoryStoreItemRepository(ds),
            new InMemoryStoreRepository(ds),
            guard,
            new InMemoryUnitOfWork());
    }

    [Fact]
    public async Task CreateInquiryAsync_WhenItemNotFound_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var userId = Guid.NewGuid();

        var result = await svc.CreateInquiryAsync(
            Guid.NewGuid(),
            userId,
            "Message",
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListMyInquiriesAsync_WhenUserHasNone_ReturnsEmpty()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var userId = Guid.NewGuid();

        var list = await svc.ListMyInquiriesAsync(userId, CancellationToken.None);

        Assert.NotNull(list);
        Assert.Empty(list);
    }

    [Fact]
    public async Task ListReceivedInquiriesAsync_WhenOwnerHasNoStores_ReturnsEmpty()
    {
        var ds = new InMemoryDataStore();
        ds.TerritoryStores.Clear();
        var svc = CreateService(ds);
        var ownerId = Guid.NewGuid();

        var list = await svc.ListReceivedInquiriesAsync(ownerId, CancellationToken.None);

        Assert.NotNull(list);
        Assert.Empty(list);
    }
}
