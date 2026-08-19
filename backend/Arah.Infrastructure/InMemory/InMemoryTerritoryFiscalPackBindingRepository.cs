using Arah.Application.Interfaces;
using Arah.Domain.Fiscal;

namespace Arah.Infrastructure.InMemory;

public sealed class InMemoryTerritoryFiscalPackBindingRepository : ITerritoryFiscalPackBindingRepository
{
    private readonly InMemoryDataStore _store;

    public InMemoryTerritoryFiscalPackBindingRepository(InMemoryDataStore store)
    {
        _store = store;
    }

    public Task<TerritoryFiscalPackBinding?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var match = _store.TerritoryFiscalPackBindings.FirstOrDefault(b => b.TerritoryId == territoryId);
        return Task.FromResult(match);
    }

    public Task AddAsync(TerritoryFiscalPackBinding binding, CancellationToken cancellationToken)
    {
        lock (_store.TerritoryFiscalPackBindings)
        {
            if (_store.TerritoryFiscalPackBindings.Any(b => b.TerritoryId == binding.TerritoryId))
            {
                throw new InvalidOperationException(
                    $"Fiscal pack binding already exists for territory {binding.TerritoryId}.");
            }

            _store.TerritoryFiscalPackBindings.Add(binding);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryFiscalPackBinding binding, CancellationToken cancellationToken)
    {
        var index = _store.TerritoryFiscalPackBindings.FindIndex(b => b.Id == binding.Id);
        if (index < 0)
        {
            throw new InvalidOperationException($"TerritoryFiscalPackBinding {binding.Id} not found.");
        }

        _store.TerritoryFiscalPackBindings[index] = binding;
        return Task.CompletedTask;
    }
}
