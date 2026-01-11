namespace Araponga.Application.Models;

public sealed record GeoAnchorInput(
    double Latitude,
    double Longitude,
    string Type);
