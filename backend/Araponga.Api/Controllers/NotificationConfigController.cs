using Araponga.Api.Contracts.Notifications;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Application.Services.Notifications;
using Araponga.Domain.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("Notifications")]
public sealed class NotificationConfigController : ControllerBase
{
    private readonly NotificationConfigService _service;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public NotificationConfigController(
        NotificationConfigService service,
        AccessEvaluator accessEvaluator,
        CurrentUserAccessor currentUserAccessor)
    {
        _service = service;
        _accessEvaluator = accessEvaluator;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Obtém configuração de notificações de um território.
    /// Requer permissão de Curator.
    /// </summary>
    [HttpGet("territories/{territoryId}/notification-config")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(NotificationConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<NotificationConfigResponse>> GetTerritoryConfig(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var hasPermission = await _accessEvaluator.HasCapabilityAsync(
            userContext.User.Id,
            territoryId,
            Domain.Membership.MembershipCapabilityType.Curator,
            cancellationToken);

        if (!hasPermission)
        {
            return Forbid();
        }

        var result = await _service.GetConfigAsync(territoryId, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(ToResponse(result.Value!));
    }

    /// <summary>
    /// Cria ou atualiza configuração de notificações de um território.
    /// Requer permissão de Curator.
    /// </summary>
    [HttpPut("territories/{territoryId}/notification-config")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(NotificationConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<NotificationConfigResponse>> UpdateTerritoryConfig(
        Guid territoryId,
        [FromBody] CreateOrUpdateNotificationConfigRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var hasPermission = await _accessEvaluator.HasCapabilityAsync(
            userContext.User.Id,
            territoryId,
            Domain.Membership.MembershipCapabilityType.Curator,
            cancellationToken);

        if (!hasPermission)
        {
            return Forbid();
        }

        var notificationTypes = request.NotificationTypes.ToDictionary(
            kvp => kvp.Key,
            kvp => new NotificationTypeConfig(
                kvp.Value.Type,
                kvp.Value.Enabled,
                kvp.Value.AllowedChannels));

        var result = await _service.CreateOrUpdateConfigAsync(
            territoryId,
            notificationTypes,
            request.AvailableChannels,
            request.Templates,
            request.DefaultChannels,
            request.Enabled,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(ToResponse(result.Value!));
    }

    /// <summary>
    /// Obtém configuração global de notificações.
    /// Requer permissão de SystemAdmin.
    /// </summary>
    [HttpGet("admin/notification-config")]
    [EnableRateLimiting("read")]
    [Authorize(Policy = "SystemAdmin")]
    [ProducesResponseType(typeof(NotificationConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<NotificationConfigResponse>> GetGlobalConfig(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetConfigAsync(null, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(ToResponse(result.Value!));
    }

    /// <summary>
    /// Cria ou atualiza configuração global de notificações.
    /// Requer permissão de SystemAdmin.
    /// </summary>
    [HttpPut("admin/notification-config")]
    [EnableRateLimiting("write")]
    [Authorize(Policy = "SystemAdmin")]
    [ProducesResponseType(typeof(NotificationConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<NotificationConfigResponse>> UpdateGlobalConfig(
        [FromBody] CreateOrUpdateNotificationConfigRequest request,
        CancellationToken cancellationToken)
    {
        var notificationTypes = request.NotificationTypes.ToDictionary(
            kvp => kvp.Key,
            kvp => new NotificationTypeConfig(
                kvp.Value.Type,
                kvp.Value.Enabled,
                kvp.Value.AllowedChannels));

        var result = await _service.CreateOrUpdateConfigAsync(
            null, // Global
            notificationTypes,
            request.AvailableChannels,
            request.Templates,
            request.DefaultChannels,
            request.Enabled,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(ToResponse(result.Value!));
    }

    private static NotificationConfigResponse ToResponse(NotificationConfig config)
    {
        return new NotificationConfigResponse(
            config.Id,
            config.TerritoryId,
            config.NotificationTypes.ToDictionary(
                kvp => kvp.Key,
                kvp => new NotificationTypeConfigResponse(
                    kvp.Value.Type,
                    kvp.Value.Enabled,
                    kvp.Value.AllowedChannels)),
            config.AvailableChannels,
            config.Templates,
            config.DefaultChannels,
            config.Enabled,
            config.CreatedAtUtc,
            config.UpdatedAtUtc);
    }
}
