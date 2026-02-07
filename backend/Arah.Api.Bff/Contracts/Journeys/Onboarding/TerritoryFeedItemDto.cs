namespace Arah.Bff.Contracts.Journeys.Onboarding;

public sealed record TerritoryFeedItemDto(
    Guid Id,
    string Title,
    string Content,
    string Type,
    DateTime CreatedAtUtc,
    IReadOnlyList<string>? Tags,
    int LikeCount,
    int ShareCount,
    int CommentCount,
    IReadOnlyList<string>? MediaUrls);
