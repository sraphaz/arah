using Araponga.Application.Interfaces;
using Araponga.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresTerritoryEventRepository : ITerritoryEventRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresTerritoryEventRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TerritoryEvent?> GetByIdAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(evt => evt.Id == eventId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<TerritoryEvent>> ListByIdsAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken)
    {
        if (eventIds.Count == 0)
        {
            return Array.Empty<TerritoryEvent>();
        }

        var records = await _dbContext.TerritoryEvents
            .AsNoTracking()
            .Where(evt => eventIds.Contains(evt.Id))
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<TerritoryEvent>> ListByTerritoryAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        EventStatus? status,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.TerritoryEvents
            .AsNoTracking()
            .Where(evt => evt.TerritoryId == territoryId);

        if (fromUtc is not null)
        {
            query = query.Where(evt => evt.StartsAtUtc >= fromUtc.Value);
        }

        if (toUtc is not null)
        {
            query = query.Where(evt => evt.StartsAtUtc <= toUtc.Value);
        }

        if (status is not null)
        {
            query = query.Where(evt => evt.Status == status);
        }

        var records = await query
            .OrderBy(evt => evt.StartsAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<TerritoryEvent>> ListByBoundingBoxAsync(
        double minLatitude,
        double maxLatitude,
        double minLongitude,
        double maxLongitude,
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.TerritoryEvents
            .AsNoTracking()
            .Where(evt => evt.Latitude >= minLatitude && evt.Latitude <= maxLatitude)
            .Where(evt => evt.Longitude >= minLongitude && evt.Longitude <= maxLongitude);

        if (territoryId is not null)
        {
            query = query.Where(evt => evt.TerritoryId == territoryId);
        }

        if (fromUtc is not null)
        {
            query = query.Where(evt => evt.StartsAtUtc >= fromUtc.Value);
        }

        if (toUtc is not null)
        {
            query = query.Where(evt => evt.StartsAtUtc <= toUtc.Value);
        }

        var records = await query
            .OrderBy(evt => evt.StartsAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public Task AddAsync(TerritoryEvent territoryEvent, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryEvents.Add(territoryEvent.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryEvent territoryEvent, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryEvents.Update(territoryEvent.ToRecord());
        return Task.CompletedTask;
    }
}
