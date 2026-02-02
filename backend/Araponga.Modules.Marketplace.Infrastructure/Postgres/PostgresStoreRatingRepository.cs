using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres;

public sealed class PostgresStoreRatingRepository : IStoreRatingRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public PostgresStoreRatingRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreRating?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.StoreRatings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<StoreRating?> GetByStoreAndUserAsync(Guid storeId, Guid userId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.StoreRatings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.StoreId == storeId && r.UserId == userId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<StoreRating>> ListByStoreIdAsync(
        Guid storeId,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.StoreRatings
            .AsNoTracking()
            .Where(r => r.StoreId == storeId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task AddAsync(StoreRating rating, CancellationToken cancellationToken)
    {
        _dbContext.StoreRatings.Add(rating.ToRecord());
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(StoreRating rating, CancellationToken cancellationToken)
    {
        var record = await _dbContext.StoreRatings
            .FirstOrDefaultAsync(r => r.Id == rating.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Rating = rating.Rating;
        record.Comment = rating.Comment;
        record.UpdatedAtUtc = rating.UpdatedAtUtc;
    }

    public async Task<double> GetAverageRatingAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var average = await _dbContext.StoreRatings
            .AsNoTracking()
            .Where(r => r.StoreId == storeId)
            .AverageAsync(r => (double?)r.Rating, cancellationToken);

        return average ?? 0.0;
    }
}
