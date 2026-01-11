using Araponga.Api.Contracts.Moderation;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Moderation;
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

    public ModerationController(
        CurrentUserAccessor currentUserAccessor,
        ReportService reportService,
        UserBlockService userBlockService)
    {
        _currentUserAccessor = currentUserAccessor;
        _reportService = reportService;
        _userBlockService = userBlockService;
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
        [FromBody] ReportRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _reportService.ReportUserAsync(
            userContext.User.Id,
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
}
