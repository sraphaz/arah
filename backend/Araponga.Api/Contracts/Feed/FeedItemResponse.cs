namespace Araponga.Api.Contracts.Feed;

public sealed record FeedItemResponse(
    Guid Id,
    string Title,
    string Content,
    string Type,
    string Visibility,
    string Status,
    Guid? MapEntityId,
    EventSummaryResponse? Event,
    bool IsHighlighted,
    int LikeCount,
    int ShareCount,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<string>? MediaUrls = null,
    int MediaCount = 0
);
