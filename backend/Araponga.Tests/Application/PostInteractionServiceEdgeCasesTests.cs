using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for PostInteractionService (Like, Comment, Share — not found, non-resident, etc.).
/// </summary>
public sealed class PostInteractionServiceEdgeCasesTests
{
    private static readonly Guid TerritoryB = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid ResidentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid CuratorId = Guid.Parse("cccccccc-aaaa-aaaa-aaaa-aaaaaaaaaaaa"); // Visitor, not Resident

    private static PostInteractionService CreateService(InMemoryDataStore ds, InMemorySharedStore sharedStore)
    {
        var feedRepo = new InMemoryFeedRepository(ds);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var userRepo = new InMemoryUserRepository(sharedStore);
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var settingsRepo = new InMemoryMembershipSettingsRepository(sharedStore);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(sharedStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var accessRules = new MembershipAccessRules(membershipRepo, settingsRepo, userRepo, featureFlags);
        var permissionRepo = new InMemorySystemPermissionRepository(sharedStore);
        var accessEvaluator = new AccessEvaluator(
            membershipRepo,
            capabilityRepo,
            permissionRepo,
            accessRules,
            cache);
        var sanctionRepo = new InMemorySanctionRepository(ds);
        var auditLogger = new InMemoryAuditLogger(ds);
        var uow = new InMemoryUnitOfWork();
        return new PostInteractionService(feedRepo, accessEvaluator, sanctionRepo, auditLogger, uow);
    }

    [Fact]
    public async Task LikeAsync_WhenPostNotFound_ReturnsFalse()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);

        var result = await svc.LikeAsync(
            TerritoryB,
            Guid.NewGuid(),
            ResidentId.ToString(),
            ResidentId,
            CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task LikeAsync_WhenWrongTerritory_ReturnsFalse()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);
        var postId = ds.Posts[0].Id;
        var otherTerritory = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var result = await svc.LikeAsync(
            otherTerritory,
            postId,
            ResidentId.ToString(),
            ResidentId,
            CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task CommentAsync_WhenPostNotFound_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);

        var result = await svc.CommentAsync(
            TerritoryB,
            Guid.NewGuid(),
            ResidentId,
            "A comment",
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CommentAsync_WhenNonResident_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);
        var postId = ds.Posts[0].Id;

        var result = await svc.CommentAsync(
            TerritoryB,
            postId,
            CuratorId,
            "A comment",
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("resident", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ShareAsync_WhenPostNotFound_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);

        var result = await svc.ShareAsync(
            TerritoryB,
            Guid.NewGuid(),
            ResidentId,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ShareAsync_WhenNonResident_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var svc = CreateService(ds, sharedStore);
        var postId = ds.Posts[0].Id;

        var result = await svc.ShareAsync(
            TerritoryB,
            postId,
            CuratorId,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("resident", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }
}
