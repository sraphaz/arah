using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Subscriptions;
using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/subscription-plans")]
[Produces("application/json")]
[Tags("Subscriptions")]
public sealed class SubscriptionPlansController : ControllerBase
{
    private readonly SubscriptionService _subscriptionService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public SubscriptionPlansController(
        SubscriptionService subscriptionService,
        CurrentUserAccessor currentUserAccessor)
    {
        _subscriptionService = subscriptionService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Lista planos disponíveis para um território.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionPlanResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanResponse>>> List(
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<SubscriptionPlan> plans;
        
        if (territoryId.HasValue)
        {
            plans = await _subscriptionService.GetAvailablePlansForTerritoryAsync(territoryId.Value, cancellationToken);
        }
        else
        {
            // Se não especificar território, retorna apenas planos globais
            plans = await _subscriptionService.GetGlobalPlansAsync(cancellationToken);
        }

        var response = plans.Select(ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Obtém plano por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionPlanResponse>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var plan = await _subscriptionService.GetPlanByIdAsync(id, cancellationToken);
        if (plan == null)
        {
            return NotFound();
        }

        return Ok(ToResponse(plan));
    }

    private static SubscriptionPlanResponse ToResponse(SubscriptionPlan plan)
    {
        return new SubscriptionPlanResponse
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Tier = plan.Tier.ToString(),
            Scope = plan.Scope.ToString(),
            TerritoryId = plan.TerritoryId,
            PricePerCycle = plan.PricePerCycle,
            BillingCycle = plan.BillingCycle?.ToString(),
            Capabilities = plan.Capabilities.Select(c => c.ToString()).ToList(),
            Limits = plan.Limits ?? new Dictionary<string, object>(),
            IsDefault = plan.IsDefault,
            TrialDays = plan.TrialDays,
            IsActive = plan.IsActive
        };
    }
}
