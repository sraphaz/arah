using Araponga.Api.Contracts.Admin;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Domain.Work;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/admin/verifications")]
[Produces("application/json")]
[Tags("Admin")]
public sealed class AdminVerificationController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly VerificationQueueService _verificationQueue;

    public AdminVerificationController(
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator,
        VerificationQueueService verificationQueue)
    {
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
        _verificationQueue = verificationQueue;
    }

    /// <summary>
    /// Decide verificação de identidade global (apenas SystemAdmin).
    /// </summary>
    [HttpPost("identity/{workItemId:guid}/decide")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DecideIdentity(
        [FromRoute] Guid workItemId,
        [FromBody] CompleteWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isAdmin = await _accessEvaluator.HasSystemPermissionAsync(
            userContext.User.Id,
            SystemPermissionType.SystemAdmin,
            cancellationToken);
        if (!isAdmin)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<WorkItemOutcome>(request.Outcome, true, out var outcome) ||
            (outcome != WorkItemOutcome.Approved && outcome != WorkItemOutcome.Rejected))
        {
            return BadRequest(new { error = $"Invalid outcome: {request.Outcome}" });
        }

        var result = await _verificationQueue.DecideIdentityAsync(
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

            return BadRequest(new { error = result.Error ?? "Unable to decide identity verification." });
        }

        return Ok(new { message = "Identity verification decided." });
    }
}

