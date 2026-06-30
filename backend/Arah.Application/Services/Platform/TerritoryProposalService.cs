using Arah.Application.Interfaces;
using Arah.Application.Interfaces.Geo;
using Arah.Domain.Territories;
using Microsoft.Extensions.Logging;

namespace Arah.Application.Services;

/// <summary>
/// Proposta comunitária de território (desenho manual no mapa) com status Pending até validação de curador.
/// </summary>
public sealed class TerritoryProposalService : ITerritoryProposalService
{
    private const double DefaultRadiusKm = 2.0;
    private const double MaxRadiusKm = 25.0;

    private readonly TerritoryService _territoryService;
    private readonly ITerritoryRepository _territoryRepository;
    private readonly ILogger<TerritoryProposalService> _logger;

    public TerritoryProposalService(
        TerritoryService territoryService,
        ITerritoryRepository territoryRepository,
        ILogger<TerritoryProposalService> logger)
    {
        _territoryService = territoryService;
        _territoryRepository = territoryRepository;
        _logger = logger;
    }

    public async Task<TerritoryProposalResult?> ProposeAsync(
        Guid proposerUserId,
        string name,
        string city,
        string stateCode,
        double latitude,
        double longitude,
        double? radiusKm,
        IReadOnlyList<TerritoryBoundaryPoint>? boundaryPolygon,
        Guid? parentTerritoryId,
        CancellationToken cancellationToken = default)
    {
        var resolvedCity = city?.Trim();
        var resolvedState = stateCode?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(resolvedCity) || string.IsNullOrWhiteSpace(resolvedState) || resolvedState.Length != 2)
        {
            return null;
        }

        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
        {
            return null;
        }

        var hasPolygon = boundaryPolygon is { Count: >= 3 };
        var resolvedRadius = radiusKm;
        if (!hasPolygon)
        {
            resolvedRadius = resolvedRadius is null or <= 0 ? DefaultRadiusKm : Math.Min(resolvedRadius.Value, MaxRadiusKm);
        }

        var resolvedName = string.IsNullOrWhiteSpace(name)
            ? $"{resolvedCity} — Célula"
            : name.Trim();

        var parentId = parentTerritoryId;
        if (parentId is null)
        {
            parentId = await ResolveParentMunicipalityTerritoryIdAsync(resolvedCity, resolvedState, cancellationToken);
        }

        var description =
            $"Proposta comunitária desenhada no mapa. Proponente: {proposerUserId}. Aguardando validação de curador.";

        var createResult = await _territoryService.CreateAsync(
            resolvedName,
            description,
            resolvedCity,
            resolvedState,
            latitude,
            longitude,
            cancellationToken,
            radiusKm: hasPolygon ? null : resolvedRadius,
            boundaryPolygon: hasPolygon ? boundaryPolygon : null,
            status: TerritoryStatus.Pending,
            parentTerritoryId: parentId);

        if (!createResult.Success || createResult.Territory is null)
        {
            _logger.LogWarning(
                "Falha ao propor território {Name}/{City}/{State}: {Error}",
                resolvedName,
                resolvedCity,
                resolvedState,
                createResult.Error);
            return null;
        }

        var distanceKm = HaversineKm(latitude, longitude, createResult.Territory.Latitude, createResult.Territory.Longitude);
        return TerritoryProposalResult.FromTerritory(createResult.Territory, distanceKm, created: true);
    }

    public Task<IReadOnlyList<Territory>> ListPendingAsync(CancellationToken cancellationToken = default)
    {
        return _territoryRepository.ListByStatusAsync(TerritoryStatus.Pending, cancellationToken);
    }

    public async Task<bool> ReviewAsync(Guid territoryId, bool approve, CancellationToken cancellationToken = default)
    {
        var territory = await _territoryRepository.GetByIdAsync(territoryId, cancellationToken);
        if (territory is null || territory.Status != TerritoryStatus.Pending)
        {
            return false;
        }

        var newStatus = approve ? TerritoryStatus.Active : TerritoryStatus.Inactive;
        return await _territoryService.UpdateStatusAsync(territoryId, newStatus, cancellationToken);
    }

    private async Task<Guid?> ResolveParentMunicipalityTerritoryIdAsync(
        string city,
        string stateCode,
        CancellationToken cancellationToken)
    {
        var matches = await _territoryRepository.SearchAsync(city, city, stateCode, cancellationToken);
        var parent = matches.FirstOrDefault(t =>
            t.ParentTerritoryId is null &&
            string.Equals(t.City, city, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(t.State, stateCode, StringComparison.OrdinalIgnoreCase));
        return parent?.Id;
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
