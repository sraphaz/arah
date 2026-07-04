using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Application.Models;
using Arah.Modules.Assets.Application.Interfaces;
using Arah.Modules.Assets.Domain;
using Arah.Domain.Feed;
using Arah.Modules.Map.Domain;

namespace Arah.Application.Services;

/// <summary>
/// Orquestra a montagem dos pins do mapa (entidades, assets, posts, mídia, alertas, eventos)
/// de um território. Mantém a lógica fora dos controllers (Clean Architecture).
/// </summary>
public sealed class MapPinsService
{
    private readonly MapService _mapService;
    private readonly FeedService _feedService;
    private readonly EventsService _eventsService;
    private readonly TerritoryAssetService _assetService;
    private readonly ITerritoryAssetRepository _assetRepository;
    private readonly IAssetGeoAnchorRepository _assetGeoAnchorRepository;
    private readonly IPostGeoAnchorRepository _postGeoAnchorRepository;

    public MapPinsService(
        MapService mapService,
        FeedService feedService,
        EventsService eventsService,
        TerritoryAssetService assetService,
        ITerritoryAssetRepository assetRepository,
        IAssetGeoAnchorRepository assetGeoAnchorRepository,
        IPostGeoAnchorRepository postGeoAnchorRepository)
    {
        _mapService = mapService;
        _feedService = feedService;
        _eventsService = eventsService;
        _assetService = assetService;
        _assetRepository = assetRepository;
        _assetGeoAnchorRepository = assetGeoAnchorRepository;
        _postGeoAnchorRepository = postGeoAnchorRepository;
    }

    public async Task<IReadOnlyList<MapPin>> ListPinsAsync(
        Guid territoryId,
        Guid? userId,
        MapPinFilters filters,
        Guid? assetId,
        IReadOnlyList<string>? assetTypes,
        CancellationToken cancellationToken)
    {
        var pins = new List<MapPin>();

        if (filters.Entities)
        {
            var entities = await _mapService.ListEntitiesAsync(
                territoryId,
                userId,
                cancellationToken);

            pins.AddRange(BuildEntityPins(entities));
        }

        if (filters.Assets)
        {
            var assetTypeList = assetId is null ? assetTypes : null;
            var assets = await _assetRepository.ListAsync(
                territoryId,
                assetId,
                assetTypeList,
                AssetStatus.Active,
                null,
                cancellationToken);

            pins.AddRange(await BuildAssetPinsAsync(assets, cancellationToken));
        }

        if (filters.Posts || filters.Alerts || filters.Media)
        {
            var posts = await _feedService.ListForTerritoryAsync(
                territoryId,
                userId,
                null,
                null,
                filterByInterests: false,
                prioritizeConnections: false,
                cancellationToken);

            pins.AddRange(await BuildPostPinsAsync(posts, filters, cancellationToken));
        }

        if (filters.Events)
        {
            var events = await _eventsService.ListEventsAsync(
                territoryId,
                null,
                null,
                null,
                cancellationToken);

            pins.AddRange(BuildEventPins(events));
        }

        return pins;
    }

    public async Task<PagedResult<MapPin>> ListPinsPagedAsync(
        Guid territoryId,
        Guid? userId,
        MapPinFilters filters,
        Guid? assetId,
        IReadOnlyList<string>? assetTypes,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var allPins = await ListPinsAsync(territoryId, userId, filters, assetId, assetTypes, cancellationToken);
        var ordered = allPins.OrderBy(p => p.PinType).ToList();
        var pagedPins = ordered.Skip(pagination.Skip).Take(pagination.Take).ToList();

        return new PagedResult<MapPin>(pagedPins, pagination.PageNumber, pagination.PageSize, ordered.Count);
    }

    private static List<MapPin> BuildEntityPins(IEnumerable<MapEntity> entities)
    {
        return entities.Select(entity => new MapPin(
            "entity",
            entity.Latitude,
            entity.Longitude,
            entity.Name,
            null,
            null,
            null,
            null,
            entity.Id,
            entity.Status.ToString().ToUpperInvariant())).ToList();
    }

    private async Task<List<MapPin>> BuildAssetPinsAsync(
        IReadOnlyList<TerritoryAsset> assets,
        CancellationToken cancellationToken)
    {
        if (assets.Count == 0)
        {
            return new List<MapPin>();
        }

        var assetIds = assets.Select(asset => asset.Id).ToList();
        var anchors = await _assetGeoAnchorRepository.ListByAssetIdsAsync(assetIds, cancellationToken);
        var assetLookup = assets.ToDictionary(asset => asset.Id, asset => asset);

        return anchors.Select(anchor =>
        {
            var asset = assetLookup[anchor.AssetId];
            return new MapPin(
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
        }).ToList();
    }

    private async Task<List<MapPin>> BuildPostPinsAsync(
        IReadOnlyList<CommunityPost> posts,
        MapPinFilters filters,
        CancellationToken cancellationToken)
    {
        var pins = new List<MapPin>();
        var postIds = posts.Select(post => post.Id).ToList();
        var anchors = await _postGeoAnchorRepository.ListByPostIdsAsync(postIds, cancellationToken);
        var postLookup = posts.ToDictionary(post => post.Id, post => post);

        foreach (var anchor in anchors.Where(anchor => postLookup.ContainsKey(anchor.PostId)))
        {
            var post = postLookup[anchor.PostId];
            var pinType = ResolvePostPinType(post, anchor.Type, filters.Media);

            if (pinType == "alert" && !filters.Alerts)
            {
                continue;
            }

            if (pinType == "post" && !filters.Posts)
            {
                continue;
            }

            if (pinType == "media" && !filters.Media)
            {
                continue;
            }

            pins.Add(new MapPin(
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

        return pins;
    }

    private static List<MapPin> BuildEventPins(IEnumerable<EventSummary> events)
    {
        return events.Select(summary => new MapPin(
            "event",
            summary.Event.Latitude,
            summary.Event.Longitude,
            summary.Event.Title,
            null,
            null,
            null,
            summary.Event.Id,
            null,
            summary.Event.Status.ToString().ToUpperInvariant())).ToList();
    }

    private static string ResolvePostPinType(CommunityPost post, string anchorType, bool includeMedia)
    {
        if (includeMedia && anchorType.Equals("MEDIA", StringComparison.OrdinalIgnoreCase))
        {
            return "media";
        }

        return post.Type == PostType.Alert ? "alert" : "post";
    }
}
