using Araponga.Application.Interfaces.Media;
using Araponga.Domain.Media;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresMediaAssetRepository : IMediaAssetRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresMediaAssetRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default)
    {
        _dbContext.MediaAssets.Add(mediaAsset.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.MediaAssets
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.DeletedAtUtc == null, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<MediaAsset>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.MediaAssets
            .AsNoTracking()
            .Where(m => m.UploadedByUserId == userId && m.DeletedAtUtc == null)
            .OrderByDescending(m => m.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<MediaAsset>> ListByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return Array.Empty<MediaAsset>();
        }

        var records = await _dbContext.MediaAssets
            .AsNoTracking()
            .Where(m => ids.Contains(m.Id) && m.DeletedAtUtc == null)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task UpdateAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.MediaAssets
            .FirstOrDefaultAsync(m => m.Id == mediaAsset.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.DeletedByUserId = mediaAsset.DeletedByUserId;
        record.DeletedAtUtc = mediaAsset.DeletedAtUtc;
    }

    public async Task<IReadOnlyList<MediaAsset>> ListDeletedAsync(CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.MediaAssets
            .AsNoTracking()
            .Where(m => m.DeletedAtUtc != null)
            .OrderByDescending(m => m.DeletedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }
}