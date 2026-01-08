using Araponga.Api.Contracts.Territories;
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

    public TerritoriesController(
        TerritoryService territoryService,
        ActiveTerritoryService activeTerritoryService)
    {
        _territoryService = territoryService;
        _activeTerritoryService = activeTerritoryService;
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
    /// Obtém um território por Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TerritoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TerritoryResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var territory = await _territoryService.GetByIdAsync(id, cancellationToken);
        return territory is null ? NotFound() : Ok(ToResponse(territory));
    }

    /// <summary>
    /// Cria um território (MVP).
    /// </summary>
    /// <remarks>
    /// Regras mínimas hoje:
    /// - Name obrigatório
    /// - SensitivityLevel: LOW|MEDIUM|HIGH (aqui validamos por convenção)
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(TerritoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TerritoryResponse>> Create(
        [FromBody] CreateTerritoryRequest request,
        CancellationToken cancellationToken)
    {
        var sensitivityLevel = ParseSensitivity(request.SensitivityLevel);
        if (sensitivityLevel is null)
        {
            return BadRequest(new { error = "SensitivityLevel must be LOW, MEDIUM, or HIGH." });
        }

        var result = await _territoryService.CreateAsync(
            request.Name,
            request.Description,
            sensitivityLevel.Value,
            request.IsPilot,
            cancellationToken);

        if (!result.Success || result.Territory is null)
        {
            return BadRequest(new { error = result.Error ?? "Invalid territory data." });
        }

        var response = ToResponse(result.Territory);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
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
            territory.Sensitivity.ToString().ToUpperInvariant(),
            territory.Status.ToString().ToUpperInvariant(),
            territory.CreatedAtUtc);
    }

    private static SensitivityLevel? ParseSensitivity(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        return Enum.TryParse<SensitivityLevel>(input, true, out var parsed) ? parsed : null;
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
