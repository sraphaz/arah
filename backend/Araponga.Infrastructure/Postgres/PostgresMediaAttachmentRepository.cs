using Araponga.Application.Interfaces.Media;
using Araponga.Domain.Media;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresMediaAttachmentRepository : IMediaAttachmentRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresMediaAttachmentRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(MediaAttachment attachment, CancellationToken cancellationToken = default)
    {
        _dbContext.MediaAttachments.Add(attachment.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<MediaAttachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.MediaAttachments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<MediaAttachment>> ListByOwnerAsync(MediaOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.MediaAttachments
            .AsNoTracking()
            .Where(a => a.OwnerType == ownerType && a.OwnerId == ownerId)
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<MediaAttachment>> ListByMediaAssetIdAsync(Guid mediaAssetId, CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.MediaAttachments
            .AsNoTracking()
            .Where(a => a.MediaAssetId == mediaAssetId)
            .OrderBy(a => a.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<MediaAttachment>> ListByOwnersAsync(MediaOwnerType ownerType, IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default)
    {
        if (ownerIds.Count == 0)
        {
            return Array.Empty<MediaAttachment>();
        }

        var records = await _dbContext.MediaAttachments
            .AsNoTracking()
            .Where(a => a.OwnerType == ownerType && ownerIds.Contains(a.OwnerId))
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task UpdateAsync(MediaAttachment attachment, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.MediaAttachments
            .FirstOrDefaultAsync(a => a.Id == attachment.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.DisplayOrder = attachment.DisplayOrder;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var record = await _dbContext.MediaAttachments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (record is not null)
        {
            _dbContext.MediaAttachments.Remove(record);
        }
    }

    public async Task DeleteByOwnerAsync(MediaOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.MediaAttachments
            .Where(a => a.OwnerType == ownerType && a.OwnerId == ownerId)
            .ToListAsync(cancellationToken);

        _dbContext.MediaAttachments.RemoveRange(records);
    }

    public async Task DeleteByMediaAssetIdAsync(Guid mediaAssetId, CancellationToken cancellationToken = default)
    {
        var records = await _dbContext.MediaAttachments
            .Where(a => a.MediaAssetId == mediaAssetId)
            .ToListAsync(cancellationToken);

        _dbContext.MediaAttachments.RemoveRange(records);
    }
}