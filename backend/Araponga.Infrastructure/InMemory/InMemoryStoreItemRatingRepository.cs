using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryStoreItemRatingRepository : IStoreItemRatingRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryStoreItemRatingRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<StoreItemRating?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var rating = _dataStore.StoreItemRatings.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(rating);
    }

    public Task<StoreItemRating?> GetByItemAndUserAsync(Guid storeItemId, Guid userId, CancellationToken cancellationToken)
    {
        var rating = _dataStore.StoreItemRatings
            .FirstOrDefault(r => r.StoreItemId == storeItemId && r.UserId == userId);
        return Task.FromResult(rating);
    }

    public Task<IReadOnlyList<StoreItemRating>> ListByItemIdAsync(
        Guid storeItemId,
        CancellationToken cancellationToken)
    {
        var ratings = _dataStore.StoreItemRatings
            .Where(r => r.StoreItemId == storeItemId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<StoreItemRating>>(ratings);
    }

    public Task AddAsync(StoreItemRating rating, CancellationToken cancellationToken)
    {
        _dataStore.StoreItemRatings.Add(rating);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(StoreItemRating rating, CancellationToken cancellationToken)
    {
        // In-memory: referência já é a mesma; nada a fazer.
        return Task.CompletedTask;
    }

    public Task<double> GetAverageRatingAsync(Guid storeItemId, CancellationToken cancellationToken)
    {
        var ratings = _dataStore.StoreItemRatings
            .Where(r => r.StoreItemId == storeItemId)
            .ToList();

        if (ratings.Count == 0)
        {
            return Task.FromResult(0.0);
        }

        var average = ratings.Average(r => r.Rating);
        return Task.FromResult(average);
    }
}
