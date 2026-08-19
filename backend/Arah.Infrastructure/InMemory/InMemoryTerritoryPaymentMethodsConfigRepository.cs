using Arah.Application.Interfaces;
using Arah.Domain.Fiscal;

namespace Arah.Infrastructure.InMemory;

public sealed class InMemoryTerritoryPaymentMethodsConfigRepository : ITerritoryPaymentMethodsConfigRepository
{
    private readonly InMemoryDataStore _store;

    public InMemoryTerritoryPaymentMethodsConfigRepository(InMemoryDataStore store)
    {
        _store = store;
    }

    public Task<TerritoryPaymentMethodsConfig?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var match = _store.TerritoryPaymentMethodsConfigs.FirstOrDefault(c => c.TerritoryId == territoryId);
        return Task.FromResult(match);
    }

    public Task AddAsync(TerritoryPaymentMethodsConfig config, CancellationToken cancellationToken)
    {
        _store.TerritoryPaymentMethodsConfigs.Add(config);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryPaymentMethodsConfig config, CancellationToken cancellationToken)
    {
        var index = _store.TerritoryPaymentMethodsConfigs.FindIndex(c => c.Id == config.Id);
        if (index < 0)
        {
            throw new InvalidOperationException($"TerritoryPaymentMethodsConfig {config.Id} not found.");
        }

        _store.TerritoryPaymentMethodsConfigs[index] = config;
        return Task.CompletedTask;
    }
}
