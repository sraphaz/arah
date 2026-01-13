using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;
using Araponga.Domain.Social;

namespace Araponga.Application.Services;

/// <summary>
/// Service responsible for filtering posts based on visibility and access rules. Extracted from FeedService.
/// </summary>
public sealed class PostFilterService
{
    private readonly AccessEvaluator _accessEvaluator;
    private readonly UserBlockCacheService? _userBlockCache;
    private readonly IUserBlockRepository _userBlockRepository;
    private readonly IPostAssetRepository _postAssetRepository;

    public PostFilterService(
        AccessEvaluator accessEvaluator,
        IUserBlockRepository userBlockRepository,
        IPostAssetRepository postAssetRepository,
        UserBlockCacheService? userBlockCache = null)
    {
        _accessEvaluator = accessEvaluator;
        _userBlockRepository = userBlockRepository;
        _postAssetRepository = postAssetRepository;
        _userBlockCache = userBlockCache;
    }

    public async Task<IReadOnlyList<CommunityPost>> FilterPostsAsync(
        IReadOnlyList<CommunityPost> posts,
        Guid territoryId,
        Guid? userId,
        Guid? mapEntityId,
        Guid? assetId,
        CancellationToken cancellationToken)
    {
        var blockedUserIds = userId is null
            ? Array.Empty<Guid>()
            : _userBlockCache is not null
                ? await _userBlockCache.GetBlockedUserIdsAsync(userId.Value, cancellationToken)
                : await _userBlockRepository.GetBlockedUserIdsAsync(userId.Value, cancellationToken);

        var visiblePosts = blockedUserIds.Count == 0
            ? posts
            : posts.Where(post => !blockedUserIds.Contains(post.AuthorUserId)).ToList();

        if (userId is null)
        {
            var publicPosts = visiblePosts
                .Where(post => post.Visibility == PostVisibility.Public && post.Status == PostStatus.Published)
                .ToList();
            return await ApplyFilters(publicPosts, mapEntityId, assetId, cancellationToken);
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId.Value, territoryId, cancellationToken);

        var statusFiltered = isResident
            ? visiblePosts.Where(post => post.Status != PostStatus.Rejected && post.Status != PostStatus.Hidden).ToList()
            : visiblePosts.Where(post => post.Visibility == PostVisibility.Public && post.Status == PostStatus.Published).ToList();

        return await ApplyFilters(statusFiltered, mapEntityId, assetId, cancellationToken);
    }

    private async Task<IReadOnlyList<CommunityPost>> ApplyFilters(
        IReadOnlyList<CommunityPost> posts,
        Guid? mapEntityId,
        Guid? assetId,
        CancellationToken cancellationToken)
    {
        var scoped = mapEntityId is null
            ? posts
            : posts.Where(post => post.MapEntityId == mapEntityId).ToList();

        if (assetId is null)
        {
            return scoped;
        }

        var postIds = await _postAssetRepository.ListPostIdsByAssetIdAsync(assetId.Value, cancellationToken);
        if (postIds.Count == 0)
        {
            return Array.Empty<CommunityPost>();
        }

        var postLookup = postIds.ToHashSet();
        return scoped.Where(post => postLookup.Contains(post.Id)).ToList();
    }

    public async Task<Common.PagedResult<CommunityPost>> FilterAndPaginateAsync(
        IReadOnlyList<CommunityPost> posts,
        Guid territoryId,
        Guid? userId,
        Guid? mapEntityId,
        Guid? assetId,
        Common.PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var filtered = await FilterPostsAsync(posts, territoryId, userId, mapEntityId, assetId, cancellationToken);

        var totalCount = filtered.Count;
        var pagedItems = filtered
            .OrderByDescending(p => p.CreatedAtUtc)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new Common.PagedResult<CommunityPost>(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }
}
