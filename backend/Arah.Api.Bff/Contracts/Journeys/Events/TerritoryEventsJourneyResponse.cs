using Arah.Bff.Contracts.Journeys.Common;

namespace Arah.Bff.Contracts.Journeys.Events;

public sealed record TerritoryEventsJourneyResponse(
    IReadOnlyList<EventItemJourneyDto> Items,
    JourneyPaginationDto Pagination);

public sealed record EventItemJourneyDto(
    EventDetailJourneyDto Event,
    EventParticipantsSummaryDto Participants,
    IReadOnlyList<EventMediaDto> Media,
    UserEventParticipationDto? UserParticipation);

public sealed record EventDetailJourneyDto(
    Guid Id,
    Guid TerritoryId,
    string Title,
    string? Description,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    double? Latitude,
    double? Longitude,
    string? LocationLabel,
    string Status,
    string CreatedByMembership,
    string? CreatedByDisplayName);

public sealed record EventParticipantsSummaryDto(int InterestedCount, int ConfirmedCount);

public sealed record EventMediaDto(string Url, string Type, string? ThumbnailUrl);

public sealed record UserEventParticipationDto(string Status);
