using Araponga.Domain.Map;

namespace Araponga.Application.Interfaces;

public interface IPostGeoAnchorRepository
{
    Task AddAsync(IReadOnlyCollection<PostGeoAnchor> anchors, CancellationToken cancellationToken);
    Task<IReadOnlyList<PostGeoAnchor>> ListByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken);
}
