using Araponga.Api.Contracts.Governance;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Araponga.Domain.Governance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/territories/{territoryId}/votings")]
[Produces("application/json")]
[Tags("Governance")]
public sealed class VotingsController : ControllerBase
{
    private readonly VotingService _votingService;
    private readonly CurrentUserAccessor _currentUserAccessor;

    public VotingsController(
        VotingService votingService,
        CurrentUserAccessor currentUserAccessor)
    {
        _votingService = votingService;
        _currentUserAccessor = currentUserAccessor;
    }

    /// <summary>
    /// Cria uma nova votação no território.
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(VotingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<VotingResponse>> CreateVoting(
        Guid territoryId,
        [FromBody] CreateVotingRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<VotingType>(request.Type, ignoreCase: true, out var votingType))
        {
            return BadRequest(new { error = "Invalid voting type." });
        }

        if (!Enum.TryParse<VotingVisibility>(request.Visibility, ignoreCase: true, out var visibility))
        {
            return BadRequest(new { error = "Invalid visibility." });
        }

        var result = await _votingService.CreateVotingAsync(
            territoryId,
            userContext.User.Id,
            votingType,
            request.Title,
            request.Description,
            request.Options,
            visibility,
            request.StartsAtUtc,
            request.EndsAtUtc,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(
            nameof(GetVoting),
            new { territoryId, votingId = result.Value!.Id },
            MapToResponse(result.Value!));
    }

    /// <summary>
    /// Lista votações do território.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VotingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<VotingResponse>>> ListVotings(
        Guid territoryId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        VotingStatus? votingStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<VotingStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            votingStatus = parsedStatus;
        }

        var votings = await _votingService.ListVotingsAsync(
            territoryId,
            votingStatus,
            null,
            cancellationToken);

        var responses = votings.Select(MapToResponse).ToList();
        return Ok(responses);
    }

    /// <summary>
    /// Obtém uma votação pelo ID.
    /// </summary>
    [HttpGet("{votingId}")]
    [ProducesResponseType(typeof(VotingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<VotingResponse>> GetVoting(
        Guid territoryId,
        Guid votingId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _votingService.GetVotingAsync(votingId, userContext.User.Id, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(MapToResponse(result.Value!));
    }

    /// <summary>
    /// Registra um voto em uma votação.
    /// </summary>
    [HttpPost("{votingId}/vote")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult> Vote(
        Guid territoryId,
        Guid votingId,
        [FromBody] VoteRequest request,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _votingService.VoteAsync(
            votingId,
            userContext.User.Id,
            request.SelectedOption,
            cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Fecha uma votação (apenas criador ou curador).
    /// </summary>
    [HttpPost("{votingId}/close")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult> CloseVoting(
        Guid territoryId,
        Guid votingId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _votingService.CloseVotingAsync(votingId, userContext.User.Id, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Obtém os resultados de uma votação.
    /// </summary>
    [HttpGet("{votingId}/results")]
    [ProducesResponseType(typeof(VotingResultsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<VotingResultsResponse>> GetResults(
        Guid territoryId,
        Guid votingId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var result = await _votingService.GetResultsAsync(votingId, cancellationToken);
        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(new VotingResultsResponse(votingId, result.Value!));
    }

    private static VotingResponse MapToResponse(Domain.Governance.Voting voting)
    {
        return new VotingResponse(
            voting.Id,
            voting.TerritoryId,
            voting.CreatedByUserId,
            voting.Type.ToString(),
            voting.Title,
            voting.Description,
            voting.Options,
            voting.Visibility.ToString(),
            voting.Status.ToString(),
            voting.StartsAtUtc,
            voting.EndsAtUtc,
            voting.CreatedAtUtc,
            voting.UpdatedAtUtc);
    }
}
