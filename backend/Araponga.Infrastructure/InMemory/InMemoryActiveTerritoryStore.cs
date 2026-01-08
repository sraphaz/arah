using Araponga.Application.Interfaces;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryActiveTerritoryStore : IActiveTerritoryStore
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryActiveTerritoryStore(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Guid?> GetAsync(string sessionId, CancellationToken cancellationToken)
    {
        if (_dataStore.ActiveTerritories.TryGetValue(sessionId, out var territoryId))
        {
            return Task.FromResult<Guid?>(territoryId);
        }

        return Task.FromResult<Guid?>(null);
    }

    public Task SetAsync(string sessionId, Guid territoryId, CancellationToken cancellationToken)
    {
        _dataStore.ActiveTerritories[sessionId] = territoryId;
        return Task.CompletedTask;
    }
}
