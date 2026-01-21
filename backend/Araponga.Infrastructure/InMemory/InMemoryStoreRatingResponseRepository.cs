using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryStoreRatingResponseRepository : IStoreRatingResponseRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryStoreRatingResponseRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<StoreRatingResponse?> GetByRatingIdAsync(Guid ratingId, CancellationToken cancellationToken)
    {
        var response = _dataStore.StoreRatingResponses
            .FirstOrDefault(r => r.RatingId == ratingId);
        return Task.FromResult(response);
    }

    public Task AddAsync(StoreRatingResponse response, CancellationToken cancellationToken)
    {
        _dataStore.StoreRatingResponses.Add(response);
        return Task.CompletedTask;
    }
}
