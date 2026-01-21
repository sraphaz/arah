using Araponga.Application.Interfaces;
using Araponga.Domain.Events;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryTerritoryEventRepository : ITerritoryEventRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTerritoryEventRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<TerritoryEvent?> GetByIdAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var evt = _dataStore.TerritoryEvents.FirstOrDefault(e => e.Id == eventId);
        return Task.FromResult(evt);
    }

    public Task<IReadOnlyList<TerritoryEvent>> ListByIdsAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken)
    {
        if (eventIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<TerritoryEvent>>(Array.Empty<TerritoryEvent>());
        }

        var events = _dataStore.TerritoryEvents
            .Where(evt => eventIds.Contains(evt.Id))
            .ToList();

        return Task.FromResult<IReadOnlyList<TerritoryEvent>>(events);
    }

    public Task<IReadOnlyList<TerritoryEvent>> ListByTerritoryAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        EventStatus? status,
        CancellationToken cancellationToken)
    {
        var query = _dataStore.TerritoryEvents
            .Where(evt => evt.TerritoryId == territoryId)
            .AsEnumerable();

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

        var results = query.OrderBy(evt => evt.StartsAtUtc).ToList();
        return Task.FromResult<IReadOnlyList<TerritoryEvent>>(results);
    }

    public Task<IReadOnlyList<TerritoryEvent>> ListByBoundingBoxAsync(
        double minLatitude,
        double maxLatitude,
        double minLongitude,
        double maxLongitude,
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var query = _dataStore.TerritoryEvents
            .Where(evt => evt.Latitude >= minLatitude && evt.Latitude <= maxLatitude)
            .Where(evt => evt.Longitude >= minLongitude && evt.Longitude <= maxLongitude)
            .AsEnumerable();

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

        var results = query.OrderBy(evt => evt.StartsAtUtc).ToList();
        return Task.FromResult<IReadOnlyList<TerritoryEvent>>(results);
    }

    public Task AddAsync(TerritoryEvent territoryEvent, CancellationToken cancellationToken)
    {
        _dataStore.TerritoryEvents.Add(territoryEvent);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryEvent territoryEvent, CancellationToken cancellationToken)
    {
        var existing = _dataStore.TerritoryEvents.FirstOrDefault(evt => evt.Id == territoryEvent.Id);
        if (existing is not null)
        {
            _dataStore.TerritoryEvents.Remove(existing);
        }

        _dataStore.TerritoryEvents.Add(territoryEvent);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TerritoryEvent>> ListByTerritoryPagedAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        EventStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var query = _dataStore.TerritoryEvents
            .Where(evt => evt.TerritoryId == territoryId)
            .AsEnumerable();

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

        var results = query
            .OrderBy(evt => evt.StartsAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();
        
        return Task.FromResult<IReadOnlyList<TerritoryEvent>>(results);
    }

    public Task<IReadOnlyList<TerritoryEvent>> ListByBoundingBoxPagedAsync(
        double minLatitude,
        double maxLatitude,
        double minLongitude,
        double maxLongitude,
        DateTime? fromUtc,
        DateTime? toUtc,
        Guid? territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var query = _dataStore.TerritoryEvents
            .Where(evt => evt.Latitude >= minLatitude && evt.Latitude <= maxLatitude)
            .Where(evt => evt.Longitude >= minLongitude && evt.Longitude <= maxLongitude)
            .AsEnumerable();

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

        var results = query
            .OrderBy(evt => evt.StartsAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();
        
        return Task.FromResult<IReadOnlyList<TerritoryEvent>>(results);
    }

    public Task<int> CountByTerritoryAsync(
        Guid territoryId,
        DateTime? fromUtc,
        DateTime? toUtc,
        EventStatus? status,
        CancellationToken cancellationToken)
    {
        var query = _dataStore.TerritoryEvents
            .Where(evt => evt.TerritoryId == territoryId)
            .AsEnumerable();

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

        const int maxInt32 = int.MaxValue;
        var count = query.Count();
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }

    public Task<IReadOnlyList<TerritoryEvent>> ListByAuthorPagedAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var events = _dataStore.TerritoryEvents
            .Where(evt => evt.CreatedByUserId == userId)
            .OrderByDescending(evt => evt.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<TerritoryEvent>>(events);
    }

    public Task<int> CountByAuthorAsync(Guid userId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = _dataStore.TerritoryEvents
            .Count(evt => evt.CreatedByUserId == userId);
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }
}
