using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresStoreItemRatingRepository : IStoreItemRatingRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresStoreItemRatingRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreItemRating?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.StoreItemRatings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<StoreItemRating?> GetByItemAndUserAsync(Guid storeItemId, Guid userId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.StoreItemRatings
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.StoreItemId == storeItemId && r.UserId == userId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<StoreItemRating>> ListByItemIdAsync(
        Guid storeItemId,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.StoreItemRatings
            .AsNoTracking()
            .Where(r => r.StoreItemId == storeItemId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task AddAsync(StoreItemRating rating, CancellationToken cancellationToken)
    {
        _dbContext.StoreItemRatings.Add(rating.ToRecord());
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(StoreItemRating rating, CancellationToken cancellationToken)
    {
        var record = await _dbContext.StoreItemRatings
            .FirstOrDefaultAsync(r => r.Id == rating.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Rating = rating.Rating;
        record.Comment = rating.Comment;
        record.UpdatedAtUtc = rating.UpdatedAtUtc;
    }

    public async Task<double> GetAverageRatingAsync(Guid storeItemId, CancellationToken cancellationToken)
    {
        var average = await _dbContext.StoreItemRatings
            .AsNoTracking()
            .Where(r => r.StoreItemId == storeItemId)
            .AverageAsync(r => (double?)r.Rating, cancellationToken);

        return average ?? 0.0;
    }
}
