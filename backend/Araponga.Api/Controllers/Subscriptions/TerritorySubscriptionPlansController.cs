using Araponga.Api.Contracts.Admin;
using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Subscriptions;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Subscriptions;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId}/subscription-plans")]
[Produces("application/json")]
[Tags("Territories - Subscriptions")]
[Authorize]
public sealed class TerritorySubscriptionPlansController : ControllerBase
{
    private readonly SubscriptionPlanAdminService _adminService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ITerritoryMembershipRepository _membershipRepository;

    public TerritorySubscriptionPlansController(
        SubscriptionPlanAdminService adminService,
        CurrentUserAccessor currentUserAccessor,
        ITerritoryMembershipRepository membershipRepository)
    {
        _adminService = adminService;
        _currentUserAccessor = currentUserAccessor;
        _membershipRepository = membershipRepository;
    }

    /// <summary>
    /// Lista planos do território (territoriais + globais).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionPlanResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanResponse>>> List(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var plans = await _adminService.GetPlansForTerritoryAsync(territoryId, cancellationToken);
        var response = plans.Select(ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Obtém plano do território por ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionPlanResponse>> Get(
        Guid territoryId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var plan = await _adminService.GetPlanByIdAsync(id, cancellationToken);
        if (plan == null)
        {
            return NotFound();
        }

        // Verificar se plano pertence ao território ou é global
        if (plan.Scope == PlanScope.Territory && plan.TerritoryId != territoryId)
        {
            return NotFound();
        }

        return Ok(ToResponse(plan));
    }

    /// <summary>
    /// Cria plano territorial (apenas curadores do território).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SubscriptionPlanResponse>> Create(
        Guid territoryId,
        [FromBody] CreatePlanRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se usuário é curador do território
        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        if (membership == null || membership.Role != MembershipRole.Resident)
        {
            return Forbid("Only territory curators can create territory plans.");
        }

        var result = await _adminService.CreateTerritoryPlanAsync(
            territoryId,
            userContext.User.Id,
            request.Name,
            request.Description,
            Enum.Parse<SubscriptionPlanTier>(request.Tier),
            request.PricePerCycle,
            !string.IsNullOrWhiteSpace(request.BillingCycle) ? Enum.Parse<SubscriptionBillingCycle>(request.BillingCycle) : null,
            request.Capabilities.Select(c => Enum.Parse<FeatureCapability>(c)).ToList(),
            request.Limits,
            request.TrialDays,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        if (result.Value is null)
        {
            return BadRequest(new ErrorResponse { Message = "Unexpected null result." });
        }

        var created = result.Value!;
        return CreatedAtAction(
            nameof(Get),
            new { territoryId, id = created.Id },
            ToResponse(created));
    }

    /// <summary>
    /// Atualiza plano territorial.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SubscriptionPlanResponse>> Update(
        Guid territoryId,
        Guid id,
        [FromBody] UpdatePlanRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se plano pertence ao território
        var plan = await _adminService.GetPlanByIdAsync(id, cancellationToken);
        if (plan == null)
        {
            return NotFound();
        }

        if (plan.Scope == PlanScope.Territory && plan.TerritoryId != territoryId)
        {
            return NotFound();
        }

        // Verificar se usuário é curador (para planos territoriais) ou SystemAdmin
        if (plan.Scope == PlanScope.Territory)
        {
            var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
                userContext.User.Id,
                territoryId,
                cancellationToken);

            if (membership == null || membership.Role != MembershipRole.Resident)
            {
                return Forbid("Only territory curators can update territory plans.");
            }
        }

        var result = await _adminService.UpdatePlanAsync(
            id,
            userContext.User.Id,
            request.Name,
            request.Description,
            request.PricePerCycle,
            !string.IsNullOrWhiteSpace(request.BillingCycle) ? Enum.Parse<SubscriptionBillingCycle>(request.BillingCycle) : null,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        if (result.Value is null)
        {
            return BadRequest(new ErrorResponse { Message = "Unexpected null result." });
        }

        var updated = result.Value!;
        return Ok(ToResponse(updated));
    }

    /// <summary>
    /// Atualiza capacidades do plano territorial.
    /// </summary>
    [HttpPatch("{id}/capabilities")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SubscriptionPlanResponse>> UpdateCapabilities(
        Guid territoryId,
        Guid id,
        [FromBody] UpdateCapabilitiesRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se plano pertence ao território
        var plan = await _adminService.GetPlanByIdAsync(id, cancellationToken);
        if (plan == null)
        {
            return NotFound();
        }

        if (plan.Scope == PlanScope.Territory && plan.TerritoryId != territoryId)
        {
            return NotFound();
        }

        // Verificar se usuário é curador (para planos territoriais)
        if (plan.Scope == PlanScope.Territory)
        {
            var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
                userContext.User.Id,
                territoryId,
                cancellationToken);

            if (membership == null || membership.Role != MembershipRole.Resident)
            {
                return Forbid("Only territory curators can update territory plans.");
            }
        }

        var result = await _adminService.UpdatePlanCapabilitiesAsync(
            id,
            userContext.User.Id,
            request.Capabilities.Select(c => Enum.Parse<FeatureCapability>(c)).ToList(),
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        if (result.Value is null)
        {
            return BadRequest(new ErrorResponse { Message = "Unexpected null result." });
        }

        var updated = result.Value!;
        return Ok(ToResponse(updated));
    }

    /// <summary>
    /// Ativa plano territorial.
    /// </summary>
    [HttpPatch("{id}/activate")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SubscriptionPlanResponse>> Activate(
        Guid territoryId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se plano pertence ao território
        var plan = await _adminService.GetPlanByIdAsync(id, cancellationToken);
        if (plan == null)
        {
            return NotFound();
        }

        if (plan.Scope == PlanScope.Territory && plan.TerritoryId != territoryId)
        {
            return NotFound();
        }

        // Verificar se usuário é curador (para planos territoriais)
        if (plan.Scope == PlanScope.Territory)
        {
            var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
                userContext.User.Id,
                territoryId,
                cancellationToken);

            if (membership == null || membership.Role != MembershipRole.Resident)
            {
                return Forbid("Only territory curators can activate territory plans.");
            }
        }

        var result = await _adminService.ActivatePlanAsync(id, userContext.User.Id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        if (result.Value is null)
        {
            return BadRequest(new ErrorResponse { Message = "Unexpected null result." });
        }

        var updated = result.Value!;
        return Ok(ToResponse(updated));
    }

    /// <summary>
    /// Desativa plano territorial.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SubscriptionPlanResponse>> Deactivate(
        Guid territoryId,
        Guid id,
        [FromBody] DeactivatePlanRequest? request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Verificar se plano pertence ao território
        var plan = await _adminService.GetPlanByIdAsync(id, cancellationToken);
        if (plan == null)
        {
            return NotFound();
        }

        if (plan.Scope == PlanScope.Territory && plan.TerritoryId != territoryId)
        {
            return NotFound();
        }

        // Verificar se usuário é curador (para planos territoriais)
        if (plan.Scope == PlanScope.Territory)
        {
            var membership = await _membershipRepository.GetByUserAndTerritoryAsync(
                userContext.User.Id,
                territoryId,
                cancellationToken);

            if (membership == null || membership.Role != MembershipRole.Resident)
            {
                return Forbid("Only territory curators can deactivate territory plans.");
            }
        }

        var result = await _adminService.DeactivatePlanAsync(
            id,
            userContext.User.Id,
            request?.Reason,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        if (result.Value is null)
        {
            return BadRequest(new ErrorResponse { Message = "Unexpected null result." });
        }

        var updated = result.Value!;
        return Ok(ToResponse(updated));
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
