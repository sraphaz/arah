using Araponga.Api.Contracts.Admin;
using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Subscriptions;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/admin/subscription-plans")]
[Produces("application/json")]
[Tags("Admin - Subscriptions")]
[Authorize(Policy = "SystemAdmin")]
public sealed class AdminSubscriptionPlansController : ControllerBase
{
    private readonly SubscriptionPlanAdminService _adminService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public AdminSubscriptionPlansController(
        SubscriptionPlanAdminService adminService,
        CurrentUserAccessor currentUserAccessor)
    {
        _adminService = adminService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Lista todos os planos (globais e territoriais).
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
            plans = await _adminService.GetPlansForTerritoryAsync(territoryId.Value, cancellationToken);
        }
        else
        {
            // Lista apenas planos globais (para listar todos, usar endpoint específico)
            plans = await _adminService.GetGlobalPlansAsync(cancellationToken);
        }

        var response = plans.Select(ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Lista apenas planos globais.
    /// </summary>
    [HttpGet("global")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionPlanResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanResponse>>> ListGlobal(
        CancellationToken cancellationToken)
    {
        var plans = await _adminService.GetGlobalPlansAsync(cancellationToken);
        var response = plans.Select(ToResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Lista planos de um território.
    /// </summary>
    [HttpGet("territory/{territoryId}")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionPlanResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanResponse>>> ListTerritory(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var plans = await _adminService.GetPlansForTerritoryAsync(territoryId, cancellationToken);
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
        var plan = await _adminService.GetPlanByIdAsync(id, cancellationToken);
        if (plan == null)
        {
            return NotFound();
        }

        return Ok(ToResponse(plan));
    }

    /// <summary>
    /// Cria plano global.
    /// </summary>
    [HttpPost("global")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionPlanResponse>> CreateGlobal(
        [FromBody] CreatePlanRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _adminService.CreateGlobalPlanAsync(
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

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Value!.Id },
            ToResponse(result.Value));
    }

    /// <summary>
    /// Cria plano territorial (SystemAdmin pode criar para qualquer território).
    /// </summary>
    [HttpPost("territory/{territoryId}")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionPlanResponse>> CreateTerritory(
        Guid territoryId,
        [FromBody] CreatePlanRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
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

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Value!.Id },
            ToResponse(result.Value));
    }

    /// <summary>
    /// Atualiza plano.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionPlanResponse>> Update(
        Guid id,
        [FromBody] UpdatePlanRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
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

        return Ok(ToResponse(result.Value!));
    }

    /// <summary>
    /// Atualiza capacidades do plano.
    /// </summary>
    [HttpPatch("{id}/capabilities")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionPlanResponse>> UpdateCapabilities(
        Guid id,
        [FromBody] UpdateCapabilitiesRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
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

        return Ok(ToResponse(result.Value!));
    }

    /// <summary>
    /// Ativa plano.
    /// </summary>
    [HttpPatch("{id}/activate")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionPlanResponse>> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _adminService.ActivatePlanAsync(id, userContext.User.Id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new ErrorResponse { Message = result.Error });
        }

        return Ok(ToResponse(result.Value!));
    }

    /// <summary>
    /// Desativa plano.
    /// </summary>
    [HttpPatch("{id}/deactivate")]
    [ProducesResponseType(typeof(SubscriptionPlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubscriptionPlanResponse>> Deactivate(
        Guid id,
        [FromBody] DeactivatePlanRequest? request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
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

        return Ok(ToResponse(result.Value!));
    }

    /// <summary>
    /// Obtém histórico de mudanças do plano.
    /// </summary>
    [HttpGet("{id}/history")]
    [ProducesResponseType(typeof(IEnumerable<PlanHistoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PlanHistoryResponse>>> GetHistory(
        Guid id,
        CancellationToken cancellationToken)
    {
        var history = await _adminService.GetPlanHistoryAsync(id, cancellationToken);
        var response = history.Select(ToHistoryResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    /// Lista funcionalidades disponíveis.
    /// </summary>
    [HttpGet("capabilities")]
    [ProducesResponseType(typeof(IEnumerable<CapabilityInfoResponse>), StatusCodes.Status200OK)]
    public Task<ActionResult<IEnumerable<CapabilityInfoResponse>>> GetCapabilities(
        CancellationToken cancellationToken)
    {
        var capabilities = Enum.GetValues<FeatureCapability>();
        var response = capabilities.Select(c => new CapabilityInfoResponse
        {
            Capability = c.ToString(),
            Category = GetCategory(c).ToString(),
            Description = GetDescription(c)
        }).ToList();

        return Task.FromResult<ActionResult<IEnumerable<CapabilityInfoResponse>>>(Ok(response));
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

    private static PlanHistoryResponse ToHistoryResponse(SubscriptionPlanHistory history)
    {
        return new PlanHistoryResponse
        {
            Id = history.Id,
            PlanId = history.PlanId,
            ChangedByUserId = history.ChangedByUserId,
            ChangeType = history.ChangeType.ToString(),
            PreviousState = history.PreviousState,
            NewState = history.NewState,
            ChangeReason = history.ChangeReason,
            ChangedAtUtc = history.ChangedAtUtc
        };
    }

    private static FeatureCategory GetCategory(FeatureCapability capability)
    {
        return capability switch
        {
            FeatureCapability.FeedBasic or FeatureCapability.PostsBasic or FeatureCapability.EventsBasic or
            FeatureCapability.MarketplaceBasic or FeatureCapability.ChatBasic => FeatureCategory.Core,
            FeatureCapability.PostsUnlimited or FeatureCapability.EventsUnlimited or
            FeatureCapability.MarketplaceAdvanced => FeatureCategory.Enhanced,
            FeatureCapability.Analytics or FeatureCapability.AIIntegration or FeatureCapability.PrioritySupport =>
                FeatureCategory.Premium,
            _ => FeatureCategory.Enterprise
        };
    }

    private static string GetDescription(FeatureCapability capability)
    {
        return capability switch
        {
            FeatureCapability.FeedBasic => "Feed comunitário básico",
            FeatureCapability.PostsBasic => "Posts básicos (limitado)",
            FeatureCapability.PostsUnlimited => "Posts ilimitados",
            FeatureCapability.EventsBasic => "Eventos básicos (limitado)",
            FeatureCapability.EventsUnlimited => "Eventos ilimitados",
            FeatureCapability.MarketplaceBasic => "Marketplace básico",
            FeatureCapability.MarketplaceAdvanced => "Marketplace avançado",
            FeatureCapability.ChatBasic => "Chat territorial básico",
            FeatureCapability.Analytics => "Analytics e métricas",
            FeatureCapability.AIIntegration => "Integração com IA",
            FeatureCapability.PrioritySupport => "Suporte prioritário",
            FeatureCapability.CustomBranding => "Branding customizado",
            FeatureCapability.APIAccess => "Acesso à API",
            FeatureCapability.AdvancedGovernance => "Governança avançada",
            FeatureCapability.TerritoryPremium => "Recursos premium territoriais",
            _ => capability.ToString()
        };
    }
}
