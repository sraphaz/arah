using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryCartItemRepository : ICartItemRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryCartItemRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<CartItem>> ListByCartIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var items = _dataStore.CartItems.Where(i => i.CartId == cartId).ToList();
        return Task.FromResult<IReadOnlyList<CartItem>>(items);
    }

    public Task<CartItem?> GetByIdAsync(Guid cartItemId, CancellationToken cancellationToken)
    {
        var item = _dataStore.CartItems.FirstOrDefault(i => i.Id == cartItemId);
        return Task.FromResult(item);
    }

    public Task<CartItem?> GetByCartAndListingAsync(Guid cartId, Guid listingId, CancellationToken cancellationToken)
    {
        var item = _dataStore.CartItems.FirstOrDefault(i => i.CartId == cartId && i.ListingId == listingId);
        return Task.FromResult(item);
    }

    public Task AddAsync(CartItem item, CancellationToken cancellationToken)
    {
        _dataStore.CartItems.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CartItem item, CancellationToken cancellationToken)
    {
        var index = _dataStore.CartItems.FindIndex(i => i.Id == item.Id);
        if (index >= 0)
        {
            _dataStore.CartItems[index] = item;
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid cartItemId, CancellationToken cancellationToken)
    {
        _dataStore.CartItems.RemoveAll(i => i.Id == cartItemId);
        return Task.CompletedTask;
    }

    public Task RemoveByCartIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        _dataStore.CartItems.RemoveAll(i => i.CartId == cartId);
        return Task.CompletedTask;
    }
}
