using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface IStoreRatingResponseRepository
{
    Task<StoreRatingResponse?> GetByRatingIdAsync(Guid ratingId, CancellationToken cancellationToken);
    Task AddAsync(StoreRatingResponse response, CancellationToken cancellationToken);
}
