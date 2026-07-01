using Arah.Api.Contracts.Core;
using Arah.Core.Application;
using Arah.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arah.Api.Controllers.Platform;

[ApiController]
[Route("api/v1/federation")]
[Authorize(Policy = "SystemAdmin")]
[Produces("application/json")]
[Tags("Federation")]
public sealed class FederationController : ControllerBase
{
    private readonly FederationIdentityService _identities;

    public FederationController(FederationIdentityService identities)
    {
        _identities = identities;
    }

    [HttpPost("identity")]
    [ProducesResponseType(typeof(FederatedIdentityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<FederatedIdentityResponse> ProvisionIdentity([FromBody] ProvisionFederatedIdentityRequest request)
    {
        if (request.HomeInstanceId == Guid.Empty)
        {
            return BadRequest("homeInstanceId is required");
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return BadRequest("displayName is required");
        }

        try
        {
            var user = _identities.Provision(request.HomeInstanceId, request.DisplayName);
            var response = ToResponse(user);
            return CreatedAtAction(nameof(ResolveIdentity), new { globalUserId = user.GlobalUserId }, response);
        }
        catch (InvalidOperationException)
        {
            return NotFound("home instance is not registered");
        }
    }

    [HttpGet("identity/{globalUserId:guid}")]
    [ProducesResponseType(typeof(FederatedIdentityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<FederatedIdentityResponse> ResolveIdentity(Guid globalUserId)
    {
        var user = _identities.Resolve(globalUserId);
        return user is null ? NotFound() : Ok(ToResponse(user));
    }

    private static FederatedIdentityResponse ToResponse(FederatedGlobalUser user) =>
        new(user.GlobalUserId, user.HomeInstanceId, user.DisplayName, user.CreatedAtUtc);
}
