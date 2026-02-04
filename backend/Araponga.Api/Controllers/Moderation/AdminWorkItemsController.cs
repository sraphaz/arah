using Araponga.Api.Contracts.Admin;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Modules.Moderation.Domain.Work;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/admin/work-items")]
[Produces("application/json")]
[Tags("Admin")]
public sealed class AdminWorkItemsController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly WorkQueueService _workQueue;

    public AdminWorkItemsController(
        CurrentUserAccessor currentUserAccessor,
        AccessEvaluator accessEvaluator,
        WorkQueueService workQueue)
    {
        _currentUserAccessor = currentUserAccessor;
        _accessEvaluator = accessEvaluator;
        _workQueue = workQueue;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<WorkItemResponse>>> List(
        [FromQuery] string? type,
        [FromQuery] string? status,
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

        WorkItemType? parsedType = null;
        if (!string.IsNullOrWhiteSpace(type))
        {
            if (!Enum.TryParse<WorkItemType>(type, true, out var t))
            {
                return BadRequest(new { error = $"Invalid type: {type}" });
            }
            parsedType = t;
        }

        WorkItemStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<WorkItemStatus>(status, true, out var s))
            {
                return BadRequest(new { error = $"Invalid status: {status}" });
            }
            parsedStatus = s;
        }

        var items = await _workQueue.ListAsync(parsedType, parsedStatus, territoryId: null, cancellationToken);
        return Ok(items.Select(ToResponse).ToList());
    }

    [HttpPost("{workItemId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(
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

        var existing = await _workQueue.GetByIdAsync(workItemId, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (!Enum.TryParse<WorkItemOutcome>(request.Outcome, true, out var parsedOutcome) ||
            parsedOutcome == WorkItemOutcome.None)
        {
            return BadRequest(new { error = $"Invalid outcome: {request.Outcome}" });
        }

        // Admin pode completar qualquer item (inclusive os territoriais) via endpoint admin.
        var result = await _workQueue.CompleteAsync(workItemId, userContext.User.Id, parsedOutcome, request.Notes, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error ?? "Unable to complete work item." });
        }

        return Ok(new { message = "Work item completed." });
    }

    private static WorkItemResponse ToResponse(WorkItem item)
        => new(
            item.Id,
            item.Type.ToString().ToUpperInvariant(),
            item.Status.ToString().ToUpperInvariant(),
            item.TerritoryId,
            item.CreatedByUserId,
            item.CreatedAtUtc,
            item.RequiredSystemPermission?.ToString().ToUpperInvariant(),
            item.RequiredCapability?.ToString().ToUpperInvariant(),
            item.SubjectType,
            item.SubjectId,
            item.PayloadJson,
            item.Outcome.ToString().ToUpperInvariant(),
            item.CompletedAtUtc,
            item.CompletedByUserId,
            item.CompletionNotes);
}

