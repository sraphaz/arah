using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Application.Services;

public sealed class FeatureFlagService
{
    private readonly IFeatureFlagService _featureFlagRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FeatureFlagCacheService? _cacheService;

    public FeatureFlagService(
        IFeatureFlagService featureFlagRepository,
        IUnitOfWork unitOfWork,
        FeatureFlagCacheService? cacheService = null)
    {
        _featureFlagRepository = featureFlagRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<FeatureFlag>> GetEnabledFlagsAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        if (_cacheService is not null)
        {
            return await _cacheService.GetEnabledFlagsAsync(territoryId, cancellationToken);
        }

        return _featureFlagRepository.GetEnabledFlags(territoryId);
    }

    /// <summary>
    /// Gets enabled flags for a territory (synchronous version for backward compatibility).
    /// </summary>
    public IReadOnlyList<FeatureFlag> GetEnabledFlags(Guid territoryId)
    {
        return GetEnabledFlagsAsync(territoryId).GetAwaiter().GetResult();
    }

    public async Task SetEnabledFlagsAsync(
        Guid territoryId,
        IReadOnlyList<FeatureFlag> flags,
        CancellationToken cancellationToken)
    {
        _featureFlagRepository.SetEnabledFlags(territoryId, flags);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidate cache when flags are updated
        _cacheService?.InvalidateFlags(territoryId);
    }
}
