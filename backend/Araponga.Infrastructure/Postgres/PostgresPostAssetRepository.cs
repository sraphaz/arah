using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresPostAssetRepository : IPostAssetRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresPostAssetRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(IReadOnlyCollection<PostAsset> postAssets, CancellationToken cancellationToken)
    {
        if (postAssets.Count == 0)
        {
            return Task.CompletedTask;
        }

        _dbContext.PostAssets.AddRange(postAssets.Select(postAsset => new Entities.PostAssetRecord
        {
            PostId = postAsset.PostId,
            AssetId = postAsset.AssetId
        }));
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Guid>> ListPostIdsByAssetIdAsync(Guid assetId, CancellationToken cancellationToken)
    {
        return await _dbContext.PostAssets
            .AsNoTracking()
            .Where(postAsset => postAsset.AssetId == assetId)
            .Select(postAsset => postAsset.PostId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
