using Araponga.Domain.Territories;

namespace Araponga.Application.Interfaces;

public interface ITerritoryRepository
{
    Task<IReadOnlyList<Territory>> ListAsync(CancellationToken cancellationToken);
    Task<Territory?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Territory territory, CancellationToken cancellationToken);
    Task<IReadOnlyList<Territory>> SearchAsync(string? query, string? city, string? state, CancellationToken cancellationToken);
    Task<IReadOnlyList<Territory>> NearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        int limit,
        CancellationToken cancellationToken);
}
