namespace Araponga.Api.Contracts.Events;

public sealed record EventResponse(
    Guid EventId,
    Guid TerritoryId,
    string Title,
    string? Description,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    double Latitude,
    double Longitude,
    string? LocationLabel,
    Guid CreatedByUserId,
    string? CreatedByDisplayName,
    string CreatedByMembership,
    string Status,
    int InterestedCount,
    int ConfirmedCount,
    string? CoverImageUrl = null,
    IReadOnlyCollection<string>? AdditionalImageUrls = null);
