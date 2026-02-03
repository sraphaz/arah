using Araponga.Api.Contracts.Media;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Application.Services.Media;
using Araponga.Domain.Media;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

/// <summary>
/// Controller para gerenciar configurações de blob storage para mídias (requer SystemAdmin).
/// Permite configuração explícita e aberta do provedor (Local, S3, AzureBlob) via painel administrativo.
/// </summary>
[ApiController]
[Route("api/v1/admin/media-storage-config")]
[Produces("application/json")]
[Tags("Admin - Media Storage")]
public sealed class MediaStorageConfigController : ControllerBase
{
    private readonly MediaStorageConfigService _configService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;

    public MediaStorageConfigController(
        MediaStorageConfigService configService,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator)
    {
        _configService = configService;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Obtém a configuração ativa de storage.
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(MediaStorageConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MediaStorageConfigResponse>> GetActive(CancellationToken cancellationToken)
    {
        var config = await _configService.GetActiveConfigAsync(cancellationToken);
        if (config is null)
        {
            return NotFound(new { error = "No active storage configuration found." });
        }

        var response = MapToResponse(config);
        return Ok(response);
    }

    /// <summary>
    /// Lista todas as configurações de storage.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MediaStorageConfigResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<MediaStorageConfigResponse>>> List(CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            return Forbid();
        }

        var configs = await _configService.ListAllAsync(cancellationToken);
        var responses = configs.Select(MapToResponse).ToList();
        return Ok(responses);
    }

    /// <summary>
    /// Obtém configuração por ID.
    /// </summary>
    [HttpGet("{configId:guid}")]
    [ProducesResponseType(typeof(MediaStorageConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MediaStorageConfigResponse>> GetById(
        [FromRoute] Guid configId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            return Forbid();
        }

        var config = await _configService.GetByIdAsync(configId, cancellationToken);
        if (config is null)
        {
            return NotFound(new { error = "Storage configuration not found." });
        }

        var response = MapToResponse(config);
        return Ok(response);
    }

    /// <summary>
    /// Cria nova configuração de storage (inativa por padrão).
    /// </summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(MediaStorageConfigResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MediaStorageConfigResponse>> Create(
        [FromBody] CreateMediaStorageConfigRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            return Forbid();
        }

        if (!Enum.TryParse<MediaStorageProvider>(request.Provider, ignoreCase: true, out var provider))
        {
            return BadRequest(new { error = $"Invalid provider: {request.Provider}. Valid values: Local, S3, AzureBlob." });
        }

        var settings = MapSettingsFromRequest(request.Settings, provider);
        if (settings is null)
        {
            return BadRequest(new { error = "Settings are required and must match the selected provider." });
        }

        var config = await _configService.CreateConfigAsync(
            provider,
            settings,
            userContext.User.Id,
            request.Description,
            cancellationToken);

        var response = MapToResponse(config);
        return CreatedAtAction(nameof(GetById), new { configId = config.Id }, response);
    }

    /// <summary>
    /// Atualiza configuração de storage existente.
    /// </summary>
    [HttpPut("{configId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(MediaStorageConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MediaStorageConfigResponse>> Update(
        [FromRoute] Guid configId,
        [FromBody] UpdateMediaStorageConfigRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            return Forbid();
        }

        var existingConfig = await _configService.GetByIdAsync(configId, cancellationToken);
        if (existingConfig is null)
        {
            return NotFound(new { error = "Storage configuration not found." });
        }

        // Se Settings foram fornecidos, validar e mapear
        MediaStorageSettings? settings = null;
        if (request.Settings is not null)
        {
            settings = MapSettingsFromRequest(request.Settings, existingConfig.Provider);
            if (settings is null)
            {
                return BadRequest(new { error = "Settings are invalid for the selected provider." });
            }
        }
        else
        {
            // Se Settings não foram fornecidos, manter os existentes
            settings = existingConfig.Settings;
        }

        var config = await _configService.UpdateConfigAsync(
            configId,
            settings,
            userContext.User.Id,
            request.Description,
            cancellationToken);

        var response = MapToResponse(config);
        return Ok(response);
    }

    /// <summary>
    /// Ativa uma configuração de storage (desativa todas as outras).
    /// </summary>
    [HttpPost("{configId:guid}/activate")]
    [ProducesResponseType(typeof(MediaStorageConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MediaStorageConfigResponse>> Activate(
        [FromRoute] Guid configId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);

        if (!isAdmin)
        {
            return Forbid();
        }

        var config = await _configService.ActivateConfigAsync(
            configId,
            userContext.User.Id,
            cancellationToken);

        var response = MapToResponse(config);
        return Ok(response);
    }

    private static MediaStorageConfigResponse MapToResponse(Domain.Media.MediaStorageConfig config)
    {
        return new MediaStorageConfigResponse(
            config.Id,
            config.Provider.ToString(),
            MapSettingsToResponse(config.Settings),
            config.IsActive,
            config.Description,
            config.CreatedAtUtc,
            config.CreatedByUserId,
            config.UpdatedAtUtc,
            config.UpdatedByUserId);
    }

    private static MediaStorageSettingsResponse MapSettingsToResponse(MediaStorageSettings settings)
    {
        return new MediaStorageSettingsResponse(
            settings.EnableUrlCache,
            settings.UrlCacheExpiration,
            settings.Local != null ? new LocalStorageSettingsResponse(settings.Local.BasePath) : null,
            settings.S3 != null ? MapS3SettingsToResponse(settings.S3) : null,
            settings.AzureBlob != null ? MapAzureBlobSettingsToResponse(settings.AzureBlob) : null);
    }

    private static S3StorageSettingsResponse MapS3SettingsToResponse(S3StorageSettings s3)
    {
        // Mascarar AccessKeyId para segurança (mostrar apenas últimos 4 caracteres)
        var maskedAccessKeyId = MaskSecret(s3.AccessKeyId, 4);
        return new S3StorageSettingsResponse(
            s3.BucketName,
            s3.Region,
            maskedAccessKeyId,
            s3.Prefix);
    }

    private static AzureBlobStorageSettingsResponse MapAzureBlobSettingsToResponse(AzureBlobStorageSettings azureBlob)
    {
        // Mascarar ConnectionString para segurança (mostrar apenas host)
        var maskedConnectionString = MaskConnectionString(azureBlob.ConnectionString);
        return new AzureBlobStorageSettingsResponse(
            maskedConnectionString,
            azureBlob.ContainerName,
            azureBlob.Prefix);
    }

    private static MediaStorageSettings? MapSettingsFromRequest(
        MediaStorageSettingsRequest? request,
        MediaStorageProvider provider)
    {
        if (request is null)
        {
            return CreateDefaultSettings(provider);
        }

        return provider switch
        {
            MediaStorageProvider.Local => new MediaStorageSettings
            {
                EnableUrlCache = request.EnableUrlCache ?? true,
                UrlCacheExpiration = request.UrlCacheExpiration ?? TimeSpan.FromHours(24),
                Local = request.Local != null ? new LocalStorageSettings(request.Local.BasePath) : null,
                S3 = null,
                AzureBlob = null
            },
            MediaStorageProvider.S3 => new MediaStorageSettings
            {
                EnableUrlCache = request.EnableUrlCache ?? true,
                UrlCacheExpiration = request.UrlCacheExpiration ?? TimeSpan.FromHours(24),
                Local = null,
                S3 = request.S3 != null ? new S3StorageSettings(
                    request.S3.BucketName,
                    request.S3.Region,
                    request.S3.AccessKeyId,
                    request.S3.Prefix) : null,
                AzureBlob = null
            },
            MediaStorageProvider.AzureBlob => new MediaStorageSettings
            {
                EnableUrlCache = request.EnableUrlCache ?? true,
                UrlCacheExpiration = request.UrlCacheExpiration ?? TimeSpan.FromHours(24),
                Local = null,
                S3 = null,
                AzureBlob = request.AzureBlob != null ? new AzureBlobStorageSettings(
                    request.AzureBlob.ConnectionString,
                    request.AzureBlob.ContainerName,
                    request.AzureBlob.Prefix) : null
            },
            _ => null
        };
    }

    private static MediaStorageSettings CreateDefaultSettings(MediaStorageProvider provider)
    {
        return provider switch
        {
            MediaStorageProvider.Local => new MediaStorageSettings
            {
                EnableUrlCache = true,
                UrlCacheExpiration = TimeSpan.FromHours(24),
                Local = new LocalStorageSettings("wwwroot/media"),
                S3 = null,
                AzureBlob = null
            },
            _ => throw new ArgumentException($"Default settings not supported for provider: {provider}", nameof(provider))
        };
    }

    private static string MaskSecret(string secret, int visibleChars)
    {
        if (string.IsNullOrWhiteSpace(secret) || secret.Length <= visibleChars)
        {
            return "****";
        }

        return new string('*', secret.Length - visibleChars) + secret.Substring(secret.Length - visibleChars);
    }

    private static string MaskConnectionString(string connectionString)
    {
        // Extrair apenas o host da connection string (ex: "DefaultEndpointsProtocol=https;AccountName=xxx;AccountKey=xxx;EndpointSuffix=core.windows.net")
        // Retornar apenas o protocolo e host para segurança
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return "****";
        }

        try
        {
            var parts = connectionString.Split(';');
            var protocol = parts.FirstOrDefault(p => p.StartsWith("DefaultEndpointsProtocol=", StringComparison.OrdinalIgnoreCase));
            var endpoint = parts.FirstOrDefault(p => p.StartsWith("BlobEndpoint=", StringComparison.OrdinalIgnoreCase) || p.StartsWith("EndpointSuffix=", StringComparison.OrdinalIgnoreCase));
            
            if (!string.IsNullOrWhiteSpace(protocol))
            {
                return $"{protocol};****";
            }

            return "****";
        }
        catch
        {
            return "****";
        }
    }
}
