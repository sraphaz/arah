namespace Araponga.Api.Contracts.Feed;

public sealed record FeedItemResponse(
    Guid Id,
    string Title,
    string Content,
    string Visibility,
    DateTime CreatedAtUtc
);
