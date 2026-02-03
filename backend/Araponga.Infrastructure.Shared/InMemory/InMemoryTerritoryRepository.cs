using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de ITerritoryRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryTerritoryRepository : ITerritoryRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryTerritoryRepository(InMemorySharedStore store) => _store = store;

    public Task<IReadOnlyList<Territory>> ListAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Territory>>(_store.Territories.ToList());

    public Task<Territory?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(_store.Territories.FirstOrDefault(t => t.Id == id));

    public Task AddAsync(Territory territory, CancellationToken cancellationToken)
    {
        _store.Territories.Add(territory);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Territory>> SearchAsync(string? query, string? city, string? state, CancellationToken cancellationToken)
    {
        var territories = _store.Territories.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(query))
            territories = territories.Where(t => t.Name.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(city))
            territories = territories.Where(t => string.Equals(t.City, city.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(state))
            territories = territories.Where(t => string.Equals(t.State, state.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<IReadOnlyList<Territory>>(territories.ToList());
    }

    public Task<IReadOnlyList<Territory>> NearbyAsync(double latitude, double longitude, double radiusKm, int limit, CancellationToken cancellationToken)
    {
        var list = _store.Territories
            .Select(t => new { Territory = t, Distance = CalculateDistance(latitude, longitude, t.Latitude, t.Longitude) })
            .Where(t => t.Distance <= radiusKm)
            .OrderBy(t => t.Distance)
            .Select(t => t.Territory)
            .Take(limit)
            .ToList();
        return Task.FromResult<IReadOnlyList<Territory>>(list);
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    public Task<IReadOnlyList<Territory>> ListPagedAsync(int skip, int take, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Territory>>(_store.Territories.OrderBy(t => t.Name).Skip(skip).Take(take).ToList());

    public Task<IReadOnlyList<Territory>> SearchPagedAsync(string? query, string? city, string? state, int skip, int take, CancellationToken cancellationToken)
    {
        var territories = _store.Territories.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(query)) territories = territories.Where(t => t.Name.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(city)) territories = territories.Where(t => string.Equals(t.City, city.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(state)) territories = territories.Where(t => string.Equals(t.State, state.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<IReadOnlyList<Territory>>(territories.OrderBy(t => t.Name).Skip(skip).Take(take).ToList());
    }

    public Task<IReadOnlyList<Territory>> NearbyPagedAsync(double latitude, double longitude, double radiusKm, int skip, int take, CancellationToken cancellationToken)
    {
        var list = _store.Territories
            .Select(t => new { Territory = t, Distance = CalculateDistance(latitude, longitude, t.Latitude, t.Longitude) })
            .Where(t => t.Distance <= radiusKm)
            .OrderBy(t => t.Distance)
            .Skip(skip)
            .Take(take)
            .Select(t => t.Territory)
            .ToList();
        return Task.FromResult<IReadOnlyList<Territory>>(list);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        const int max = int.MaxValue;
        var c = _store.Territories.Count;
        return Task.FromResult(c > max ? max : c);
    }

    public Task<int> CountSearchAsync(string? query, string? city, string? state, CancellationToken cancellationToken)
    {
        var territories = _store.Territories.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(query)) territories = territories.Where(t => t.Name.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(city)) territories = territories.Where(t => string.Equals(t.City, city.Trim(), StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(state)) territories = territories.Where(t => string.Equals(t.State, state.Trim(), StringComparison.OrdinalIgnoreCase));
        const int max = int.MaxValue;
        var c = territories.Count();
        return Task.FromResult(c > max ? max : c);
    }
}
