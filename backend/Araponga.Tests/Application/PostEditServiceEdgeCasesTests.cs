using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Services;
using Araponga.Application.Services.Media;
using Araponga.Domain.Feed;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for PostEditService (empty title, empty content, post not found).
/// </summary>
public sealed class PostEditServiceEdgeCasesTests
{
    private static PostEditService CreateService(InMemoryDataStore ds)
    {
        var feedRepo = new InMemoryFeedRepository(ds);
        var mediaAttachmentRepo = new InMemoryMediaAttachmentRepository(ds);
        var mediaAssetRepo = new InMemoryMediaAssetRepository(ds);
        var geoAnchorRepo = new InMemoryPostGeoAnchorRepository(ds);
        var featureFlags = new InMemoryFeatureFlagService();
        var mediaConfig = new TerritoryMediaConfigService(
            new InMemoryTerritoryMediaConfigRepository(ds),
            featureFlags,
            new InMemoryUnitOfWork(),
            new InMemoryGlobalMediaLimits());
        var uow = new InMemoryUnitOfWork();
        return new PostEditService(
            feedRepo,
            mediaAttachmentRepo,
            mediaAssetRepo,
            geoAnchorRepo,
            featureFlags,
            mediaConfig,
            uow,
            null);
    }

    [Fact]
    public async Task EditPostAsync_WithEmptyTitle_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var result = await svc.EditPostAsync(
            postId,
            userId,
            "",
            "Content",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Title", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task EditPostAsync_WithEmptyContent_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var result = await svc.EditPostAsync(
            postId,
            userId,
            "Title",
            "  ",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Content", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task EditPostAsync_WhenPostNotFound_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var result = await svc.EditPostAsync(
            postId,
            userId,
            "Title",
            "Content",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }
}
