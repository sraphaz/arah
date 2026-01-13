using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Territories;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Territories;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories")]
[Produces("application/json")]
[Tags("Territories")]
public sealed class TerritoriesController : ControllerBase
{
    private readonly TerritoryService _territoryService;
    private readonly ActiveTerritoryService _activeTerritoryService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    public TerritoriesController(
        TerritoryService territoryService,
        ActiveTerritoryService activeTerritoryService,
        CurrentUserAccessor currentUserAccessor)
    {
        _territoryService = territoryService;
        _activeTerritoryService = activeTerritoryService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Lista territórios disponíveis para descoberta (MVP).
    /// </summary>
    /// <remarks>
    /// No MVP inicial, a listagem pode ser pública. Futuramente:
    /// - filtragem por proximidade geográfica
    /// - visibilidade por perfil (visitante/morador)
    /// - feature flags por território
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TerritoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TerritoryResponse>>> List(CancellationToken cancellationToken)
    {
        var territories = await _territoryService.ListAvailableAsync(cancellationToken);
        var response = territories.Select(ToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Lista territórios disponíveis (paginado).
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<TerritoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TerritoryResponse>>> ListPaged(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _territoryService.ListAvailablePagedAsync(pagination, cancellationToken);
        var response = new PagedResponse<TerritoryResponse>(
            pagedResult.Items.Select(ToResponse).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);
        return Ok(response);
    }

    /// <summary>
    /// Obtém um território por Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TerritoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TerritoryResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var territory = await _territoryService.GetByIdAsync(id, cancellationToken);
        return territory is null ? NotFound() : Ok(ToResponse(territory));
    }

    /// <summary>
    /// Sugere um território geográfico para validação.
    /// </summary>
    [HttpPost("suggestions")]
    [ProducesResponseType(typeof(TerritoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TerritoryResponse>> Suggest(
        [FromBody] SuggestTerritoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _territoryService.CreateAsync(
            request.Name,
            request.Description,
            request.City,
            request.State,
            request.Latitude,
            request.Longitude,
            cancellationToken);

        if (!result.Success || result.Territory is null)
        {
            return BadRequest(new { error = result.Error ?? "Invalid territory data." });
        }

        var response = ToResponse(result.Territory);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Busca territórios por texto e localização administrativa.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<TerritoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TerritoryResponse>>> Search(
        [FromQuery] string? q,
        [FromQuery] string? city,
        [FromQuery] string? state,
        CancellationToken cancellationToken)
    {
        var territories = await _territoryService.SearchAsync(q, city, state, cancellationToken);
        var response = territories.Select(ToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Busca territórios por texto e localização administrativa (paginado).
    /// </summary>
    [HttpGet("search/paged")]
    [ProducesResponseType(typeof(PagedResponse<TerritoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TerritoryResponse>>> SearchPaged(
        [FromQuery] string? q,
        [FromQuery] string? city,
        [FromQuery] string? state,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _territoryService.SearchPagedAsync(q, city, state, pagination, cancellationToken);
        var response = new PagedResponse<TerritoryResponse>(
            pagedResult.Items.Select(ToResponse).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);
        return Ok(response);
    }

    /// <summary>
    /// Busca territórios próximos usando latitude e longitude.
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(IEnumerable<TerritoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<TerritoryResponse>>> Nearby(
        [FromQuery] double? lat,
        [FromQuery] double? lng,
        [FromQuery] double? radiusKm,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        if (lat is null || lng is null)
        {
            return BadRequest(new { error = "lat and lng are required." });
        }

        const double defaultRadiusKm = 25;
        const int defaultLimit = 20;

        var resolvedRadius = radiusKm ?? defaultRadiusKm;
        var resolvedLimit = limit ?? defaultLimit;

        if (resolvedRadius <= 0 || resolvedLimit <= 0)
        {
            return BadRequest(new { error = "radiusKm and limit must be positive." });
        }

        var territories = await _territoryService.NearbyAsync(
            lat.Value,
            lng.Value,
            resolvedRadius,
            resolvedLimit,
            cancellationToken);
        var response = territories.Select(ToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Busca territórios próximos usando latitude e longitude (paginado).
    /// </summary>
    [HttpGet("nearby/paged")]
    [ProducesResponseType(typeof(PagedResponse<TerritoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<TerritoryResponse>>> NearbyPaged(
        [FromQuery] double? lat,
        [FromQuery] double? lng,
        [FromQuery] double? radiusKm,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (lat is null || lng is null)
        {
            return BadRequest(new { error = "lat and lng are required." });
        }

        const double defaultRadiusKm = 25;
        var resolvedRadius = radiusKm ?? defaultRadiusKm;

        if (resolvedRadius <= 0)
        {
            return BadRequest(new { error = "radiusKm must be positive." });
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _territoryService.NearbyPagedAsync(
            lat.Value,
            lng.Value,
            resolvedRadius,
            pagination,
            cancellationToken);

        var response = new PagedResponse<TerritoryResponse>(
            pagedResult.Items.Select(ToResponse).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Seleciona o território ativo da sessão atual.
    /// </summary>
    /// <remarks>
    /// Use o header X-Session-Id para identificar o cliente atual.
    /// </remarks>
    [HttpPost("selection")]
    [ProducesResponseType(typeof(TerritorySelectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TerritorySelectionResponse>> SelectTerritory(
        [FromBody] TerritorySelectionRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetSessionId(out var sessionId))
        {
            return BadRequest(new { error = "X-Session-Id header is required." });
        }

        var stored = await _activeTerritoryService.SetActiveAsync(sessionId, request.TerritoryId, cancellationToken);
        if (!stored)
        {
            return BadRequest(new { error = "Territory not found." });
        }

        return Ok(new TerritorySelectionResponse(sessionId, request.TerritoryId));
    }

    /// <summary>
    /// Obtém o território ativo da sessão atual.
    /// </summary>
    [HttpGet("selection")]
    [ProducesResponseType(typeof(TerritorySelectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TerritorySelectionResponse>> GetSelection(CancellationToken cancellationToken)
    {
        if (!TryGetSessionId(out var sessionId))
        {
            return BadRequest(new { error = "X-Session-Id header is required." });
        }

        var territoryId = await _activeTerritoryService.GetActiveAsync(sessionId, cancellationToken);
        if (territoryId is null)
        {
            return NotFound();
        }

        return Ok(new TerritorySelectionResponse(sessionId, territoryId.Value));
    }

    private static TerritoryResponse ToResponse(Territory territory)
    {
        return new TerritoryResponse(
            territory.Id,
            territory.Name,
            territory.Description,
            territory.Status.ToString().ToUpperInvariant(),
            territory.City,
            territory.State,
            territory.Latitude,
            territory.Longitude,
            territory.CreatedAtUtc);
    }

    private bool TryGetSessionId(out string sessionId)
    {
        if (Request.Headers.TryGetValue(ApiHeaders.SessionId, out var header) &&
            !string.IsNullOrWhiteSpace(header))
        {
            sessionId = header.ToString();
            return true;
        }

        sessionId = string.Empty;
        return false;
    }
}
