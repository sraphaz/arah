using Araponga.Api;
using Araponga.Api.Configuration;
using Araponga.Api.Contracts.Memberships;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Social;
using Araponga.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/membership")]
[Produces("application/json")]
[Tags("Memberships")]
public sealed class MembershipsController : ControllerBase
{
    private readonly MembershipService _membershipService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly TerritoryService _territoryService;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly PresencePolicyOptions _presencePolicy;

    public MembershipsController(
        MembershipService membershipService,
        CurrentUserAccessor currentUserAccessor,
        TerritoryService territoryService,
        AccessEvaluator accessEvaluator,
        IOptions<PresencePolicyOptions> presencePolicy)
    {
        _membershipService = membershipService;
        _currentUserAccessor = currentUserAccessor;
        _territoryService = territoryService;
        _accessEvaluator = accessEvaluator;
        _presencePolicy = presencePolicy.Value;
    }

    /// <summary>
    /// Declara vínculo social com um território (visitor/resident).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MembershipResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembershipResponse>> DeclareMembership(
        [FromRoute] Guid territoryId,
        [FromBody] DeclareMembershipRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var territory = await _territoryService.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null)
        {
            return NotFound();
        }

        if (!Enum.TryParse<MembershipRole>(request.Role, true, out var role))
        {
            return BadRequest(new { error = "Invalid role." });
        }

        if (RequiresPresence(role) &&
            !GeoHeaderReader.TryGetCoordinates(Request.Headers, out _, out _))
        {
            return BadRequest(new { error = "X-Geo-Latitude and X-Geo-Longitude headers are required to declare membership." });
        }

        var membership = await _membershipService.DeclareMembershipAsync(
            userContext.User.Id,
            territoryId,
            role,
            cancellationToken);

        var response = new MembershipResponse(
            membership.Id,
            membership.UserId,
            membership.TerritoryId,
            membership.Role.ToString().ToUpperInvariant(),
            membership.VerificationStatus.ToString().ToUpperInvariant(),
            membership.CreatedAtUtc);

        return CreatedAtAction(nameof(DeclareMembership), new { territoryId }, response);
    }

    /// <summary>
    /// Retorna o status do vínculo do usuário com o território.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(MembershipStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MembershipStatusResponse>> GetStatus(
        [FromRoute] Guid territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var status = await _membershipService.GetStatusAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        var role = await _accessEvaluator.GetRoleAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        var response = new MembershipStatusResponse(
            userContext.User.Id,
            territoryId,
            role?.ToString().ToUpperInvariant() ?? "NONE",
            status?.ToString().ToUpperInvariant() ?? "NONE");

        return Ok(response);
    }

    /// <summary>
    /// Valida um vínculo de morador (curadoria).
    /// </summary>
    [HttpPatch("{membershipId:guid}/validation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Validate(
        [FromRoute] Guid territoryId,
        [FromRoute] Guid membershipId,
        [FromBody] ValidateMembershipRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var canValidate = await _accessEvaluator.IsResidentAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        if (!canValidate)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (!Enum.TryParse<VerificationStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { error = "Invalid status." });
        }

        var success = await _membershipService.ValidateAsync(
            membershipId,
            userContext.User.Id,
            territoryId,
            status,
            cancellationToken);
        return success ? NoContent() : BadRequest(new { error = "Membership not found." });
    }

    private bool RequiresPresence(MembershipRole role)
    {
        return _presencePolicy.Policy switch
        {
            PresencePolicy.None => false,
            PresencePolicy.ResidentOnly => role == MembershipRole.Resident,
            PresencePolicy.VisitorAndResident => role == MembershipRole.Resident || role == MembershipRole.Visitor,
            _ => role == MembershipRole.Resident
        };
    }
}
