using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.JoinRequests;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Social.JoinRequests;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("JoinRequests")]
public sealed class JoinRequestsController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly TerritoryService _territoryService;
    private readonly JoinRequestService _joinRequestService;
    private readonly AccessEvaluator _accessEvaluator;

    public JoinRequestsController(
        CurrentUserAccessor currentUserAccessor,
        TerritoryService territoryService,
        JoinRequestService joinRequestService,
        AccessEvaluator accessEvaluator)
    {
        _currentUserAccessor = currentUserAccessor;
        _territoryService = territoryService;
        _joinRequestService = joinRequestService;
        _accessEvaluator = accessEvaluator;
    }

    [HttpPost("territories/{territoryId:guid}/join-requests")]
    [ProducesResponseType(typeof(JoinRequestCreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(JoinRequestCreatedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JoinRequestCreatedResponse>> CreateJoinRequest(
        [FromRoute] Guid territoryId,
        [FromBody] CreateJoinRequestRequest request,
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

        if (request.RecipientUserIds is null || request.RecipientUserIds.Count == 0)
        {
            return BadRequest(new { error = "Recipient list is required." });
        }

        var (created, error, joinRequest) = await _joinRequestService.CreateAsync(
            userContext.User.Id,
            territoryId,
            request.RecipientUserIds,
            request.Message,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(error))
        {
            return BadRequest(new { error });
        }

        if (joinRequest is null)
        {
            return BadRequest(new { error = "Unable to create join request." });
        }

        var response = new JoinRequestCreatedResponse(
            joinRequest.Id,
            joinRequest.Status.ToString().ToUpperInvariant(),
            joinRequest.CreatedAtUtc);

        if (created)
        {
            return Created($"/api/v1/join-requests/{joinRequest.Id}", response);
        }

        return Ok(response);
    }

    [HttpGet("join-requests/incoming")]
    [ProducesResponseType(typeof(IReadOnlyList<IncomingJoinRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<IncomingJoinRequestResponse>>> ListIncoming(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var parsedStatus = TerritoryJoinRequestStatus.Pending;
        if (!string.IsNullOrWhiteSpace(status) &&
            (!Enum.TryParse(status, true, out parsedStatus) ||
             parsedStatus != TerritoryJoinRequestStatus.Pending))
        {
            return BadRequest(new { error = "Unsupported status." });
        }

        var incoming = await _joinRequestService.ListIncomingAsync(userContext.User.Id, cancellationToken);
        var response = incoming
            .Select(request => new IncomingJoinRequestResponse(
                request.Id,
                request.TerritoryId,
                request.RequesterUserId,
                request.RequesterDisplayName,
                request.Message,
                request.CreatedAtUtc))
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Lista join requests recebidos (paginado).
    /// </summary>
    [HttpGet("join-requests/incoming/paged")]
    [ProducesResponseType(typeof(PagedResponse<IncomingJoinRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<IncomingJoinRequestResponse>>> ListIncomingPaged(
        [FromQuery] string? status,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var parsedStatus = TerritoryJoinRequestStatus.Pending;
        if (!string.IsNullOrWhiteSpace(status) &&
            (!Enum.TryParse(status, true, out parsedStatus) ||
             parsedStatus != TerritoryJoinRequestStatus.Pending))
        {
            return BadRequest(new { error = "Unsupported status." });
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _joinRequestService.ListIncomingPagedAsync(userContext.User.Id, pagination, cancellationToken);
        const int maxInt32 = int.MaxValue;
        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var response = new PagedResponse<IncomingJoinRequestResponse>(
            pagedResult.Items.Select(request => new IncomingJoinRequestResponse(
                request.Id,
                request.TerritoryId,
                request.RequesterUserId,
                request.RequesterDisplayName,
                request.Message,
                request.CreatedAtUtc)).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    [HttpPost("join-requests/{id:guid}/approve")]
    [ProducesResponseType(typeof(JoinRequestActionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JoinRequestActionResponse>> Approve(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Obter request para pegar territoryId
        var joinRequest = await _joinRequestService.GetByIdAsync(id, cancellationToken);
        var isCurator = joinRequest != null 
            ? await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, joinRequest.TerritoryId, MembershipCapabilityType.Curator, cancellationToken)
            : false;

        var result = await _joinRequestService.ApproveAsync(
            id,
            userContext.User.Id,
            isCurator,
            cancellationToken);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var request = result.Request!;
        var response = new JoinRequestActionResponse(
            request.Id,
            request.Status.ToString().ToUpperInvariant(),
            request.DecidedAtUtc,
            request.DecidedByUserId);

        return Ok(response);
    }

    [HttpPost("join-requests/{id:guid}/reject")]
    [ProducesResponseType(typeof(JoinRequestActionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JoinRequestActionResponse>> Reject(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        // Obter request para pegar territoryId
        var joinRequest = await _joinRequestService.GetByIdAsync(id, cancellationToken);
        var isCurator = joinRequest != null 
            ? await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, joinRequest.TerritoryId, MembershipCapabilityType.Curator, cancellationToken)
            : false;

        var result = await _joinRequestService.RejectAsync(
            id,
            userContext.User.Id,
            isCurator,
            cancellationToken);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var request = result.Request!;
        var response = new JoinRequestActionResponse(
            request.Id,
            request.Status.ToString().ToUpperInvariant(),
            request.DecidedAtUtc,
            request.DecidedByUserId);

        return Ok(response);
    }

    [HttpPost("join-requests/{id:guid}/cancel")]
    [ProducesResponseType(typeof(JoinRequestActionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JoinRequestActionResponse>> Cancel(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _joinRequestService.CancelAsync(
            id,
            userContext.User.Id,
            cancellationToken);

        if (!result.Found)
        {
            return NotFound();
        }

        if (result.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var request = result.Request!;
        var response = new JoinRequestActionResponse(
            request.Id,
            request.Status.ToString().ToUpperInvariant(),
            request.DecidedAtUtc,
            request.DecidedByUserId);

        return Ok(response);
    }
}
