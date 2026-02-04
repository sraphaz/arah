using Araponga.Modules.Assets.Application.Interfaces;
using Araponga.Modules.Assets.Domain;
using Araponga.Modules.Assets.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Assets.Infrastructure.Postgres;

public sealed class PostgresAssetRepository : ITerritoryAssetRepository
{
    private readonly AssetsDbContext _dbContext;

    public PostgresAssetRepository(AssetsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<TerritoryAsset>> ListAsync(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        IQueryable<TerritoryAssetRecord> query = _dbContext.TerritoryAssets.AsNoTracking()
            .Where(asset => asset.TerritoryId == territoryId);

        if (assetId is not null)
        {
            query = query.Where(asset => asset.Id == assetId);
        }

        if (types is not null && types.Count > 0)
        {
            query = query.Where(asset => types.Contains(asset.Type));
        }

        if (status is not null)
        {
            query = query.Where(asset => asset.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(asset => EF.Functions.ILike(asset.Name, pattern) ||
                                         (asset.Description != null && EF.Functions.ILike(asset.Description, pattern)));
        }

        var records = await query.ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<TerritoryAsset>> ListByIdsAsync(
        IReadOnlyCollection<Guid> assetIds,
        CancellationToken cancellationToken)
    {
        if (assetIds.Count == 0)
        {
            return Array.Empty<TerritoryAsset>();
        }

        var records = await _dbContext.TerritoryAssets
            .AsNoTracking()
            .Where(asset => assetIds.Contains(asset.Id))
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<TerritoryAsset?> GetByIdAsync(Guid assetId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryAssets
            .AsNoTracking()
            .FirstOrDefaultAsync(asset => asset.Id == assetId, cancellationToken);
        return record?.ToDomain();
    }

    public Task AddAsync(TerritoryAsset asset, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryAssets.Add(asset.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(TerritoryAsset asset, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryAssets
            .FirstOrDefaultAsync(existing => existing.Id == asset.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Type = asset.Type;
        record.Name = asset.Name;
        record.Description = asset.Description;
        record.Status = asset.Status;
        record.UpdatedByUserId = asset.UpdatedByUserId;
        record.UpdatedAtUtc = asset.UpdatedAtUtc;
        record.ArchivedByUserId = asset.ArchivedByUserId;
        record.ArchivedAtUtc = asset.ArchivedAtUtc;
        record.ArchiveReason = asset.ArchiveReason;
    }

    public async Task<IReadOnlyList<TerritoryAsset>> ListPagedAsync(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        IQueryable<TerritoryAssetRecord> query = _dbContext.TerritoryAssets.AsNoTracking()
            .Where(asset => asset.TerritoryId == territoryId);

        if (assetId is not null)
        {
            query = query.Where(asset => asset.Id == assetId);
        }

        if (types is not null && types.Count > 0)
        {
            query = query.Where(asset => types.Contains(asset.Type));
        }

        if (status is not null)
        {
            query = query.Where(asset => asset.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(asset => EF.Functions.ILike(asset.Name, pattern) ||
                                         (asset.Description != null && EF.Functions.ILike(asset.Description, pattern)));
        }

        var records = await query
            .OrderByDescending(asset => asset.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<int> CountAsync(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        IQueryable<TerritoryAssetRecord> query = _dbContext.TerritoryAssets
            .Where(asset => asset.TerritoryId == territoryId);

        if (assetId is not null)
        {
            query = query.Where(asset => asset.Id == assetId);
        }

        if (types is not null && types.Count > 0)
        {
            query = query.Where(asset => types.Contains(asset.Type));
        }

        if (status is not null)
        {
            query = query.Where(asset => asset.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(asset => EF.Functions.ILike(asset.Name, pattern) ||
                                         (asset.Description != null && EF.Functions.ILike(asset.Description, pattern)));
        }

        const int maxInt32 = int.MaxValue;
        var count = await query.CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }
}
