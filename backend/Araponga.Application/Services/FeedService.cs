using Araponga.Application.Interfaces;
using Araponga.Application.Events;
using Araponga.Domain.Feed;
using Araponga.Domain.Geo;
using Araponga.Domain.Moderation;
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
    private readonly IPostGeoAnchorRepository _postGeoAnchorRepository;
    private readonly IPostAssetRepository _postAssetRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly ISanctionRepository _sanctionRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public FeedService(
        IFeedRepository feedRepository,
        AccessEvaluator accessEvaluator,
        IFeatureFlagService featureFlags,
        IAuditLogger auditLogger,
        IUserBlockRepository userBlockRepository,
        IMapRepository mapRepository,
        IPostGeoAnchorRepository postGeoAnchorRepository,
        IPostAssetRepository postAssetRepository,
        IAssetRepository assetRepository,
        ISanctionRepository sanctionRepository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _feedRepository = feedRepository;
        _accessEvaluator = accessEvaluator;
        _featureFlags = featureFlags;
        _auditLogger = auditLogger;
        _userBlockRepository = userBlockRepository;
        _mapRepository = mapRepository;
        _postGeoAnchorRepository = postGeoAnchorRepository;
        _postAssetRepository = postAssetRepository;
        _assetRepository = assetRepository;
        _sanctionRepository = sanctionRepository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CommunityPost>> ListForTerritoryAsync(
        Guid territoryId,
        Guid? userId,
        Guid? mapEntityId,
        Guid? assetId,
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
            ? visiblePosts.Where(post => post.Status != PostStatus.Rejected && post.Status != PostStatus.Hidden).ToList()
            : visiblePosts.Where(post => post.Visibility == PostVisibility.Public && post.Status == PostStatus.Published).ToList();

        var scoped = mapEntityId is null
            ? statusFiltered
            : statusFiltered.Where(post => post.MapEntityId == mapEntityId).ToList();

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
        IReadOnlyCollection<Models.GeoAnchorInput>? geoAnchors,
        IReadOnlyCollection<Guid>? assetIds,
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

        if (mapEntityId is not null)
        {
            var entity = await _mapRepository.GetByIdAsync(mapEntityId.Value, cancellationToken);
            if (entity is null || entity.TerritoryId != territoryId)
            {
                return (false, "Map entity not found for territory.", null);
            }
        }

        var postingRestricted = await _sanctionRepository.HasActiveSanctionAsync(
            userId,
            territoryId,
            SanctionType.PostingRestriction,
            DateTime.UtcNow,
            cancellationToken);

        if (postingRestricted)
        {
            return (false, "User is restricted from posting in this territory.", null);
        }

        var normalizedAssetIds = assetIds?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (normalizedAssetIds is not null && normalizedAssetIds.Count > 0)
        {
            var assets = await _assetRepository.ListByIdsAsync(normalizedAssetIds, cancellationToken);
            if (assets.Count != normalizedAssetIds.Count || assets.Any(asset => asset.TerritoryId != territoryId))
            {
                return (false, "Asset not found for territory.", null);
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

        var anchors = BuildPostAnchors(post.Id, geoAnchors);

        await _postGeoAnchorRepository.AddAsync(anchors, cancellationToken);

        if (normalizedAssetIds is not null && normalizedAssetIds.Count > 0)
        {
            var postAssets = normalizedAssetIds.Select(assetId => new PostAsset(post.Id, assetId)).ToList();
            await _postAssetRepository.AddAsync(postAssets, cancellationToken);
        }

        await _auditLogger.LogAsync(
            new Models.AuditEntry("post.created", userId, territoryId, post.Id, DateTime.UtcNow),
            cancellationToken);

        await _eventBus.PublishAsync(
            new PostCreatedEvent(post.Id, territoryId, userId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return (true, null, post);
    }

    private static IReadOnlyCollection<Domain.Map.PostGeoAnchor> BuildPostAnchors(
        Guid postId,
        IReadOnlyCollection<Models.GeoAnchorInput>? geoAnchors)
    {
        const int MaxAnchors = 50;
        const int Precision = 5;

        if (geoAnchors is null || geoAnchors.Count == 0)
        {
            return Array.Empty<Domain.Map.PostGeoAnchor>();
        }

        var now = DateTime.UtcNow;
        const string AnchorType = "POST";

        return geoAnchors
            .Where(anchor => GeoCoordinate.IsValid(anchor.Latitude, anchor.Longitude))
            .Select(anchor => new
            {
                Latitude = Math.Round(anchor.Latitude, Precision, MidpointRounding.AwayFromZero),
                Longitude = Math.Round(anchor.Longitude, Precision, MidpointRounding.AwayFromZero)
            })
            .Distinct()
            .Take(MaxAnchors)
            .Select(anchor => new Domain.Map.PostGeoAnchor(
                Guid.NewGuid(),
                postId,
                anchor.Latitude,
                anchor.Longitude,
                AnchorType,
                now))
            .ToList();
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

        if (userId is not null)
        {
            var interactionRestricted = await _sanctionRepository.HasActiveSanctionAsync(
                userId.Value,
                territoryId,
                SanctionType.InteractionRestriction,
                DateTime.UtcNow,
                cancellationToken);

            if (interactionRestricted)
            {
                return false;
            }
        }

        await _feedRepository.AddLikeAsync(postId, actorId, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
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

        var interactionRestricted = await _sanctionRepository.HasActiveSanctionAsync(
            userId,
            territoryId,
            SanctionType.InteractionRestriction,
            DateTime.UtcNow,
            cancellationToken);

        if (interactionRestricted)
        {
            return (false, "User is restricted from interacting in this territory.");
        }

        var comment = new PostComment(Guid.NewGuid(), postId, userId, content, DateTime.UtcNow);
        await _feedRepository.AddCommentAsync(comment, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("comment.created", userId, territoryId, comment.Id, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

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

        var interactionRestricted = await _sanctionRepository.HasActiveSanctionAsync(
            userId,
            territoryId,
            SanctionType.InteractionRestriction,
            DateTime.UtcNow,
            cancellationToken);

        if (interactionRestricted)
        {
            return (false, "User is restricted from interacting in this territory.");
        }

        await _feedRepository.AddShareAsync(postId, userId, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("post.shared", userId, territoryId, postId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

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
