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
    
    /// <summary>
    /// Lists territories with pagination.
    /// </summary>
    Task<IReadOnlyList<Territory>> ListPagedAsync(
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Searches territories with pagination.
    /// </summary>
    Task<IReadOnlyList<Territory>> SearchPagedAsync(
        string? query,
        string? city,
        string? state,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Finds nearby territories with pagination.
    /// </summary>
    Task<IReadOnlyList<Territory>> NearbyPagedAsync(
        double latitude,
        double longitude,
        double radiusKm,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts all territories.
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts territories matching search criteria.
    /// </summary>
    Task<int> CountSearchAsync(
        string? query,
        string? city,
        string? state,
        CancellationToken cancellationToken);
}
