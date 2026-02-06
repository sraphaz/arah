namespace Araponga.Api.Contracts.Territories;

/// <summary>
/// Ponto geográfico (latitude, longitude). Usado no polígono do perímetro do território.
/// </summary>
public sealed record GeoPointDto(double Latitude, double Longitude);
