using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryStoreRepository : IStoreRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryStoreRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<TerritoryStore?> GetByIdAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var store = _dataStore.TerritoryStores.FirstOrDefault(s => s.Id == storeId);
        return Task.FromResult(store);
    }

    public Task<TerritoryStore?> GetByOwnerAsync(Guid territoryId, Guid ownerUserId, CancellationToken cancellationToken)
    {
        var store = _dataStore.TerritoryStores.FirstOrDefault(s => s.TerritoryId == territoryId && s.OwnerUserId == ownerUserId);
        return Task.FromResult(store);
    }

    public Task<IReadOnlyList<TerritoryStore>> ListByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken)
    {
        var stores = _dataStore.TerritoryStores.Where(s => s.OwnerUserId == ownerUserId).ToList();
        return Task.FromResult<IReadOnlyList<TerritoryStore>>(stores);
    }

    public Task<IReadOnlyList<TerritoryStore>> ListByIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken)
    {
        if (storeIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<TerritoryStore>>(Array.Empty<TerritoryStore>());
        }

        var stores = _dataStore.TerritoryStores.Where(s => storeIds.Contains(s.Id)).ToList();
        return Task.FromResult<IReadOnlyList<TerritoryStore>>(stores);
    }

    public Task AddAsync(TerritoryStore store, CancellationToken cancellationToken)
    {
        _dataStore.TerritoryStores.Add(store);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryStore store, CancellationToken cancellationToken)
    {
        var index = _dataStore.TerritoryStores.FindIndex(s => s.Id == store.Id);
        if (index >= 0)
        {
            _dataStore.TerritoryStores[index] = store;
        }

        return Task.CompletedTask;
    }
}
