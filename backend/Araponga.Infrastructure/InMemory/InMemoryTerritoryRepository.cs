using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryTerritoryRepository : ITerritoryRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTerritoryRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<Territory>> ListAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Territory>>(_dataStore.Territories.ToList());
    }

    public Task<Territory?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var territory = _dataStore.Territories.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(territory);
    }

    public Task AddAsync(Territory territory, CancellationToken cancellationToken)
    {
        _dataStore.Territories.Add(territory);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Territory>> SearchAsync(
        string? query,
        string? city,
        string? state,
        CancellationToken cancellationToken)
    {
        var territories = _dataStore.Territories.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            territories = territories.Where(t =>
                t.Name.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            territories = territories.Where(t =>
                string.Equals(t.City, city.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(state))
        {
            territories = territories.Where(t =>
                string.Equals(t.State, state.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult<IReadOnlyList<Territory>>(territories.ToList());
    }

    public Task<IReadOnlyList<Territory>> NearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        int limit,
        CancellationToken cancellationToken)
    {
        var territories = _dataStore.Territories
            .Select(t => new
            {
                Territory = t,
                Distance = CalculateDistance(latitude, longitude, t.Latitude, t.Longitude)
            })
            .Where(t => t.Distance <= radiusKm)
            .OrderBy(t => t.Distance)
            .Select(t => t.Territory)
            .Take(limit)
            .ToList();

        return Task.FromResult<IReadOnlyList<Territory>>(territories);
    }

    private static double CalculateDistance(
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2)
    {
        const double Radius = 6371.0;
        var lat1 = DegreesToRadians(latitude1);
        var lat2 = DegreesToRadians(latitude2);
        var deltaLat = DegreesToRadians(latitude2 - latitude1);
        var deltaLon = DegreesToRadians(longitude2 - longitude1);

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return Radius * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
