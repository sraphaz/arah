using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Models;
using Araponga.Domain.Media;

namespace Araponga.Application.Services.Media;

/// <summary>
/// Service para gerenciar configurações de mídia por território.
/// </summary>
public sealed class TerritoryMediaConfigService
{
    private readonly ITerritoryMediaConfigRepository _configRepository;
    private readonly IFeatureFlagService _featureFlagService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGlobalMediaLimits _globalLimits;

    public TerritoryMediaConfigService(
        ITerritoryMediaConfigRepository configRepository,
        IFeatureFlagService featureFlagService,
        IUnitOfWork unitOfWork,
        IGlobalMediaLimits globalLimits)
    {
        _configRepository = configRepository;
        _featureFlagService = featureFlagService;
        _unitOfWork = unitOfWork;
        _globalLimits = globalLimits;
    }

    /// <summary>
    /// Obtém configuração de mídia para um território (com valores padrão se não existir).
    /// </summary>
    public async Task<TerritoryMediaConfig> GetConfigAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        return await _configRepository.GetOrCreateDefaultAsync(territoryId, cancellationToken);
    }

    /// <summary>
    /// Atualiza configuração de mídia para um território.
    /// </summary>
    public async Task<TerritoryMediaConfig> UpdateConfigAsync(
        Guid territoryId,
        TerritoryMediaConfig config,
        Guid updatedByUserId,
        CancellationToken cancellationToken = default)
    {
        if (config.TerritoryId != territoryId)
        {
            throw new ArgumentException("Territory ID mismatch.", nameof(config));
        }

        // Validar limites contra valores globais
        ValidateLimits(config);

        config.UpdatedAtUtc = DateTime.UtcNow;
        config.UpdatedByUserId = updatedByUserId;

        await _configRepository.SaveAsync(config, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return config;
    }

    /// <summary>
    /// Valida limites configurados contra valores globais (MediaStorageOptions).
    /// </summary>
    private void ValidateLimits(TerritoryMediaConfig config)
    {
        // Validar Posts
        ValidateContentLimits(config.Posts, "Posts");

        // Validar Events
        ValidateContentLimits(config.Events, "Events");

        // Validar Marketplace
        ValidateContentLimits(config.Marketplace, "Marketplace");

        // Validar Chat
        ValidateChatLimits(config.Chat);
    }

    private void ValidateContentLimits(MediaContentConfig config, string context)
    {
        // Validar tamanhos máximos não podem exceder valores globais
        if (config.MaxImageSizeBytes > _globalLimits.MaxImageSizeBytes)
        {
            throw new ArgumentException(
                $"{context}: MaxImageSizeBytes ({config.MaxImageSizeBytes}) cannot exceed global limit ({_globalLimits.MaxImageSizeBytes}).",
                nameof(config));
        }

        if (config.MaxVideoSizeBytes > _globalLimits.MaxVideoSizeBytes)
        {
            throw new ArgumentException(
                $"{context}: MaxVideoSizeBytes ({config.MaxVideoSizeBytes}) cannot exceed global limit ({_globalLimits.MaxVideoSizeBytes}).",
                nameof(config));
        }

        if (config.MaxAudioSizeBytes > _globalLimits.MaxAudioSizeBytes)
        {
            throw new ArgumentException(
                $"{context}: MaxAudioSizeBytes ({config.MaxAudioSizeBytes}) cannot exceed global limit ({_globalLimits.MaxAudioSizeBytes}).",
                nameof(config));
        }

        // Validar limites mínimos razoáveis
        const long minSizeBytes = 1024; // 1KB
        if (config.MaxImageSizeBytes < minSizeBytes ||
            config.MaxVideoSizeBytes < minSizeBytes ||
            config.MaxAudioSizeBytes < minSizeBytes)
        {
            throw new ArgumentException(
                $"{context}: Size limits must be at least {minSizeBytes} bytes (1KB).",
                nameof(config));
        }

        // Validar contagens máximas
        if (config.MaxMediaCount < 1 || config.MaxMediaCount > 100)
        {
            throw new ArgumentException(
                $"{context}: MaxMediaCount must be between 1 and 100.",
                nameof(config));
        }

        if (config.MaxVideoCount < 0 || config.MaxVideoCount > config.MaxMediaCount)
        {
            throw new ArgumentException(
                $"{context}: MaxVideoCount must be between 0 and MaxMediaCount ({config.MaxMediaCount}).",
                nameof(config));
        }

        if (config.MaxAudioCount < 0 || config.MaxAudioCount > config.MaxMediaCount)
        {
            throw new ArgumentException(
                $"{context}: MaxAudioCount must be between 0 and MaxMediaCount ({config.MaxMediaCount}).",
                nameof(config));
        }
    }

    private void ValidateChatLimits(MediaChatConfig config)
    {
        // Vídeos sempre desabilitados no chat
        if (config.VideosEnabled)
        {
            throw new ArgumentException(
                "Chat: VideosEnabled must be false (videos are not allowed in chat).",
                nameof(config));
        }

        // Validar tamanhos máximos
        if (config.MaxImageSizeBytes > _globalLimits.MaxImageSizeBytes)
        {
            throw new ArgumentException(
                $"Chat: MaxImageSizeBytes ({config.MaxImageSizeBytes}) cannot exceed global limit ({_globalLimits.MaxImageSizeBytes}).",
                nameof(config));
        }

        if (config.MaxAudioSizeBytes > _globalLimits.MaxAudioSizeBytes)
        {
            throw new ArgumentException(
                $"Chat: MaxAudioSizeBytes ({config.MaxAudioSizeBytes}) cannot exceed global limit ({_globalLimits.MaxAudioSizeBytes}).",
                nameof(config));
        }

        // Validar limites mínimos
        const long minSizeBytes = 1024; // 1KB
        if (config.MaxImageSizeBytes < minSizeBytes || config.MaxAudioSizeBytes < minSizeBytes)
        {
            throw new ArgumentException(
                $"Chat: Size limits must be at least {minSizeBytes} bytes (1KB).",
                nameof(config));
        }
    }

    /// <summary>
    /// Obtém limites efetivos para um tipo de conteúdo, aplicando fallback para valores globais se não configurado.
    /// </summary>
    public async Task<MediaContentConfig> GetEffectiveContentLimitsAsync(
        Guid territoryId,
        MediaContentType contentType,
        CancellationToken cancellationToken = default)
    {
        var config = await GetContentConfigAsync(territoryId, contentType, cancellationToken);
        return ApplyGlobalDefaults(config);
    }

    /// <summary>
    /// Obtém limites efetivos para chat, aplicando fallback para valores globais se não configurado.
    /// </summary>
    public async Task<MediaChatConfig> GetEffectiveChatLimitsAsync(
        Guid territoryId,
        CancellationToken cancellationToken = default)
    {
        var config = await GetChatConfigAsync(territoryId, cancellationToken);
        return ApplyGlobalDefaultsForChat(config);
    }

    /// <summary>
    /// Obtém limites efetivos aplicando fallback para valores globais se não configurado.
    /// Retorna uma cópia para não modificar o original.
    /// </summary>
    private MediaContentConfig ApplyGlobalDefaults(MediaContentConfig config)
    {
        // Criar cópia para não modificar o original
        var effective = new MediaContentConfig
        {
            ImagesEnabled = config.ImagesEnabled,
            VideosEnabled = config.VideosEnabled,
            AudioEnabled = config.AudioEnabled,
            MaxMediaCount = config.MaxMediaCount,
            MaxVideoCount = config.MaxVideoCount,
            MaxAudioCount = config.MaxAudioCount,
            MaxImageSizeBytes = Math.Min(config.MaxImageSizeBytes, _globalLimits.MaxImageSizeBytes),
            MaxVideoSizeBytes = Math.Min(config.MaxVideoSizeBytes, _globalLimits.MaxVideoSizeBytes),
            MaxAudioSizeBytes = Math.Min(config.MaxAudioSizeBytes, _globalLimits.MaxAudioSizeBytes),
            MaxVideoDurationSeconds = config.MaxVideoDurationSeconds,
            MaxAudioDurationSeconds = config.MaxAudioDurationSeconds,
            // Aplicar tipos MIME globais como fallback
            AllowedImageMimeTypes = config.AllowedImageMimeTypes != null && config.AllowedImageMimeTypes.Count > 0
                ? new List<string>(config.AllowedImageMimeTypes)
                : new List<string>(_globalLimits.AllowedImageMimeTypes),
            AllowedVideoMimeTypes = config.AllowedVideoMimeTypes != null && config.AllowedVideoMimeTypes.Count > 0
                ? new List<string>(config.AllowedVideoMimeTypes)
                : new List<string>(_globalLimits.AllowedVideoMimeTypes),
            AllowedAudioMimeTypes = config.AllowedAudioMimeTypes != null && config.AllowedAudioMimeTypes.Count > 0
                ? new List<string>(config.AllowedAudioMimeTypes)
                : new List<string>(_globalLimits.AllowedAudioMimeTypes)
        };

        return effective;
    }

    private MediaChatConfig ApplyGlobalDefaultsForChat(MediaChatConfig config)
    {
        // Criar cópia para não modificar o original
        var effective = new MediaChatConfig
        {
            ImagesEnabled = config.ImagesEnabled,
            AudioEnabled = config.AudioEnabled,
            VideosEnabled = false, // Sempre false para chat
            MaxImageSizeBytes = Math.Min(config.MaxImageSizeBytes, _globalLimits.MaxImageSizeBytes),
            MaxAudioSizeBytes = Math.Min(config.MaxAudioSizeBytes, _globalLimits.MaxAudioSizeBytes),
            MaxAudioDurationSeconds = config.MaxAudioDurationSeconds,
            // Aplicar tipos MIME globais como fallback
            AllowedImageMimeTypes = config.AllowedImageMimeTypes != null && config.AllowedImageMimeTypes.Count > 0
                ? new List<string>(config.AllowedImageMimeTypes)
                : new List<string>(_globalLimits.AllowedImageMimeTypes),
            AllowedAudioMimeTypes = config.AllowedAudioMimeTypes != null && config.AllowedAudioMimeTypes.Count > 0
                ? new List<string>(config.AllowedAudioMimeTypes)
                : new List<string>(_globalLimits.AllowedAudioMimeTypes)
        };

        return effective;
    }

    /// <summary>
    /// Verifica se um tipo de mídia está habilitado para um território e tipo de conteúdo.
    /// </summary>
    public async Task<bool> IsMediaTypeEnabledAsync(
        Guid territoryId,
        MediaType mediaType,
        MediaContentType contentType,
        CancellationToken cancellationToken = default)
    {
        // Verificar feature flags primeiro
        var featureFlagEnabled = mediaType switch
        {
            MediaType.Image => _featureFlagService.IsEnabled(territoryId, FeatureFlag.MediaImagesEnabled),
            MediaType.Video => _featureFlagService.IsEnabled(territoryId, FeatureFlag.MediaVideosEnabled),
            MediaType.Audio => _featureFlagService.IsEnabled(territoryId, FeatureFlag.MediaAudioEnabled),
            _ => false
        };

        if (!featureFlagEnabled && contentType != MediaContentType.Chat)
        {
            return false;
        }

        // Verificar configuração específica do território
        var config = await GetConfigAsync(territoryId, cancellationToken);

        return contentType switch
        {
            MediaContentType.Posts => IsMediaTypeEnabledInConfig(config.Posts, mediaType),
            MediaContentType.Events => IsMediaTypeEnabledInConfig(config.Events, mediaType),
            MediaContentType.Marketplace => IsMediaTypeEnabledInConfig(config.Marketplace, mediaType),
            MediaContentType.Chat => IsMediaTypeEnabledInChat(config.Chat, mediaType),
            _ => false
        };
    }

    private static bool IsMediaTypeEnabledInConfig(MediaContentConfig config, MediaType mediaType)
    {
        return mediaType switch
        {
            MediaType.Image => config.ImagesEnabled,
            MediaType.Video => config.VideosEnabled,
            MediaType.Audio => config.AudioEnabled,
            _ => false
        };
    }

    private static bool IsMediaTypeEnabledInChat(MediaChatConfig config, MediaType mediaType)
    {
        // Verificar feature flags específicas do chat
        // TODO: Injetar IFeatureFlagService para verificar ChatMediaImagesEnabled e ChatMediaAudioEnabled
        // Por enquanto, usar apenas configuração

        return mediaType switch
        {
            MediaType.Image => config.ImagesEnabled,
            MediaType.Audio => config.AudioEnabled,
            MediaType.Video => config.VideosEnabled, // Sempre false para chat
            _ => false
        };
    }

    /// <summary>
    /// Obtém limites configurados para um tipo de conteúdo.
    /// </summary>
    public async Task<MediaContentConfig> GetContentConfigAsync(
        Guid territoryId,
        MediaContentType contentType,
        CancellationToken cancellationToken = default)
    {
        var config = await GetConfigAsync(territoryId, cancellationToken);

        return contentType switch
        {
            MediaContentType.Posts => config.Posts,
            MediaContentType.Events => config.Events,
            MediaContentType.Marketplace => config.Marketplace,
            _ => throw new ArgumentException($"Invalid content type: {contentType}", nameof(contentType))
        };
    }

    /// <summary>
    /// Obtém configuração de chat.
    /// </summary>
    public async Task<MediaChatConfig> GetChatConfigAsync(
        Guid territoryId,
        CancellationToken cancellationToken = default)
    {
        var config = await GetConfigAsync(territoryId, cancellationToken);
        return config.Chat;
    }
}

/// <summary>
/// Tipo de conteúdo que pode ter mídias associadas.
/// </summary>
public enum MediaContentType
{
    Posts = 1,
    Events = 2,
    Marketplace = 3,
    Chat = 4
}
