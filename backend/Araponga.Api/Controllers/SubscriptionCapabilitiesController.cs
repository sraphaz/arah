using Araponga.Api.Contracts.Subscriptions;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/subscriptions/me")]
[Produces("application/json")]
[Tags("Subscriptions")]
[Authorize]
public sealed class SubscriptionCapabilitiesController : ControllerBase
{
    private readonly SubscriptionCapabilityService _capabilityService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public SubscriptionCapabilitiesController(
        SubscriptionCapabilityService capabilityService,
        CurrentUserAccessor currentUserAccessor)
    {
        _capabilityService = capabilityService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Obtém minhas capacidades.
    /// </summary>
    [HttpGet("capabilities")]
    [ProducesResponseType(typeof(CapabilitiesResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CapabilitiesResponse>> GetMyCapabilities(
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var capabilities = await _capabilityService.GetUserCapabilitiesAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        return Ok(new CapabilitiesResponse
        {
            Capabilities = capabilities.Select(c => c.ToString()).ToList()
        });
    }

    /// <summary>
    /// Obtém meus limites.
    /// </summary>
    [HttpGet("limits")]
    [ProducesResponseType(typeof(LimitsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LimitsResponse>> GetMyLimits(
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var limits = await _capabilityService.GetUserLimitsAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        return Ok(new LimitsResponse
        {
            Limits = limits
        });
    }

    /// <summary>
    /// Verifica se tenho uma capacidade específica.
    /// </summary>
    [HttpPost("check-capability")]
    [ProducesResponseType(typeof(CheckCapabilityResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CheckCapabilityResponse>> CheckCapability(
        [FromBody] CheckCapabilityRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<FeatureCapability>(request.Capability, out var capability))
        {
            return BadRequest(new { Message = "Invalid capability." });
        }

        var hasCapability = await _capabilityService.CheckCapabilityAsync(
            userContext.User.Id,
            request.TerritoryId,
            capability,
            cancellationToken);

        return Ok(new CheckCapabilityResponse
        {
            HasCapability = hasCapability
        });
    }
}
