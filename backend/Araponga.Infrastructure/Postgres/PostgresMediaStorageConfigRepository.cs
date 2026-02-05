using Araponga.Application.Interfaces.Media;
using Araponga.Domain.Media;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresMediaStorageConfigRepository : IMediaStorageConfigRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresMediaStorageConfigRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MediaStorageConfig?> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.MediaStorageConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IsActive, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<MediaStorageConfig?> GetByIdAsync(Guid configId, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.MediaStorageConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == configId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<MediaStorageConfig>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.MediaStorageConfigs
            .AsNoTracking()
            .OrderBy(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task AddAsync(MediaStorageConfig config, CancellationToken cancellationToken = default)
    {
        var record = config.ToRecord();
        _dbContext.MediaStorageConfigs.Add(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MediaStorageConfig config, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.MediaStorageConfigs.FirstOrDefaultAsync(c => c.Id == config.Id, cancellationToken);
        if (record is null)
        {
            _dbContext.MediaStorageConfigs.Add(config.ToRecord());
        }
        else
        {
            var updated = config.ToRecord();
            record.Provider = updated.Provider;
            record.SettingsJson = updated.SettingsJson;
            record.IsActive = updated.IsActive;
            record.Description = updated.Description;
            record.UpdatedAtUtc = updated.UpdatedAtUtc;
            record.UpdatedByUserId = updated.UpdatedByUserId;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAllAsync(CancellationToken cancellationToken = default)
    {
        var active = await _dbContext.MediaStorageConfigs.Where(c => c.IsActive).ToListAsync(cancellationToken);
        foreach (var record in active)
        {
            record.IsActive = false;
            record.UpdatedAtUtc = DateTime.UtcNow;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
