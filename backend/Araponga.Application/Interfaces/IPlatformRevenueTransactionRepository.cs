using Araponga.Domain.Financial;

namespace Araponga.Application.Interfaces;

public interface IPlatformRevenueTransactionRepository
{
    Task<PlatformRevenueTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<PlatformRevenueTransaction>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<List<PlatformRevenueTransaction>> GetByCheckoutIdAsync(Guid checkoutId, CancellationToken cancellationToken);
    Task AddAsync(PlatformRevenueTransaction transaction, CancellationToken cancellationToken);
}
