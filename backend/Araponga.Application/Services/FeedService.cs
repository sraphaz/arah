using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;
using Araponga.Domain.Social;

namespace Araponga.Application.Services;

public sealed class FeedService
{
    private readonly IFeedRepository _feedRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IFeatureFlagService _featureFlags;
    private readonly IAuditLogger _auditLogger;
    private readonly IUserBlockRepository _userBlockRepository;
    private readonly IMapRepository _mapRepository;

    public FeedService(
        IFeedRepository feedRepository,
        AccessEvaluator accessEvaluator,
        IFeatureFlagService featureFlags,
        IAuditLogger auditLogger,
        IUserBlockRepository userBlockRepository,
        IMapRepository mapRepository)
    {
        _feedRepository = feedRepository;
        _accessEvaluator = accessEvaluator;
        _featureFlags = featureFlags;
        _auditLogger = auditLogger;
        _userBlockRepository = userBlockRepository;
        _mapRepository = mapRepository;
    }

    public async Task<IReadOnlyList<CommunityPost>> ListForTerritoryAsync(
        Guid territoryId,
        Guid? userId,
        Guid? mapEntityId,
        CancellationToken cancellationToken)
    {
        var posts = await _feedRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        var blockedUserIds = userId is null
            ? Array.Empty<Guid>()
            : await _userBlockRepository.GetBlockedUserIdsAsync(userId.Value, cancellationToken);

        var visiblePosts = blockedUserIds.Count == 0
            ? posts
            : posts.Where(post => !blockedUserIds.Contains(post.AuthorUserId)).ToList();

        if (userId is null)
        {
            return visiblePosts
                .Where(post => post.Visibility == PostVisibility.Public && post.Status == PostStatus.Published)
                .ToList();
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId.Value, territoryId, cancellationToken);

        var statusFiltered = isResident
            ? visiblePosts.Where(post => post.Status != PostStatus.Rejected).ToList()
            : visiblePosts.Where(post => post.Visibility == PostVisibility.Public && post.Status == PostStatus.Published).ToList();

        if (mapEntityId is null)
        {
            return statusFiltered;
        }

        return statusFiltered
            .Where(post => post.MapEntityId == mapEntityId)
            .ToList();
    }

    public async Task<IReadOnlyList<CommunityPost>> ListForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _feedRepository.ListByAuthorAsync(userId, cancellationToken);
    }

    public async Task<(bool success, string? error, CommunityPost? post)> CreatePostAsync(
        Guid territoryId,
        Guid userId,
        string title,
        string content,
        PostType type,
        PostVisibility visibility,
        PostStatus status,
        Guid? mapEntityId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
        {
            return (false, "Title and content are required.", null);
        }

        if (type == PostType.Alert && !_featureFlags.IsEnabled(territoryId, Models.FeatureFlag.AlertPosts))
        {
            return (false, "Alert posts are disabled for this territory.", null);
        }

        if (type == PostType.Event && !_featureFlags.IsEnabled(territoryId, Models.FeatureFlag.EventPosts))
        {
            return (false, "Event posts are disabled for this territory.", null);
        }

        if (mapEntityId is not null)
        {
            var entity = await _mapRepository.GetByIdAsync(mapEntityId.Value, cancellationToken);
            if (entity is null || entity.TerritoryId != territoryId)
            {
                return (false, "Map entity not found for territory.", null);
            }
        }

        var post = new CommunityPost(
            Guid.NewGuid(),
            territoryId,
            userId,
            title,
            content,
            type,
            visibility,
            status,
            mapEntityId,
            DateTime.UtcNow);

        await _feedRepository.AddPostAsync(post, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("post.created", userId, territoryId, post.Id, DateTime.UtcNow),
            cancellationToken);

        return (true, null, post);
    }

    public async Task<(bool success, string? error)> ApproveEventAsync(
        Guid territoryId,
        Guid postId,
        Guid approverUserId,
        PostStatus status,
        CancellationToken cancellationToken)
    {
        var isResident = await _accessEvaluator.IsResidentAsync(approverUserId, territoryId, cancellationToken);
        if (!isResident)
        {
            return (false, "Only residents can approve events.");
        }

        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null || post.TerritoryId != territoryId)
        {
            return (false, "Post not found.");
        }

        if (post.Type != PostType.Event)
        {
            return (false, "Only event posts can be approved.");
        }

        if (status != PostStatus.Published && status != PostStatus.Rejected)
        {
            return (false, "Invalid approval status.");
        }

        await _feedRepository.UpdateStatusAsync(postId, status, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry(
                $"event.{status.ToString().ToLowerInvariant()}",
                approverUserId,
                territoryId,
                postId,
                DateTime.UtcNow),
            cancellationToken);

        return (true, null);
    }

    public async Task<bool> LikeAsync(
        Guid territoryId,
        Guid postId,
        string actorId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null || post.TerritoryId != territoryId)
        {
            return false;
        }

        if (post.Visibility == PostVisibility.ResidentsOnly)
        {
            if (userId is null)
            {
                return false;
            }

            var membershipRole = await _accessEvaluator.GetRoleAsync(userId.Value, territoryId, cancellationToken);
            if (membershipRole != MembershipRole.Resident)
            {
                return false;
            }
        }

        await _feedRepository.AddLikeAsync(postId, actorId, cancellationToken);
        return true;
    }

    public async Task<(bool success, string? error)> CommentAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        string content,
        CancellationToken cancellationToken)
    {
        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null || post.TerritoryId != territoryId)
        {
            return (false, "Post not found.");
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        if (!isResident)
        {
            return (false, "Only residents can comment.");
        }

        var comment = new PostComment(Guid.NewGuid(), postId, userId, content, DateTime.UtcNow);
        await _feedRepository.AddCommentAsync(comment, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("comment.created", userId, territoryId, comment.Id, DateTime.UtcNow),
            cancellationToken);

        return (true, null);
    }

    public async Task<(bool success, string? error)> ShareAsync(
        Guid territoryId,
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null || post.TerritoryId != territoryId)
        {
            return (false, "Post not found.");
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        if (!isResident)
        {
            return (false, "Only residents can share.");
        }

        await _feedRepository.AddShareAsync(postId, userId, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("post.shared", userId, territoryId, postId, DateTime.UtcNow),
            cancellationToken);

        return (true, null);
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
