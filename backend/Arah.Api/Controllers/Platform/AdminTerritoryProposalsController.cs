using Arah.Api.Contracts.Journeys.Onboarding;
using Arah.Application.Interfaces.Geo;
using Arah.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers;

/// <summary>
/// Revisão administrativa de propostas de território (status Pending).
/// </summary>
[ApiController]
[Route("api/v1/admin/territories")]
[Produces("application/json")]
[Tags("Admin - Territories")]
[Authorize(Policy = "SystemAdmin")]
public sealed class AdminTerritoryProposalsController : ControllerBase
{
    private readonly ITerritoryProposalService _proposalService;
    private readonly TerritoryService _territoryService;

    public AdminTerritoryProposalsController(
        ITerritoryProposalService proposalService,
        TerritoryService territoryService)
    {
        _proposalService = proposalService;
        _territoryService = territoryService;
    }

    /// <summary>
    /// Lista territórios aguardando validação (Pending).
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IEnumerable<PendingTerritoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PendingTerritoryResponse>>> ListPending(CancellationToken cancellationToken)
    {
        var pending = await _proposalService.ListPendingAsync(cancellationToken);
        var response = pending.Select(t => new PendingTerritoryResponse(
            t.Id,
            t.Name,
            t.City,
            t.State,
            t.Description,
            t.Latitude,
            t.Longitude,
            t.CreatedAtUtc,
            t.ParentTerritoryId));
        return Ok(response);
    }

    /// <summary>
    /// Aprova ou rejeita proposta de território.
    /// </summary>
    [HttpPost("{territoryId:guid}/review")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Review(
        Guid territoryId,
        [FromBody] ReviewTerritoryProposalRequest request,
        CancellationToken cancellationToken)
    {
        var reviewed = await _proposalService.ReviewAsync(territoryId, request.Approve, cancellationToken);
        if (!reviewed)
        {
            return NotFound(new { error = "Territory proposal not found or not pending." });
        }

        var territory = await _territoryService.GetByIdAsync(territoryId, cancellationToken);
        return Ok(new
        {
            territoryId,
            status = territory?.Status.ToString() ?? (request.Approve ? "Active" : "Inactive"),
            approved = request.Approve
        });
    }
}
