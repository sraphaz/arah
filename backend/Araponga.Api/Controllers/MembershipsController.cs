using Araponga.Api.Contracts.Memberships;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/memberships")]
[Produces("application/json")]
[Tags("Memberships")]
public sealed class MembershipsController : ControllerBase
{
    private readonly MembershipService _membershipService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly TerritoryService _territoryService;

    public MembershipsController(
        MembershipService membershipService,
        CurrentUserAccessor currentUserAccessor,
        TerritoryService territoryService)
    {
        _membershipService = membershipService;
        _currentUserAccessor = currentUserAccessor;
        _territoryService = territoryService;
    }

    /// <summary>
    /// Declara vínculo como morador do território.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MembershipResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembershipResponse>> DeclareMembership(
        [FromRoute] Guid territoryId,
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

        var membership = await _membershipService.DeclareResidentAsync(
            userContext.User.Id,
            territoryId,
            cancellationToken);

        var response = new MembershipResponse(
            membership.Id,
            membership.UserId,
            membership.TerritoryId,
            membership.Status.ToString().ToUpperInvariant(),
            membership.CreatedAtUtc);

        return CreatedAtAction(nameof(DeclareMembership), new { territoryId }, response);
    }
}
