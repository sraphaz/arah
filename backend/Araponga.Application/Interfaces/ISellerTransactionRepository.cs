using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface ISellerTransactionRepository
{
    Task<SellerTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<SellerTransaction?> GetByCheckoutIdAsync(Guid checkoutId, CancellationToken cancellationToken);
    Task<List<SellerTransaction>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<List<SellerTransaction>> GetBySellerUserIdAsync(Guid sellerUserId, CancellationToken cancellationToken);
    Task<List<SellerTransaction>> GetByStatusAsync(Guid territoryId, SellerTransactionStatus status, CancellationToken cancellationToken);
    Task<List<SellerTransaction>> GetReadyForPayoutAsync(Guid territoryId, CancellationToken cancellationToken);
    Task AddAsync(SellerTransaction transaction, CancellationToken cancellationToken);
    Task UpdateAsync(SellerTransaction transaction, CancellationToken cancellationToken);
}
