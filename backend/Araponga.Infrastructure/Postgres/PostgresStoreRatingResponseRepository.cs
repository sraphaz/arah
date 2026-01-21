using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresStoreRatingResponseRepository : IStoreRatingResponseRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresStoreRatingResponseRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreRatingResponse?> GetByRatingIdAsync(Guid ratingId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.StoreRatingResponses
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RatingId == ratingId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task AddAsync(StoreRatingResponse response, CancellationToken cancellationToken)
    {
        _dbContext.StoreRatingResponses.Add(response.ToRecord());
        await Task.CompletedTask;
    }
}
