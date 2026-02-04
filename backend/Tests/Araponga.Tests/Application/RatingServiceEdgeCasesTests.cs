using Araponga.Application.Services;
using Araponga.Modules.Marketplace.Domain;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for RatingService (store not found, item not found, comment too long, RespondToRating validation).
/// </summary>
public sealed class RatingServiceEdgeCasesTests
{
    private static RatingService CreateService(InMemoryDataStore ds)
    {
        return new RatingService(
            new InMemoryStoreRatingRepository(ds),
            new InMemoryStoreItemRatingRepository(ds),
            new InMemoryStoreRatingResponseRepository(ds),
            new InMemoryStoreRepository(ds),
            new InMemoryStoreItemRepository(ds),
            new InMemoryCheckoutRepository(ds),
            new InMemoryUnitOfWork());
    }

    [Fact]
    public async Task RateStoreAsync_WhenStoreNotFound_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var userId = Guid.NewGuid();

        var result = await svc.RateStoreAsync(Guid.NewGuid(), userId, 4, null, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RateStoreAsync_WhenCommentExceeds2000_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var storeRepo = new InMemoryStoreRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var storeId = Guid.NewGuid();
        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var userId = Guid.NewGuid();
        var store = new Store(
            storeId,
            territoryId,
            userId,
            "Test Store",
            "Desc",
            StoreStatus.Active,
            false,
            StoreContactVisibility.Public,
            null, null, null, null, null, null,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await storeRepo.AddAsync(store, CancellationToken.None);
        await uow.CommitAsync(CancellationToken.None);

        var svc = CreateService(ds);
        var longComment = new string('x', 2001);

        var result = await svc.RateStoreAsync(storeId, userId, 4, longComment, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("2000", result.Error ?? "");
    }

    [Fact]
    public async Task RateItemAsync_WhenItemNotFound_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var userId = Guid.NewGuid();

        var result = await svc.RateItemAsync(Guid.NewGuid(), userId, 4, null, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RateItemAsync_WhenRatingOutOfRange_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var userId = Guid.NewGuid();

        var result = await svc.RateItemAsync(Guid.NewGuid(), userId, 0, null, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("1", result.Error ?? "");
        Assert.Contains("5", result.Error ?? "");
    }

    [Fact]
    public async Task RespondToRatingAsync_WhenEmptyResponse_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var storeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ratingId = Guid.NewGuid();

        var result = await svc.RespondToRatingAsync(ratingId, storeId, userId, "", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("required", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RespondToRatingAsync_WhenStoreNotFound_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var userId = Guid.NewGuid();

        var result = await svc.RespondToRatingAsync(Guid.NewGuid(), Guid.NewGuid(), userId, "Thanks!", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }
}
