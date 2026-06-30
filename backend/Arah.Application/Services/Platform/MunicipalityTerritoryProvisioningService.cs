using Arah.Application.Interfaces.Geo;
using Arah.Domain.Territories;
using Microsoft.Extensions.Logging;

namespace Arah.Application.Services;

/// <summary>
/// Cria ou reutiliza território alinhado ao município IBGE (contorno oficial).
/// </summary>
public sealed class MunicipalityTerritoryProvisioningService : IMunicipalityTerritoryProvisioningService
{
    private readonly TerritoryService _territoryService;
    private readonly IIbgeBoundaryResolver _ibgeBoundaryResolver;
    private readonly IReverseGeocodingService _reverseGeocodingService;
    private readonly ILogger<MunicipalityTerritoryProvisioningService> _logger;

    public MunicipalityTerritoryProvisioningService(
        TerritoryService territoryService,
        IIbgeBoundaryResolver ibgeBoundaryResolver,
        IReverseGeocodingService reverseGeocodingService,
        ILogger<MunicipalityTerritoryProvisioningService> logger)
    {
        _territoryService = territoryService;
        _ibgeBoundaryResolver = ibgeBoundaryResolver;
        _reverseGeocodingService = reverseGeocodingService;
        _logger = logger;
    }

    public async Task<MunicipalityTerritoryProvisioningResult?> SuggestOrCreateAsync(
        double latitude,
        double longitude,
        string? city,
        string? stateCode,
        CancellationToken cancellationToken = default)
    {
        var resolvedCity = city?.Trim();
        var resolvedState = stateCode?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(resolvedCity) || string.IsNullOrWhiteSpace(resolvedState))
        {
            var reverse = await _reverseGeocodingService.ReverseAsync(latitude, longitude, cancellationToken);
            if (reverse is null)
            {
                _logger.LogWarning("Reverse geocode falhou para {Latitude}, {Longitude}", latitude, longitude);
                return null;
            }

            resolvedCity = reverse.City;
            resolvedState = reverse.StateCode;
        }

        if (resolvedState.Length != 2)
        {
            return null;
        }

        var existing = await FindExistingMunicipalityTerritoryAsync(resolvedCity, resolvedState, cancellationToken);
        if (existing is not null)
        {
            var distanceKm = HaversineKm(latitude, longitude, existing.Latitude, existing.Longitude);
            return MunicipalityTerritoryProvisioningResult.FromTerritory(existing, distanceKm, created: false, ibgeCode: null);
        }

        var boundary = await _ibgeBoundaryResolver.ResolveByCityAsync(resolvedCity, resolvedState, cancellationToken);
        if (boundary is null)
        {
            _logger.LogWarning("Malha IBGE não encontrada para {City}/{State}", resolvedCity, resolvedState);
            return null;
        }

        var description =
            $"Perímetro oficial do município (malha IBGE {boundary.IbgeCode}). SIRGAS 2000.";

        var createResult = await _territoryService.CreateAsync(
            boundary.MunicipalityName,
            description,
            boundary.City,
            boundary.StateCode,
            boundary.CentroidLatitude,
            boundary.CentroidLongitude,
            cancellationToken,
            radiusKm: null,
            boundaryPolygon: boundary.BoundaryPolygon,
            status: TerritoryStatus.Active);

        if (!createResult.Success || createResult.Territory is null)
        {
            _logger.LogWarning("Falha ao criar território {City}/{State}: {Error}", resolvedCity, resolvedState, createResult.Error);
            return null;
        }

        var distance = HaversineKm(latitude, longitude, createResult.Territory.Latitude, createResult.Territory.Longitude);
        return MunicipalityTerritoryProvisioningResult.FromTerritory(
            createResult.Territory,
            distance,
            created: true,
            ibgeCode: boundary.IbgeCode);
    }

    private async Task<Territory?> FindExistingMunicipalityTerritoryAsync(
        string city,
        string stateCode,
        CancellationToken cancellationToken)
    {
        var matches = await _territoryService.SearchAsync(city, city, stateCode, cancellationToken);
        return matches.FirstOrDefault(t =>
            string.Equals(t.City, city, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(t.State, stateCode, StringComparison.OrdinalIgnoreCase) &&
            (string.Equals(t.Name, city, StringComparison.OrdinalIgnoreCase) ||
             t.Description?.Contains("IBGE", StringComparison.OrdinalIgnoreCase) == true));
    }

    private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double r = 6371;
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return r * c;
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}
