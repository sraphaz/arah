using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;

namespace Araponga.Application.Services;

/// <summary>
/// Service for managing feed operations. Delegates to specialized services for creation, interactions, and filtering.
/// </summary>
public sealed class FeedService
{
    private readonly IFeedRepository _feedRepository;
    private readonly PostCreationService _postCreationService;
    private readonly PostInteractionService _postInteractionService;
    private readonly PostFilterService _postFilterService;

    public FeedService(
        IFeedRepository feedRepository,
        PostCreationService postCreationService,
        PostInteractionService postInteractionService,
        PostFilterService postFilterService)
    {
        _feedRepository = feedRepository;
        _postCreationService = postCreationService;
        _postInteractionService = postInteractionService;
        _postFilterService = postFilterService;
    }

    public async Task<IReadOnlyList<CommunityPost>> ListForTerritoryAsync(
        Guid territoryId,
        Guid? userId,
        Guid? mapEntityId,
        Guid? assetId,
        CancellationToken cancellationToken)
    {
        var posts = await _feedRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        return await _postFilterService.FilterPostsAsync(posts, territoryId, userId, mapEntityId, assetId, cancellationToken);
    }

    public async Task<Common.PagedResult<CommunityPost>> ListForTerritoryPagedAsync(
        Guid territoryId,
        Guid? userId,
        Guid? mapEntityId,
        Guid? assetId,
        Common.PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var posts = await _feedRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        return await _postFilterService.FilterAndPaginateAsync(posts, territoryId, userId, mapEntityId, assetId, pagination, cancellationToken);
    }

    public Task<IReadOnlyList<CommunityPost>> ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _feedRepository.ListByAuthorAsync(userId, cancellationToken);
    }

    public Task<(bool success, string? error, CommunityPost? post)> CreatePostAsync(
        Guid territoryId,
        Guid userId,
        string title,
        string content,
        PostType type,
        PostVisibility visibility,
        PostStatus status,
        Guid? mapEntityId,
        IReadOnlyCollection<Models.GeoAnchorInput>? geoAnchors,
        IReadOnlyCollection<Guid>? assetIds,
        CancellationToken cancellationToken)
    {
        return _postCreationService.CreatePostAsync(
            territoryId, userId, title, content, type, visibility, status,
            mapEntityId, geoAnchors, assetIds, cancellationToken);
    }

    public Task<bool> LikeAsync(
        Guid territoryId,
        Guid postId,
        string actorId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        return _postInteractionService.LikeAsync(territoryId, postId, actorId, userId, cancellationToken);
    }

    public Task<(bool success, string? error)> CommentAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        string content,
        CancellationToken cancellationToken)
    {
        return _postInteractionService.CommentAsync(territoryId, postId, userId, content, cancellationToken);
    }

    public Task<(bool success, string? error)> ShareAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _postInteractionService.ShareAsync(territoryId, postId, userId, cancellationToken);
    }

    public Task<int> GetLikeCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        return _feedRepository.GetLikeCountAsync(postId, cancellationToken);
    }

    public Task<int> GetShareCountAsync(Guid postId, CancellationToken cancellationToken)
    {
        return _feedRepository.GetShareCountAsync(postId, cancellationToken);
    }
}
