using Araponga.Application.Interfaces.Media;
using Araponga.Domain.Media;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryMediaAttachmentRepository : IMediaAttachmentRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryMediaAttachmentRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(MediaAttachment attachment, CancellationToken cancellationToken = default)
    {
        _dataStore.MediaAttachments.Add(attachment);
        return Task.CompletedTask;
    }

    public Task<MediaAttachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var attachment = _dataStore.MediaAttachments.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(attachment);
    }

    public Task<IReadOnlyList<MediaAttachment>> ListByOwnerAsync(MediaOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var attachments = _dataStore.MediaAttachments
            .Where(a => a.OwnerType == ownerType && a.OwnerId == ownerId)
            .OrderBy(a => a.DisplayOrder)
            .ToList();
        return Task.FromResult<IReadOnlyList<MediaAttachment>>(attachments);
    }

    public Task<IReadOnlyList<MediaAttachment>> ListByMediaAssetIdAsync(Guid mediaAssetId, CancellationToken cancellationToken = default)
    {
        var attachments = _dataStore.MediaAttachments
            .Where(a => a.MediaAssetId == mediaAssetId)
            .OrderBy(a => a.CreatedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<MediaAttachment>>(attachments);
    }

    public Task<IReadOnlyList<MediaAttachment>> ListByOwnersAsync(MediaOwnerType ownerType, IReadOnlyCollection<Guid> ownerIds, CancellationToken cancellationToken = default)
    {
        if (ownerIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<MediaAttachment>>(Array.Empty<MediaAttachment>());
        }

        var attachments = _dataStore.MediaAttachments
            .Where(a => a.OwnerType == ownerType && ownerIds.Contains(a.OwnerId))
            .OrderBy(a => a.DisplayOrder)
            .ToList();
        return Task.FromResult<IReadOnlyList<MediaAttachment>>(attachments);
    }

    public Task UpdateAsync(MediaAttachment attachment, CancellationToken cancellationToken = default)
    {
        var existing = _dataStore.MediaAttachments.FirstOrDefault(a => a.Id == attachment.Id);
        if (existing is null)
        {
            return Task.CompletedTask;
        }

        var index = _dataStore.MediaAttachments.IndexOf(existing);
        if (index >= 0)
        {
            _dataStore.MediaAttachments[index] = attachment;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var attachment = _dataStore.MediaAttachments.FirstOrDefault(a => a.Id == id);
        if (attachment is not null)
        {
            _dataStore.MediaAttachments.Remove(attachment);
        }

        return Task.CompletedTask;
    }

    public Task DeleteByOwnerAsync(MediaOwnerType ownerType, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var attachments = _dataStore.MediaAttachments
            .Where(a => a.OwnerType == ownerType && a.OwnerId == ownerId)
            .ToList();

        foreach (var attachment in attachments)
        {
            _dataStore.MediaAttachments.Remove(attachment);
        }

        return Task.CompletedTask;
    }

    public Task DeleteByMediaAssetIdAsync(Guid mediaAssetId, CancellationToken cancellationToken = default)
    {
        var attachments = _dataStore.MediaAttachments
            .Where(a => a.MediaAssetId == mediaAssetId)
            .ToList();

        foreach (var attachment in attachments)
        {
            _dataStore.MediaAttachments.Remove(attachment);
        }

        return Task.CompletedTask;
    }
}