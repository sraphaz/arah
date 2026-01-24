using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class RatingServiceTests
{
    [Fact]
    public async Task RateStoreAsync_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var storeRatingRepository = new InMemoryStoreRatingRepository(dataStore);
        var itemRatingRepository = new InMemoryStoreItemRatingRepository(dataStore);
        var ratingResponseRepository = new InMemoryStoreRatingResponseRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new RatingService(
            storeRatingRepository,
            itemRatingRepository,
            ratingResponseRepository,
            storeRepository,
            itemRepository,
            checkoutRepository,
            unitOfWork);

        var storeId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Criar loja
        var store = new Store(
            storeId,
            Guid.NewGuid(),
            userId,
            "Test Store",
            "Description",
            StoreStatus.Active,
            false,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await storeRepository.AddAsync(store, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Act
        var result = await service.RateStoreAsync(storeId, userId, 5, "Great store!", CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(5, result.Value.Rating);
        Assert.Equal("Great store!", result.Value.Comment);
    }

    [Fact]
    public async Task RateStoreAsync_WhenInvalidRating_ReturnsFailure()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var storeRatingRepository = new InMemoryStoreRatingRepository(dataStore);
        var itemRatingRepository = new InMemoryStoreItemRatingRepository(dataStore);
        var ratingResponseRepository = new InMemoryStoreRatingResponseRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new RatingService(
            storeRatingRepository,
            itemRatingRepository,
            ratingResponseRepository,
            storeRepository,
            itemRepository,
            checkoutRepository,
            unitOfWork);

        var storeId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var store = new Store(
            storeId,
            Guid.NewGuid(),
            userId,
            "Test Store",
            "Description",
            StoreStatus.Active,
            false,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await storeRepository.AddAsync(store, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Act
        var result = await service.RateStoreAsync(storeId, userId, 6, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("between 1 and 5", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RateStoreAsync_WhenStoreNotFound_ReturnsFailure()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var storeRatingRepository = new InMemoryStoreRatingRepository(dataStore);
        var itemRatingRepository = new InMemoryStoreItemRatingRepository(dataStore);
        var ratingResponseRepository = new InMemoryStoreRatingResponseRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new RatingService(
            storeRatingRepository,
            itemRatingRepository,
            ratingResponseRepository,
            storeRepository,
            itemRepository,
            checkoutRepository,
            unitOfWork);

        // Act
        var result = await service.RateStoreAsync(Guid.NewGuid(), Guid.NewGuid(), 5, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RateItemAsync_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var storeRatingRepository = new InMemoryStoreRatingRepository(dataStore);
        var itemRatingRepository = new InMemoryStoreItemRatingRepository(dataStore);
        var ratingResponseRepository = new InMemoryStoreRatingResponseRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new RatingService(
            storeRatingRepository,
            itemRatingRepository,
            ratingResponseRepository,
            storeRepository,
            itemRepository,
            checkoutRepository,
            unitOfWork);

        var storeId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Criar loja e item
        var territoryId = Guid.NewGuid();
        var store = new Store(
            storeId,
            territoryId,
            userId,
            "Test Store",
            "Description",
            StoreStatus.Active,
            false,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await storeRepository.AddAsync(store, CancellationToken.None);

        var item = new StoreItem(
            itemId,
            territoryId,
            storeId,
            ItemType.Product,
            "Test Item",
            "Description",
            null,
            null,
            ItemPricingType.Fixed,
            100.0m,
            "BRL",
            null,
            null,
            null,
            ItemStatus.Active,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await itemRepository.AddAsync(item, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Act
        var result = await service.RateItemAsync(itemId, userId, 4, "Good item", CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(4, result.Value.Rating);
        Assert.Equal("Good item", result.Value.Comment);
    }
}
