using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Application.Interfaces.Media;
using Arah.Application.Models;
using Arah.Application.Services;
using Arah.Domain.Feed;
using Arah.Domain.Media;

namespace Arah.Api.Services.Journeys.Backend;

public sealed class InProcessFeedJourneyBackend : IFeedJourneyBackend
{
    private readonly FeedService _feedService;
    private readonly EventsService _eventsService;
    private readonly MediaService _mediaService;
    private readonly IUserRepository _userRepository;
    private readonly TerritoryFeatureFlagGuard _featureGuard;
    private readonly IMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly IPostGeoAnchorRepository _postGeoAnchorRepository;

    public InProcessFeedJourneyBackend(
        FeedService feedService,
        EventsService eventsService,
        MediaService mediaService,
        IUserRepository userRepository,
        TerritoryFeatureFlagGuard featureGuard,
        IMediaAttachmentRepository mediaAttachmentRepository,
        IPostGeoAnchorRepository postGeoAnchorRepository)
    {
        _feedService = feedService;
        _eventsService = eventsService;
        _mediaService = mediaService;
        _userRepository = userRepository;
        _featureGuard = featureGuard;
        _mediaAttachmentRepository = mediaAttachmentRepository;
        _postGeoAnchorRepository = postGeoAnchorRepository;
    }

    public Task<bool> IsConnectionsPrioritizeEnabledAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        var enabled = _featureGuard.IsEnabled(territoryId, FeatureFlag.ConnectionsEnabled)
            && _featureGuard.IsEnabled(territoryId, FeatureFlag.ConnectionsFeedPrioritize);
        return Task.FromResult(enabled);
    }

    public async Task<BackendPagedResult<BackendFeedPost>> ListFeedPagedAsync(
        Guid territoryId,
        Guid? userId,
        int pageNumber,
        int pageSize,
        bool filterByInterests,
        bool prioritizeConnections,
        Guid? mapEntityId,
        Guid? assetId,
        CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationParameters(pageNumber, pageSize);
        var paged = await _feedService.ListForTerritoryPagedAsync(
            territoryId, userId, mapEntityId, assetId, pagination,
            filterByInterests, prioritizeConnections, cancellationToken);
        var items = paged.Items.Select(ToBackendPost).ToList();
        return new BackendPagedResult<BackendFeedPost>(
            items, paged.PageNumber, paged.PageSize, paged.TotalCount, paged.TotalPages,
            paged.HasPreviousPage, paged.HasNextPage);
    }

    public async Task<IReadOnlyDictionary<Guid, BackendPostCounts>> GetCountsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken = default)
    {
        var counts = await _feedService.GetCountsByPostIdsAsync(postIds, cancellationToken);
        return counts.ToDictionary(kv => kv.Key, kv => new BackendPostCounts(kv.Value.LikeCount, kv.Value.ShareCount, kv.Value.CommentCount));
    }

    public async Task<BackendPagedResult<BackendPostComment>> ListCommentsPagedAsync(
        Guid territoryId,
        Guid postId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var pagination = new PaginationParameters(pageNumber, pageSize);
        var paged = await _feedService.ListCommentsForPostPagedAsync(
            territoryId,
            postId,
            pagination,
            cancellationToken);

        var items = paged.Items
            .Select(comment => new BackendPostComment(
                comment.Id,
                comment.PostId,
                comment.UserId,
                comment.Content,
                comment.CreatedAtUtc))
            .ToList();

        return new BackendPagedResult<BackendPostComment>(
            items,
            paged.PageNumber,
            paged.PageSize,
            paged.TotalCount,
            paged.TotalPages,
            paged.HasPreviousPage,
            paged.HasNextPage);
    }

    public async Task<IReadOnlyDictionary<Guid, BackendEventSummary>> GetEventSummariesByIdsAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken = default)
    {
        var dict = await _eventsService.GetSummariesByIdsAsync(eventIds, cancellationToken);
        return dict.ToDictionary(kv => kv.Key, kv => ToBackendEventSummary(kv.Value));
    }

    public async Task<IReadOnlyDictionary<Guid, IReadOnlyList<string>>> GetMediaUrlsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<Guid, IReadOnlyList<string>>();
        foreach (var postId in postIds)
        {
            var urls = await GetMediaUrlsForPostAsync(postId, cancellationToken);
            if (urls.Count > 0)
                result[postId] = urls;
        }
        return result;
    }

    public async Task<IReadOnlyList<BackendUserInfo>> GetUsersByIdsAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.ListByIdsAsync(userIds, cancellationToken);
        return users.Select(u => new BackendUserInfo(u.Id, u.DisplayName, null)).ToList();
    }

    public async Task<BackendCreatePostResult> CreatePostAsync(
        Guid territoryId,
        Guid userId,
        string title,
        string content,
        string type,
        string visibility,
        Guid? mapEntityId,
        IReadOnlyList<Guid>? mediaIds,
        IReadOnlyList<string>? tags,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<PostType>(type, true, out var postType) ||
            !Enum.TryParse<PostVisibility>(visibility, true, out var vis))
        {
            return new BackendCreatePostResult(false, null, "Invalid type or visibility.");
        }
        var result = await _feedService.CreatePostAsync(
            territoryId, userId, title, content, postType, vis, PostStatus.Published,
            mapEntityId, null, null, mediaIds, cancellationToken, tags);
        if (!result.IsSuccess || result.Value is null)
            return new BackendCreatePostResult(false, null, result.Error);
        return new BackendCreatePostResult(true, ToBackendPost(result.Value), null);
    }

    public Task<bool> LikeAsync(Guid territoryId, Guid postId, string actorId, Guid? userId, CancellationToken cancellationToken = default)
    {
        return _feedService.LikeAsync(territoryId, postId, actorId, userId, cancellationToken);
    }

    public async Task<(bool Success, string? Error)> CommentAsync(Guid territoryId, Guid postId, Guid userId, string content, CancellationToken cancellationToken = default)
    {
        var result = await _feedService.CommentAsync(territoryId, postId, userId, content, cancellationToken);
        return (result.IsSuccess, result.IsSuccess ? null : result.Error);
    }

    public async Task<(bool Success, string? Error)> ShareAsync(Guid territoryId, Guid postId, Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _feedService.ShareAsync(territoryId, postId, userId, cancellationToken);
        return (result.IsSuccess, result.IsSuccess ? null : result.Error);
    }

    public async Task<BackendFeedPost?> GetPostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        var post = await _feedService.GetPostAsync(postId, cancellationToken);
        return post is null ? null : ToBackendPost(post);
    }

    public async Task<bool> DeletePostAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var post = await _feedService.GetPostAsync(postId, cancellationToken);
        if (post is null || post.TerritoryId != territoryId || post.AuthorUserId != userId)
        {
            return false;
        }

        await _mediaAttachmentRepository.DeleteByOwnerAsync(MediaOwnerType.Post, postId, cancellationToken);
        await _postGeoAnchorRepository.DeleteByPostIdAsync(postId, cancellationToken);
        await _feedService.DeletePostAsync(postId, cancellationToken);
        return true;
    }

    public static BackendFeedPost ToBackendPost(CommunityPost p)
    {
        return new BackendFeedPost(
            p.Id, p.TerritoryId, p.AuthorUserId, p.Title, p.Content,
            p.Type.ToString().ToUpperInvariant(), p.Visibility.ToString().ToUpperInvariant(), p.Status.ToString().ToUpperInvariant(),
            p.MapEntityId, p.ReferenceType, p.ReferenceId, p.CreatedAtUtc,
            p.Tags?.Count > 0 ? p.Tags : null);
    }

    private static BackendEventSummary ToBackendEventSummary(Application.Models.EventSummary s)
    {
        var e = s.Event;
        return new BackendEventSummary(
            e.Id, e.Title, e.Description, e.StartsAtUtc, e.EndsAtUtc,
            e.Latitude, e.Longitude, e.LocationLabel, e.Status.ToString().ToUpperInvariant(),
            e.CreatedByMembership.ToString().ToUpperInvariant(), s.CreatedByDisplayName,
            s.InterestedCount, s.ConfirmedCount);
    }

    private async Task<IReadOnlyList<string>> GetMediaUrlsForPostAsync(Guid postId, CancellationToken cancellationToken)
    {
        var assets = await _mediaService.ListMediaByOwnerAsync(MediaOwnerType.Post, postId, cancellationToken);
        var urls = new List<string>();
        foreach (var a in assets)
        {
            var urlResult = await _mediaService.GetMediaUrlAsync(a.Id, null, cancellationToken);
            if (urlResult.IsSuccess && urlResult.Value is not null)
                urls.Add(urlResult.Value);
        }
        return urls;
    }
}
