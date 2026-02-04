using Araponga.Application.Common;
using Araponga.Application.Models;

namespace Araponga.Application.Services;

/// <summary>
/// Centraliza as regras de feature flags por território, garantindo mensagens consistentes
/// e um único ponto de decisão para "gates" de features.
/// </summary>
public sealed class TerritoryFeatureFlagGuard
{

    private readonly FeatureFlagCacheService _flags;

    public TerritoryFeatureFlagGuard(FeatureFlagCacheService flags)
    {
        _flags = flags;
    }

    public bool IsEnabled(Guid territoryId, FeatureFlag flag) => _flags.IsEnabled(territoryId, flag);

    public OperationResult EnsureMarketplaceEnabled(Guid territoryId)
    {
        return _flags.IsEnabled(territoryId, FeatureFlag.MarketplaceEnabled)
            ? OperationResult.Success()
            : OperationResult.Failure(Constants.FeatureFlagErrors.MarketplaceDisabled);
    }

    public OperationResult EnsureAlertPostsEnabled(Guid territoryId)
    {
        return _flags.IsEnabled(territoryId, FeatureFlag.AlertPosts)
            ? OperationResult.Success()
            : OperationResult.Failure(Constants.FeatureFlagErrors.AlertPostsDisabled);
    }

    public OperationResult EnsureChatEnabled(Guid territoryId)
    {
        return _flags.IsEnabled(territoryId, FeatureFlag.ChatEnabled)
            ? OperationResult.Success()
            : OperationResult.Failure(Constants.FeatureFlagErrors.ChatDisabled);
    }

    public OperationResult EnsureChatDmEnabled(Guid territoryId)
    {
        // DM é um "subfeature" do chat: exige ChatEnabled e ChatDmEnabled
        if (!_flags.IsEnabled(territoryId, FeatureFlag.ChatEnabled))
        {
            return OperationResult.Failure(Constants.FeatureFlagErrors.ChatDisabled);
        }

        return _flags.IsEnabled(territoryId, FeatureFlag.ChatDmEnabled)
            ? OperationResult.Success()
            : OperationResult.Failure(Constants.FeatureFlagErrors.ChatDmDisabled);
    }
}
