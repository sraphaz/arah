using Arah.Api.Contracts.Transactions;
using Arah.Application.Interfaces;
using Arah.Domain.Subscriptions;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}")]
[Produces("application/json")]
[Tags("Commercial Plans — FASE55")]
public sealed class TerritoryCommercialPlansController : ControllerBase
{
    private readonly ISubscriptionPlanRepository _plans;

    public TerritoryCommercialPlansController(ISubscriptionPlanRepository plans)
    {
        _plans = plans;
    }

    /// <summary>
    /// Planos comerciais do território (alias FASE55 — GET /territories/{id}/plans).
    /// </summary>
    [HttpGet("plans")]
    [ProducesResponseType(typeof(IEnumerable<CommercialPlanResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CommercialPlanResponse>>> ListCommercialPlans(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var territoryPlans = await _plans.GetPlansForTerritoryAsync(territoryId, cancellationToken);
        var commercial = territoryPlans
            .Where(p => p.IsActive && p.Capabilities.Contains(FeatureCapability.MarketplaceAdvanced))
            .Select(p => new CommercialPlanResponse(
                p.Id,
                p.Name,
                p.Description,
                p.PricePerCycle ?? 0m,
                p.BillingCycle?.ToString(),
                p.Capabilities.Select(c => c.ToString()).ToList()));

        return Ok(commercial);
    }
}
