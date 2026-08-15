namespace Arah.Api.Contracts.Assets;

public sealed record CreateAssetRequest(
    Guid TerritoryId,
    string Type,
    string Name,
    string? Description,
    IReadOnlyCollection<AssetGeoAnchorRequest> GeoAnchors,
    string? Subtype = null);
