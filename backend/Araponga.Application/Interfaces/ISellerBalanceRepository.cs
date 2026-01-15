using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface ISellerBalanceRepository
{
    Task<SellerBalance?> GetByTerritoryAndSellerAsync(Guid territoryId, Guid sellerUserId, CancellationToken cancellationToken);
    Task<List<SellerBalance>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<List<SellerBalance>> GetBySellerUserIdAsync(Guid sellerUserId, CancellationToken cancellationToken);
    Task AddAsync(SellerBalance balance, CancellationToken cancellationToken);
    Task UpdateAsync(SellerBalance balance, CancellationToken cancellationToken);
}
