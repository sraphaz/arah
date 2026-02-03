using Araponga.Api.Contracts.Admin;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Work;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/moderation/cases")]
[Produces("application/json")]
[Tags("Moderation")]
public sealed class TerritoryModerationCasesController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly ModerationCaseService _moderationCases;

    public TerritoryModerationCasesController(
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator,
        ModerationCaseService moderationCases)
    {
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
        _moderationCases = moderationCases;
    }

    /// <summary>
    /// Decide um caso de moderação (work item MODERATIONCASE).
    /// Curator ou Moderator pode decidir.
    /// </summary>
    [HttpPost("{workItemId:guid}/decide")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Decide(
        [FromRoute] Guid territoryId,
        [FromRoute] Guid workItemId,
        [FromBody] CompleteWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var canModerate =
            await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Curator, cancellationToken) ||
            await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Moderator, cancellationToken);
        if (!canModerate)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<WorkItemOutcome>(request.Outcome, true, out var outcome) ||
            outcome == WorkItemOutcome.None)
        {
            return BadRequest(new { error = $"Invalid outcome: {request.Outcome}" });
        }

        var result = await _moderationCases.DecideAsync(
            workItemId,
            userContext.User.Id,
            outcome,
            request.Notes,
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (string.Equals(result.Error, "Work item not found.", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }

            return BadRequest(new { error = result.Error ?? "Unable to decide moderation case." });
        }

        return Ok(new { message = "Moderation case decided." });
    }
}

