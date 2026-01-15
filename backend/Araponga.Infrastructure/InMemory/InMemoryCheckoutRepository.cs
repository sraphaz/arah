using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryCheckoutRepository : ICheckoutRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryCheckoutRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Checkout?> GetByIdAsync(Guid checkoutId, CancellationToken cancellationToken)
    {
        var checkout = _dataStore.Checkouts.FirstOrDefault(c => c.Id == checkoutId);
        return Task.FromResult(checkout);
    }

    public Task AddAsync(Checkout checkout, CancellationToken cancellationToken)
    {
        _dataStore.Checkouts.Add(checkout);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Checkout checkout, CancellationToken cancellationToken)
    {
        var index = _dataStore.Checkouts.FindIndex(c => c.Id == checkout.Id);
        if (index >= 0)
        {
            _dataStore.Checkouts[index] = checkout;
        }

        return Task.CompletedTask;
    }
}
