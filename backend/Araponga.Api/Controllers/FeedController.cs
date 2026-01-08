using Araponga.Api.Contracts.Feed;
using Araponga.Api.Security;
using Araponga.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/feed")]
[Produces("application/json")]
[Tags("Feed")]
public sealed class FeedController : ControllerBase
{
    private readonly FeedService _feedService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ActiveTerritoryService _activeTerritoryService;

    public FeedController(
        FeedService feedService,
        CurrentUserAccessor currentUserAccessor,
        ActiveTerritoryService activeTerritoryService)
    {
        _feedService = feedService;
        _currentUserAccessor = currentUserAccessor;
        _activeTerritoryService = activeTerritoryService;
    }

    /// <summary>
    /// Visualiza o feed do território ativo.
    /// </summary>
    /// <remarks>
    /// Visitantes veem somente posts públicos; moradores veem todo o conteúdo.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FeedItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<FeedItemResponse>>> GetFeed(CancellationToken cancellationToken)
    {
        var sessionId = GetSessionId();
        if (sessionId is null)
        {
            return BadRequest(new { error = "X-Session-Id header is required." });
        }

        var territoryId = await _activeTerritoryService.GetActiveAsync(sessionId, cancellationToken);
        if (territoryId is null)
        {
            return BadRequest(new { error = "No active territory selected." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status == TokenStatus.Invalid)
        {
            return Unauthorized();
        }

        var posts = await _feedService.ListForTerritoryAsync(
            territoryId.Value,
            userContext.User?.Id,
            cancellationToken);

        var response = posts.Select(post =>
            new FeedItemResponse(
                post.Id,
                post.Title,
                post.Content,
                post.Visibility.ToString().ToUpperInvariant(),
                post.CreatedAtUtc));

        return Ok(response);
    }

    private string? GetSessionId()
    {
        return Request.Headers.TryGetValue(ApiHeaders.SessionId, out var header) &&
               !string.IsNullOrWhiteSpace(header)
            ? header.ToString()
            : null;
    }
}
