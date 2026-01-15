using Araponga.Application.Common;
using Araponga.Application.Models;

namespace Araponga.Application.Services;

/// <summary>
/// Centraliza as regras de feature flags por território, garantindo mensagens consistentes
/// e um único ponto de decisão para "gates" de features.
/// </summary>
public sealed class TerritoryFeatureFlagGuard
{
    private const string MarketplaceDisabledError = "Marketplace is disabled for this territory.";
    private const string AlertPostsDisabledError = "Alert posts are disabled for this territory.";
    private const string ChatDisabledError = "Chat is disabled for this territory.";
    private const string ChatDmDisabledError = "Direct messages are disabled for this territory.";

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
            : OperationResult.Failure(MarketplaceDisabledError);
    }

    public OperationResult EnsureAlertPostsEnabled(Guid territoryId)
    {
        return _flags.IsEnabled(territoryId, FeatureFlag.AlertPosts)
            ? OperationResult.Success()
            : OperationResult.Failure(AlertPostsDisabledError);
    }

    public OperationResult EnsureChatEnabled(Guid territoryId)
    {
        return _flags.IsEnabled(territoryId, FeatureFlag.ChatEnabled)
            ? OperationResult.Success()
            : OperationResult.Failure(ChatDisabledError);
    }

    public OperationResult EnsureChatDmEnabled(Guid territoryId)
    {
        // DM é um "subfeature" do chat: exige ChatEnabled e ChatDmEnabled
        if (!_flags.IsEnabled(territoryId, FeatureFlag.ChatEnabled))
        {
            return OperationResult.Failure(ChatDisabledError);
        }

        return _flags.IsEnabled(territoryId, FeatureFlag.ChatDmEnabled)
            ? OperationResult.Success()
            : OperationResult.Failure(ChatDmDisabledError);
    }
}
