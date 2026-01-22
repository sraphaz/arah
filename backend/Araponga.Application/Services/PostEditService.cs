using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Models;
using Araponga.Application.Services.Media;
using Araponga.Domain.Feed;
using Araponga.Domain.Media;
using MediaContentType = Araponga.Application.Services.Media.MediaContentType;

namespace Araponga.Application.Services;

/// <summary>
/// Service responsible for editing posts. Follows TDD/BDD principles.
/// </summary>
public sealed class PostEditService
{
    private readonly IFeedRepository _feedRepository;
    private readonly IMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly IPostGeoAnchorRepository _postGeoAnchorRepository;
    private readonly IFeatureFlagService _featureFlags;
    private readonly TerritoryMediaConfigService _mediaConfigService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CacheInvalidationService? _cacheInvalidation;

    public PostEditService(
        IFeedRepository feedRepository,
        IMediaAttachmentRepository mediaAttachmentRepository,
        IMediaAssetRepository mediaAssetRepository,
        IPostGeoAnchorRepository postGeoAnchorRepository,
        IFeatureFlagService featureFlags,
        TerritoryMediaConfigService mediaConfigService,
        IUnitOfWork unitOfWork,
        CacheInvalidationService? cacheInvalidation = null)
    {
        _feedRepository = feedRepository;
        _mediaAttachmentRepository = mediaAttachmentRepository;
        _mediaAssetRepository = mediaAssetRepository;
        _postGeoAnchorRepository = postGeoAnchorRepository;
        _featureFlags = featureFlags;
        _mediaConfigService = mediaConfigService;
        _unitOfWork = unitOfWork;
        _cacheInvalidation = cacheInvalidation;
    }

    /// <summary>
    /// Edits a post. Only the author can edit their own post.
    /// </summary>
    public async Task<Result<CommunityPost>> EditPostAsync(
        Guid postId,
        Guid userId,
        string title,
        string content,
        IReadOnlyCollection<Guid>? mediaIds,
        IReadOnlyCollection<Models.GeoAnchorInput>? geoAnchors,
        CancellationToken cancellationToken)
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<CommunityPost>.Failure("Title is required.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result<CommunityPost>.Failure("Content is required.");
        }

        // Buscar post
        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null)
        {
            return Result<CommunityPost>.Failure("Post not found.");
        }

        // Validar autorização - apenas autor pode editar
        if (post.AuthorUserId != userId)
        {
            return Result<CommunityPost>.Failure("Only the post author can edit the post.");
        }

        // Validar feature flag (se necessário)
        // TODO: Adicionar feature flag PostEditingEnabled quando implementado

        // Validar mídias se fornecidas
        if (mediaIds is not null && mediaIds.Count > 0)
        {
            var normalizedMediaIds = mediaIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (normalizedMediaIds.Count > 0)
            {
                // Validar que todas as mídias existem e pertencem ao usuário
                var mediaAssets = new List<MediaAsset>();
                foreach (var mediaId in normalizedMediaIds)
                {
                    var mediaAsset = await _mediaAssetRepository.GetByIdAsync(mediaId, cancellationToken);
                    if (mediaAsset is null)
                    {
                        return Result<CommunityPost>.Failure($"Media asset {mediaId} not found.");
                    }

                    if (mediaAsset.UploadedByUserId != userId)
                    {
                        return Result<CommunityPost>.Failure($"Media asset {mediaId} does not belong to the user.");
                    }

                    mediaAssets.Add(mediaAsset);
                }

                // Obter limites efetivos da configuração territorial (com fallback para global)
                var limits = await _mediaConfigService.GetEffectiveContentLimitsAsync(
                    post.TerritoryId,
                    MediaContentType.Posts,
                    cancellationToken);

                // Validar quantidade máxima de mídias
                if (normalizedMediaIds.Count > limits.MaxMediaCount)
                {
                    return Result<CommunityPost>.Failure($"Maximum {limits.MaxMediaCount} media items allowed per post.");
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
                foreach (var image in images)
                {
                    if (image.SizeBytes > limits.MaxImageSizeBytes)
                    {
                        var maxSizeMB = limits.MaxImageSizeBytes / (1024.0 * 1024.0);
                        return Result<CommunityPost>.Failure($"Image size exceeds {maxSizeMB:F1}MB limit for posts.");
                    }
                    // Validar tipo MIME se configurado
                    if (limits.AllowedImageMimeTypes != null && limits.AllowedImageMimeTypes.Count > 0)
                    {
                        if (!limits.AllowedImageMimeTypes.Contains(image.MimeType, StringComparer.OrdinalIgnoreCase))
                        {
                            return Result<CommunityPost>.Failure($"Image MIME type '{image.MimeType}' is not allowed for posts.");
                        }
                    }
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

                // Validar tamanhos
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
        }

        // Editar post usando método do domínio
        post.Edit(title, content);

        // Atualizar geo anchors se fornecidos
        if (geoAnchors is not null)
        {
            // Remover anchors existentes
            await _postGeoAnchorRepository.DeleteByPostIdAsync(postId, cancellationToken);

            // Adicionar novos anchors
            var newAnchors = BuildPostAnchors(postId, geoAnchors);
            if (newAnchors.Count > 0)
            {
                await _postGeoAnchorRepository.AddAsync(newAnchors, cancellationToken);
            }
        }

        // Atualizar mídias associadas
        if (mediaIds is not null)
        {
            var normalizedMediaIds = mediaIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            // Remover attachments existentes
            var existingAttachments = await _mediaAttachmentRepository.ListByOwnerAsync(
                MediaOwnerType.Post,
                postId,
                cancellationToken);

            foreach (var attachment in existingAttachments)
            {
                await _mediaAttachmentRepository.DeleteAsync(attachment.Id, cancellationToken);
            }

            // Adicionar novos attachments
            if (normalizedMediaIds.Count > 0)
            {
                var now = DateTime.UtcNow;
                foreach (var (mediaId, index) in normalizedMediaIds.Select((id, idx) => (id, idx)))
                {
                    var attachment = new MediaAttachment(
                        Guid.NewGuid(),
                        mediaId,
                        MediaOwnerType.Post,
                        postId,
                        index,
                        now);

                    await _mediaAttachmentRepository.AddAsync(attachment, cancellationToken);
                }
            }
        }

        // Atualizar post no repositório
        await _feedRepository.UpdatePostAsync(post, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidar cache de feed do território após editar post
        _cacheInvalidation?.InvalidateFeedCache(post.TerritoryId);

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
            .Where(anchor => Domain.Geo.GeoCoordinate.IsValid(anchor.Latitude, anchor.Longitude))
            .Select(anchor => new
            {
                Latitude = Math.Round(anchor.Latitude, Constants.Posts.GeoAnchorPrecision, MidpointRounding.AwayFromZero),
                Longitude = Math.Round(anchor.Longitude, Constants.Posts.GeoAnchorPrecision, MidpointRounding.AwayFromZero),
                Type = anchor.Type
            })
            .Select(anchor => new Domain.Map.PostGeoAnchor(
                Guid.NewGuid(),
                postId,
                anchor.Latitude,
                anchor.Longitude,
                anchor.Type,
                now))
            .ToList();
    }
}
