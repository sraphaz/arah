using Araponga.Application.Models;

namespace Araponga.Application.Interfaces;

public interface IFeatureFlagService
{
    bool IsEnabled(Guid territoryId, FeatureFlag flag);
    IReadOnlyList<FeatureFlag> GetEnabledFlags(Guid territoryId);
    void SetEnabledFlags(Guid territoryId, IReadOnlyList<FeatureFlag> flags);
}
