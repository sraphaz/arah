using Araponga.Api.Contracts.Territories;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories")]
[Produces("application/json")]
[Tags("Territories")]
public sealed class TerritoriesController : ControllerBase
{
    // MVP: armazenamento em memória apenas para deixar a API "apresentável" hoje.
    // Na próxima sessão nós plugaríamos EF Core + Postgres e isso vira persistente.
    private static readonly List<TerritoryResponse> Territories = new()
    {
        new TerritoryResponse(
            Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name: "Sertão do Camburi",
            Description: "Território piloto para comunidade local, entidades do mapa e saúde do território.",
            SensitivityLevel: "HIGH",
            IsPilot: true,
            CreatedAtUtc: DateTime.UtcNow
        )
    };

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
    public ActionResult<IEnumerable<TerritoryResponse>> List()
    {
        return Ok(Territories.OrderBy(t => t.Name));
    }

    /// <summary>
    /// Obtém um território por Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TerritoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TerritoryResponse> GetById([FromRoute] Guid id)
    {
        var t = Territories.FirstOrDefault(x => x.Id == id);
        return t is null ? NotFound() : Ok(t);
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
    public ActionResult<TerritoryResponse> Create([FromBody] CreateTerritoryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { error = "Name is required." });

        var sl = (request.SensitivityLevel ?? "LOW").Trim().ToUpperInvariant();
        if (sl is not ("LOW" or "MEDIUM" or "HIGH"))
            return BadRequest(new { error = "SensitivityLevel must be LOW, MEDIUM, or HIGH." });

        var created = new TerritoryResponse(
            Id: Guid.NewGuid(),
            Name: request.Name.Trim(),
            Description: request.Description?.Trim(),
            SensitivityLevel: sl,
            IsPilot: request.IsPilot,
            CreatedAtUtc: DateTime.UtcNow
        );

        Territories.Add(created);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
