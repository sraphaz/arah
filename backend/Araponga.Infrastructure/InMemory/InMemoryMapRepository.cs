using Araponga.Application.Interfaces;
using Araponga.Domain.Map;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryMapRepository : IMapRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryMapRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<MapEntity>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var entities = _dataStore.MapEntities
            .Where(entity => entity.TerritoryId == territoryId)
            .ToList();

        return Task.FromResult<IReadOnlyList<MapEntity>>(entities);
    }

    public Task<MapEntity?> GetByIdAsync(Guid entityId, CancellationToken cancellationToken)
    {
        var entity = _dataStore.MapEntities.FirstOrDefault(e => e.Id == entityId);
        return Task.FromResult(entity);
    }

    public Task AddAsync(MapEntity entity, CancellationToken cancellationToken)
    {
        _dataStore.MapEntities.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(Guid entityId, MapEntityStatus status, CancellationToken cancellationToken)
    {
        var entity = _dataStore.MapEntities.FirstOrDefault(e => e.Id == entityId);
        if (entity is null)
        {
            return Task.CompletedTask;
        }

        _dataStore.MapEntities.Remove(entity);
        var updated = new MapEntity(
            entity.Id,
            entity.TerritoryId,
            entity.CreatedByUserId,
            entity.Name,
            entity.Category,
            status,
            entity.Visibility,
            entity.ConfirmationCount,
            entity.CreatedAtUtc);
        _dataStore.MapEntities.Add(updated);
        return Task.CompletedTask;
    }

    public Task IncrementConfirmationAsync(Guid entityId, CancellationToken cancellationToken)
    {
        var entity = _dataStore.MapEntities.FirstOrDefault(e => e.Id == entityId);
        if (entity is null)
        {
            return Task.CompletedTask;
        }

        _dataStore.MapEntities.Remove(entity);
        var updated = new MapEntity(
            entity.Id,
            entity.TerritoryId,
            entity.CreatedByUserId,
            entity.Name,
            entity.Category,
            entity.Status,
            entity.Visibility,
            entity.ConfirmationCount + 1,
            entity.CreatedAtUtc);
        _dataStore.MapEntities.Add(updated);
        return Task.CompletedTask;
    }
}
