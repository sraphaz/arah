namespace Araponga.Api.Contracts.Assets;

public sealed record UpdateAssetRequest(
    string Type,
    string Name,
    string? Description,
    IReadOnlyCollection<AssetGeoAnchorRequest> GeoAnchors);
