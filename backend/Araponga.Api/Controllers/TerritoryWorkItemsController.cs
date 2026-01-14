using Araponga.Api.Contracts.Admin;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Work;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId:guid}/work-items")]
[Produces("application/json")]
[Tags("Admin")]
public sealed class TerritoryWorkItemsController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly WorkQueueService _workQueue;

    public TerritoryWorkItemsController(
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
        [FromRoute] Guid territoryId,
        [FromQuery] string? type,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Curador OU Moderador pode listar (SystemAdmin já é implícito no AccessEvaluator.HasCapabilityAsync)
        var canList =
            await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Curator, cancellationToken) ||
            await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Moderator, cancellationToken);

        if (!canList)
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

        var items = await _workQueue.ListAsync(parsedType, parsedStatus, territoryId, cancellationToken);
        return Ok(items.Select(AdminWorkItemsControllerExtensions.ToResponse).ToList());
    }

    [HttpPost("{workItemId:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(
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

        var item = await _workQueue.GetByIdAsync(workItemId, cancellationToken);
        if (item is null || item.TerritoryId != territoryId)
        {
            return NotFound();
        }

        if (item.RequiredSystemPermission.HasValue)
        {
            var ok = await _accessEvaluator.HasSystemPermissionAsync(
                userContext.User.Id,
                item.RequiredSystemPermission.Value,
                cancellationToken);
            if (!ok)
            {
                return Unauthorized();
            }
        }

        if (!Enum.TryParse<WorkItemOutcome>(request.Outcome, true, out var parsedOutcome) ||
            parsedOutcome == WorkItemOutcome.None)
        {
            return BadRequest(new { error = $"Invalid outcome: {request.Outcome}" });
        }

        // Precisa respeitar o "required" do item
        if (item.RequiredCapability.HasValue)
        {
            var ok = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, item.RequiredCapability.Value, cancellationToken);
            if (!ok)
            {
                return Unauthorized();
            }
        }
        else
        {
            // Sem requiredCapability, pelo menos Curator ou Moderator para completar itens territoriais
            var ok =
                await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Curator, cancellationToken) ||
                await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, territoryId, MembershipCapabilityType.Moderator, cancellationToken);
            if (!ok)
            {
                return Unauthorized();
            }
        }

        var result = await _workQueue.CompleteAsync(workItemId, userContext.User.Id, parsedOutcome, request.Notes, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error ?? "Unable to complete work item." });
        }

        return Ok(new { message = "Work item completed." });
    }
}

internal static class AdminWorkItemsControllerExtensions
{
    public static WorkItemResponse ToResponse(WorkItem item)
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

