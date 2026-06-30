using Arah.Domain.Territories;

namespace Arah.Application.Interfaces.Geo;

/// <summary>
/// Resolve contorno municipal oficial via IBGE (malhas territoriais v3 + localidades).
/// </summary>
public interface IIbgeBoundaryResolver
{
    Task<IbgeMunicipalityBoundary?> ResolveByCityAsync(
        string city,
        string stateCode,
        CancellationToken cancellationToken = default);
}

public sealed record IbgeMunicipalityBoundary(
    int IbgeCode,
    string MunicipalityName,
    string City,
    string StateCode,
    double CentroidLatitude,
    double CentroidLongitude,
    IReadOnlyList<TerritoryBoundaryPoint> BoundaryPolygon);
