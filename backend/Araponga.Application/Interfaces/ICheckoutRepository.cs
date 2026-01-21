using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface ICheckoutRepository
{
    Task<Checkout?> GetByIdAsync(Guid checkoutId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Checkout>> ListByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Checkout>> ListAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Checkout checkout, CancellationToken cancellationToken);
    Task UpdateAsync(Checkout checkout, CancellationToken cancellationToken);
}
