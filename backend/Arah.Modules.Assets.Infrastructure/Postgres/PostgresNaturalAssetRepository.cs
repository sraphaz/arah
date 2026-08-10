using Arah.Modules.Assets.Application.Interfaces;
using Arah.Modules.Assets.Domain;
using Arah.Modules.Assets.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Modules.Assets.Infrastructure.Postgres;

public sealed class PostgresNaturalAssetRepository : INaturalAssetRepository
{
    private readonly AssetsDbContext _dbContext;

    public PostgresNaturalAssetRepository(AssetsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NaturalAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.NaturalAssets.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<NaturalAsset>> ListPagedAsync(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var records = await BuildQuery(territoryId, status, types)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ThenBy(a => a.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task<int> CountAsync(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types,
        CancellationToken cancellationToken)
    {
        return BuildQuery(territoryId, status, types).CountAsync(cancellationToken);
    }

    public async Task AddAsync(NaturalAsset asset, CancellationToken cancellationToken)
    {
        await _dbContext.NaturalAssets.AddAsync(asset.ToRecord(), cancellationToken);
    }

    public async Task<bool> UpdateAsync(NaturalAsset asset, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.NaturalAssets.FirstOrDefaultAsync(a => a.Id == asset.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        var record = asset.ToRecord();
        existing.Type = record.Type;
        existing.Name = record.Name;
        existing.Description = record.Description;
        existing.Status = record.Status;
        existing.Latitude = record.Latitude;
        existing.Longitude = record.Longitude;
        existing.WaterType = record.WaterType;
        existing.PotabilityNotes = record.PotabilityNotes;
        existing.LastTestedAtUtc = record.LastTestedAtUtc;
        existing.UpdatedByUserId = record.UpdatedByUserId;
        existing.UpdatedAtUtc = record.UpdatedAtUtc;
        return true;
    }

    private IQueryable<NaturalAssetRecord> BuildQuery(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types)
    {
        var query = _dbContext.NaturalAssets.AsNoTracking()
            .Where(a => a.TerritoryId == territoryId);

        if (status is not null)
        {
            query = query.Where(a => a.Status == status);
        }

        if (types is { Count: > 0 })
        {
            var normalized = types
                .Select(NaturalAssetType.Normalize)
                .Where(t => t is not null)
                .Cast<string>()
                .ToHashSet(StringComparer.Ordinal);
            if (normalized.Count > 0)
            {
                query = query.Where(a => normalized.Contains(a.Type));
            }
        }

        return query;
    }
}
