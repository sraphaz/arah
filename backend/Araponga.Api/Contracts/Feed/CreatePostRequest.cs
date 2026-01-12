namespace Araponga.Api.Contracts.Feed;

public sealed record CreatePostRequest(
    string Title,
    string Content,
    string Type,
    string Visibility,
    Guid? MapEntityId,
    IReadOnlyCollection<GeoAnchorRequest>? GeoAnchors,
    IReadOnlyCollection<Guid>? AssetIds
);
