namespace Arah.Bff.Contracts.Journeys.Feed;

public sealed record PostInteractionRequestWithComment(
    Guid PostId,
    Guid TerritoryId,
    string Action,
    string? CommentContent);
