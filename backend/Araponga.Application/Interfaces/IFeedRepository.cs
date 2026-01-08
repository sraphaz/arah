using Araponga.Domain.Feed;

namespace Araponga.Application.Interfaces;

public interface IFeedRepository
{
    Task<IReadOnlyList<CommunityPost>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
}
