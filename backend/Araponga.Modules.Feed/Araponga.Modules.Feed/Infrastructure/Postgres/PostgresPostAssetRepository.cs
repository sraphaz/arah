using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;
using Araponga.Modules.Feed.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Feed.Infrastructure.Postgres;

public sealed class PostgresPostAssetRepository : IPostAssetRepository
{
    private readonly FeedDbContext _dbContext;

    public PostgresPostAssetRepository(FeedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(IReadOnlyCollection<PostAsset> postAssets, CancellationToken cancellationToken)
    {
        if (postAssets.Count == 0)
        {
            return Task.CompletedTask;
        }

        _dbContext.PostAssets.AddRange(postAssets.Select(pa => pa.ToRecord()));
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Guid>> ListPostIdsByAssetIdAsync(Guid assetId, CancellationToken cancellationToken)
    {
        return await _dbContext.PostAssets
            .AsNoTracking()
            .Where(pa => pa.AssetId == assetId)
            .Select(pa => pa.PostId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
