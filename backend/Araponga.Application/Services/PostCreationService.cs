using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Metrics;
using Araponga.Application.Events;
using Araponga.Application.Models;
using Araponga.Domain.Feed;
using Araponga.Domain.Geo;
using Araponga.Domain.Moderation;

namespace Araponga.Application.Services;

/// <summary>
/// Service responsible for creating posts. Extracted from FeedService to improve separation of concerns.
/// </summary>
public sealed class PostCreationService
{
    private readonly IFeedRepository _feedRepository;
    private readonly IMapRepository _mapRepository;
    private readonly ITerritoryAssetRepository _assetRepository;
    private readonly IPostGeoAnchorRepository _postGeoAnchorRepository;
    private readonly IPostAssetRepository _postAssetRepository;
    private readonly ISanctionRepository _sanctionRepository;
    private readonly IFeatureFlagService _featureFlags;
    private readonly IAuditLogger _auditLogger;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CacheInvalidationService? _cacheInvalidation;

    public PostCreationService(
        IFeedRepository feedRepository,
        IMapRepository mapRepository,
        ITerritoryAssetRepository assetRepository,
        IPostGeoAnchorRepository postGeoAnchorRepository,
        IPostAssetRepository postAssetRepository,
        ISanctionRepository sanctionRepository,
        IFeatureFlagService featureFlags,
        IAuditLogger auditLogger,
        IEventBus eventBus,
        IUnitOfWork unitOfWork,
        CacheInvalidationService? cacheInvalidation = null)
    {
        _feedRepository = feedRepository;
        _mapRepository = mapRepository;
        _assetRepository = assetRepository;
        _postGeoAnchorRepository = postGeoAnchorRepository;
        _postAssetRepository = postAssetRepository;
        _sanctionRepository = sanctionRepository;
        _featureFlags = featureFlags;
        _auditLogger = auditLogger;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task<Result<CommunityPost>> CreatePostAsync(
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
            return Result<CommunityPost>.Failure("Title and content are required.");
        }

        if (type == PostType.Alert && !_featureFlags.IsEnabled(territoryId, Models.FeatureFlag.AlertPosts))
        {
            return Result<CommunityPost>.Failure("Alert posts are disabled for this territory.");
        }

        if (mapEntityId is not null)
        {
            var entity = await _mapRepository.GetByIdAsync(mapEntityId.Value, cancellationToken);
            if (entity is null || entity.TerritoryId != territoryId)
            {
                return Result<CommunityPost>.Failure("Map entity not found for territory.");
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
            return Result<CommunityPost>.Failure("User is restricted from posting in this territory.");
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
                return Result<CommunityPost>.Failure("Asset not found for territory.");
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

        // Invalidar cache de feed do território após criar post
        _cacheInvalidation?.InvalidateFeedCache(territoryId);

        // Record business metric
        ArapongaMetrics.PostsCreated.Add(1, new KeyValuePair<string, object?>("territory_id", territoryId));

        return Result<CommunityPost>.Success(post);
    }

    private static IReadOnlyCollection<Domain.Map.PostGeoAnchor> BuildPostAnchors(
        Guid postId,
        IReadOnlyCollection<Models.GeoAnchorInput>? geoAnchors)
    {
        if (geoAnchors is null || geoAnchors.Count == 0)
        {
            return Array.Empty<Domain.Map.PostGeoAnchor>();
        }

        var now = DateTime.UtcNow;

        return geoAnchors
            .Where(anchor => GeoCoordinate.IsValid(anchor.Latitude, anchor.Longitude))
            .Select(anchor => new
            {
                Latitude = Math.Round(anchor.Latitude, Constants.Posts.GeoAnchorPrecision, MidpointRounding.AwayFromZero),
                Longitude = Math.Round(anchor.Longitude, Constants.Posts.GeoAnchorPrecision, MidpointRounding.AwayFromZero)
            })
            .Distinct()
            .Take(Constants.Posts.MaxAnchors)
            .Select(anchor => new Domain.Map.PostGeoAnchor(
                Guid.NewGuid(),
                postId,
                anchor.Latitude,
                anchor.Longitude,
                Constants.Posts.PostAnchorType,
                now))
            .ToList();
    }
}
