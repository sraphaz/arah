using Araponga.Api.Contracts.Admin;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Configuration;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/admin/system-configs")]
[Produces("application/json")]
[Tags("Admin")]
public sealed class AdminSystemConfigController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly SystemConfigService _systemConfigService;

    public AdminSystemConfigController(
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator,
        SystemConfigService systemConfigService)
    {
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
        _systemConfigService = systemConfigService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SystemConfigResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<SystemConfigResponse>>> List(
        [FromQuery] string? category,
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
            return Unauthorized();
        }

        SystemConfigCategory? parsedCategory = null;
        if (!string.IsNullOrWhiteSpace(category))
        {
            if (!Enum.TryParse<SystemConfigCategory>(category, true, out var categoryEnum))
            {
                return BadRequest(new { error = $"Invalid category: {category}" });
            }

            parsedCategory = categoryEnum;
        }

        var configs = await _systemConfigService.ListAsync(parsedCategory, cancellationToken);
        var response = configs.Select(ToResponse).ToList();
        return Ok(response);
    }

    [HttpGet("{key}")]
    [ProducesResponseType(typeof(SystemConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SystemConfigResponse>> GetByKey(
        [FromRoute] string key,
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
            return Unauthorized();
        }

        var config = await _systemConfigService.GetByKeyAsync(key, cancellationToken);
        if (config is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(config));
    }

    [HttpPut("{key}")]
    [ProducesResponseType(typeof(SystemConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SystemConfigResponse>> Upsert(
        [FromRoute] string key,
        [FromBody] UpsertSystemConfigRequest request,
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
            return Unauthorized();
        }

        if (!Enum.TryParse<SystemConfigCategory>(request.Category, true, out var parsedCategory))
        {
            return BadRequest(new { error = $"Invalid category: {request.Category}" });
        }

        var result = await _systemConfigService.UpsertAsync(
            key,
            request.Value,
            parsedCategory,
            request.Description,
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to upsert system config." });
        }

        return Ok(ToResponse(result.Value));
    }

    private static SystemConfigResponse ToResponse(SystemConfig config)
        => new(
            config.Key,
            config.Value,
            config.Category.ToString().ToUpperInvariant(),
            config.Description,
            config.CreatedAtUtc,
            config.CreatedByUserId,
            config.UpdatedAtUtc,
            config.UpdatedByUserId);
}

