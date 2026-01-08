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
}
