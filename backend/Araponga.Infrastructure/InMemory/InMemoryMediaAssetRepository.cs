using Araponga.Application.Interfaces.Media;
using Araponga.Domain.Media;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryMediaAssetRepository : IMediaAssetRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryMediaAssetRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default)
    {
        _dataStore.MediaAssets.Add(mediaAsset);
        return Task.CompletedTask;
    }

    public Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mediaAsset = _dataStore.MediaAssets.FirstOrDefault(m => m.Id == id && !m.IsDeleted);
        return Task.FromResult(mediaAsset);
    }

    public Task<IReadOnlyList<MediaAsset>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var mediaAssets = _dataStore.MediaAssets
            .Where(m => m.UploadedByUserId == userId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<MediaAsset>>(mediaAssets);
    }

    public Task<IReadOnlyList<MediaAsset>> ListByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<MediaAsset>>(Array.Empty<MediaAsset>());
        }

        var mediaAssets = _dataStore.MediaAssets
            .Where(m => ids.Contains(m.Id) && !m.IsDeleted)
            .ToList();
        return Task.FromResult<IReadOnlyList<MediaAsset>>(mediaAssets);
    }

    public Task UpdateAsync(MediaAsset mediaAsset, CancellationToken cancellationToken = default)
    {
        var existing = _dataStore.MediaAssets.FirstOrDefault(m => m.Id == mediaAsset.Id);
        if (existing is null)
        {
            return Task.CompletedTask;
        }

        var index = _dataStore.MediaAssets.IndexOf(existing);
        if (index >= 0)
        {
            _dataStore.MediaAssets[index] = mediaAsset;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<MediaAsset>> ListDeletedAsync(CancellationToken cancellationToken = default)
    {
        var mediaAssets = _dataStore.MediaAssets
            .Where(m => m.IsDeleted)
            .OrderByDescending(m => m.DeletedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<MediaAsset>>(mediaAssets);
    }
}