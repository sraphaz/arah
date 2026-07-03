using Arah.Api.Contracts.Common;
using Arah.Api.Contracts.Map;
using Arah.Api.Security;
using Arah.Application.Common;
using Arah.Application.Models;
using Arah.Application.Services;
using Arah.Modules.Map.Domain;
using Arah.Domain.Membership;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Arah.Api.Controllers;

[ApiController]
[Route("api/v1/map")]
[Produces("application/json")]
[Tags("Map")]
public sealed class MapController : ControllerBase
{
    private readonly MapService _mapService;
    private readonly MapPinsService _mapPinsService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ActiveTerritoryService _activeTerritoryService;
    private readonly AccessEvaluator _accessEvaluator;

    public MapController(
        MapService mapService,
        MapPinsService mapPinsService,
        CurrentUserAccessor currentUserAccessor,
        ActiveTerritoryService activeTerritoryService,
        AccessEvaluator accessEvaluator)
    {
        _mapService = mapService;
        _mapPinsService = mapPinsService;
        _currentUserAccessor = currentUserAccessor;
        _activeTerritoryService = activeTerritoryService;
        _accessEvaluator = accessEvaluator;
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
    public async Task<ActionResult<IEnumerable<MapEntityResponse>>> GetEntities(
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var entities = await _mapService.ListEntitiesAsync(
            resolvedTerritoryId.Value,
            userContext.User?.Id,
            cancellationToken);

        var response = entities.Select(entity =>
            new MapEntityResponse(
                entity.Id,
                entity.Name,
                entity.Category,
                entity.Latitude,
                entity.Longitude,
                entity.Status.ToString().ToUpperInvariant(),
                entity.Visibility.ToString().ToUpperInvariant(),
                entity.ConfirmationCount,
                entity.CreatedAtUtc));

        return Ok(response);
    }

    /// <summary>
    /// Lista entidades do mapa do território (paginado).
    /// </summary>
    [HttpGet("entities/paged")]
    [ProducesResponseType(typeof(PagedResponse<MapEntityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<MapEntityResponse>>> GetEntitiesPaged(
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
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _mapService.ListEntitiesPagedAsync(
            resolvedTerritoryId.Value,
            userContext.User?.Id,
            pagination,
            cancellationToken);

        const int maxInt32 = int.MaxValue;
        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var response = new PagedResponse<MapEntityResponse>(
            pagedResult.Items.Select(entity => new MapEntityResponse(
                entity.Id,
                entity.Name,
                entity.Category,
                entity.Latitude,
                entity.Longitude,
                entity.Status.ToString().ToUpperInvariant(),
                entity.Visibility.ToString().ToUpperInvariant(),
                entity.ConfirmationCount,
                entity.CreatedAtUtc)).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Sugere uma nova entidade no mapa.
    /// </summary>
    [HttpPost("entities")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(MapEntityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<MapEntityResponse>> SuggestEntity(
        [FromQuery] Guid? territoryId,
        [FromBody] SuggestMapEntityRequest request,
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

        var result = await _mapService.SuggestAsync(
            resolvedTerritoryId.Value,
            userContext.User.Id,
            request.Name,
            request.Category,
            request.Latitude,
            request.Longitude,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to suggest entity." });
        }

        var response = new MapEntityResponse(
            result.Value.Id,
            result.Value.Name,
            result.Value.Category,
            result.Value.Latitude,
            result.Value.Longitude,
            result.Value.Status.ToString().ToUpperInvariant(),
            result.Value.Visibility.ToString().ToUpperInvariant(),
            result.Value.ConfirmationCount,
            result.Value.CreatedAtUtc);

        return CreatedAtAction(nameof(GetEntities), new { }, response);
    }

    /// <summary>
    /// Lista pins (entidades + posts) para o mapa do território.
    /// </summary>
    [HttpGet("pins")]
    [ProducesResponseType(typeof(IEnumerable<MapPinResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<MapPinResponse>>> GetPins(
        [FromQuery] Guid? territoryId,
        [FromQuery] string? types,
        [FromQuery] Guid? assetId,
        [FromQuery] string? assetTypes,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var filters = MapPinFilters.Parse(types);
        var pins = await _mapPinsService.ListPinsAsync(
            resolvedTerritoryId.Value,
            userContext.User?.Id,
            filters,
            assetId,
            ParseCsv(assetTypes),
            cancellationToken);

        return Ok(pins.Select(ToResponse));
    }

    /// <summary>
    /// Lista pins (entidades + posts) para o mapa do território (paginado).
    /// </summary>
    [HttpGet("pins/paged")]
    [ProducesResponseType(typeof(PagedResponse<MapPinResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<MapPinResponse>>> GetPinsPaged(
        [FromQuery] Guid? territoryId,
        [FromQuery] string? types,
        [FromQuery] Guid? assetId,
        [FromQuery] string? assetTypes,
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
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var filters = MapPinFilters.Parse(types);
        var pagination = new PaginationParameters(pageNumber, pageSize);

        var pagedResult = await _mapPinsService.ListPinsPagedAsync(
            resolvedTerritoryId.Value,
            userContext.User?.Id,
            filters,
            assetId,
            ParseCsv(assetTypes),
            pagination,
            cancellationToken);

        var response = new PagedResponse<MapPinResponse>(
            pagedResult.Items.Select(ToResponse).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    private static MapPinResponse ToResponse(MapPin pin) =>
        new(
            pin.PinType,
            pin.Latitude,
            pin.Longitude,
            pin.Title,
            pin.AssetId,
            pin.PostId,
            pin.MediaId,
            pin.EventId,
            pin.EntityId,
            pin.Status);

    private static IReadOnlyList<string>? ParseCsv(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => value.ToLowerInvariant())
            .ToList();
    }

    /// <summary>
    /// Valida uma entidade sugerida (curadoria).
    /// </summary>
    [HttpPatch("entities/{entityId:guid}/validation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ValidateEntity(
        [FromRoute] Guid entityId,
        [FromQuery] Guid? territoryId,
        [FromBody] ValidateMapEntityRequest request,
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

        if (!Enum.TryParse<MapEntityStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { error = "Invalid status." });
        }

        var success = await _mapService.ValidateAsync(
            resolvedTerritoryId.Value,
            entityId,
            userContext.User.Id,
            status,
            cancellationToken);

        return success ? NoContent() : BadRequest(new { error = "Entity not found." });
    }

    /// <summary>
    /// Confirma uma entidade existente.
    /// </summary>
    [HttpPost("entities/{entityId:guid}/confirmations")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmEntity(
        [FromRoute] Guid entityId,
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

        var result = await _mapService.ConfirmAsync(
            resolvedTerritoryId.Value,
            entityId,
            userContext.User.Id,
            cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Relaciona um morador a uma entidade do território.
    /// </summary>
    [HttpPost("entities/{entityId:guid}/relations")]
    [ProducesResponseType(typeof(MapEntityRelationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MapEntityRelationResponse>> RelateEntity(
        [FromRoute] Guid entityId,
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

        var result = await _mapService.RelateAsync(
            resolvedTerritoryId.Value,
            entityId,
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error is null
                ? Ok(new MapEntityRelationResponse(userContext.User.Id, entityId, DateTime.UtcNow))
                : BadRequest(new { error = result.Error });
        }

        var response = new MapEntityRelationResponse(
            result.Value!.UserId,
            result.Value.EntityId,
            result.Value.CreatedAtUtc);

        return CreatedAtAction(nameof(RelateEntity), new { entityId }, response);
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
