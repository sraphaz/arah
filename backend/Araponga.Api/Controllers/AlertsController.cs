using Araponga.Api.Contracts.Alerts;
using Araponga.Api.Contracts.Common;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Health;
using Araponga.Domain.Membership;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/alerts")]
[Produces("application/json")]
[Tags("Alerts")]
public sealed class AlertsController : ControllerBase
{
    private readonly ActiveTerritoryService _activeTerritoryService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly HealthService _healthService;

    public AlertsController(
        ActiveTerritoryService activeTerritoryService,
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator,
        HealthService healthService)
    {
        _activeTerritoryService = activeTerritoryService;
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
        _healthService = healthService;
    }

    /// <summary>
    /// Lista alertas do território ativo.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AlertResponse>>> GetAlerts(
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userContext.User.Id, resolvedTerritoryId.Value, cancellationToken);
        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, resolvedTerritoryId.Value, MembershipCapabilityType.Curator, cancellationToken);
        if (!isResident && !isCurator)
        {
            return Unauthorized();
        }

        var alerts = await _healthService.ListAlertsAsync(resolvedTerritoryId.Value, cancellationToken);

        var response = alerts.Select(alert => new AlertResponse(
            alert.Id,
            alert.Title,
            alert.Description,
            alert.Status.ToString().ToUpperInvariant(),
            alert.CreatedAtUtc));

        return Ok(response);
    }

    /// <summary>
    /// Lista alertas do território ativo (paginado).
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<AlertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<AlertResponse>>> GetAlertsPaged(
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userContext.User.Id, resolvedTerritoryId.Value, cancellationToken);
        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, resolvedTerritoryId.Value, MembershipCapabilityType.Curator, cancellationToken);
        if (!isResident && !isCurator)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _healthService.ListAlertsPagedAsync(resolvedTerritoryId.Value, pagination, cancellationToken);

        var response = new PagedResponse<AlertResponse>(
            pagedResult.Items.Select(alert => new AlertResponse(
                alert.Id,
                alert.Title,
                alert.Description,
                alert.Status.ToString().ToUpperInvariant(),
                alert.CreatedAtUtc)).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Reporta um alerta ambiental.
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(AlertResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AlertResponse>> ReportAlert(
        [FromQuery] Guid? territoryId,
        [FromBody] ReportAlertRequest request,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userContext.User.Id, resolvedTerritoryId.Value, cancellationToken);
        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, resolvedTerritoryId.Value, MembershipCapabilityType.Curator, cancellationToken);
        if (!isResident && !isCurator)
        {
            return Unauthorized();
        }

        var result = await _healthService.ReportAlertAsync(
            resolvedTerritoryId.Value,
            userContext.User.Id,
            request.Title,
            request.Description,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to report alert." });
        }

        var response = new AlertResponse(
            result.Value.Id,
            result.Value.Title,
            result.Value.Description,
            result.Value.Status.ToString().ToUpperInvariant(),
            result.Value.CreatedAtUtc);

        return CreatedAtAction(nameof(ReportAlert), new { }, response);
    }

    /// <summary>
    /// Valida alerta ambiental (curadoria).
    /// </summary>
    [HttpPatch("{alertId:guid}/validation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ValidateAlert(
        [FromRoute] Guid alertId,
        [FromQuery] Guid? territoryId,
        [FromBody] ValidateAlertRequest request,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, resolvedTerritoryId.Value, MembershipCapabilityType.Curator, cancellationToken);
        if (!isCurator)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<HealthAlertStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { error = "Invalid status." });
        }

        var success = await _healthService.ValidateAlertAsync(
            resolvedTerritoryId.Value,
            alertId,
            userContext.User.Id,
            status,
            cancellationToken);

        return success ? NoContent() : BadRequest(new { error = "Alert not found." });
    }

    private string? GetSessionId()
    {
        return Request.Headers.TryGetValue(ApiHeaders.SessionId, out var header) &&
               !string.IsNullOrWhiteSpace(header)
            ? header.ToString()
            : null;
    }

    private async Task<Guid?> ResolveTerritoryIdAsync(Guid? territoryId, CancellationToken cancellationToken)
    {
        if (territoryId is not null)
        {
            return territoryId;
        }

        var sessionId = GetSessionId();
        if (sessionId is null)
        {
            return null;
        }

        return await _activeTerritoryService.GetActiveAsync(sessionId, cancellationToken);
    }
}
