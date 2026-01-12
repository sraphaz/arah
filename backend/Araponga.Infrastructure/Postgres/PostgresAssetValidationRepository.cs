using Araponga.Application.Interfaces;
using Araponga.Domain.Assets;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresAssetValidationRepository : IAssetValidationRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresAssetValidationRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(Guid assetId, Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.AssetValidations
            .AsNoTracking()
            .AnyAsync(validation => validation.AssetId == assetId && validation.UserId == userId, cancellationToken);
    }

    public Task AddAsync(AssetValidation validation, CancellationToken cancellationToken)
    {
        _dbContext.AssetValidations.Add(new Entities.AssetValidationRecord
        {
            AssetId = validation.AssetId,
            UserId = validation.UserId,
            CreatedAtUtc = validation.CreatedAtUtc
        });
        return Task.CompletedTask;
    }

    public Task<int> CountByAssetIdAsync(Guid assetId, CancellationToken cancellationToken)
    {
        return _dbContext.AssetValidations
            .AsNoTracking()
            .CountAsync(validation => validation.AssetId == assetId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, int>> CountByAssetIdsAsync(
        IReadOnlyCollection<Guid> assetIds,
        CancellationToken cancellationToken)
    {
        if (assetIds.Count == 0)
        {
            return new Dictionary<Guid, int>();
        }

        var counts = await _dbContext.AssetValidations
            .AsNoTracking()
            .Where(validation => assetIds.Contains(validation.AssetId))
            .GroupBy(validation => validation.AssetId)
            .Select(group => new { group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        return counts.ToDictionary(item => item.Key, item => item.Count);
    }
}
