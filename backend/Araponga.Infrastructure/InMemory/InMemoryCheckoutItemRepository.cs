using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryCheckoutItemRepository : ICheckoutItemRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryCheckoutItemRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddRangeAsync(IReadOnlyList<CheckoutItem> items, CancellationToken cancellationToken)
    {
        _dataStore.CheckoutItems.AddRange(items);
        return Task.CompletedTask;
    }
}
