using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserAsync(Guid territoryId, Guid userId, CancellationToken cancellationToken);
    Task<Cart?> GetByIdAsync(Guid cartId, CancellationToken cancellationToken);
    Task AddAsync(Cart cart, CancellationToken cancellationToken);
    Task UpdateAsync(Cart cart, CancellationToken cancellationToken);
}
