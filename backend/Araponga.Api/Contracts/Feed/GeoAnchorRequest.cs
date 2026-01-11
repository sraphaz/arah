namespace Araponga.Api.Contracts.Feed;

public sealed record GeoAnchorRequest(
    double Latitude,
    double Longitude,
    string Type);
