using Araponga.Domain.Feed;
using Araponga.Domain.Map;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Moderation;
using Araponga.Domain.Social;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class RepositoryTests
{
    private static readonly Guid TerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task TerritoryRepository_ListAndGetById()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryRepository(dataStore);

        var territories = await repository.ListAsync(CancellationToken.None);
        Assert.NotEmpty(territories);

        var territory = await repository.GetByIdAsync(TerritoryId, CancellationToken.None);
        Assert.NotNull(territory);
        Assert.Equal(TerritoryId, territory!.Id);
    }

    [Fact]
    public async Task UserRepository_AddAndGetByProvider()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryUserRepository(dataStore);

        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@araponga.com",
            "123.456.789-00",
            null,
            "(11) 90000-0000",
            "Rua Teste",
            "google",
            "test-external-id",
            UserRole.Visitor,
            DateTime.UtcNow);

        await repository.AddAsync(user, CancellationToken.None);

        var found = await repository.GetByProviderAsync("google", "test-external-id", CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(user.Id, found!.Id);
    }

    [Fact]
    public async Task MembershipRepository_GetByUserAndTerritory()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);

        var membership = await repository.GetByUserAndTerritoryAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.NotNull(membership);
        Assert.Equal(UserId, membership!.UserId);
        Assert.Equal(TerritoryId, membership.TerritoryId);
    }

    [Fact]
    public async Task FeedRepository_AddAndGetPost()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);

        var post = new CommunityPost(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            "Test Post",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await repository.AddPostAsync(post, CancellationToken.None);

        var found = await repository.GetPostAsync(post.Id, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(post.Id, found!.Id);
        Assert.Equal("Test Post", found.Title);
    }

    [Fact]
    public async Task MapRepository_ListEntities()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryMapRepository(dataStore);

        var entities = await repository.ListByTerritoryAsync(TerritoryId, CancellationToken.None);
        Assert.NotEmpty(entities);
    }

    [Fact]
    public async Task ReportRepository_AddAndList()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryReportRepository(dataStore);

        var report = new ModerationReport(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            ReportTargetType.Post,
            Guid.NewGuid(),
            "SPAM",
            "Detalhes",
            ReportStatus.Open,
            DateTime.UtcNow);

        await repository.AddAsync(report, CancellationToken.None);

        var reports = await repository.ListAsync(
            TerritoryId,
            ReportTargetType.Post,
            ReportStatus.Open,
            null,
            null,
            CancellationToken.None);

        Assert.Contains(reports, r => r.Id == report.Id);
    }

    [Fact]
    public async Task StoreRepository_AddAndGetByOwner()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreRepository(dataStore);

        var store = new TerritoryStore(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
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

        await repository.AddAsync(store, CancellationToken.None);

        var found = await repository.GetByOwnerAsync(TerritoryId, UserId, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(store.Id, found!.Id);
    }

    [Fact]
    public async Task ListingRepository_AddAndSearch()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryListingRepository(dataStore);
        var storeId = Guid.NewGuid();

        var listing = new StoreListing(
            Guid.NewGuid(),
            TerritoryId,
            storeId,
            ListingType.Product,
            "Test Product",
            "Description",
            "Category",
            "tags",
            ListingPricingType.Fixed,
            10m,
            "BRL",
            "unidade",
            null,
            null,
            ListingStatus.Active,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await repository.AddAsync(listing, CancellationToken.None);

        var results = await repository.SearchAsync(
            TerritoryId,
            ListingType.Product,
            "Test",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        Assert.Contains(results, l => l.Id == listing.Id);
    }

    [Fact]
    public async Task CartRepository_AddAndGet()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryCartRepository(dataStore);

        var cart = new Cart(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await repository.AddAsync(cart, CancellationToken.None);

        var found = await repository.GetByUserAsync(TerritoryId, UserId, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(cart.Id, found!.Id);
    }
}
