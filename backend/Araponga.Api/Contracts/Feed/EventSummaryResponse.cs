namespace Araponga.Api.Contracts.Feed;

public sealed record EventSummaryResponse(
    Guid EventId,
    string Title,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    double Latitude,
    double Longitude,
    string? LocationLabel,
    string Status,
    Guid CreatedByUserId,
    string? CreatedByDisplayName,
    string CreatedByMembership,
    int InterestedCount,
    int ConfirmedCount);
