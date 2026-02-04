using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

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

    public Task<IReadOnlyList<Checkout>> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var checkouts = _dataStore.Checkouts
            .Where(c => c.BuyerUserId == userId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<Checkout>>(checkouts);
    }

    public Task<IReadOnlyList<Checkout>> ListAllAsync(CancellationToken cancellationToken)
    {
        var checkouts = _dataStore.Checkouts
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<Checkout>>(checkouts);
    }
}
