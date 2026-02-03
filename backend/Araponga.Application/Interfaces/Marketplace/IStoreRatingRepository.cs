using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface IStoreRatingRepository
{
    Task<StoreRating?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<StoreRating?> GetByStoreAndUserAsync(Guid storeId, Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<StoreRating>> ListByStoreIdAsync(
        Guid storeId,
        CancellationToken cancellationToken);
    Task AddAsync(StoreRating rating, CancellationToken cancellationToken);
    Task UpdateAsync(StoreRating rating, CancellationToken cancellationToken);
    Task<double> GetAverageRatingAsync(Guid storeId, CancellationToken cancellationToken);
}
