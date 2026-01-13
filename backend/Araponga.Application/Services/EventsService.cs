using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Events;
using Araponga.Domain.Feed;
using Araponga.Domain.Geo;
using Araponga.Domain.Social;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

public sealed class EventsService
{
    private readonly ITerritoryEventRepository _eventRepository;
    private readonly EventCacheService? _eventCache;
    private readonly IEventParticipationRepository _participationRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventsService(
        ITerritoryEventRepository eventRepository,
        IEventParticipationRepository participationRepository,
        IFeedRepository feedRepository,
        AccessEvaluator accessEvaluator,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        EventCacheService? eventCache = null)
    {
        _eventRepository = eventRepository;
        _participationRepository = participationRepository;
        _feedRepository = feedRepository;
        _accessEvaluator = accessEvaluator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _eventCache = eventCache;
    }

    public async Task<Result<EventSummary>> CreateEventAsync(
        Guid territoryId,
        Guid userId,
        string title,
        string? description,
        DateTime startsAtUtc,
        DateTime? endsAtUtc,
        double latitude,
        double longitude,
        string? locationLabel,
        CancellationToken cancellationToken)
    {
        if (territoryId == Guid.Empty)
        {
            return Result<EventSummary>.Failure("Territory ID is required.");
        }

        if (userId == Guid.Empty)
        {
            return Result<EventSummary>.Failure("User ID is required.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<EventSummary>.Failure("Title is required.");
        }

        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            return Result<EventSummary>.Failure("Invalid latitude/longitude.");
        }

        if (endsAtUtc is not null && endsAtUtc.Value < startsAtUtc)
        {
            return Result<EventSummary>.Failure("EndsAtUtc must be after StartsAtUtc.");
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        var membership = isResident ? MembershipRole.Resident : MembershipRole.Visitor;
        var now = DateTime.UtcNow;

        var territoryEvent = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            title,
            description,
            startsAtUtc,
            endsAtUtc,
            latitude,
            longitude,
            locationLabel,
            userId,
            membership,
            EventStatus.Scheduled,
            now,
            now);

        await _eventRepository.AddAsync(territoryEvent, cancellationToken);

        var postContent = string.IsNullOrWhiteSpace(description) ? title.Trim() : description.Trim();
        var post = new CommunityPost(
            Guid.NewGuid(),
            territoryId,
            userId,
            title,
            postContent,
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            now,
            "EVENT",
            territoryEvent.Id);

        await _feedRepository.AddPostAsync(post, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidate cache when event is created
        _eventCache?.InvalidateTerritoryEvents(territoryId);

        var displayName = await ResolveDisplayNameAsync(userId, cancellationToken);
        var summary = new EventSummary(territoryEvent, 0, 0, displayName);

        return Result<EventSummary>.Success(summary);
    }

    public async Task<Result<EventSummary>> UpdateEventAsync(
        Guid eventId,
        Guid actorUserId,
        string? title,
        string? description,
        DateTime? startsAtUtc,
        DateTime? endsAtUtc,
        double? latitude,
        double? longitude,
        string? locationLabel,
        CancellationToken cancellationToken)
    {
        var territoryEvent = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (territoryEvent is null)
        {
            return Result<EventSummary>.Failure("Event not found.");
        }

        var canManage = await CanManageEventAsync(territoryEvent, actorUserId, cancellationToken);
        if (!canManage)
        {
            return Result<EventSummary>.Failure("User is not allowed to update this event.");
        }

        var updatedTitle = title ?? territoryEvent.Title;
        var updatedDescription = description ?? territoryEvent.Description;
        var updatedStartsAt = startsAtUtc ?? territoryEvent.StartsAtUtc;
        var updatedEndsAt = endsAtUtc ?? territoryEvent.EndsAtUtc;
        var updatedLatitude = latitude ?? territoryEvent.Latitude;
        var updatedLongitude = longitude ?? territoryEvent.Longitude;
        var updatedLocationLabel = locationLabel ?? territoryEvent.LocationLabel;

        territoryEvent.Update(
            updatedTitle,
            updatedDescription,
            updatedStartsAt,
            updatedEndsAt,
            updatedLatitude,
            updatedLongitude,
            updatedLocationLabel,
            DateTime.UtcNow);

        await _eventRepository.UpdateAsync(territoryEvent, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidate cache when event is updated
        _eventCache?.InvalidateTerritoryEvents(territoryEvent.TerritoryId);

        var summaries = await BuildSummariesAsync(new List<TerritoryEvent> { territoryEvent }, cancellationToken);
        var summary = summaries.FirstOrDefault();
        if (summary is null)
        {
            return Result<EventSummary>.Failure("Unable to build event summary.");
        }
        return Result<EventSummary>.Success(summary);
    }

    public async Task<OperationResult> CancelEventAsync(
        Guid eventId,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        var territoryEvent = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (territoryEvent is null)
        {
            return OperationResult.Failure("Event not found.");
        }

        var canManage = await CanManageEventAsync(territoryEvent, actorUserId, cancellationToken);
        if (!canManage)
        {
            return OperationResult.Failure("User is not allowed to cancel this event.");
        }

        territoryEvent.Cancel(DateTime.UtcNow);
        await _eventRepository.UpdateAsync(territoryEvent, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    public async Task<OperationResult> SetParticipationAsync(
        Guid eventId,
        Guid userId,
        EventParticipationStatus status,
        CancellationToken cancellationToken)
    {
        var territoryEvent = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (territoryEvent is null)
        {
            return OperationResult.Failure("Event not found.");
        }

        var existing = await _participationRepository.GetAsync(eventId, userId, cancellationToken);
        var now = DateTime.UtcNow;

        if (existing is null)
        {
            var participation = new EventParticipation(eventId, userId, status, now, now);
            await _participationRepository.UpsertAsync(participation, cancellationToken);
        }
        else
        {
            existing.UpdateStatus(status, now);
            await _participationRepository.UpsertAsync(existing, cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        return OperationResult.Success();
    }

    public async Task<IReadOnlyList<EventSummary>> ListEventsAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        EventStatus? status,
        CancellationToken cancellationToken)
    {
        var events = _eventCache is not null
            ? await _eventCache.GetEventsByTerritoryAsync(territoryId, fromUtc, toUtc, status, cancellationToken)
            : await _eventRepository.ListByTerritoryAsync(territoryId, fromUtc, toUtc, status, cancellationToken);
        return await BuildSummariesAsync(events, cancellationToken);
    }

    public async Task<IReadOnlyList<EventSummary>> GetEventsNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            return Array.Empty<EventSummary>();
        }

        var latDelta = radiusKm / 110.574;
        var cosLat = Math.Cos(latitude * Math.PI / 180.0);
        var safeCos = Math.Abs(cosLat) < 0.000001 ? 0.000001 : cosLat;
        var lngDelta = radiusKm / (111.320 * safeCos);

        var minLat = latitude - latDelta;
        var maxLat = latitude + latDelta;
        var minLng = longitude - lngDelta;
        var maxLng = longitude + lngDelta;

        var candidates = await _eventRepository.ListByBoundingBoxAsync(
            minLat,
            maxLat,
            minLng,
            maxLng,
            fromUtc,
            toUtc,
            territoryId,
            cancellationToken);

        var filtered = candidates
            .Select(evt => new
            {
                Event = evt,
                Distance = CalculateDistance(latitude, longitude, evt.Latitude, evt.Longitude)
            })
            .Where(item => item.Distance <= radiusKm)
            .OrderBy(item => item.Distance)
            .ThenBy(item => item.Event.StartsAtUtc)
            .Select(item => item.Event)
            .ToList();

        return await BuildSummariesAsync(filtered, cancellationToken);
    }

    public async Task<PagedResult<EventSummary>> ListEventsPagedAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        EventStatus? status,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var events = _eventCache is not null
            ? await _eventCache.GetEventsByTerritoryAsync(territoryId, fromUtc, toUtc, status, cancellationToken)
            : await _eventRepository.ListByTerritoryAsync(territoryId, fromUtc, toUtc, status, cancellationToken);
        var summaries = await BuildSummariesAsync(events, cancellationToken);

        var totalCount = summaries.Count;
        var pagedItems = summaries
            .OrderByDescending(s => s.Event.StartsAtUtc)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new PagedResult<EventSummary>(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public async Task<PagedResult<EventSummary>> GetEventsNearbyPagedAsync(
        double latitude,
        double longitude,
        double radiusKm,
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? territoryId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            return new PagedResult<EventSummary>(Array.Empty<EventSummary>(), pagination.PageNumber, pagination.PageSize, 0);
        }

        var latDelta = radiusKm / 110.574;
        var cosLat = Math.Cos(latitude * Math.PI / 180.0);
        var safeCos = Math.Abs(cosLat) < 0.000001 ? 0.000001 : cosLat;
        var lngDelta = radiusKm / (111.320 * safeCos);

        var minLat = latitude - latDelta;
        var maxLat = latitude + latDelta;
        var minLng = longitude - lngDelta;
        var maxLng = longitude + lngDelta;

        var candidates = await _eventRepository.ListByBoundingBoxAsync(
            minLat,
            maxLat,
            minLng,
            maxLng,
            fromUtc,
            toUtc,
            territoryId,
            cancellationToken);

        var filtered = candidates
            .Select(evt => new
            {
                Event = evt,
                Distance = CalculateDistance(latitude, longitude, evt.Latitude, evt.Longitude)
            })
            .Where(item => item.Distance <= radiusKm)
            .OrderBy(item => item.Distance)
            .ThenBy(item => item.Event.StartsAtUtc)
            .Select(item => item.Event)
            .ToList();

        var summaries = await BuildSummariesAsync(filtered, cancellationToken);
        var totalCount = summaries.Count;
        var pagedItems = summaries
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new PagedResult<EventSummary>(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public async Task<IReadOnlyDictionary<Guid, EventSummary>> GetSummariesByIdsAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken)
    {
        if (eventIds.Count == 0)
        {
            return new Dictionary<Guid, EventSummary>();
        }

        var events = await _eventRepository.ListByIdsAsync(eventIds, cancellationToken);
        var summaries = await BuildSummariesAsync(events, cancellationToken);
        return summaries.ToDictionary(summary => summary.Event.Id, summary => summary);
    }

    private async Task<IReadOnlyList<EventSummary>> BuildSummariesAsync(
        IReadOnlyList<TerritoryEvent> events,
        CancellationToken cancellationToken)
    {
        if (events.Count == 0)
        {
            return Array.Empty<EventSummary>();
        }

        var eventIds = events.Select(evt => evt.Id).ToList();
        var counts = await _participationRepository.GetCountsAsync(eventIds, cancellationToken);
        var countLookup = counts.ToDictionary(count => count.EventId, count => count);

        var userIds = events.Select(evt => evt.CreatedByUserId).Distinct().ToList();
        var displayNameLookup = new Dictionary<Guid, string>();

        foreach (var userId in userIds)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is not null)
            {
                displayNameLookup[userId] = user.DisplayName;
            }
        }

        return events.Select(evt =>
        {
            countLookup.TryGetValue(evt.Id, out var participation);
            displayNameLookup.TryGetValue(evt.CreatedByUserId, out var displayName);

            return new EventSummary(
                evt,
                participation?.InterestedCount ?? 0,
                participation?.ConfirmedCount ?? 0,
                displayName);
        }).ToList();
    }

    private async Task<bool> CanManageEventAsync(
        TerritoryEvent territoryEvent,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        if (territoryEvent.CreatedByUserId == actorUserId)
        {
            return true;
        }

        var user = await _userRepository.GetByIdAsync(actorUserId, cancellationToken);
        return user is not null && user.Role == UserRole.Curator;
    }

    private async Task<string?> ResolveDisplayNameAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user?.DisplayName;
    }

    private static double CalculateDistance(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        const double Radius = 6371.0;
        var lat1 = DegreesToRadians(latitude1);
        var lat2 = DegreesToRadians(latitude2);
        var deltaLat = DegreesToRadians(latitude2 - latitude1);
        var deltaLon = DegreesToRadians(longitude2 - longitude1);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Radius * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
