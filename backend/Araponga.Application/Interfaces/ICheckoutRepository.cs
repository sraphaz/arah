using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface ICheckoutRepository
{
    Task AddAsync(Checkout checkout, CancellationToken cancellationToken);
    Task UpdateAsync(Checkout checkout, CancellationToken cancellationToken);
}
