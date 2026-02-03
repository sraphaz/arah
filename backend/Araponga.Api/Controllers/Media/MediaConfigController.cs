using Araponga.Api.Contracts.Media;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Application.Services.Media;
using Araponga.Domain.Media;
using Araponga.Domain.Membership;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para gerenciar configurações de mídia por território.
/// </summary>
[ApiController]
[Route("api/v1/territories/{territoryId:guid}/media-config")]
[Produces("application/json")]
[Tags("Media Config")]
public sealed class MediaConfigController : ControllerBase
{
    private readonly TerritoryMediaConfigService _configService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;

    public MediaConfigController(
        TerritoryMediaConfigService configService,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator)
    {
        _configService = configService;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Obtém configuração de mídia de um território (requer autenticação).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TerritoryMediaConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TerritoryMediaConfigResponse>> Get(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var config = await _configService.GetConfigAsync(territoryId, cancellationToken);
        var response = MapToResponse(config);
        return Ok(response);
    }

    /// <summary>
    /// Atualiza configuração de mídia de um território (requer Curator).
    /// </summary>
    [HttpPut]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TerritoryMediaConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TerritoryMediaConfigResponse>> Update(
        [FromRoute] Guid territoryId,
        [FromBody] UpdateTerritoryMediaConfigRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isCurator = await _accessEvaluator.HasCapabilityAsync(
            userContext.User.Id,
            territoryId,
            MembershipCapabilityType.Curator,
            cancellationToken);
        if (!isCurator)
        {
            return Unauthorized();
        }

        var existingConfig = await _configService.GetConfigAsync(territoryId, cancellationToken);
        var updatedConfig = UpdateConfigFromRequest(existingConfig, request);

        var savedConfig = await _configService.UpdateConfigAsync(
            territoryId,
            updatedConfig,
            userContext.User.Id,
            cancellationToken);

        var response = MapToResponse(savedConfig);
        return Ok(response);
    }

    private static TerritoryMediaConfigResponse MapToResponse(TerritoryMediaConfig config)
    {
        return new TerritoryMediaConfigResponse(
            config.TerritoryId,
            MapContentConfigToResponse(config.Posts),
            MapContentConfigToResponse(config.Events),
            MapContentConfigToResponse(config.Marketplace),
            MapChatConfigToResponse(config.Chat),
            config.UpdatedAtUtc,
            config.UpdatedByUserId);
    }

    private static MediaContentConfigResponse MapContentConfigToResponse(MediaContentConfig config)
    {
        return new MediaContentConfigResponse(
            config.ImagesEnabled,
            config.VideosEnabled,
            config.AudioEnabled,
            config.MaxMediaCount,
            config.MaxVideoCount,
            config.MaxAudioCount,
            config.MaxImageSizeBytes,
            config.MaxVideoSizeBytes,
            config.MaxAudioSizeBytes,
            config.MaxVideoDurationSeconds,
            config.MaxAudioDurationSeconds,
            config.AllowedImageMimeTypes,
            config.AllowedVideoMimeTypes,
            config.AllowedAudioMimeTypes);
    }

    private static MediaChatConfigResponse MapChatConfigToResponse(MediaChatConfig config)
    {
        return new MediaChatConfigResponse(
            config.ImagesEnabled,
            config.AudioEnabled,
            config.VideosEnabled,
            config.MaxImageSizeBytes,
            config.MaxAudioSizeBytes,
            config.MaxAudioDurationSeconds,
            config.AllowedImageMimeTypes,
            config.AllowedAudioMimeTypes);
    }

    private static TerritoryMediaConfig UpdateConfigFromRequest(
        TerritoryMediaConfig existing,
        UpdateTerritoryMediaConfigRequest request)
    {
        if (request.Posts is not null)
        {
            existing.Posts = UpdateContentConfig(existing.Posts, request.Posts);
        }

        if (request.Events is not null)
        {
            existing.Events = UpdateContentConfig(existing.Events, request.Events);
        }

        if (request.Marketplace is not null)
        {
            existing.Marketplace = UpdateContentConfig(existing.Marketplace, request.Marketplace);
        }

        if (request.Chat is not null)
        {
            existing.Chat = UpdateChatConfig(existing.Chat, request.Chat);
        }

        return existing;
    }

    private static MediaContentConfig UpdateContentConfig(
        MediaContentConfig existing,
        MediaContentConfigRequest request)
    {
        return new MediaContentConfig
        {
            ImagesEnabled = request.ImagesEnabled ?? existing.ImagesEnabled,
            VideosEnabled = request.VideosEnabled ?? existing.VideosEnabled,
            AudioEnabled = request.AudioEnabled ?? existing.AudioEnabled,
            MaxMediaCount = request.MaxMediaCount ?? existing.MaxMediaCount,
            MaxVideoCount = request.MaxVideoCount ?? existing.MaxVideoCount,
            MaxAudioCount = request.MaxAudioCount ?? existing.MaxAudioCount,
            MaxImageSizeBytes = request.MaxImageSizeBytes ?? existing.MaxImageSizeBytes,
            MaxVideoSizeBytes = request.MaxVideoSizeBytes ?? existing.MaxVideoSizeBytes,
            MaxAudioSizeBytes = request.MaxAudioSizeBytes ?? existing.MaxAudioSizeBytes,
            MaxVideoDurationSeconds = request.MaxVideoDurationSeconds ?? existing.MaxVideoDurationSeconds,
            MaxAudioDurationSeconds = request.MaxAudioDurationSeconds ?? existing.MaxAudioDurationSeconds,
            AllowedImageMimeTypes = request.AllowedImageMimeTypes != null ? new List<string>(request.AllowedImageMimeTypes) : existing.AllowedImageMimeTypes,
            AllowedVideoMimeTypes = request.AllowedVideoMimeTypes != null ? new List<string>(request.AllowedVideoMimeTypes) : existing.AllowedVideoMimeTypes,
            AllowedAudioMimeTypes = request.AllowedAudioMimeTypes != null ? new List<string>(request.AllowedAudioMimeTypes) : existing.AllowedAudioMimeTypes
        };
    }

    private static MediaChatConfig UpdateChatConfig(
        MediaChatConfig existing,
        MediaChatConfigRequest request)
    {
        return new MediaChatConfig
        {
            ImagesEnabled = request.ImagesEnabled ?? existing.ImagesEnabled,
            AudioEnabled = request.AudioEnabled ?? existing.AudioEnabled,
            VideosEnabled = false, // Sempre bloqueado para chat
            MaxImageSizeBytes = request.MaxImageSizeBytes ?? existing.MaxImageSizeBytes,
            MaxAudioSizeBytes = request.MaxAudioSizeBytes ?? existing.MaxAudioSizeBytes,
            MaxAudioDurationSeconds = request.MaxAudioDurationSeconds ?? existing.MaxAudioDurationSeconds,
            AllowedImageMimeTypes = request.AllowedImageMimeTypes != null ? new List<string>(request.AllowedImageMimeTypes) : existing.AllowedImageMimeTypes,
            AllowedAudioMimeTypes = request.AllowedAudioMimeTypes != null ? new List<string>(request.AllowedAudioMimeTypes) : existing.AllowedAudioMimeTypes
        };
    }
}
