using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryCartRepository : ICartRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryCartRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<Cart?> GetByUserAsync(Guid territoryId, Guid userId, CancellationToken cancellationToken)
    {
        var cart = _dataStore.Carts.FirstOrDefault(c => c.TerritoryId == territoryId && c.UserId == userId);
        return Task.FromResult(cart);
    }

    public Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var cart = _dataStore.Carts.FirstOrDefault(c => c.Id == cartId);
        return Task.FromResult(cart);
    }

    public Task AddAsync(Cart cart, CancellationToken cancellationToken)
    {
        _dataStore.Carts.Add(cart);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Cart cart, CancellationToken cancellationToken)
    {
        var index = _dataStore.Carts.FindIndex(c => c.Id == cart.Id);
        if (index >= 0)
        {
            _dataStore.Carts[index] = cart;
        }

        return Task.CompletedTask;
    }
}
