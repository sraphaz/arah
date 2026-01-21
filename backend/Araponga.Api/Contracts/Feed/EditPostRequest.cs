namespace Araponga.Api.Contracts.Feed;

public sealed record EditPostRequest(
    string Title,
    string Content,
    IReadOnlyCollection<GeoAnchorRequest>? GeoAnchors,
    IReadOnlyCollection<Guid>? MediaIds
);
