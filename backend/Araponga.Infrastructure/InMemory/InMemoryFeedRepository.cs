using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryFeedRepository : IFeedRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryFeedRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<IReadOnlyList<CommunityPost>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var posts = _dataStore.Posts
            .Where(post => post.TerritoryId == territoryId)
            .ToList();

        return Task.FromResult<IReadOnlyList<CommunityPost>>(posts);
    }
}
