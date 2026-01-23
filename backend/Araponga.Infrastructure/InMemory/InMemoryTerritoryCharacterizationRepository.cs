using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryTerritoryCharacterizationRepository : ITerritoryCharacterizationRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTerritoryCharacterizationRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task UpsertAsync(TerritoryCharacterization characterization, CancellationToken cancellationToken)
    {
        var index = _dataStore.TerritoryCharacterizations.FindIndex(c => c.TerritoryId == characterization.TerritoryId);
        if (index >= 0)
        {
            _dataStore.TerritoryCharacterizations[index] = characterization;
        }
        else
        {
            _dataStore.TerritoryCharacterizations.Add(characterization);
        }

        return Task.CompletedTask;
    }

    public Task<TerritoryCharacterization?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var characterization = _dataStore.TerritoryCharacterizations.FirstOrDefault(c => c.TerritoryId == territoryId);
        return Task.FromResult(characterization);
    }
}
