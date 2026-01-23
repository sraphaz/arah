using Araponga.Application.Common;
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
    private readonly InterestFilterService? _interestFilterService;

    public FeedService(
        IFeedRepository feedRepository,
        PostCreationService postCreationService,
        PostInteractionService postInteractionService,
        PostFilterService postFilterService,
        InterestFilterService? interestFilterService = null)
    {
        _feedRepository = feedRepository;
        _postCreationService = postCreationService;
        _postInteractionService = postInteractionService;
        _postFilterService = postFilterService;
        _interestFilterService = interestFilterService;
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
        bool filterByInterests = false,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _feedRepository.CountByTerritoryAsync(territoryId, cancellationToken);
        var posts = await _feedRepository.ListByTerritoryPagedAsync(territoryId, pagination.Skip, pagination.Take, cancellationToken);
        var filtered = await _postFilterService.FilterPostsAsync(posts, territoryId, userId, mapEntityId, assetId, cancellationToken);

        // Aplicar filtro de interesses se solicitado e se serviço disponível
        if (filterByInterests && _interestFilterService is not null && userId.HasValue)
        {
            filtered = await _interestFilterService.FilterFeedByInterestsAsync(
                filtered,
                userId.Value,
                territoryId,
                cancellationToken);
        }

        return new Common.PagedResult<CommunityPost>(filtered, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public Task<IReadOnlyList<CommunityPost>> ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _feedRepository.ListByAuthorAsync(userId, cancellationToken);
    }

    public async Task<Common.PagedResult<CommunityPost>> ListForUserPagedAsync(
        Guid userId,
        Common.PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var totalCount = await _feedRepository.CountByAuthorAsync(userId, cancellationToken);
        var posts = await _feedRepository.ListByAuthorPagedAsync(userId, pagination.Skip, pagination.Take, cancellationToken);

        return new Common.PagedResult<CommunityPost>(posts, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public Task<Result<CommunityPost>> CreatePostAsync(
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
        IReadOnlyCollection<Guid>? mediaIds,
        CancellationToken cancellationToken)
    {
        return _postCreationService.CreatePostAsync(
            territoryId, userId, title, content, type, visibility, status,
            mapEntityId, geoAnchors, assetIds, mediaIds, cancellationToken);
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

    public Task<OperationResult> CommentAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        string content,
        CancellationToken cancellationToken)
    {
        return _postInteractionService.CommentAsync(territoryId, postId, userId, content, cancellationToken);
    }

    public Task<OperationResult> ShareAsync(
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

    public Task<IReadOnlyDictionary<Guid, PostCounts>> GetCountsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken)
    {
        return _feedRepository.GetCountsByPostIdsAsync(postIds, cancellationToken);
    }

    public Task<CommunityPost?> GetPostAsync(Guid postId, CancellationToken cancellationToken)
    {
        return _feedRepository.GetPostAsync(postId, cancellationToken);
    }

    public Task DeletePostAsync(Guid postId, CancellationToken cancellationToken)
    {
        return _feedRepository.DeletePostAsync(postId, cancellationToken);
    }
}
