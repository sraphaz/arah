using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface ICheckoutItemRepository
{
    Task AddRangeAsync(IReadOnlyList<CheckoutItem> items, CancellationToken cancellationToken);
}
