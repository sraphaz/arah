using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryStoreRatingRepository : IStoreRatingRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryStoreRatingRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<StoreRating?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var rating = _dataStore.StoreRatings.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(rating);
    }

    public Task<StoreRating?> GetByStoreAndUserAsync(Guid storeId, Guid userId, CancellationToken cancellationToken)
    {
        var rating = _dataStore.StoreRatings
            .FirstOrDefault(r => r.StoreId == storeId && r.UserId == userId);
        return Task.FromResult(rating);
    }

    public Task<IReadOnlyList<StoreRating>> ListByStoreIdAsync(
        Guid storeId,
        CancellationToken cancellationToken)
    {
        var ratings = _dataStore.StoreRatings
            .Where(r => r.StoreId == storeId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<StoreRating>>(ratings);
    }

    public Task AddAsync(StoreRating rating, CancellationToken cancellationToken)
    {
        _dataStore.StoreRatings.Add(rating);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(StoreRating rating, CancellationToken cancellationToken)
    {
        // In-memory: referência já é a mesma; nada a fazer.
        return Task.CompletedTask;
    }

    public Task<double> GetAverageRatingAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var ratings = _dataStore.StoreRatings
            .Where(r => r.StoreId == storeId)
            .ToList();

        if (ratings.Count == 0)
        {
            return Task.FromResult(0.0);
        }

        var average = ratings.Average(r => r.Rating);
        return Task.FromResult(average);
    }
}
