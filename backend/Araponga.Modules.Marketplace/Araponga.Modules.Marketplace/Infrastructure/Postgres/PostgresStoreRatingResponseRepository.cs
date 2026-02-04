using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres;

public sealed class PostgresStoreRatingResponseRepository : IStoreRatingResponseRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public PostgresStoreRatingResponseRepository(MarketplaceDbContext dbContext)
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
