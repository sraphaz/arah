using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Application.Services;

public sealed class FeatureFlagService
{
    private readonly IFeatureFlagService _featureFlagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FeatureFlagService(IFeatureFlagService featureFlagRepository, IUnitOfWork unitOfWork)
    {
        _featureFlagRepository = featureFlagRepository;
        _unitOfWork = unitOfWork;
    }

    public IReadOnlyList<FeatureFlag> GetEnabledFlags(Guid territoryId)
    {
        return _featureFlagRepository.GetEnabledFlags(territoryId);
    }

    public async Task SetEnabledFlagsAsync(
        Guid territoryId,
        IReadOnlyList<FeatureFlag> flags,
        CancellationToken cancellationToken)
    {
        _featureFlagRepository.SetEnabledFlags(territoryId, flags);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
