using Araponga.Application.Interfaces;
using Araponga.Domain.Assets;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Assets.Infrastructure.Postgres;

public sealed class PostgresAssetGeoAnchorRepository : IAssetGeoAnchorRepository
{
    private readonly AssetsDbContext _dbContext;

    public PostgresAssetGeoAnchorRepository(AssetsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<AssetGeoAnchor>> ListByAssetIdsAsync(
        IReadOnlyCollection<Guid> assetIds,
        CancellationToken cancellationToken)
    {
        if (assetIds.Count == 0)
        {
            return Array.Empty<AssetGeoAnchor>();
        }

        var records = await _dbContext.AssetGeoAnchors
            .AsNoTracking()
            .Where(anchor => assetIds.Contains(anchor.AssetId))
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public Task AddAsync(IReadOnlyCollection<AssetGeoAnchor> anchors, CancellationToken cancellationToken)
    {
        _dbContext.AssetGeoAnchors.AddRange(anchors.Select(anchor => anchor.ToRecord()));
        return Task.CompletedTask;
    }

    public async Task ReplaceForAssetAsync(
        Guid assetId,
        IReadOnlyCollection<AssetGeoAnchor> anchors,
        CancellationToken cancellationToken)
    {
        var existing = await _dbContext.AssetGeoAnchors
            .Where(anchor => anchor.AssetId == assetId)
            .ToListAsync(cancellationToken);

        if (existing.Count > 0)
        {
            _dbContext.AssetGeoAnchors.RemoveRange(existing);
        }

        _dbContext.AssetGeoAnchors.AddRange(anchors.Select(anchor => anchor.ToRecord()));
    }
}
