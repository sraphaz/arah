using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Application.Interfaces;

public interface ICartItemRepository
{
    Task<IReadOnlyList<CartItem>> ListByCartIdAsync(Guid cartId, CancellationToken cancellationToken);
    Task<CartItem?> GetByIdAsync(Guid cartItemId, CancellationToken cancellationToken);
    Task<CartItem?> GetByCartAndListingAsync(Guid cartId, Guid itemId, CancellationToken cancellationToken);
    Task AddAsync(CartItem item, CancellationToken cancellationToken);
    Task UpdateAsync(CartItem item, CancellationToken cancellationToken);
    Task RemoveAsync(Guid cartItemId, CancellationToken cancellationToken);
    Task RemoveByCartIdAsync(Guid cartId, CancellationToken cancellationToken);
}
