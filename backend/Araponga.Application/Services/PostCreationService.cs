using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Metrics;
using Araponga.Application.Events;
using Araponga.Application.Models;
using Araponga.Application.Services.Media;
using Araponga.Domain.Feed;
using Araponga.Domain.Geo;
using Araponga.Domain.Media;
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
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly IMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly ISanctionRepository _sanctionRepository;
    private readonly IFeatureFlagService _featureFlags;
    private readonly TerritoryMediaConfigService _mediaConfigService;
    private readonly IAuditLogger _auditLogger;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CacheInvalidationService? _cacheInvalidation;
    private readonly AccessEvaluator? _accessEvaluator;
    private readonly TerritoryModerationService? _moderationService;

    public PostCreationService(
        IFeedRepository feedRepository,
        IMapRepository mapRepository,
        ITerritoryAssetRepository assetRepository,
        IPostGeoAnchorRepository postGeoAnchorRepository,
        IPostAssetRepository postAssetRepository,
        IMediaAssetRepository mediaAssetRepository,
        IMediaAttachmentRepository mediaAttachmentRepository,
        ISanctionRepository sanctionRepository,
        IFeatureFlagService featureFlags,
        TerritoryMediaConfigService mediaConfigService,
        IAuditLogger auditLogger,
        IEventBus eventBus,
        IUnitOfWork unitOfWork,
        CacheInvalidationService? cacheInvalidation = null,
        AccessEvaluator? accessEvaluator = null,
        TerritoryModerationService? moderationService = null)
    {
        _feedRepository = feedRepository;
        _mapRepository = mapRepository;
        _assetRepository = assetRepository;
        _postGeoAnchorRepository = postGeoAnchorRepository;
        _postAssetRepository = postAssetRepository;
        _mediaAssetRepository = mediaAssetRepository;
        _mediaAttachmentRepository = mediaAttachmentRepository;
        _sanctionRepository = sanctionRepository;
        _featureFlags = featureFlags;
        _mediaConfigService = mediaConfigService;
        _auditLogger = auditLogger;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
        _cacheInvalidation = cacheInvalidation;
        _accessEvaluator = accessEvaluator;
        _moderationService = moderationService;
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
        IReadOnlyCollection<Guid>? mediaIds,
        CancellationToken cancellationToken)
    {
        // Verificar aceite de políticas obrigatórias
        if (_accessEvaluator is not null)
        {
            var policiesResult = await _accessEvaluator.HasAcceptedRequiredPoliciesAsync(userId, cancellationToken);
            if (policiesResult.IsFailure || !policiesResult.Value)
            {
                var pendingPolicies = await _accessEvaluator.GetPendingPoliciesAsync(userId, cancellationToken);
                var errorMessage = "You must accept the required terms of service and privacy policies before creating posts.";
                if (pendingPolicies is not null && !pendingPolicies.IsEmpty)
                {
                    var pendingTermsCount = pendingPolicies.RequiredTerms.Count;
                    var pendingPoliciesCount = pendingPolicies.RequiredPrivacyPolicies.Count;
                    if (pendingTermsCount > 0 || pendingPoliciesCount > 0)
                    {
                        errorMessage = $"You must accept {pendingTermsCount + pendingPoliciesCount} required policy(ies) before creating posts.";
                    }
                }
                return Result<CommunityPost>.Failure(errorMessage);
            }
        }

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

        // Validar e normalizar mediaIds
        var normalizedMediaIds = mediaIds?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (normalizedMediaIds is not null && normalizedMediaIds.Count > 0)
        {
            // Obter limites efetivos da configuração territorial (com fallback para global)
            var limits = await _mediaConfigService.GetEffectiveContentLimitsAsync(
                territoryId,
                MediaContentType.Posts,
                cancellationToken);

            // Validar quantidade máxima de mídias
            if (normalizedMediaIds.Count > limits.MaxMediaCount)
            {
                return Result<CommunityPost>.Failure($"Maximum {limits.MaxMediaCount} media items allowed per post.");
            }

            var mediaAssets = await _mediaAssetRepository.ListByIdsAsync(normalizedMediaIds, cancellationToken);
            if (mediaAssets.Count != normalizedMediaIds.Count)
            {
                return Result<CommunityPost>.Failure("One or more media assets not found.");
            }

            // Validar que todas as mídias pertencem ao usuário
            if (mediaAssets.Any(media => media.UploadedByUserId != userId || media.IsDeleted))
            {
                return Result<CommunityPost>.Failure("One or more media assets are invalid or do not belong to the user.");
            }

            // Validar tipos de mídia habilitados e quantidades
            var images = mediaAssets.Where(media => media.MediaType == Domain.Media.MediaType.Image).ToList();
            var videos = mediaAssets.Where(media => media.MediaType == Domain.Media.MediaType.Video).ToList();
            var audios = mediaAssets.Where(media => media.MediaType == Domain.Media.MediaType.Audio).ToList();

            // Validar imagens
            if (images.Count > 0 && !limits.ImagesEnabled)
            {
                return Result<CommunityPost>.Failure("Images are not enabled for posts in this territory.");
            }

            // Validar vídeos
            if (videos.Count > 0 && !limits.VideosEnabled)
            {
                return Result<CommunityPost>.Failure("Videos are not enabled for posts in this territory.");
            }
            if (videos.Count > limits.MaxVideoCount)
            {
                return Result<CommunityPost>.Failure($"Maximum {limits.MaxVideoCount} video(s) allowed per post.");
            }
            foreach (var video in videos)
            {
                if (video.SizeBytes > limits.MaxVideoSizeBytes)
                {
                    var maxSizeMB = limits.MaxVideoSizeBytes / (1024.0 * 1024.0);
                    return Result<CommunityPost>.Failure($"Video size exceeds {maxSizeMB:F1}MB limit for posts.");
                }
                // Validar tipo MIME se configurado
                if (limits.AllowedVideoMimeTypes != null && limits.AllowedVideoMimeTypes.Count > 0)
                {
                    if (!limits.AllowedVideoMimeTypes.Contains(video.MimeType, StringComparer.OrdinalIgnoreCase))
                    {
                        return Result<CommunityPost>.Failure($"Video MIME type '{video.MimeType}' is not allowed for posts.");
                    }
                }
            }

            // Validar áudios
            if (audios.Count > 0 && !limits.AudioEnabled)
            {
                return Result<CommunityPost>.Failure("Audio is not enabled for posts in this territory.");
            }
            if (audios.Count > limits.MaxAudioCount)
            {
                return Result<CommunityPost>.Failure($"Maximum {limits.MaxAudioCount} audio(s) allowed per post.");
            }
            foreach (var audio in audios)
            {
                if (audio.SizeBytes > limits.MaxAudioSizeBytes)
                {
                    var maxSizeMB = limits.MaxAudioSizeBytes / (1024.0 * 1024.0);
                    return Result<CommunityPost>.Failure($"Audio size exceeds {maxSizeMB:F1}MB limit for posts.");
                }
                // Validar tipo MIME se configurado
                if (limits.AllowedAudioMimeTypes != null && limits.AllowedAudioMimeTypes.Count > 0)
                {
                    if (!limits.AllowedAudioMimeTypes.Contains(audio.MimeType, StringComparer.OrdinalIgnoreCase))
                    {
                        return Result<CommunityPost>.Failure($"Audio MIME type '{audio.MimeType}' is not allowed for posts.");
                    }
                }
            }
        }

        // Verificar regras de moderação comunitária
        if (_moderationService is not null)
        {
            // Criar post temporário para validação
            var tempPost = new CommunityPost(
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

            var moderationResult = await _moderationService.ApplyRulesAsync(tempPost, cancellationToken);
            if (moderationResult.IsFailure)
            {
                return Result<CommunityPost>.Failure(moderationResult.Error ?? "Post violates territory moderation rules.");
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

        // Criar MediaAttachments para as mídias associadas ao post
        if (normalizedMediaIds is not null && normalizedMediaIds.Count > 0)
        {
            var now = DateTime.UtcNow;
            foreach (var (mediaId, index) in normalizedMediaIds.Select((id, idx) => (id, idx)))
            {
                var attachment = new MediaAttachment(
                    Guid.NewGuid(),
                    mediaId,
                    MediaOwnerType.Post,
                    post.Id,
                    index,
                    now);

                await _mediaAttachmentRepository.AddAsync(attachment, cancellationToken);
            }
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
