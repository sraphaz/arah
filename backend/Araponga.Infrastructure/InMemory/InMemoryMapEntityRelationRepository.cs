using Araponga.Modules.Map.Application.Interfaces;
using Araponga.Modules.Map.Domain;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryMapEntityRelationRepository : IMapEntityRelationRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryMapEntityRelationRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<bool> ExistsAsync(Guid userId, Guid entityId, CancellationToken cancellationToken)
    {
        var exists = _dataStore.MapEntityRelations.Any(relation =>
            relation.UserId == userId &&
            relation.EntityId == entityId);

        return Task.FromResult(exists);
    }

    public Task AddAsync(MapEntityRelation relation, CancellationToken cancellationToken)
    {
        _dataStore.MapEntityRelations.Add(relation);
        return Task.CompletedTask;
    }
}
