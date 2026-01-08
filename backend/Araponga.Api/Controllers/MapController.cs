using Araponga.Api.Contracts.Map;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/map")]
[Produces("application/json")]
[Tags("Map")]
public sealed class MapController : ControllerBase
{
    private readonly MapService _mapService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ActiveTerritoryService _activeTerritoryService;

    public MapController(
        MapService mapService,
        CurrentUserAccessor currentUserAccessor,
        ActiveTerritoryService activeTerritoryService)
    {
        _mapService = mapService;
        _currentUserAccessor = currentUserAccessor;
        _activeTerritoryService = activeTerritoryService;
    }

    /// <summary>
    /// Visualiza entidades do mapa no território ativo.
    /// </summary>
    /// <remarks>
    /// Visitantes veem apenas entidades públicas; moradores veem todas.
    /// </remarks>
    [HttpGet("entities")]
    [ProducesResponseType(typeof(IEnumerable<MapEntityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<MapEntityResponse>>> GetEntities(CancellationToken cancellationToken)
    {
        var sessionId = GetSessionId();
        if (sessionId is null)
        {
            return BadRequest(new { error = "X-Session-Id header is required." });
        }

        var territoryId = await _activeTerritoryService.GetActiveAsync(sessionId, cancellationToken);
        if (territoryId is null)
        {
            return BadRequest(new { error = "No active territory selected." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status == TokenStatus.Invalid)
        {
            return Unauthorized();
        }

        var entities = await _mapService.ListEntitiesAsync(
            territoryId.Value,
            userContext.User?.Id,
            cancellationToken);

        var response = entities.Select(entity =>
            new MapEntityResponse(
                entity.Id,
                entity.Name,
                entity.Category,
                entity.Visibility.ToString().ToUpperInvariant(),
                entity.CreatedAtUtc));

        return Ok(response);
    }

    private string? GetSessionId()
    {
        return Request.Headers.TryGetValue(ApiHeaders.SessionId, out var header) &&
               !string.IsNullOrWhiteSpace(header)
            ? header.ToString()
            : null;
    }
}
