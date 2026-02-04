using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Application.Interfaces;

public interface IPlatformFeeConfigRepository
{
    Task<PlatformFeeConfig?> GetActiveAsync(Guid territoryId, ItemType itemType, CancellationToken cancellationToken);
    Task<IReadOnlyList<PlatformFeeConfig>> ListActiveAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<PlatformFeeConfig?> GetByIdAsync(Guid configId, CancellationToken cancellationToken);
    Task AddAsync(PlatformFeeConfig config, CancellationToken cancellationToken);
    Task UpdateAsync(PlatformFeeConfig config, CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists active fee configs with pagination.
    /// </summary>
    Task<IReadOnlyList<PlatformFeeConfig>> ListActivePagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts active fee configs.
    /// </summary>
    Task<int> CountActiveAsync(Guid territoryId, CancellationToken cancellationToken);
}
