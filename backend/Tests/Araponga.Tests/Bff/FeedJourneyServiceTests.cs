using Araponga.Api.Contracts.Journeys.Feed;
using Araponga.Api.Services.Journeys;
using Araponga.Api.Services.Journeys.Backend;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Bff;

public sealed class FeedJourneyServiceTests
{
    private static readonly Guid TerritoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid UserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid PostId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static FeedJourneyService CreateService(Mock<IFeedJourneyBackend>? backendMock = null)
    {
        var mock = backendMock ?? new Mock<IFeedJourneyBackend>();
        var logger = new Mock<ILogger<FeedJourneyService>>();
        return new FeedJourneyService(mock.Object, logger.Object);
    }

    [Fact]
    public async Task GetTerritoryFeedAsync_ReturnsResponse_WhenBackendReturnsData()
    {
        var backend = new Mock<IFeedJourneyBackend>();
        backend.Setup(x => x.IsConnectionsPrioritizeEnabledAsync(TerritoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        backend.Setup(x => x.ListFeedPagedAsync(
                TerritoryId, UserId, 1, 20, false, false, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendPagedResult<BackendFeedPost>(
                new List<BackendFeedPost>
                {
                    new(PostId, TerritoryId, UserId, "T", "C", "POST", "PUBLIC", "PUBLISHED",
                        null, null, null, DateTime.UtcNow, null)
                },
                1, 20, 1, 1, false, false));
        backend.Setup(x => x.GetCountsByPostIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, BackendPostCounts> { { PostId, new BackendPostCounts(0, 0) } });
        backend.Setup(x => x.GetEventSummariesByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, BackendEventSummary>());
        backend.Setup(x => x.GetMediaUrlsByPostIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, IReadOnlyList<string>>());
        backend.Setup(x => x.GetUsersByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BackendUserInfo> { new(UserId, "User", null) });

        var service = CreateService(backend);
        var result = await service.GetTerritoryFeedAsync(TerritoryId, UserId, 1, 20, false, null, null);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(PostId, result.Items[0].Post.Id);
        Assert.Equal(1, result.Pagination.TotalCount);
    }

    [Fact]
    public async Task CreatePostAsync_ReturnsNull_WhenBackendFails()
    {
        var backend = new Mock<IFeedJourneyBackend>();
        backend.Setup(x => x.CreatePostAsync(
                TerritoryId, UserId, "T", "C", "POST", "PUBLIC", null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendCreatePostResult(false, null, "Error"));

        var service = CreateService(backend);
        var request = new CreatePostJourneyRequest("T", "C", "POST", "PUBLIC", TerritoryId, null, null);
        var result = await service.CreatePostAsync(TerritoryId, UserId, request, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreatePostAsync_ReturnsResponse_WhenBackendSucceeds()
    {
        var backend = new Mock<IFeedJourneyBackend>();
        var post = new BackendFeedPost(PostId, TerritoryId, UserId, "T", "C", "POST", "PUBLIC", "PUBLISHED",
            null, null, null, DateTime.UtcNow, null);
        backend.Setup(x => x.CreatePostAsync(
                TerritoryId, UserId, "T", "C", "POST", "PUBLIC", null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BackendCreatePostResult(true, post, null));
        backend.Setup(x => x.GetMediaUrlsByPostIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, IReadOnlyList<string>>());

        var service = CreateService(backend);
        var request = new CreatePostJourneyRequest("T", "C", "POST", "PUBLIC", TerritoryId, null, null);
        var result = await service.CreatePostAsync(TerritoryId, UserId, request, null);

        Assert.NotNull(result);
        Assert.Equal(PostId, result.Post.Id);
    }

    [Fact]
    public async Task InteractAsync_ReturnsNull_WhenActionIsInvalid()
    {
        var backend = new Mock<IFeedJourneyBackend>();
        var service = CreateService(backend);
        var request = new PostInteractionRequest(PostId, TerritoryId, "INVALID");
        var result = await service.InteractAsync(TerritoryId, UserId, "sess", request, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task InteractAsync_Like_ReturnsResponse_WhenBackendSucceeds()
    {
        var backend = new Mock<IFeedJourneyBackend>();
        backend.Setup(x => x.LikeAsync(TerritoryId, PostId, It.IsAny<string>(), UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var post = new BackendFeedPost(PostId, TerritoryId, UserId, "T", "C", "POST", "PUBLIC", "PUBLISHED",
            null, null, null, DateTime.UtcNow, null);
        backend.Setup(x => x.GetPostAsync(PostId, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        backend.Setup(x => x.GetCountsByPostIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, BackendPostCounts> { { PostId, new BackendPostCounts(1, 0) } });

        var service = CreateService(backend);
        var request = new PostInteractionRequest(PostId, TerritoryId, "LIKE");
        var result = await service.InteractAsync(TerritoryId, UserId, "sess", request, null);

        Assert.NotNull(result);
        Assert.True(result.UserInteractions.Liked);
    }

    [Fact]
    public async Task InteractAsync_Comment_ReturnsNull_WhenCommentContentEmpty()
    {
        var backend = new Mock<IFeedJourneyBackend>();
        var service = CreateService(backend);
        var request = new PostInteractionRequest(PostId, TerritoryId, "COMMENT");
        var result = await service.InteractAsync(TerritoryId, UserId, "sess", request, "");

        Assert.Null(result);
    }

    [Fact]
    public async Task InteractAsync_Share_ReturnsNull_WhenUserIdNull()
    {
        var backend = new Mock<IFeedJourneyBackend>();
        var service = CreateService(backend);
        var request = new PostInteractionRequest(PostId, TerritoryId, "SHARE");
        var result = await service.InteractAsync(TerritoryId, null, "sess", request, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task InteractAsync_ReturnsNull_WhenBackendLikeFails()
    {
        var backend = new Mock<IFeedJourneyBackend>();
        backend.Setup(x => x.LikeAsync(TerritoryId, PostId, It.IsAny<string>(), UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var service = CreateService(backend);
        var request = new PostInteractionRequest(PostId, TerritoryId, "LIKE");
        var result = await service.InteractAsync(TerritoryId, UserId, "sess", request, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task InteractAsync_ReturnsNull_WhenGetPostAfterLikeReturnsNull()
    {
        var backend = new Mock<IFeedJourneyBackend>();
        backend.Setup(x => x.LikeAsync(TerritoryId, PostId, It.IsAny<string>(), UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        backend.Setup(x => x.GetPostAsync(PostId, It.IsAny<CancellationToken>())).ReturnsAsync((BackendFeedPost?)null);
        var service = CreateService(backend);
        var request = new PostInteractionRequest(PostId, TerritoryId, "LIKE");
        var result = await service.InteractAsync(TerritoryId, UserId, "sess", request, null);

        Assert.Null(result);
    }
}
