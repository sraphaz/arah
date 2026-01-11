using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresTerritoryRepository : ITerritoryRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresTerritoryRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Territory>> ListAsync(CancellationToken cancellationToken)
    {
        var records = await _dbContext.Territories
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<Territory?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.Territories
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task AddAsync(Territory territory, CancellationToken cancellationToken)
    {
        _dbContext.Territories.Add(territory.ToRecord());
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Territory>> SearchAsync(
        string? query,
        string? city,
        string? state,
        CancellationToken cancellationToken)
    {
        var territories = _dbContext.Territories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var trimmed = query.Trim();
            territories = territories.Where(t => EF.Functions.ILike(t.Name, $"%{trimmed}%"));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var trimmed = city.Trim();
            territories = territories.Where(t => t.City == trimmed);
        }

        if (!string.IsNullOrWhiteSpace(state))
        {
            var trimmed = state.Trim();
            territories = territories.Where(t => t.State == trimmed);
        }

        var records = await territories.ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Territory>> NearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        int limit,
        CancellationToken cancellationToken)
    {
        const double radius = 6371.0;
        var lat1 = latitude * Math.PI / 180.0;
        var lon1 = longitude * Math.PI / 180.0;

        var territories = await _dbContext.Territories
            .AsNoTracking()
            .Select(t => new
            {
                Territory = t,
                Distance = radius * 2 * Math.Asin(Math.Sqrt(
                    Math.Pow(Math.Sin((t.Latitude * Math.PI / 180.0 - lat1) / 2), 2) +
                    Math.Cos(lat1) * Math.Cos(t.Latitude * Math.PI / 180.0) *
                    Math.Pow(Math.Sin((t.Longitude * Math.PI / 180.0 - lon1) / 2), 2)))
            })
            .Where(t => t.Distance <= radiusKm)
            .OrderBy(t => t.Distance)
            .Select(t => t.Territory)
            .Take(limit)
            .ToListAsync(cancellationToken);
        return territories.Select(record => record.ToDomain()).ToList();
    }
}
