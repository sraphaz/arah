using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for PostFilterService (empty posts, userId null, FilterAndPaginate).
/// </summary>
public sealed class PostFilterServiceEdgeCasesTests
{
    private static PostFilterService CreateService(InMemoryDataStore ds, InMemorySharedStore sharedStore)
    {
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var settingsRepo = new InMemoryMembershipSettingsRepository(sharedStore);
        var userRepo = new InMemoryUserRepository(sharedStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var accessRules = new MembershipAccessRules(membershipRepo, settingsRepo, userRepo, featureFlags);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(sharedStore);
        var permissionRepo = new InMemorySystemPermissionRepository(sharedStore);
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var accessEvaluator = new AccessEvaluator(
            membershipRepo,
            capabilityRepo,
            permissionRepo,
            accessRules,
            cache);
        var blockRepo = new InMemoryUserBlockRepository(ds);
        var postAssetRepo = new InMemoryPostAssetRepository(ds);
        return new PostFilterService(accessEvaluator, blockRepo, postAssetRepo, null);
    }

    [Fact]
    public async Task FilterPostsAsync_WithEmptyPosts_ReturnsEmpty()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);
        var territoryId = ds.Territories[0].Id;

        var result = await svc.FilterPostsAsync(
            Array.Empty<CommunityPost>(),
            territoryId,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FilterPostsAsync_WithUserIdNull_ReturnsOnlyPublicPublished()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);
        var territoryId = ds.Territories[0].Id;
        var authorId = Guid.NewGuid();
        var posts = new List<CommunityPost>
        {
            new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                authorId,
                "P1",
                "C1",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow),
            new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                authorId,
                "P2",
                "C2",
                PostType.General,
                PostVisibility.ResidentsOnly,
                PostStatus.Published,
                null,
                DateTime.UtcNow)
        };

        var result = await svc.FilterPostsAsync(
            posts,
            territoryId,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.All(result, p =>
        {
            Assert.Equal(PostVisibility.Public, p.Visibility);
            Assert.Equal(PostStatus.Published, p.Status);
        });
    }

    [Fact]
    public async Task FilterAndPaginateAsync_WithEmptyPosts_ReturnsEmptyPaged()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);
        var territoryId = ds.Territories[0].Id;
        var paging = new PaginationParameters(1, 10);

        var result = await svc.FilterAndPaginateAsync(
            Array.Empty<CommunityPost>(),
            territoryId,
            null,
            null,
            null,
            paging,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task FilterPostsAsync_WithMapEntityIdFilter_ReturnsOnlyMatchingPosts()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);
        var territoryId = ds.Territories[0].Id;
        var authorId = Guid.NewGuid();
        var mapEntityId = Guid.NewGuid();
        var posts = new List<CommunityPost>
        {
            new CommunityPost(Guid.NewGuid(), territoryId, authorId, "P1", "C1", PostType.General,
                PostVisibility.Public, PostStatus.Published, mapEntityId, DateTime.UtcNow),
            new CommunityPost(Guid.NewGuid(), territoryId, authorId, "P2", "C2", PostType.General,
                PostVisibility.Public, PostStatus.Published, Guid.NewGuid(), DateTime.UtcNow)
        };

        var result = await svc.FilterPostsAsync(
            posts,
            territoryId,
            null,
            mapEntityId,
            null,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(mapEntityId, result[0].MapEntityId);
    }
}
