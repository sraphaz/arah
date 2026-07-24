using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Application.Interfaces.Media;
using Arah.Application.Metrics;
using Arah.Application.Models;
using Arah.Application.Services;
using Arah.Application.Services.Media;
using Arah.Domain.Events;
using Arah.Domain.Feed;
using Arah.Domain.Geo;
using Arah.Domain.Media;
using Arah.Domain.Membership;
using Arah.Domain.Users;

namespace Arah.Application.Services;

public sealed class EventsService
{
    private readonly ITerritoryEventRepository _eventRepository;
    private readonly EventCacheService? _eventCache;
    private readonly IEventParticipationRepository _participationRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly IMediaAssetRepository _mediaAssetRepository;
    private readonly IMediaAttachmentRepository _mediaAttachmentRepository;
    private readonly TerritoryMediaConfigService _mediaConfigService;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CacheInvalidationService? _cacheInvalidation;

    public EventsService(
        ITerritoryEventRepository eventRepository,
        IEventParticipationRepository participationRepository,
        IFeedRepository feedRepository,
        IMediaAssetRepository mediaAssetRepository,
        IMediaAttachmentRepository mediaAttachmentRepository,
        TerritoryMediaConfigService mediaConfigService,
        AccessEvaluator accessEvaluator,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        EventCacheService? eventCache = null,
        CacheInvalidationService? cacheInvalidation = null)
    {
        _eventRepository = eventRepository;
        _participationRepository = participationRepository;
        _feedRepository = feedRepository;
        _mediaAssetRepository = mediaAssetRepository;
        _mediaAttachmentRepository = mediaAttachmentRepository;
        _mediaConfigService = mediaConfigService;
        _accessEvaluator = accessEvaluator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _eventCache = eventCache;
        _cacheInvalidation = cacheInvalidation;
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
        Guid? coverMediaId,
        IReadOnlyCollection<Guid>? additionalMediaIds,
        CancellationToken cancellationToken)
    {
        var identifierError = ValidateEventIdentifiers(territoryId, userId);
        if (identifierError is not null)
        {
            return Result<EventSummary>.Failure(identifierError);
        }

        var policyError = await ValidateRequiredPoliciesAsync(userId, cancellationToken);
        if (policyError is not null)
        {
            return Result<EventSummary>.Failure(policyError);
        }

        var detailsError = ValidateEventDetails(title, latitude, longitude, startsAtUtc, endsAtUtc);
        if (detailsError is not null)
        {
            return Result<EventSummary>.Failure(detailsError);
        }

        var (allMediaIds, normalizedAdditionalMediaIds) = CollectMediaIds(coverMediaId, additionalMediaIds);

        if (allMediaIds.Count > 0)
        {
            var mediaError = await ValidateEventMediaAsync(
                territoryId,
                userId,
                allMediaIds,
                normalizedAdditionalMediaIds,
                cancellationToken);
            if (mediaError is not null)
            {
                return Result<EventSummary>.Failure(mediaError);
            }
        }

        var membership = await ResolveMembershipAsync(userId, territoryId, cancellationToken);
        var now = DateTime.UtcNow;

        var territoryEvent = BuildScheduledEvent(
            territoryId,
            userId,
            title,
            description,
            startsAtUtc,
            endsAtUtc,
            latitude,
            longitude,
            locationLabel,
            membership,
            now);

        await _eventRepository.AddAsync(territoryEvent, cancellationToken);

        await PersistEventMediaAttachmentsAsync(
            territoryEvent.Id,
            coverMediaId,
            normalizedAdditionalMediaIds,
            allMediaIds,
            now,
            cancellationToken);

        await PublishEventPostAsync(territoryEvent, territoryId, userId, title, description, now, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        RegisterEventCreatedSideEffects(territoryId, territoryEvent.Id);

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
        Guid? coverMediaId,
        IReadOnlyCollection<Guid>? additionalMediaIds,
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

        ApplyEventUpdates(
            territoryEvent,
            title,
            description,
            startsAtUtc,
            endsAtUtc,
            latitude,
            longitude,
            locationLabel);

        // Atualizar mídias se fornecidas
        if (coverMediaId is not null || additionalMediaIds is not null)
        {
            await ReplaceEventMediaAttachmentsAsync(
                territoryEvent.Id,
                coverMediaId,
                additionalMediaIds,
                cancellationToken);
        }

        await _eventRepository.UpdateAsync(territoryEvent, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        InvalidateEventCaches(territoryEvent.TerritoryId, territoryEvent.Id);

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

        // Deletar mídias associadas ao evento quando cancelado
        await _mediaAttachmentRepository.DeleteByOwnerAsync(MediaOwnerType.Event, territoryEvent.Id, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidar cache de eventos do território
        _cacheInvalidation?.InvalidateEventCache(territoryEvent.TerritoryId, territoryEvent.Id);

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
        // Cache não é usado em métodos paginados pois a paginação no repositório é mais eficiente
        var totalCount = await _eventRepository.CountByTerritoryAsync(territoryId, fromUtc, toUtc, status, cancellationToken);
        var eventsPaged = await _eventRepository.ListByTerritoryPagedAsync(territoryId, fromUtc, toUtc, status, pagination.Skip, pagination.Take, cancellationToken);
        var summariesPaged = await BuildSummariesAsync(eventsPaged, cancellationToken);

        return new PagedResult<EventSummary>(summariesPaged, pagination.PageNumber, pagination.PageSize, totalCount);
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

        // Para nearby, ainda precisamos filtrar por distância em memória
        // então vamos buscar mais resultados e filtrar depois
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

        const int maxInt32 = int.MaxValue;
        var count = filtered.Count;
        var totalCount = count > maxInt32 ? maxInt32 : count;
        var pagedItems = filtered
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        var summaries = await BuildSummariesAsync(pagedItems, cancellationToken);
        return new PagedResult<EventSummary>(summaries, pagination.PageNumber, pagination.PageSize, totalCount);
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
        var users = await _userRepository.ListByIdsAsync(userIds, cancellationToken);
        var displayNameLookup = users.ToDictionary(u => u.Id, u => u.DisplayName);

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

    public async Task<Result<IReadOnlyList<EventParticipant>>> GetEventParticipantsAsync(
        Guid eventId,
        EventParticipationStatus? status,
        CancellationToken cancellationToken)
    {
        var territoryEvent = await _eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (territoryEvent is null)
        {
            return Result<IReadOnlyList<EventParticipant>>.Failure("Event not found.");
        }

        var participations = await _participationRepository.ListByEventIdAsync(eventId, status, cancellationToken);

        var participants = new List<EventParticipant>();
        foreach (var participation in participations)
        {
            var displayName = await ResolveDisplayNameAsync(participation.UserId, cancellationToken);
            participants.Add(new EventParticipant(
                participation.UserId,
                displayName ?? "Unknown",
                participation.Status,
                participation.CreatedAtUtc,
                participation.UpdatedAtUtc));
        }

        return Result<IReadOnlyList<EventParticipant>>.Success(participants);
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

        // Obter territoryId do evento
        var territoryId = territoryEvent.TerritoryId;

        // Verificar se tem capacidade de Curator no território
        return await _accessEvaluator.HasCapabilityAsync(
            actorUserId,
            territoryId,
            MembershipCapabilityType.Curator,
            cancellationToken);
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
        var lat1 = DegreesToRadians(latitude1);
        var lat2 = DegreesToRadians(latitude2);
        var deltaLat = DegreesToRadians(latitude2 - latitude1);
        var deltaLon = DegreesToRadians(longitude2 - longitude1);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Constants.Geography.EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    private static string? ValidateEventIdentifiers(Guid territoryId, Guid userId)
    {
        if (territoryId == Guid.Empty)
        {
            return "Territory ID is required.";
        }

        if (userId == Guid.Empty)
        {
            return "User ID is required.";
        }

        return null;
    }

    private async Task<string?> ValidateRequiredPoliciesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var policiesResult = await _accessEvaluator.HasAcceptedRequiredPoliciesAsync(userId, cancellationToken);
        if (!policiesResult.IsFailure && policiesResult.Value)
        {
            return null;
        }

        var pendingPolicies = await _accessEvaluator.GetPendingPoliciesAsync(userId, cancellationToken);
        var errorMessage = "You must accept the required terms of service and privacy policies before creating events.";
        if (pendingPolicies is not null && !pendingPolicies.IsEmpty)
        {
            var pendingTermsCount = pendingPolicies.RequiredTerms.Count;
            var pendingPoliciesCount = pendingPolicies.RequiredPrivacyPolicies.Count;
            if (pendingTermsCount > 0 || pendingPoliciesCount > 0)
            {
                errorMessage = $"You must accept {pendingTermsCount + pendingPoliciesCount} required policy(ies) before creating events.";
            }
        }

        return errorMessage;
    }

    private static string? ValidateEventDetails(
        string title,
        double latitude,
        double longitude,
        DateTime startsAtUtc,
        DateTime? endsAtUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "Title is required.";
        }

        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            return "Invalid latitude/longitude.";
        }

        if (endsAtUtc is not null && endsAtUtc.Value < startsAtUtc)
        {
            return "EndsAtUtc must be after StartsAtUtc.";
        }

        return null;
    }

    private static (List<Guid> AllMediaIds, List<Guid>? NormalizedAdditionalMediaIds) CollectMediaIds(
        Guid? coverMediaId,
        IReadOnlyCollection<Guid>? additionalMediaIds)
    {
        var normalizedAdditionalMediaIds = additionalMediaIds?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var allMediaIds = new List<Guid>();
        if (coverMediaId.HasValue && coverMediaId.Value != Guid.Empty)
        {
            allMediaIds.Add(coverMediaId.Value);
        }
        if (normalizedAdditionalMediaIds is not null && normalizedAdditionalMediaIds.Count > 0)
        {
            allMediaIds.AddRange(normalizedAdditionalMediaIds);
        }

        return (allMediaIds, normalizedAdditionalMediaIds);
    }

    private async Task<string?> ValidateEventMediaAsync(
        Guid territoryId,
        Guid userId,
        List<Guid> allMediaIds,
        List<Guid>? normalizedAdditionalMediaIds,
        CancellationToken cancellationToken)
    {
        // Obter limites efetivos da configuração territorial (com fallback para global)
        var limits = await _mediaConfigService.GetEffectiveContentLimitsAsync(
            territoryId,
            MediaContentType.Events,
            cancellationToken);

        // Validar quantidade máxima de mídias (1 capa + adicionais)
        const int maxTotalMedia = 6; // 1 capa + 5 adicionais
        if (allMediaIds.Count > maxTotalMedia)
        {
            return $"Maximum {maxTotalMedia} media items allowed per event (1 cover + {maxTotalMedia - 1} additional).";
        }
        if (normalizedAdditionalMediaIds is not null && normalizedAdditionalMediaIds.Count > limits.MaxMediaCount - 1) // -1 para capa
        {
            return $"Maximum {limits.MaxMediaCount - 1} additional media items allowed per event (plus 1 cover).";
        }

        var mediaAssets = await _mediaAssetRepository.ListByIdsAsync(allMediaIds, cancellationToken);
        if (mediaAssets.Count != allMediaIds.Count)
        {
            return "One or more media assets not found.";
        }

        // Validar que todas as mídias pertencem ao usuário
        if (mediaAssets.Any(media => media.UploadedByUserId != userId || media.IsDeleted))
        {
            return "One or more media assets are invalid or do not belong to the user.";
        }

        var images = mediaAssets.Where(media => media.MediaType == Domain.Media.MediaType.Image).ToList();
        var videos = mediaAssets.Where(media => media.MediaType == Domain.Media.MediaType.Video).ToList();
        var audios = mediaAssets.Where(media => media.MediaType == Domain.Media.MediaType.Audio).ToList();

        // Validar imagens
        if (images.Count > 0 && !limits.ImagesEnabled)
        {
            return "Images are not enabled for events in this territory.";
        }

        var videoError = ValidateEventVideos(videos, limits);
        if (videoError is not null)
        {
            return videoError;
        }

        return ValidateEventAudios(audios, limits);
    }

    private static string? ValidateEventVideos(IReadOnlyList<MediaAsset> videos, MediaContentConfig limits)
    {
        if (videos.Count > 0 && !limits.VideosEnabled)
        {
            return "Videos are not enabled for events in this territory.";
        }
        if (videos.Count > limits.MaxVideoCount)
        {
            return $"Maximum {limits.MaxVideoCount} video(s) allowed per event.";
        }
        foreach (var video in videos)
        {
            if (video.SizeBytes > limits.MaxVideoSizeBytes)
            {
                var maxSizeMB = limits.MaxVideoSizeBytes / (1024.0 * 1024.0);
                return $"Video size exceeds {maxSizeMB:F1}MB limit for events.";
            }
            // Validar tipo MIME se configurado
            if (limits.AllowedVideoMimeTypes != null && limits.AllowedVideoMimeTypes.Count > 0)
            {
                if (!limits.AllowedVideoMimeTypes.Contains(video.MimeType, StringComparer.OrdinalIgnoreCase))
                {
                    return $"Video MIME type '{video.MimeType}' is not allowed for events.";
                }
            }
        }

        return null;
    }

    private static string? ValidateEventAudios(IReadOnlyList<MediaAsset> audios, MediaContentConfig limits)
    {
        if (audios.Count > 0 && !limits.AudioEnabled)
        {
            return "Audio is not enabled for events in this territory.";
        }
        if (audios.Count > limits.MaxAudioCount)
        {
            return $"Maximum {limits.MaxAudioCount} audio(s) allowed per event.";
        }
        foreach (var audio in audios)
        {
            if (audio.SizeBytes > limits.MaxAudioSizeBytes)
            {
                var maxSizeMB = limits.MaxAudioSizeBytes / (1024.0 * 1024.0);
                return $"Audio size exceeds {maxSizeMB:F1}MB limit for events.";
            }
            // Validar tipo MIME se configurado
            if (limits.AllowedAudioMimeTypes != null && limits.AllowedAudioMimeTypes.Count > 0)
            {
                if (!limits.AllowedAudioMimeTypes.Contains(audio.MimeType, StringComparer.OrdinalIgnoreCase))
                {
                    return $"Audio MIME type '{audio.MimeType}' is not allowed for events.";
                }
            }
        }

        return null;
    }

    private async Task<MembershipRole> ResolveMembershipAsync(
        Guid userId,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        return isResident ? MembershipRole.Resident : MembershipRole.Visitor;
    }

    private static TerritoryEvent BuildScheduledEvent(
        Guid territoryId,
        Guid userId,
        string title,
        string? description,
        DateTime startsAtUtc,
        DateTime? endsAtUtc,
        double latitude,
        double longitude,
        string? locationLabel,
        MembershipRole membership,
        DateTime now)
    {
        return new TerritoryEvent(
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
    }

    private async Task PersistEventMediaAttachmentsAsync(
        Guid eventId,
        Guid? coverMediaId,
        IReadOnlyList<Guid>? normalizedAdditionalMediaIds,
        IReadOnlyCollection<Guid> allMediaIds,
        DateTime now,
        CancellationToken cancellationToken)
    {
        if (allMediaIds.Count == 0)
        {
            return;
        }

        // Imagem de capa (DisplayOrder = 0)
        if (coverMediaId.HasValue && coverMediaId.Value != Guid.Empty)
        {
            var coverAttachment = new MediaAttachment(
                Guid.NewGuid(),
                coverMediaId.Value,
                MediaOwnerType.Event,
                eventId,
                0,
                now);

            await _mediaAttachmentRepository.AddAsync(coverAttachment, cancellationToken);
        }

        // Imagens adicionais (DisplayOrder = 1+)
        if (normalizedAdditionalMediaIds is not null && normalizedAdditionalMediaIds.Count > 0)
        {
            foreach (var (mediaId, index) in normalizedAdditionalMediaIds.Select((id, idx) => (id, idx + 1)))
            {
                var attachment = new MediaAttachment(
                    Guid.NewGuid(),
                    mediaId,
                    MediaOwnerType.Event,
                    eventId,
                    index,
                    now);

                await _mediaAttachmentRepository.AddAsync(attachment, cancellationToken);
            }
        }
    }

    private async Task ReplaceEventMediaAttachmentsAsync(
        Guid eventId,
        Guid? coverMediaId,
        IReadOnlyCollection<Guid>? additionalMediaIds,
        CancellationToken cancellationToken)
    {
        // Remover attachments existentes
        await _mediaAttachmentRepository.DeleteByOwnerAsync(MediaOwnerType.Event, eventId, cancellationToken);

        var (allMediaIds, normalizedAdditionalMediaIds) = CollectMediaIds(coverMediaId, additionalMediaIds);

        // Adicionar novos attachments
        if (allMediaIds.Count > 0)
        {
            var now = DateTime.UtcNow;
            await PersistEventMediaAttachmentsAsync(
                eventId,
                coverMediaId,
                normalizedAdditionalMediaIds,
                allMediaIds,
                now,
                cancellationToken);
        }
    }

    private async Task PublishEventPostAsync(
        TerritoryEvent territoryEvent,
        Guid territoryId,
        Guid userId,
        string title,
        string? description,
        DateTime now,
        CancellationToken cancellationToken)
    {
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
    }

    private static void ApplyEventUpdates(
        TerritoryEvent territoryEvent,
        string? title,
        string? description,
        DateTime? startsAtUtc,
        DateTime? endsAtUtc,
        double? latitude,
        double? longitude,
        string? locationLabel)
    {
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
    }

    private void RegisterEventCreatedSideEffects(Guid territoryId, Guid eventId)
    {
        // Invalidar cache de eventos do território
        _cacheInvalidation?.InvalidateEventCache(territoryId, eventId);

        // Record business metric
        ArahMetrics.EventsCreated.Add(1, new KeyValuePair<string, object?>("territory_id", territoryId));

        // Invalidate cache when event is created
        _eventCache?.InvalidateTerritoryEvents(territoryId);
    }

    private void InvalidateEventCaches(Guid territoryId, Guid eventId)
    {
        // Invalidar cache de eventos do território
        _cacheInvalidation?.InvalidateEventCache(territoryId, eventId);

        // Invalidate cache when event is updated
        _eventCache?.InvalidateTerritoryEvents(territoryId);
    }
}
