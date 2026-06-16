using Arah.Api.Contracts.Journeys.Common;
using Arah.Api.Contracts.Journeys.Feed;
using Arah.Api.Services.Journeys.Backend;
using Microsoft.Extensions.Logging;

namespace Arah.Api.Services.Journeys;

public sealed class FeedJourneyService : IFeedJourneyService
{
    private readonly IFeedJourneyBackend _backend;
    private readonly ILogger<FeedJourneyService> _logger;

    public FeedJourneyService(
        IFeedJourneyBackend backend,
        ILogger<FeedJourneyService> logger)
    {
        _backend = backend;
        _logger = logger;
    }

    public async Task<TerritoryFeedJourneyResponse?> GetTerritoryFeedAsync(
        Guid territoryId,
        Guid? userId,
        int pageNumber,
        int pageSize,
        bool filterByInterests,
        Guid? mapEntityId,
        Guid? assetId,
        CancellationToken cancellationToken = default)
    {
        var prioritizeConnections = await _backend.IsConnectionsPrioritizeEnabledAsync(territoryId, cancellationToken);

        var pagedResult = await _backend.ListFeedPagedAsync(
            territoryId,
            userId,
            pageNumber,
            pageSize,
            filterByInterests,
            prioritizeConnections,
            mapEntityId,
            assetId,
            cancellationToken);

        var posts = pagedResult.Items;
        var postIds = posts.Select(p => p.Id).ToList();
        var counts = await _backend.GetCountsByPostIdsAsync(postIds, cancellationToken);
        var eventIds = posts
            .Where(p => string.Equals(p.ReferenceType, "EVENT", StringComparison.OrdinalIgnoreCase))
            .Select(p => p.ReferenceId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
        var eventLookup = eventIds.Count > 0
            ? await _backend.GetEventSummariesByIdsAsync(eventIds, cancellationToken)
            : new Dictionary<Guid, BackendEventSummary>();
        var mediaByPost = await _backend.GetMediaUrlsByPostIdsAsync(postIds, cancellationToken);
        var authorIds = posts.Select(p => p.AuthorUserId).Distinct().ToList();
        var users = await _backend.GetUsersByIdsAsync(authorIds, cancellationToken);
        var userMap = users.ToDictionary(u => u.Id, u => u);

        var items = new List<TerritoryFeedItemJourneyDto>();
        foreach (var post in posts)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new BackendPostCounts(0, 0));
            var mediaList = mediaByPost.GetValueOrDefault(post.Id, Array.Empty<string>());
            userMap.TryGetValue(post.AuthorUserId, out var author);
            var authorDto = author is null ? null : new TerritoryFeedAuthorDto(author.Id, author.DisplayName, author.AvatarUrl);
            var mediaDtos = mediaList.Select(url => new TerritoryFeedMediaDto(url, "IMAGE", null)).ToList();
            var postDto = MapToPostDto(post);
            var countsDto = new TerritoryFeedCountsDto(postCounts.LikeCount, postCounts.ShareCount, postCounts.CommentCount);
            var isAuthor = userId.HasValue && post.AuthorUserId == userId.Value;
            var metadata = new ItemMetadataDto(isAuthor, isAuthor, true, true);
            items.Add(new TerritoryFeedItemJourneyDto(
                postDto,
                countsDto,
                mediaDtos,
                authorDto,
                new UserInteractionsDto(false, false, false),
                metadata));
        }

        const int maxInt32 = int.MaxValue;
        var totalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var totalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var paginationDto = new JourneyPaginationDto(
            pagedResult.PageNumber, pagedResult.PageSize, totalCount, totalPages,
            pagedResult.HasPreviousPage, pagedResult.HasNextPage);

        var filters = new TerritoryFeedFiltersDto(
            new[] { "POST", "ALERT", "EVENT" },
            Array.Empty<string>(),
            new[] { "PUBLIC", "RESIDENTS_ONLY" });

        return new TerritoryFeedJourneyResponse(items, paginationDto, filters);
    }

    public async Task<CreatePostJourneyResponse?> CreatePostAsync(
        Guid territoryId,
        Guid userId,
        CreatePostJourneyRequest request,
        IReadOnlyList<Guid>? mediaIds,
        CancellationToken cancellationToken = default)
    {
        var result = await _backend.CreatePostAsync(
            territoryId,
            userId,
            request.Title,
            request.Content,
            request.Type,
            request.Visibility,
            request.MapEntityId,
            mediaIds ?? request.MediaIds,
            request.Tags,
            cancellationToken);

        if (!result.Success || result.Post is null)
            return null;

        var post = result.Post;
        var mediaUrls = (await _backend.GetMediaUrlsByPostIdsAsync(new[] { post.Id }, cancellationToken))
            .GetValueOrDefault(post.Id, Array.Empty<string>());
        var postDto = MapToPostDto(post);
        return new CreatePostJourneyResponse(postDto, mediaUrls.ToList(), null);
    }

    public async Task<PostInteractionResponse?> InteractAsync(
        Guid territoryId,
        Guid? userId,
        string sessionIdOrUserId,
        PostInteractionRequest request,
        string? commentContent,
        CancellationToken cancellationToken = default)
    {
        var actorId = userId.HasValue ? $"user:{userId.Value}" : $"session:{sessionIdOrUserId}";
        bool success;
        switch (request.Action.ToUpperInvariant())
        {
            case "LIKE":
                success = await _backend.LikeAsync(territoryId, request.PostId, actorId, userId, cancellationToken);
                break;
            case "COMMENT":
                if (string.IsNullOrWhiteSpace(commentContent) || !userId.HasValue)
                    return null;
                var commentResult = await _backend.CommentAsync(territoryId, request.PostId, userId.Value, commentContent, cancellationToken);
                success = commentResult.Success;
                break;
            case "SHARE":
                if (!userId.HasValue)
                    return null;
                var shareResult = await _backend.ShareAsync(territoryId, request.PostId, userId.Value, cancellationToken);
                success = shareResult.Success;
                break;
            default:
                return null;
        }

        if (!success)
            return null;

        var post = await _backend.GetPostAsync(request.PostId, cancellationToken);
        if (post is null)
            return null;

        var counts = await _backend.GetCountsByPostIdsAsync(new[] { post.Id }, cancellationToken);
        var postCounts = counts.GetValueOrDefault(post.Id, new BackendPostCounts(0, 0));
        var postDto = MapToPostDto(post);
        var countsDto = new TerritoryFeedCountsDto(postCounts.LikeCount, postCounts.ShareCount, postCounts.CommentCount);
        var userInteractions = new UserInteractionsDto(
            request.Action.Equals("LIKE", StringComparison.OrdinalIgnoreCase),
            request.Action.Equals("SHARE", StringComparison.OrdinalIgnoreCase),
            request.Action.Equals("COMMENT", StringComparison.OrdinalIgnoreCase));

        return new PostInteractionResponse(postDto, countsDto, userInteractions);
    }

    public async Task<PostCommentsJourneyResponse?> GetPostCommentsAsync(
        Guid territoryId,
        Guid postId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var post = await _backend.GetPostAsync(postId, cancellationToken);
        if (post is null || post.TerritoryId != territoryId)
        {
            return null;
        }

        var paged = await _backend.ListCommentsPagedAsync(
            territoryId,
            postId,
            pageNumber,
            pageSize,
            cancellationToken);

        var userIds = paged.Items.Select(comment => comment.UserId).Distinct().ToList();
        var users = await _backend.GetUsersByIdsAsync(userIds, cancellationToken);
        var userMap = users.ToDictionary(user => user.Id, user => user);

        var items = paged.Items.Select(comment =>
        {
            userMap.TryGetValue(comment.UserId, out var author);
            var authorDto = author is null
                ? new PostCommentAuthorDto(comment.UserId, "Usuário", null)
                : new PostCommentAuthorDto(author.Id, author.DisplayName, author.AvatarUrl);
            return new PostCommentItemDto(comment.Id, comment.Content, comment.CreatedAtUtc, authorDto);
        }).ToList();

        const int maxInt32 = int.MaxValue;
        var totalCount = paged.TotalCount > maxInt32 ? maxInt32 : paged.TotalCount;
        var totalPages = paged.TotalPages > maxInt32 ? maxInt32 : paged.TotalPages;
        var paginationDto = new JourneyPaginationDto(
            paged.PageNumber,
            paged.PageSize,
            totalCount,
            totalPages,
            paged.HasPreviousPage,
            paged.HasNextPage);

        return new PostCommentsJourneyResponse(items, paginationDto);
    }

    public async Task<bool> DeletePostAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _backend.DeletePostAsync(territoryId, postId, userId, cancellationToken);
    }

    private static TerritoryFeedPostDto MapToPostDto(BackendFeedPost post)
    {
        return new TerritoryFeedPostDto(
            post.Id,
            post.Title,
            post.Content,
            post.Type,
            post.Visibility,
            post.Status,
            post.MapEntityId,
            post.CreatedAtUtc,
            post.Tags?.Count > 0 ? post.Tags : null);
    }
}
