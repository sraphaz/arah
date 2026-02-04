using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Application.Interfaces;

public interface ICheckoutItemRepository
{
    Task AddRangeAsync(IReadOnlyList<CheckoutItem> items, CancellationToken cancellationToken);
}
