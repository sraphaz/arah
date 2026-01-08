using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;

namespace Araponga.Application.Services;

public sealed class FeedService
{
    private readonly IFeedRepository _feedRepository;
    private readonly AccessEvaluator _accessEvaluator;

    public FeedService(IFeedRepository feedRepository, AccessEvaluator accessEvaluator)
    {
        _feedRepository = feedRepository;
        _accessEvaluator = accessEvaluator;
    }

    public async Task<IReadOnlyList<CommunityPost>> ListForTerritoryAsync(
        Guid territoryId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var posts = await _feedRepository.ListByTerritoryAsync(territoryId, cancellationToken);

        if (userId is null)
        {
            return posts
                .Where(post => post.Visibility == PostVisibility.Public)
                .ToList();
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId.Value, territoryId, cancellationToken);

        return isResident
            ? posts.ToList()
            : posts.Where(post => post.Visibility == PostVisibility.Public).ToList();
    }
}
