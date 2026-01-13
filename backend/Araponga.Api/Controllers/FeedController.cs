using Araponga.Api;
using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Feed;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/feed")]
[Produces("application/json")]
[Tags("Feed")]
public sealed class FeedController : ControllerBase
{
    private readonly FeedService _feedService;
    private readonly EventsService _eventsService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ActiveTerritoryService _activeTerritoryService;
    private readonly AccessEvaluator _accessEvaluator;

    public FeedController(
        FeedService feedService,
        EventsService eventsService,
        CurrentUserAccessor currentUserAccessor,
        ActiveTerritoryService activeTerritoryService,
        AccessEvaluator accessEvaluator)
    {
        _feedService = feedService;
        _eventsService = eventsService;
        _currentUserAccessor = currentUserAccessor;
        _activeTerritoryService = activeTerritoryService;
        _accessEvaluator = accessEvaluator;
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
    public async Task<ActionResult<IEnumerable<FeedItemResponse>>> GetFeed(
        [FromQuery] Guid? territoryId,
        [FromQuery] Guid? mapEntityId,
        [FromQuery] Guid? assetId,
        CancellationToken cancellationToken)
    {
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var posts = await _feedService.ListForTerritoryAsync(
            resolvedTerritoryId.Value,
            userContext.User?.Id,
            mapEntityId,
            assetId,
            cancellationToken);

        var eventLookup = await LoadEventSummariesAsync(posts, cancellationToken);
        var postIds = posts.Select(p => p.Id).ToList();
        var counts = await _feedService.GetCountsByPostIdsAsync(postIds, cancellationToken);
        
        var response = new List<FeedItemResponse>();
        foreach (var post in posts)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new PostCounts(0, 0));
            var eventSummary = ResolveEventSummary(post, eventLookup);

            response.Add(new FeedItemResponse(
                post.Id,
                post.Title,
                post.Content,
                post.Type.ToString().ToUpperInvariant(),
                post.Visibility.ToString().ToUpperInvariant(),
                post.Status.ToString().ToUpperInvariant(),
                post.MapEntityId,
                eventSummary,
                post.Type == PostType.Alert,
                postCounts.LikeCount,
                postCounts.ShareCount,
                post.CreatedAtUtc));
        }

        return Ok(response);
    }

    /// <summary>
    /// Visualiza o feed do território ativo (paginado).
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<FeedItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<FeedItemResponse>>> GetFeedPaged(
        [FromQuery] Guid? territoryId,
        [FromQuery] Guid? mapEntityId,
        [FromQuery] Guid? assetId,
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
        if (userContext.Status != TokenStatus.Valid)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _feedService.ListForTerritoryPagedAsync(
            resolvedTerritoryId.Value,
            userContext.User?.Id,
            mapEntityId,
            assetId,
            pagination,
            cancellationToken);

        var eventLookup = await LoadEventSummariesAsync(pagedResult.Items, cancellationToken);
        var postIds = pagedResult.Items.Select(p => p.Id).ToList();
        var counts = await _feedService.GetCountsByPostIdsAsync(postIds, cancellationToken);
        
        var items = new List<FeedItemResponse>();
        foreach (var post in pagedResult.Items)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new PostCounts(0, 0));
            var eventSummary = ResolveEventSummary(post, eventLookup);

            items.Add(new FeedItemResponse(
                post.Id,
                post.Title,
                post.Content,
                post.Type.ToString().ToUpperInvariant(),
                post.Visibility.ToString().ToUpperInvariant(),
                post.Status.ToString().ToUpperInvariant(),
                post.MapEntityId,
                eventSummary,
                post.Type == PostType.Alert,
                postCounts.LikeCount,
                postCounts.ShareCount,
                post.CreatedAtUtc));
        }

        var response = new PagedResponse<FeedItemResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Visualiza o feed pessoal do usuário autenticado.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(IEnumerable<FeedItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<FeedItemResponse>>> GetMyFeed(
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var posts = await _feedService.ListForUserAsync(userContext.User.Id, cancellationToken);
        var eventLookup = await LoadEventSummariesAsync(posts, cancellationToken);
        var postIds = posts.Select(p => p.Id).ToList();
        var counts = await _feedService.GetCountsByPostIdsAsync(postIds, cancellationToken);
        
        var response = new List<FeedItemResponse>();
        foreach (var post in posts)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new PostCounts(0, 0));
            var eventSummary = ResolveEventSummary(post, eventLookup);

            response.Add(new FeedItemResponse(
                post.Id,
                post.Title,
                post.Content,
                post.Type.ToString().ToUpperInvariant(),
                post.Visibility.ToString().ToUpperInvariant(),
                post.Status.ToString().ToUpperInvariant(),
                post.MapEntityId,
                eventSummary,
                post.Type == PostType.Alert,
                postCounts.LikeCount,
                postCounts.ShareCount,
                post.CreatedAtUtc));
        }

        return Ok(response);
    }

    /// <summary>
    /// Visualiza o feed pessoal do usuário autenticado (paginado).
    /// </summary>
    [HttpGet("me/paged")]
    [ProducesResponseType(typeof(PagedResponse<FeedItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<FeedItemResponse>>> GetMyFeedPaged(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var pagedResult = await _feedService.ListForUserPagedAsync(userContext.User.Id, pagination, cancellationToken);
        var eventLookup = await LoadEventSummariesAsync(pagedResult.Items, cancellationToken);
        var postIds = pagedResult.Items.Select(p => p.Id).ToList();
        var counts = await _feedService.GetCountsByPostIdsAsync(postIds, cancellationToken);
        
        var items = new List<FeedItemResponse>();
        foreach (var post in pagedResult.Items)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new PostCounts(0, 0));
            var eventSummary = ResolveEventSummary(post, eventLookup);

            items.Add(new FeedItemResponse(
                post.Id,
                post.Title,
                post.Content,
                post.Type.ToString().ToUpperInvariant(),
                post.Visibility.ToString().ToUpperInvariant(),
                post.Status.ToString().ToUpperInvariant(),
                post.MapEntityId,
                eventSummary,
                post.Type == PostType.Alert,
                postCounts.LikeCount,
                postCounts.ShareCount,
                post.CreatedAtUtc));
        }

        var response = new PagedResponse<FeedItemResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Cria um post comunitário no território ativo.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FeedItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FeedItemResponse>> CreatePost(
        [FromQuery] Guid? territoryId,
        [FromBody] CreatePostRequest request,
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

        if (!Enum.TryParse<PostType>(request.Type, true, out var postType) ||
            !Enum.TryParse<PostVisibility>(request.Visibility, true, out var visibility))
        {
            return BadRequest(new { error = "Invalid post type or visibility." });
        }

        var isResident = await _accessEvaluator.IsResidentAsync(
            userContext.User.Id,
            resolvedTerritoryId.Value,
            cancellationToken);
        if (!isResident)
        {
            return Unauthorized();
        }

        var result = await _feedService.CreatePostAsync(
            resolvedTerritoryId.Value,
            userContext.User.Id,
            request.Title,
            request.Content,
            postType,
            visibility,
            PostStatus.Published,
            request.MapEntityId,
            request.GeoAnchors?.Select(anchor => new Application.Models.GeoAnchorInput(
                anchor.Latitude,
                anchor.Longitude,
                anchor.Type)).ToList(),
            request.AssetIds,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to create post." });
        }

        var post = result.Value;
        var response = new FeedItemResponse(
            post.Id,
            post.Title,
            post.Content,
            post.Type.ToString().ToUpperInvariant(),
            post.Visibility.ToString().ToUpperInvariant(),
            post.Status.ToString().ToUpperInvariant(),
            post.MapEntityId,
            ResolveEventSummary(post, new Dictionary<Guid, Application.Models.EventSummary>()),
            post.Type == PostType.Alert,
            0,
            0,
            post.CreatedAtUtc);

        return CreatedAtAction(nameof(GetFeed), new { }, response);
    }

    /// <summary>
    /// Curte um post do território ativo.
    /// </summary>
    [HttpPost("{postId:guid}/likes")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Like(
        [FromRoute] Guid postId,
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var sessionId = GetSessionId();
        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status == TokenStatus.Invalid)
        {
            return Unauthorized();
        }

        var actorId = userContext.User is null ? $"session:{sessionId ?? "unknown"}" : $"user:{userContext.User.Id}";
        var success = await _feedService.LikeAsync(
            resolvedTerritoryId.Value,
            postId,
            actorId,
            userContext.User?.Id,
            cancellationToken);

        return success ? NoContent() : BadRequest(new { error = "Unable to like post." });
    }

    /// <summary>
    /// Comenta em um post do território ativo.
    /// </summary>
    [HttpPost("{postId:guid}/comments")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Comment(
        [FromRoute] Guid postId,
        [FromQuery] Guid? territoryId,
        [FromBody] AddCommentRequest request,
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

        var result = await _feedService.CommentAsync(
            resolvedTerritoryId.Value,
            postId,
            userContext.User.Id,
            request.Content,
            cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Compartilha um post do território ativo.
    /// </summary>
    [HttpPost("{postId:guid}/shares")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Share(
        [FromRoute] Guid postId,
        [FromQuery] Guid? territoryId,
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

        var result = await _feedService.ShareAsync(
            resolvedTerritoryId.Value,
            postId,
            userContext.User.Id,
            cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    private async Task<Dictionary<Guid, Application.Models.EventSummary>> LoadEventSummariesAsync(
        IReadOnlyCollection<CommunityPost> posts,
        CancellationToken cancellationToken)
    {
        var eventIds = posts
            .Where(post => string.Equals(post.ReferenceType, "EVENT", StringComparison.OrdinalIgnoreCase))
            .Select(post => post.ReferenceId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        if (eventIds.Count == 0)
        {
            return new Dictionary<Guid, Application.Models.EventSummary>();
        }

        return new Dictionary<Guid, Application.Models.EventSummary>(
            await _eventsService.GetSummariesByIdsAsync(eventIds, cancellationToken));
    }

    private static EventSummaryResponse? ResolveEventSummary(
        CommunityPost post,
        IReadOnlyDictionary<Guid, Application.Models.EventSummary> eventLookup)
    {
        if (!string.Equals(post.ReferenceType, "EVENT", StringComparison.OrdinalIgnoreCase) ||
            post.ReferenceId is null ||
            !eventLookup.TryGetValue(post.ReferenceId.Value, out var summary))
        {
            return null;
        }

        var evt = summary.Event;
        return new EventSummaryResponse(
            evt.Id,
            evt.Title,
            evt.StartsAtUtc,
            evt.EndsAtUtc,
            evt.Latitude,
            evt.Longitude,
            evt.LocationLabel,
            evt.Status.ToString().ToUpperInvariant(),
            evt.CreatedByUserId,
            summary.CreatedByDisplayName,
            evt.CreatedByMembership.ToString().ToUpperInvariant(),
            summary.InterestedCount,
            summary.ConfirmedCount);
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
