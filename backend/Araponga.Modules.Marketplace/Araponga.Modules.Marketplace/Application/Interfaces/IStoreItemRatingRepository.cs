using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Application.Interfaces;

public interface IStoreItemRatingRepository
{
    Task<StoreItemRating?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<StoreItemRating?> GetByItemAndUserAsync(Guid storeItemId, Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<StoreItemRating>> ListByItemIdAsync(
        Guid storeItemId,
        CancellationToken cancellationToken);
    Task AddAsync(StoreItemRating rating, CancellationToken cancellationToken);
    Task UpdateAsync(StoreItemRating rating, CancellationToken cancellationToken);
    Task<double> GetAverageRatingAsync(Guid storeItemId, CancellationToken cancellationToken);
}
