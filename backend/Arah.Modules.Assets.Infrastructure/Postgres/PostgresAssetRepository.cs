using Arah.Modules.Assets.Application.Interfaces;
using Arah.Modules.Assets.Domain;
using Arah.Modules.Assets.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arah.Modules.Assets.Infrastructure.Postgres;

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
        CancellationToken cancellationToken,
        IReadOnlyCollection<string>? subtypes = null,
        IReadOnlyCollection<string>? typesOrSubtypes = null)
    {
        var query = BuildQuery(territoryId, assetId, types, status, search, subtypes, typesOrSubtypes);
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
        record.Subtype = asset.Subtype;
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
        CancellationToken cancellationToken,
        IReadOnlyCollection<string>? subtypes = null,
        IReadOnlyCollection<string>? typesOrSubtypes = null)
    {
        var query = BuildQuery(territoryId, assetId, types, status, search, subtypes, typesOrSubtypes);
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
        CancellationToken cancellationToken,
        IReadOnlyCollection<string>? subtypes = null,
        IReadOnlyCollection<string>? typesOrSubtypes = null)
    {
        var query = BuildQuery(territoryId, assetId, types, status, search, subtypes, typesOrSubtypes);
        const int maxInt32 = int.MaxValue;
        var count = await query.CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }

    private IQueryable<TerritoryAssetRecord> BuildQuery(
        Guid territoryId,
        Guid? assetId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        IReadOnlyCollection<string>? subtypes,
        IReadOnlyCollection<string>? typesOrSubtypes)
    {
        var query = _dbContext.TerritoryAssets.AsNoTracking()
            .Where(asset => asset.TerritoryId == territoryId);

        query = ApplyIdentityFilter(query, assetId);
        query = ApplyClassificationFilters(query, types, subtypes, typesOrSubtypes);
        query = ApplyStatusFilter(query, status);
        query = ApplySearchFilter(query, search);
        return query;
    }

    private static IQueryable<TerritoryAssetRecord> ApplyIdentityFilter(
        IQueryable<TerritoryAssetRecord> query,
        Guid? assetId)
    {
        if (assetId is null)
        {
            return query;
        }

        return query.Where(asset => asset.Id == assetId);
    }

    private static IQueryable<TerritoryAssetRecord> ApplyClassificationFilters(
        IQueryable<TerritoryAssetRecord> query,
        IReadOnlyCollection<string>? types,
        IReadOnlyCollection<string>? subtypes,
        IReadOnlyCollection<string>? typesOrSubtypes)
    {
        var normalizedTypes = TerritoryAssetTypeMatch.NormalizeFilter(types);
        var normalizedSubtypes = TerritoryAssetTypeMatch.NormalizeFilter(subtypes);
        var normalizedTypesOrSubtypes = TerritoryAssetTypeMatch.NormalizeFilter(typesOrSubtypes);

        if (normalizedTypes is not null)
        {
            var typeList = normalizedTypes.ToList();
            query = query.Where(asset => typeList.Contains(asset.Type));
        }

        if (normalizedSubtypes is not null)
        {
            var subtypeList = normalizedSubtypes.ToList();
            query = query.Where(asset => asset.Subtype != null && subtypeList.Contains(asset.Subtype));
        }

        if (normalizedTypesOrSubtypes is not null)
        {
            // Mapa assetTypes: legado type=river OU ponte natural+subtype.
            var keys = normalizedTypesOrSubtypes.ToList();
            query = query.Where(asset =>
                keys.Contains(asset.Type) ||
                (asset.Subtype != null && keys.Contains(asset.Subtype)));
        }

        return query;
    }

    private static IQueryable<TerritoryAssetRecord> ApplyStatusFilter(
        IQueryable<TerritoryAssetRecord> query,
        AssetStatus? status)
    {
        if (status is null)
        {
            return query;
        }

        return query.Where(asset => asset.Status == status);
    }

    private static IQueryable<TerritoryAssetRecord> ApplySearchFilter(
        IQueryable<TerritoryAssetRecord> query,
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var pattern = $"%{search}%";
        return query.Where(asset =>
            EF.Functions.ILike(asset.Name, pattern) ||
            (asset.Description != null && EF.Functions.ILike(asset.Description, pattern)));
    }
}
