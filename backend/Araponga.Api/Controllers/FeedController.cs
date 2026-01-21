using Araponga.Api;
using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Feed;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Domain.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/feed")]
[Produces("application/json")]
[Tags("Feed")]
public sealed class FeedController : ControllerBase
{
    private readonly FeedService _feedService;
    private readonly PostEditService _postEditService;
    private readonly EventsService _eventsService;
    private readonly MediaService _mediaService;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ActiveTerritoryService _activeTerritoryService;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IPostGeoAnchorRepository _postGeoAnchorRepository;
    private readonly IMediaAttachmentRepository _mediaAttachmentRepository;

    public FeedController(
        FeedService feedService,
        PostEditService postEditService,
        EventsService eventsService,
        MediaService mediaService,
        CurrentUserAccessor currentUserAccessor,
        ActiveTerritoryService activeTerritoryService,
        AccessEvaluator accessEvaluator,
        IPostGeoAnchorRepository postGeoAnchorRepository,
        IMediaAttachmentRepository mediaAttachmentRepository)
    {
        _feedService = feedService;
        _postEditService = postEditService;
        _eventsService = eventsService;
        _mediaService = mediaService;
        _currentUserAccessor = currentUserAccessor;
        _activeTerritoryService = activeTerritoryService;
        _accessEvaluator = accessEvaluator;
        _postGeoAnchorRepository = postGeoAnchorRepository;
        _mediaAttachmentRepository = mediaAttachmentRepository;
    }

    /// <summary>
    /// Visualiza o feed do território ativo.
    /// </summary>
    /// <remarks>
    /// Visitantes veem somente posts públicos; moradores veem todo o conteúdo.
    /// </remarks>
    [HttpGet]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(typeof(IEnumerable<FeedItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
        var mediaUrlsByPost = await LoadMediaUrlsByPostIdsAsync(postIds, cancellationToken);
        
        var response = new List<FeedItemResponse>();
        foreach (var post in posts)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new PostCounts(0, 0));
            var eventSummary = ResolveEventSummary(post, eventLookup);
            var mediaUrls = mediaUrlsByPost.GetValueOrDefault(post.Id, Array.Empty<string>());

            const int maxInt32 = int.MaxValue;
            var mediaCount = mediaUrls.Count > maxInt32 ? maxInt32 : mediaUrls.Count;
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
                post.CreatedAtUtc,
                mediaUrls.Count > 0 ? mediaUrls : null,
                mediaCount));
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
        var mediaUrlsByPost = await LoadMediaUrlsByPostIdsAsync(postIds, cancellationToken);
        
        const int maxInt32 = int.MaxValue;
        var items = new List<FeedItemResponse>();
        foreach (var post in pagedResult.Items)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new PostCounts(0, 0));
            var eventSummary = ResolveEventSummary(post, eventLookup);
            var mediaUrls = mediaUrlsByPost.GetValueOrDefault(post.Id, Array.Empty<string>());

            var mediaCount = mediaUrls.Count > maxInt32 ? maxInt32 : mediaUrls.Count;
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
                post.CreatedAtUtc,
                mediaUrls.Count > 0 ? mediaUrls : null,
                mediaCount));
        }

        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var response = new PagedResponse<FeedItemResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
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
        var mediaUrlsByPost = await LoadMediaUrlsByPostIdsAsync(postIds, cancellationToken);
        
        var response = new List<FeedItemResponse>();
        foreach (var post in posts)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new PostCounts(0, 0));
            var eventSummary = ResolveEventSummary(post, eventLookup);
            var mediaUrls = mediaUrlsByPost.GetValueOrDefault(post.Id, Array.Empty<string>());

            const int maxInt32 = int.MaxValue;
            var mediaCount = mediaUrls.Count > maxInt32 ? maxInt32 : mediaUrls.Count;
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
                post.CreatedAtUtc,
                mediaUrls.Count > 0 ? mediaUrls : null,
                mediaCount));
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
        var mediaUrlsByPost = await LoadMediaUrlsByPostIdsAsync(postIds, cancellationToken);
        
        const int maxInt32 = int.MaxValue;
        var items = new List<FeedItemResponse>();
        foreach (var post in pagedResult.Items)
        {
            var postCounts = counts.GetValueOrDefault(post.Id, new PostCounts(0, 0));
            var eventSummary = ResolveEventSummary(post, eventLookup);
            var mediaUrls = mediaUrlsByPost.GetValueOrDefault(post.Id, Array.Empty<string>());

            var mediaCount = mediaUrls.Count > maxInt32 ? maxInt32 : mediaUrls.Count;
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
                post.CreatedAtUtc,
                mediaUrls.Count > 0 ? mediaUrls : null,
                mediaCount));
        }

        var safeTotalCount = pagedResult.TotalCount > maxInt32 ? maxInt32 : pagedResult.TotalCount;
        var safeTotalPages = pagedResult.TotalPages > maxInt32 ? maxInt32 : pagedResult.TotalPages;
        var response = new PagedResponse<FeedItemResponse>(
            items,
            pagedResult.PageNumber,
            pagedResult.PageSize,
            safeTotalCount,
            safeTotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Cria um post comunitário no território ativo.
    /// </summary>
    [HttpPost]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(FeedItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
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
            request.MediaIds,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to create post." });
        }

        var post = result.Value;
        var mediaUrls = await LoadMediaUrlsForPostAsync(post.Id, cancellationToken);
        
        const int maxInt32 = int.MaxValue;
        var mediaCount = mediaUrls.Count > maxInt32 ? maxInt32 : mediaUrls.Count;
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
            post.CreatedAtUtc,
            mediaUrls.Count > 0 ? mediaUrls : null,
            mediaCount);

        return CreatedAtAction(nameof(GetFeed), new { }, response);
    }

    /// <summary>
    /// Curte um post do território ativo.
    /// </summary>
    [HttpPost("{postId:guid}/likes")]
    [EnableRateLimiting("write")]
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
    [EnableRateLimiting("write")]
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
    [EnableRateLimiting("write")]
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

    /// <summary>
    /// Edita um post do território ativo.
    /// </summary>
    /// <remarks>
    /// Apenas o autor do post pode editá-lo. Permite editar título, conteúdo, mídias e localização.
    /// </remarks>
    [HttpPatch("{postId:guid}")]
    [EnableRateLimiting("write")]
    [ProducesResponseType(typeof(FeedItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<FeedItemResponse>> EditPost(
        [FromRoute] Guid postId,
        [FromQuery] Guid? territoryId,
        [FromBody] EditPostRequest request,
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

        // Converter GeoAnchors se fornecidos
        var geoAnchors = request.GeoAnchors?.Select(anchor => new Application.Models.GeoAnchorInput(
            anchor.Latitude,
            anchor.Longitude,
            anchor.Type)).ToList();

        var result = await _postEditService.EditPostAsync(
            postId,
            userContext.User.Id,
            request.Title,
            request.Content,
            request.MediaIds,
            geoAnchors,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(new { error = result.Error });
            }
            if (result.Error?.Contains("author", StringComparison.OrdinalIgnoreCase) == true)
            {
                return Unauthorized(new { error = result.Error });
            }
            return BadRequest(new { error = result.Error });
        }

        var post = result.Value;
        var mediaUrls = await LoadMediaUrlsForPostAsync(post.Id, cancellationToken);
        var postCounts = await _feedService.GetCountsByPostIdsAsync(new[] { post.Id }, cancellationToken);
        var counts = postCounts.GetValueOrDefault(post.Id, new PostCounts(0, 0));
        var eventLookup = await LoadEventSummariesAsync(new[] { post }, cancellationToken);
        var eventSummary = ResolveEventSummary(post, eventLookup);

        const int maxInt32 = int.MaxValue;
        var mediaCount = mediaUrls.Count > maxInt32 ? maxInt32 : mediaUrls.Count;
        var response = new FeedItemResponse(
            post.Id,
            post.Title,
            post.Content,
            post.Type.ToString().ToUpperInvariant(),
            post.Visibility.ToString().ToUpperInvariant(),
            post.Status.ToString().ToUpperInvariant(),
            post.MapEntityId,
            eventSummary,
            post.Type == PostType.Alert,
            counts.LikeCount,
            counts.ShareCount,
            post.CreatedAtUtc,
            mediaUrls.Count > 0 ? mediaUrls : null,
            mediaCount);

        return Ok(response);
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

    private async Task<IReadOnlyList<string>> LoadMediaUrlsForPostAsync(Guid postId, CancellationToken cancellationToken)
    {
        var mediaAssets = await _mediaService.ListMediaByOwnerAsync(MediaOwnerType.Post, postId, cancellationToken);
        var urls = new List<string>();

        foreach (var mediaAsset in mediaAssets)
        {
            var urlResult = await _mediaService.GetMediaUrlAsync(mediaAsset.Id, null, cancellationToken);
            if (urlResult.IsSuccess && urlResult.Value is not null)
            {
                urls.Add(urlResult.Value);
            }
        }

        return urls;
    }

    private async Task<Dictionary<Guid, IReadOnlyList<string>>> LoadMediaUrlsByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken)
    {
        if (postIds.Count == 0)
        {
            return new Dictionary<Guid, IReadOnlyList<string>>();
        }

        var result = new Dictionary<Guid, IReadOnlyList<string>>();

        // Buscar attachments em batch
        var attachmentsByPost = new Dictionary<Guid, List<Domain.Media.MediaAsset>>();
        
        foreach (var postId in postIds)
        {
            var mediaAssets = await _mediaService.ListMediaByOwnerAsync(MediaOwnerType.Post, postId, cancellationToken);
            if (mediaAssets.Count > 0)
            {
                attachmentsByPost[postId] = mediaAssets.ToList();
            }
        }

        // Buscar URLs para todas as mídias
        foreach (var (postId, mediaAssets) in attachmentsByPost)
        {
            var urls = new List<string>();
            foreach (var mediaAsset in mediaAssets)
            {
                var urlResult = await _mediaService.GetMediaUrlAsync(mediaAsset.Id, null, cancellationToken);
                if (urlResult.IsSuccess && urlResult.Value is not null)
                {
                    urls.Add(urlResult.Value);
                }
            }

            if (urls.Count > 0)
            {
                result[postId] = urls;
            }
        }

        return result;
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

    /// <summary>
    /// Deleta um post.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [EnableRateLimiting("feed")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(
        [FromRoute] Guid id,
        [FromQuery] Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var userContext = await _currentUserAccessor.GetAsync(Request, cancellationToken);
        if (userContext.Status != TokenStatus.Valid || userContext.User is null)
        {
            return Unauthorized();
        }

        var resolvedTerritoryId = await ResolveTerritoryIdAsync(territoryId, cancellationToken);
        if (resolvedTerritoryId is null)
        {
            return BadRequest(new { error = "territoryId (query) or X-Session-Id header is required." });
        }

        var post = await _feedService.GetPostAsync(id, cancellationToken);
        if (post is null)
        {
            return NotFound();
        }

        if (post.AuthorUserId != userContext.User.Id)
        {
            return BadRequest(new { error = "Only the post author can delete the post." });
        }

        // Deletar mídias associadas
        await _mediaAttachmentRepository.DeleteByOwnerAsync(MediaOwnerType.Post, id, cancellationToken);

        // Deletar geo anchors associados
        await _postGeoAnchorRepository.DeleteByPostIdAsync(id, cancellationToken);

        // Deletar o post
        await _feedService.DeletePostAsync(id, cancellationToken);

        return NoContent();
    }
}
