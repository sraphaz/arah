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
}
