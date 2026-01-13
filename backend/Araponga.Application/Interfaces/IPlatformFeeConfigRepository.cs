using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface IPlatformFeeConfigRepository
{
    Task<PlatformFeeConfig?> GetActiveAsync(Guid territoryId, ListingType listingType, CancellationToken cancellationToken);
    Task<IReadOnlyList<PlatformFeeConfig>> ListActiveAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<PlatformFeeConfig?> GetByIdAsync(Guid configId, CancellationToken cancellationToken);
    Task AddAsync(PlatformFeeConfig config, CancellationToken cancellationToken);
    Task UpdateAsync(PlatformFeeConfig config, CancellationToken cancellationToken);
}
