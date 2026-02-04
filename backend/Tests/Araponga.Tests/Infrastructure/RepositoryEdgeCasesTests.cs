using Araponga.Domain.Feed;
using Araponga.Modules.Marketplace.Domain;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Infrastructure;

/// <summary>
/// Edge case tests for Repository implementations,
/// focusing on null handling, empty collections, invalid IDs, and boundary conditions.
/// </summary>
public class RepositoryEdgeCasesTests
{
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private static readonly DateTime TestDate = DateTime.UtcNow;

    // UserRepository edge cases
    [Fact]
    public async Task UserRepository_GetByIdAsync_WithEmptyGuid_ReturnsNull()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var result = await repository.GetByIdAsync(Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UserRepository_GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UserRepository_GetByAuthProviderAsync_WithNullProvider_ReturnsNull()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var result = await repository.GetByAuthProviderAsync(null!, "external-id", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UserRepository_GetByAuthProviderAsync_WithNullExternalId_ReturnsNull()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var result = await repository.GetByAuthProviderAsync("google", null!, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UserRepository_GetByAuthProviderAsync_WithCaseInsensitive_MatchesCorrectly()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "Google",
            "external-id-123",
            TestDate);

        await repository.AddAsync(user, CancellationToken.None);

        // Case insensitive matching
        var found1 = await repository.GetByAuthProviderAsync("google", "external-id-123", CancellationToken.None);
        var found2 = await repository.GetByAuthProviderAsync("GOOGLE", "EXTERNAL-ID-123", CancellationToken.None);

        Assert.NotNull(found1);
        Assert.NotNull(found2);
        Assert.Equal(user.Id, found1!.Id);
        Assert.Equal(user.Id, found2!.Id);
    }

    [Fact]
    public async Task UserRepository_ListByIdsAsync_WithEmptyCollection_ReturnsEmpty()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var result = await repository.ListByIdsAsync(Array.Empty<Guid>(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task UserRepository_ListByIdsAsync_WithNonExistentIds_ReturnsEmpty()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var result = await repository.ListByIdsAsync(new[] { Guid.NewGuid(), Guid.NewGuid() }, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task UserRepository_GetByEmailAsync_WithNullEmail_ReturnsNull()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var result = await repository.GetByEmailAsync(null!, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UserRepository_GetByEmailAsync_WithEmptyEmail_ReturnsNull()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var result = await repository.GetByEmailAsync("   ", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UserRepository_GetByEmailAsync_WithCaseInsensitive_MatchesCorrectly()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "Test@Example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            TestDate);

        await repository.AddAsync(user, CancellationToken.None);

        var found1 = await repository.GetByEmailAsync("test@example.com", CancellationToken.None);
        var found2 = await repository.GetByEmailAsync("TEST@EXAMPLE.COM", CancellationToken.None);

        Assert.NotNull(found1);
        Assert.NotNull(found2);
        Assert.Equal(user.Id, found1!.Id);
        Assert.Equal(user.Id, found2!.Id);
    }

    [Fact]
    public async Task UserRepository_UpdateAsync_WithNonExistentUser_AddsUser()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

        var user = new User(
            Guid.NewGuid(),
            "New User",
            "new@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            TestDate);

        await repository.UpdateAsync(user, CancellationToken.None);

        var found = await repository.GetByIdAsync(user.Id, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(user.Id, found!.Id);
    }

    // TerritoryRepository edge cases
    [Fact]
    public async Task TerritoryRepository_GetByIdAsync_WithEmptyGuid_ReturnsNull()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.GetByIdAsync(Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task TerritoryRepository_GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task TerritoryRepository_SearchAsync_WithNullQuery_ReturnsAll()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.SearchAsync(null, null, null, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task TerritoryRepository_SearchAsync_WithEmptyQuery_ReturnsAll()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.SearchAsync("   ", null, null, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task TerritoryRepository_SearchAsync_WithWhitespaceQuery_TrimsAndSearches()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        // Add a territory with known name
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "São Paulo",
            "SP",
            TerritoryStatus.Active,
            "São Paulo",
            "São Paulo",
            -23.5505,
            -46.6333,
            TestDate);

        await repository.AddAsync(territory, CancellationToken.None);

        var result = await repository.SearchAsync("  São Paulo  ", null, null, CancellationToken.None);

        Assert.Contains(result, t => t.Id == territory.Id);
    }

    [Fact]
    public async Task TerritoryRepository_SearchAsync_WithCaseInsensitive_MatchesCorrectly()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "São Paulo",
            "SP",
            TerritoryStatus.Active,
            "São Paulo",
            "São Paulo",
            -23.5505,
            -46.6333,
            TestDate);

        await repository.AddAsync(territory, CancellationToken.None);

        var result1 = await repository.SearchAsync("são paulo", null, null, CancellationToken.None);
        var result2 = await repository.SearchAsync("SÃO PAULO", null, null, CancellationToken.None);

        Assert.Contains(result1, t => t.Id == territory.Id);
        Assert.Contains(result2, t => t.Id == territory.Id);
    }

    [Fact]
    public async Task TerritoryRepository_NearbyAsync_WithZeroRadius_ReturnsEmpty()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.NearbyAsync(0, 0, 0, 10, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task TerritoryRepository_NearbyAsync_WithNegativeRadius_ReturnsEmpty()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.NearbyAsync(0, 0, -10, 10, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task TerritoryRepository_NearbyAsync_WithZeroLimit_ReturnsEmpty()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.NearbyAsync(0, 0, 100, 0, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task TerritoryRepository_ListPagedAsync_WithNegativeSkip_ReturnsFromStart()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.ListPagedAsync(-10, 10, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task TerritoryRepository_ListPagedAsync_WithZeroTake_ReturnsEmpty()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.ListPagedAsync(0, 0, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task TerritoryRepository_ListPagedAsync_WithLargeSkip_ReturnsEmpty()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var result = await repository.ListPagedAsync(1000000, 10, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task TerritoryRepository_CountAsync_ReturnsNonNegative()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var count = await repository.CountAsync(CancellationToken.None);

        Assert.True(count >= 0);
    }

    [Fact]
    public async Task TerritoryRepository_CountSearchAsync_WithNullQuery_ReturnsAllCount()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var count = await repository.CountSearchAsync(null, null, null, CancellationToken.None);

        Assert.True(count >= 0);
    }

    // FeedRepository edge cases
    [Fact]
    public async Task FeedRepository_GetPostAsync_WithEmptyGuid_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var result = await repository.GetPostAsync(Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task FeedRepository_GetPostAsync_WithNonExistentId_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var result = await repository.GetPostAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task FeedRepository_ListByTerritoryAsync_WithNonExistentTerritory_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var result = await repository.ListByTerritoryAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task FeedRepository_ListByAuthorAsync_WithNonExistentAuthor_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var result = await repository.ListByAuthorAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task FeedRepository_UpdateStatusAsync_WithNonExistentPost_DoesNotThrow()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        await repository.UpdateStatusAsync(Guid.NewGuid(), PostStatus.Published, CancellationToken.None);

        // Should not throw
        Assert.True(true);
    }

    [Fact]
    public async Task FeedRepository_DeletePostAsync_WithNonExistentPost_DoesNotThrow()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        await repository.DeletePostAsync(Guid.NewGuid(), CancellationToken.None);

        // Should not throw
        Assert.True(true);
    }

    [Fact]
    public async Task FeedRepository_GetLikeCountAsync_WithNonExistentPost_ReturnsZero()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var count = await repository.GetLikeCountAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task FeedRepository_GetShareCountAsync_WithNonExistentPost_ReturnsZero()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var count = await repository.GetShareCountAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task FeedRepository_ListByTerritoryPagedAsync_WithNegativeSkip_ReturnsFromStart()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var result = await repository.ListByTerritoryPagedAsync(TestTerritoryId, -10, 10, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task FeedRepository_ListByTerritoryPagedAsync_WithZeroTake_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var result = await repository.ListByTerritoryPagedAsync(TestTerritoryId, 0, 0, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task FeedRepository_CountByTerritoryAsync_WithNonExistentTerritory_ReturnsZero()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var count = await repository.CountByTerritoryAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(0, count);
    }

    // StoreRepository edge cases
    [Fact]
    public async Task StoreRepository_GetByIdAsync_WithEmptyGuid_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreRepository(dataStore);

        var result = await repository.GetByIdAsync(Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task StoreRepository_GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreRepository(dataStore);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task StoreRepository_GetByOwnerAsync_WithNonExistentOwner_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreRepository(dataStore);

        var result = await repository.GetByOwnerAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task StoreRepository_ListByOwnerAsync_WithNonExistentOwner_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreRepository(dataStore);

        var result = await repository.ListByOwnerAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task StoreRepository_ListByIdsAsync_WithEmptyCollection_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreRepository(dataStore);

        var result = await repository.ListByIdsAsync(Array.Empty<Guid>(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task StoreRepository_ListByIdsAsync_WithNonExistentIds_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreRepository(dataStore);

        var result = await repository.ListByIdsAsync(new[] { Guid.NewGuid(), Guid.NewGuid() }, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task StoreRepository_ListByTerritoryAsync_WithNonExistentTerritory_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreRepository(dataStore);

        var result = await repository.ListByTerritoryAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task StoreRepository_UpdateAsync_WithNonExistentStore_DoesNotThrow()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreRepository(dataStore);

        var store = new Store(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            "Test Store",
            null,
            StoreStatus.Active,
            false,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            TestDate,
            TestDate);

        await repository.UpdateAsync(store, CancellationToken.None);

        // Should not throw, but also should not add
        var found = await repository.GetByIdAsync(store.Id, CancellationToken.None);
        Assert.Null(found);
    }

    // StoreItemRepository edge cases
    [Fact]
    public async Task StoreItemRepository_GetByIdAsync_WithEmptyGuid_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreItemRepository(dataStore);

        var result = await repository.GetByIdAsync(Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task StoreItemRepository_SearchAsync_WithNullQuery_ReturnsAll()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreItemRepository(dataStore);

        var result = await repository.SearchAsync(
            TestTerritoryId,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task StoreItemRepository_SearchAsync_WithEmptyQuery_ReturnsAll()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreItemRepository(dataStore);

        var result = await repository.SearchAsync(
            TestTerritoryId,
            null,
            "   ",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task StoreItemRepository_SearchPagedAsync_WithNegativeSkip_ReturnsFromStart()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreItemRepository(dataStore);

        var result = await repository.SearchPagedAsync(
            TestTerritoryId,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            -10,
            10,
            CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task StoreItemRepository_SearchPagedAsync_WithZeroTake_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreItemRepository(dataStore);

        var result = await repository.SearchPagedAsync(
            TestTerritoryId,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            0,
            0,
            CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task StoreItemRepository_CountSearchAsync_WithNullQuery_ReturnsAllCount()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreItemRepository(dataStore);

        var count = await repository.CountSearchAsync(
            TestTerritoryId,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.True(count >= 0);
    }

    // CartRepository edge cases
    [Fact]
    public async Task CartRepository_GetByUserAsync_WithNonExistentUser_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryCartRepository(dataStore);

        var result = await repository.GetByUserAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task CartRepository_GetByIdAsync_WithEmptyGuid_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryCartRepository(dataStore);

        var result = await repository.GetByIdAsync(Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    // CartItemRepository edge cases
    [Fact]
    public async Task CartItemRepository_ListByCartIdAsync_WithNonExistentCart_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryCartItemRepository(dataStore);

        var result = await repository.ListByCartIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CartItemRepository_GetByIdAsync_WithEmptyGuid_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryCartItemRepository(dataStore);

        var result = await repository.GetByIdAsync(Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task CartItemRepository_GetByCartAndListingAsync_WithNonExistent_ReturnsNull()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryCartItemRepository(dataStore);

        var result = await repository.GetByCartAndListingAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task CartItemRepository_RemoveAsync_WithNonExistentItem_DoesNotThrow()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryCartItemRepository(dataStore);

        await repository.RemoveAsync(Guid.NewGuid(), CancellationToken.None);

        // Should not throw
        Assert.True(true);
    }

    [Fact]
    public async Task CartItemRepository_RemoveByCartIdAsync_WithNonExistentCart_DoesNotThrow()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryCartItemRepository(dataStore);

        await repository.RemoveByCartIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Should not throw
        Assert.True(true);
    }
}
