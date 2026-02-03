using Araponga.Domain.Feed;
using Araponga.Domain.Map;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Moderation;
using Araponga.Domain.Membership;
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
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var territories = await repository.ListAsync(CancellationToken.None);
        Assert.NotEmpty(territories);

        var territory = await repository.GetByIdAsync(TerritoryId, CancellationToken.None);
        Assert.NotNull(territory);
        Assert.Equal(TerritoryId, territory!.Id);
    }

    [Fact]
    public async Task UserRepository_AddAndGetByAuthProvider()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryUserRepository(sharedStore);

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
            DateTime.UtcNow);

        await repository.AddAsync(user, CancellationToken.None);

        var found = await repository.GetByAuthProviderAsync("google", "test-external-id", CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(user.Id, found!.Id);
        Assert.Equal("google", found.AuthProvider);
        Assert.Equal("test-external-id", found.ExternalId);
    }

    [Fact]
    public async Task MembershipRepository_GetByUserAndTerritory()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryMembershipRepository(sharedStore);

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
            UserId,
            TerritoryId,
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

        var store = new Store(
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
    public async Task StoreItemRepository_AddAndSearch()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreItemRepository(dataStore);
        var storeId = Guid.NewGuid();

        var item = new StoreItem(
            Guid.NewGuid(),
            TerritoryId,
            storeId,
            ItemType.Product,
            "Test Product",
            "Description",
            "Category",
            "tags",
            ItemPricingType.Fixed,
            10m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await repository.AddAsync(item, CancellationToken.None);

        var results = await repository.SearchAsync(
            TerritoryId,
            ItemType.Product,
            "Test",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.Contains(results, i => i.Id == item.Id);
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

    [Fact]
    public async Task FeedRepository_ListByTerritoryPagedAsync_ReturnsPagedResults()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryFeedRepository(dataStore);
        var userId = dataStore.Users[0].Id;

        // Obter contagem inicial (pode haver posts pré-existentes)
        var initialCount = await repository.CountByTerritoryAsync(TerritoryId, CancellationToken.None);

        // Criar múltiplos posts
        for (int i = 0; i < 15; i++)
        {
            var post = new CommunityPost(
                Guid.NewGuid(),
                TerritoryId,
                userId,
                $"Post {i}",
                $"Content {i}",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow.AddMinutes(-i));
            await repository.AddPostAsync(post, CancellationToken.None);
        }

        // Primeira página
        var page1 = await repository.ListByTerritoryPagedAsync(TerritoryId, 0, 10, CancellationToken.None);
        Assert.True(page1.Count <= 10);
        Assert.True(page1.Count > 0);

        // Segunda página
        var page2 = await repository.ListByTerritoryPagedAsync(TerritoryId, 10, 10, CancellationToken.None);
        Assert.True(page2.Count > 0);

        // Contagem total deve incluir os novos posts
        var totalCount = await repository.CountByTerritoryAsync(TerritoryId, CancellationToken.None);
        Assert.True(totalCount >= initialCount + 15);
        Assert.True(totalCount >= 15);
    }

    [Fact]
    public async Task TerritoryRepository_ListPagedAsync_ReturnsPagedResults()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryTerritoryRepository(sharedStore);

        var page1 = await repository.ListPagedAsync(0, 5, CancellationToken.None);
        Assert.True(page1.Count <= 5);

        var totalCount = await repository.CountAsync(CancellationToken.None);
        Assert.True(totalCount >= 0);
    }

    [Fact]
    public async Task StoreItemRepository_SearchPagedAsync_ReturnsPagedResults()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryStoreItemRepository(dataStore);

        // Criar alguns itens
        for (int i = 0; i < 12; i++)
        {
            var item = new StoreItem(
                Guid.NewGuid(),
                TerritoryId,
                Guid.NewGuid(),
                ItemType.Product,
                $"Product {i}",
                $"Description {i}",
                "Category",
                "tags",
                ItemPricingType.Fixed,
                10m,
                "BRL",
                "unidade",
                null,
                null,
                ItemStatus.Active,
                DateTime.UtcNow,
                DateTime.UtcNow);
            await repository.AddAsync(item, CancellationToken.None);
        }

        var page1 = await repository.SearchPagedAsync(
            TerritoryId,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            0,
            10,
            CancellationToken.None);
        Assert.True(page1.Count <= 10);

        var totalCount = await repository.CountSearchAsync(
            TerritoryId,
            null,
            null,
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);
        Assert.True(totalCount >= 12);
    }
}
