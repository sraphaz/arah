using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Application.Interfaces;

public interface IStoreRatingResponseRepository
{
    Task<StoreRatingResponse?> GetByRatingIdAsync(Guid ratingId, CancellationToken cancellationToken);
    Task AddAsync(StoreRatingResponse response, CancellationToken cancellationToken);
}
