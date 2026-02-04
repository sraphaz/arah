using Araponga.Application.Interfaces;
using Araponga.Modules.Map.Domain;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryPostGeoAnchorRepository : IPostGeoAnchorRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryPostGeoAnchorRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(IReadOnlyCollection<PostGeoAnchor> anchors, CancellationToken cancellationToken)
    {
        if (anchors.Count > 0)
        {
            _dataStore.PostGeoAnchors.AddRange(anchors);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<PostGeoAnchor>> ListByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken)
    {
        if (postIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<PostGeoAnchor>>(Array.Empty<PostGeoAnchor>());
        }

        var anchors = _dataStore.PostGeoAnchors
            .Where(anchor => postIds.Contains(anchor.PostId))
            .ToList();

        return Task.FromResult<IReadOnlyList<PostGeoAnchor>>(anchors);
    }

    public Task DeleteByPostIdAsync(Guid postId, CancellationToken cancellationToken)
    {
        _dataStore.PostGeoAnchors.RemoveAll(anchor => anchor.PostId == postId);
        return Task.CompletedTask;
    }
}
