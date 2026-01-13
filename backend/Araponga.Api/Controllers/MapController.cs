using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Map;
using Araponga.Api.Security;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Assets;
using Araponga.Domain.Feed;
using Araponga.Domain.Map;
using Microsoft.AspNetCore.Mvc;

namespace Araponga.Api.Controllers;

[ApiController]
[Route("api/v1/map")]
[Produces("application/json")]
[Tags("Map")]
public sealed class MapController : ControllerBase
{
    private readonly MapService _mapService;
    private readonly FeedService _feedService;
    private readonly EventsService _eventsService;
    private readonly IPostGeoAnchorRepository _postGeoAnchorRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetGeoAnchorRepository _assetGeoAnchorRepository;
    private readonly CurrentUserAccessor _currentUserAccessor;
    private readonly ActiveTerritoryService _activeTerritoryService;
    private readonly AccessEvaluator _accessEvaluator;

    public MapController(
        MapService mapService,
        FeedService feedService,
        EventsService eventsService,
        IPostGeoAnchorRepository postGeoAnchorRepository,
        IAssetRepository assetRepository,
        IAssetGeoAnchorRepository assetGeoAnchorRepository,
        CurrentUserAccessor currentUserAccessor,
        ActiveTerritoryService activeTerritoryService,
        AccessEvaluator accessEvaluator)
    {
        _mapService = mapService;
        _feedService = feedService;
        _eventsService = eventsService;
        _postGeoAnchorRepository = postGeoAnchorRepository;
        _assetRepository = assetRepository;
        _assetGeoAnchorRepository = assetGeoAnchorRepository;
        _currentUserAccessor = currentUserAccessor;
        _activeTerritoryService = activeTerritoryService;
        _accessEvaluator = accessEvaluator;
    }

    /// <summary>
    /// Visualiza entidades do mapa no território ativo.
    /// </summary>
    /// <remarks>
    /// Visitantes veem apenas entidades públicas; moradores veem todas.
    /// </remarks>
    [HttpGet("entities")]
    [ProducesResponseType(typeof(IEnumerable<MapEntityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<MapEntityResponse>>> GetEntities(
        [FromQuery] Guid? territoryId,
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

        var entities = await _mapService.ListEntitiesAsync(
            resolvedTerritoryId.Value,
            userContext.User?.Id,
            cancellationToken);

        var response = entities.Select(entity =>
            new MapEntityResponse(
                entity.Id,
                entity.Name,
                entity.Category,
                entity.Latitude,
                entity.Longitude,
                entity.Status.ToString().ToUpperInvariant(),
                entity.Visibility.ToString().ToUpperInvariant(),
                entity.ConfirmationCount,
                entity.CreatedAtUtc));

        return Ok(response);
    }

    /// <summary>
    /// Lista entidades do mapa do território (paginado).
    /// </summary>
    [HttpGet("entities/paged")]
    [ProducesResponseType(typeof(PagedResponse<MapEntityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<MapEntityResponse>>> GetEntitiesPaged(
        [FromQuery] Guid? territoryId,
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
        var pagedResult = await _mapService.ListEntitiesPagedAsync(
            resolvedTerritoryId.Value,
            userContext.User?.Id,
            pagination,
            cancellationToken);

        var response = new PagedResponse<MapEntityResponse>(
            pagedResult.Items.Select(entity => new MapEntityResponse(
                entity.Id,
                entity.Name,
                entity.Category,
                entity.Latitude,
                entity.Longitude,
                entity.Status.ToString().ToUpperInvariant(),
                entity.Visibility.ToString().ToUpperInvariant(),
                entity.ConfirmationCount,
                entity.CreatedAtUtc)).ToList(),
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalCount,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage);

        return Ok(response);
    }

    /// <summary>
    /// Sugere uma nova entidade no mapa.
    /// </summary>
    [HttpPost("entities")]
    [ProducesResponseType(typeof(MapEntityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MapEntityResponse>> SuggestEntity(
        [FromQuery] Guid? territoryId,
        [FromBody] SuggestMapEntityRequest request,
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

        var result = await _mapService.SuggestAsync(
            resolvedTerritoryId.Value,
            userContext.User.Id,
            request.Name,
            request.Category,
            request.Latitude,
            request.Longitude,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error ?? "Unable to suggest entity." });
        }

        var response = new MapEntityResponse(
            result.Value.Id,
            result.Value.Name,
            result.Value.Category,
            result.Value.Latitude,
            result.Value.Longitude,
            result.Value.Status.ToString().ToUpperInvariant(),
            result.Value.Visibility.ToString().ToUpperInvariant(),
            result.Value.ConfirmationCount,
            result.Value.CreatedAtUtc);

        return CreatedAtAction(nameof(GetEntities), new { }, response);
    }

    /// <summary>
    /// Lista pins (entidades + posts) para o mapa do território.
    /// </summary>
    [HttpGet("pins")]
    [ProducesResponseType(typeof(IEnumerable<MapPinResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<MapPinResponse>>> GetPins(
        [FromQuery] Guid? territoryId,
        [FromQuery] string? types,
        [FromQuery] Guid? assetId,
        [FromQuery] string? assetTypes,
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

        var typeSet = ParseTypes(types);
        var includeEntities = typeSet.Contains("entity");
        var includeAssets = typeSet.Contains("asset");
        var includeAlerts = typeSet.Contains("alert");
        var includePosts = typeSet.Contains("post");
        var includeMedia = typeSet.Contains("media");
        var includeEvents = typeSet.Contains("event");

        var pins = new List<MapPinResponse>();

        if (includeEntities)
        {
            var entities = await _mapService.ListEntitiesAsync(
                resolvedTerritoryId.Value,
                userContext.User?.Id,
                cancellationToken);

            pins.AddRange(entities.Select(entity => new MapPinResponse(
                "entity",
                entity.Latitude,
                entity.Longitude,
                entity.Name,
                null,
                null,
                null,
                null,
                entity.Id,
                entity.Status.ToString().ToUpperInvariant())));
        }

        if (includeAssets)
        {
            var assetTypeList = assetId is null ? ParseCsv(assetTypes) : null;
            var assets = await _assetRepository.ListAsync(
                resolvedTerritoryId.Value,
                assetId,
                assetTypeList,
                AssetStatus.Active,
                null,
                cancellationToken);

            if (assets.Count > 0)
            {
                var assetIds = assets.Select(asset => asset.Id).ToList();
                var anchors = await _assetGeoAnchorRepository.ListByAssetIdsAsync(assetIds, cancellationToken);
                var assetLookup = assets.ToDictionary(asset => asset.Id, asset => asset);

                pins.AddRange(anchors.Select(anchor =>
                {
                    var asset = assetLookup[anchor.AssetId];
                    return new MapPinResponse(
                        "asset",
                        anchor.Latitude,
                        anchor.Longitude,
                        asset.Name,
                        asset.Id,
                        null,
                        null,
                        null,
                        null,
                        asset.Status.ToString().ToUpperInvariant());
                }));
            }
        }

        if (includePosts || includeAlerts || includeMedia)
        {
            var posts = await _feedService.ListForTerritoryAsync(
                resolvedTerritoryId.Value,
                userContext.User?.Id,
                null,
                null,
                cancellationToken);

            var postIds = posts.Select(post => post.Id).ToList();
            var anchors = await _postGeoAnchorRepository.ListByPostIdsAsync(postIds, cancellationToken);
            var postLookup = posts.ToDictionary(post => post.Id, post => post);

            foreach (var anchor in anchors.Where(anchor => postLookup.ContainsKey(anchor.PostId)))
            {
                var post = postLookup[anchor.PostId];
                var pinType = ResolvePostPinType(post, anchor.Type, includeMedia);

                if (pinType == "alert" && !includeAlerts)
                {
                    continue;
                }

                if (pinType == "post" && !includePosts)
                {
                    continue;
                }

                if (pinType == "media" && !includeMedia)
                {
                    continue;
                }

                pins.Add(new MapPinResponse(
                    pinType,
                    anchor.Latitude,
                    anchor.Longitude,
                    post.Title,
                    null,
                    pinType is "post" or "alert" ? post.Id : null,
                    pinType == "media" ? post.Id : null,
                    null,
                    null,
                    post.Status.ToString().ToUpperInvariant()));
            }
        }

        if (includeEvents)
        {
            var events = await _eventsService.ListEventsAsync(
                resolvedTerritoryId.Value,
                null,
                null,
                null,
                cancellationToken);

            pins.AddRange(events.Select(summary => new MapPinResponse(
                "event",
                summary.Event.Latitude,
                summary.Event.Longitude,
                summary.Event.Title,
                null,
                null,
                null,
                summary.Event.Id,
                null,
                summary.Event.Status.ToString().ToUpperInvariant())));
        }

        return Ok(pins);
    }

    private static string ResolvePostPinType(CommunityPost post, string anchorType, bool includeMedia)
    {
        if (includeMedia && anchorType.Equals("MEDIA", StringComparison.OrdinalIgnoreCase))
        {
            return "media";
        }

        return post.Type == PostType.Alert ? "alert" : "post";
    }

    private static IReadOnlyCollection<string> ParseTypes(string? types)
    {
        if (string.IsNullOrWhiteSpace(types))
        {
            return new HashSet<string>(new[] { "post", "media", "entity", "alert", "asset", "event" });
        }

        return new HashSet<string>(
            types.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => value.ToLowerInvariant()));
    }

    private static IReadOnlyCollection<string>? ParseCsv(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => value.ToLowerInvariant())
            .ToList();
    }

    /// <summary>
    /// Valida uma entidade sugerida (curadoria).
    /// </summary>
    [HttpPatch("entities/{entityId:guid}/validation")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ValidateEntity(
        [FromRoute] Guid entityId,
        [FromQuery] Guid? territoryId,
        [FromBody] ValidateMapEntityRequest request,
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

        if (!_accessEvaluator.IsCurator(userContext.User))
        {
            return Unauthorized();
        }

        if (!Enum.TryParse<MapEntityStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { error = "Invalid status." });
        }

        var success = await _mapService.ValidateAsync(
            resolvedTerritoryId.Value,
            entityId,
            userContext.User.Id,
            status,
            cancellationToken);

        return success ? NoContent() : BadRequest(new { error = "Entity not found." });
    }

    /// <summary>
    /// Confirma uma entidade existente.
    /// </summary>
    [HttpPost("entities/{entityId:guid}/confirmations")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmEntity(
        [FromRoute] Guid entityId,
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

        var result = await _mapService.ConfirmAsync(
            resolvedTerritoryId.Value,
            entityId,
            userContext.User.Id,
            cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
    }

    /// <summary>
    /// Relaciona um morador a uma entidade do território.
    /// </summary>
    [HttpPost("entities/{entityId:guid}/relations")]
    [ProducesResponseType(typeof(MapEntityRelationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MapEntityRelationResponse>> RelateEntity(
        [FromRoute] Guid entityId,
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

        var result = await _mapService.RelateAsync(
            resolvedTerritoryId.Value,
            entityId,
            userContext.User.Id,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error is null
                ? Ok(new MapEntityRelationResponse(userContext.User.Id, entityId, DateTime.UtcNow))
                : BadRequest(new { error = result.Error });
        }

        var response = new MapEntityRelationResponse(
            result.Value!.UserId,
            result.Value.EntityId,
            result.Value.CreatedAtUtc);

        return CreatedAtAction(nameof(RelateEntity), new { entityId }, response);
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
