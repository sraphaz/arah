using Araponga.Api;
using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Moderation;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Moderation;
using Araponga.Domain.Membership;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[Tags("Moderation")]
public sealed class ModerationController : ControllerBase
{
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ReportService _reportService;
    private readonly UserBlockService _userBlockService;
    private readonly ActiveTerritoryService _activeTerritoryService;
    private readonly AccessEvaluator _accessEvaluator;

    public ModerationController(
        CurrentUserAccessor currentUserAccessor,
        ReportService reportService,
        UserBlockService userBlockService,
        ActiveTerritoryService activeTerritoryService,
        AccessEvaluator accessEvaluator)
    {
        _currentUserAccessor = currentUserAccessor;
        _reportService = reportService;
        _userBlockService = userBlockService;
        _activeTerritoryService = activeTerritoryService;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Reporta um post do feed.
    /// </summary>
    [HttpPost("reports/posts/{postId:guid}")]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportResponse>> ReportPost(
        [FromRoute] Guid postId,
        [FromBody] ReportRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _reportService.ReportPostAsync(
            userContext.User.Id,
            postId,
            request.Reason,
            request.Details,
            cancellationToken);

        if (result.error == "Post not found.")
        {
            return NotFound();
        }

        if (result.error is not null)
        {
            return BadRequest(new { error = result.error });
        }

        if (!result.created || result.report is null)
        {
            return Ok(new ReportResponse(
                Guid.Empty,
                ReportTargetType.Post.ToString().ToUpperInvariant(),
                postId,
                request.Reason,
                request.Details,
                true,
                DateTime.UtcNow));
        }

        var response = new ReportResponse(
            result.report.Id,
            result.report.TargetType.ToString().ToUpperInvariant(),
            result.report.TargetId,
            result.report.Reason,
            result.report.Details,
            false,
            result.report.CreatedAtUtc);

        return CreatedAtAction(nameof(ReportPost), new { postId }, response);
    }

    /// <summary>
    /// Reporta um usuário.
    /// </summary>
    [HttpPost("reports/users/{userId:guid}")]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportResponse>> ReportUser(
        [FromRoute] Guid userId,
        [FromQuery] Guid? territoryId,
        [FromBody] ReportRequest request,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _reportService.ReportUserAsync(
            userContext.User.Id,
            resolvedTerritoryId.Value,
            userId,
            request.Reason,
            request.Details,
            cancellationToken);

        if (result.error == "User not found.")
        {
            return NotFound();
        }

        if (result.error is not null)
        {
            return BadRequest(new { error = result.error });
        }

        if (!result.created || result.report is null)
        {
            return Ok(new ReportResponse(
                Guid.Empty,
                ReportTargetType.User.ToString().ToUpperInvariant(),
                userId,
                request.Reason,
                request.Details,
                true,
                DateTime.UtcNow));
        }

        var response = new ReportResponse(
            result.report.Id,
            result.report.TargetType.ToString().ToUpperInvariant(),
            result.report.TargetId,
            result.report.Reason,
            result.report.Details,
            false,
            result.report.CreatedAtUtc);

        return CreatedAtAction(nameof(ReportUser), new { userId }, response);
    }

    /// <summary>
    /// Lista reports para curadoria.
    /// </summary>
    [HttpGet("reports")]
    [ProducesResponseType(typeof(IEnumerable<ReportSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ReportSummaryResponse>>> ListReports(
        [FromQuery] Guid? territoryId,
        [FromQuery] string? targetType,
        [FromQuery] string? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, resolvedTerritoryId.Value, MembershipCapabilityType.Curator, cancellationToken);
        if (!isCurator)
        {
            return Unauthorized();
        }

        ReportTargetType? parsedTargetType = null;
        if (!string.IsNullOrWhiteSpace(targetType))
        {
            if (!Enum.TryParse<ReportTargetType>(targetType, true, out var targetEnum))
            {
                return BadRequest(new { error = "Invalid targetType." });
            }

            parsedTargetType = targetEnum;
        }

        ReportStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<ReportStatus>(status, true, out var statusEnum))
            {
                return BadRequest(new { error = "Invalid status." });
            }

            parsedStatus = statusEnum;
        }

        var reports = await _reportService.ListAsync(
            resolvedTerritoryId.Value,
            parsedTargetType,
            parsedStatus,
            from,
            to,
            cancellationToken);

        var response = reports.Select(report => new ReportSummaryResponse(
            report.Id,
            report.TerritoryId,
            report.TargetType.ToString().ToUpperInvariant(),
            report.TargetId,
            report.Reason,
            report.Status.ToString().ToUpperInvariant(),
            report.CreatedAtUtc));

        return Ok(response);
    }

    /// <summary>
    /// Lista reports do território (paginado).
    /// </summary>
    [HttpGet("reports/paged")]
    [ProducesResponseType(typeof(PagedResponse<ReportSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<ReportSummaryResponse>>> ListReportsPaged(
        [FromQuery] Guid? territoryId,
        [FromQuery] string? targetType,
        [FromQuery] string? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var isCurator = await _accessEvaluator.HasCapabilityAsync(userContext.User.Id, resolvedTerritoryId.Value, MembershipCapabilityType.Curator, cancellationToken);
        if (!isCurator)
        {
            return Forbid();
        }

        ReportTargetType? parsedTargetType = null;
        if (!string.IsNullOrWhiteSpace(targetType))
        {
            if (!Enum.TryParse<ReportTargetType>(targetType, true, out var targetTypeEnum))
            {
                return BadRequest(new { error = "Invalid targetType." });
            }

            parsedTargetType = targetTypeEnum;
        }

        ReportStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<ReportStatus>(status, true, out var statusEnum))
            {
                return BadRequest(new { error = "Invalid status." });
            }

            parsedStatus = statusEnum;
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _reportService.ListPagedAsync(
            resolvedTerritoryId.Value,
            parsedTargetType,
            parsedStatus,
            from,
            to,
            pagination,
            cancellationToken);

        var response = new PagedResponse<ReportSummaryResponse>(
            pagedResult.Items.Select(report => new ReportSummaryResponse(
                report.Id,
                report.TerritoryId,
                report.TargetType.ToString().ToUpperInvariant(),
                report.TargetId,
                report.Reason,
                report.Status.ToString().ToUpperInvariant(),
                report.CreatedAtUtc)).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Bloqueia um usuário para não ver seu conteúdo.
    /// </summary>
    [HttpPost("blocks/users/{userId:guid}")]
    [ProducesResponseType(typeof(BlockResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BlockResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlockResponse>> BlockUser(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _userBlockService.BlockAsync(
            userContext.User.Id,
            userId,
            cancellationToken);

        if (result.error == "User not found.")
        {
            return NotFound();
        }

        if (result.error is not null)
        {
            return BadRequest(new { error = result.error });
        }

        if (!result.created || result.block is null)
        {
            return Ok(new BlockResponse(
                userContext.User.Id,
                userId,
                true,
                DateTime.UtcNow));
        }

        var response = new BlockResponse(
            result.block.BlockerUserId,
            result.block.BlockedUserId,
            false,
            result.block.CreatedAtUtc);

        return CreatedAtAction(nameof(BlockUser), new { userId }, response);
    }

    /// <summary>
    /// Remove o bloqueio de um usuário.
    /// </summary>
    [HttpDelete("blocks/users/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnblockUser(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        await _userBlockService.UnblockAsync(userContext.User.Id, userId, cancellationToken);
        return NoContent();
    }

    private string? GetSessionId()
    {
        return Request.Headers.TryGetValue(ApiHeaders.SessionId, out var header) &&
               !string.IsNullOrWhiteSpace(header)
            ? header.ToString()
            : null;
    }

    private async Task<Guid?> ResolveTerritoryIdAsync(Guid? territoryId, CancellationToken cancellationToken)
    {
        if (territoryId is not null)
        {
            return territoryId;
        }

        var sessionId = GetSessionId();
        if (sessionId is null)
        {
            return null;
        }

        return await _activeTerritoryService.GetActiveAsync(sessionId, cancellationToken);
    }
}
