namespace Araponga.Domain.Territories;

/// <summary>
/// Um ponto (latitude, longitude) que compõe o polígono do perímetro do território.
/// O polígono é uma lista ordenada de pontos; o perímetro fecha ligando o último ao primeiro.
/// </summary>
public sealed record TerritoryBoundaryPoint(double Latitude, double Longitude);
