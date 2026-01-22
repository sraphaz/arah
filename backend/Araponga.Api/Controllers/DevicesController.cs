using Araponga.Api.Contracts.Devices;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/users/me/devices")]
[Authorize]
[Produces("application/json")]
[Tags("Devices")]
public sealed class DevicesController : ControllerBase
{
    private readonly PushNotificationService _pushNotificationService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public DevicesController(
        PushNotificationService pushNotificationService,
        CurrentUserAccessor currentUserAccessor)
    {
        _pushNotificationService = pushNotificationService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Registra um dispositivo para receber notificações push.
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterDevice(
        [FromBody] RegisterDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _pushNotificationService.RegisterDeviceAsync(
            userContext.User.Id,
            request.DeviceToken,
            request.Platform,
            request.DeviceName,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        var device = result.Value;
        if (device is null)
        {
            return BadRequest(new { error = "Device registration failed." });
        }

        return CreatedAtAction(
            nameof(GetDevice),
            new { deviceId = device.Id },
            new DeviceResponse(
                device.Id,
                device.Platform,
                device.DeviceName,
                device.RegisteredAtUtc,
                device.LastUsedAtUtc,
                device.IsActive));
    }

    /// <summary>
    /// Lista todos os dispositivos ativos do usuário.
    /// </summary>
    [HttpGet]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(IReadOnlyList<DeviceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListDevices(CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _pushNotificationService.ListDevicesAsync(
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        var devices = result.Value ?? Array.Empty<Araponga.Domain.Users.UserDevice>();
        var responses = devices.Select(d => new DeviceResponse(
            d.Id,
            d.Platform,
            d.DeviceName,
            d.RegisteredAtUtc,
            d.LastUsedAtUtc,
            d.IsActive)).ToList();

        return Ok(responses);
    }

    /// <summary>
    /// Obtém um dispositivo específico.
    /// </summary>
    [HttpGet("{deviceId:guid}")]
    [EnableRateLimiting("read")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDevice(Guid deviceId, CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _pushNotificationService.GetDeviceAsync(
            userContext.User.Id,
            deviceId,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        var device = result.Value;
        if (device is null)
        {
            return NotFound();
        }

        return Ok(new DeviceResponse(
            device.Id,
            device.Platform,
            device.DeviceName,
            device.RegisteredAtUtc,
            device.LastUsedAtUtc,
            device.IsActive));
    }

    /// <summary>
    /// Remove um dispositivo.
    /// </summary>
    [HttpDelete("{deviceId:guid}")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnregisterDevice(Guid deviceId, CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _pushNotificationService.UnregisterDeviceAsync(
            userContext.User.Id,
            deviceId,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }
}
