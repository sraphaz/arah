namespace Araponga.Api.Contracts.Assets;

public sealed record AssetGeoAnchorResponse(
    Guid Id,
    double Latitude,
    double Longitude);
