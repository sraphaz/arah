using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de ITerritoryCharacterizationRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryTerritoryCharacterizationRepository : ITerritoryCharacterizationRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryTerritoryCharacterizationRepository(InMemorySharedStore store) => _store = store;

    public Task UpsertAsync(TerritoryCharacterization characterization, CancellationToken cancellationToken)
    {
        var i = _store.TerritoryCharacterizations.FindIndex(c => c.TerritoryId == characterization.TerritoryId);
        if (i >= 0) _store.TerritoryCharacterizations[i] = characterization;
        else _store.TerritoryCharacterizations.Add(characterization);
        return Task.CompletedTask;
    }

    public Task<TerritoryCharacterization?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
        => Task.FromResult(_store.TerritoryCharacterizations.FirstOrDefault(c => c.TerritoryId == territoryId));
}
