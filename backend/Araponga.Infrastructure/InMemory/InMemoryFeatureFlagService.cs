using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryFeatureFlagService : IFeatureFlagService
{
    private readonly Dictionary<Guid, HashSet<FeatureFlag>> _flags = new();

    public InMemoryFeatureFlagService()
    {
        _flags[Guid.Parse("11111111-1111-1111-1111-111111111111")] = new HashSet<FeatureFlag>
        {
            FeatureFlag.EventPosts,
            FeatureFlag.ChatEnabled,
            FeatureFlag.ChatTerritoryPublicChannel
        };

        _flags[Guid.Parse("22222222-2222-2222-2222-222222222222")] = new HashSet<FeatureFlag>
        {
            FeatureFlag.AlertPosts,
            FeatureFlag.EventPosts,
            FeatureFlag.MarketplaceEnabled,
            FeatureFlag.ChatEnabled,
            FeatureFlag.ChatTerritoryPublicChannel,
            FeatureFlag.ChatTerritoryResidentsChannel,
            FeatureFlag.ChatGroups
        };
    }

    public bool IsEnabled(Guid territoryId, FeatureFlag flag)
    {
        return _flags.TryGetValue(territoryId, out var flags) && flags.Contains(flag);
    }

    public IReadOnlyList<FeatureFlag> GetEnabledFlags(Guid territoryId)
    {
        return _flags.TryGetValue(territoryId, out var flags)
            ? flags.OrderBy(flag => flag).ToList()
            : Array.Empty<FeatureFlag>();
    }

    public void SetEnabledFlags(Guid territoryId, IReadOnlyList<FeatureFlag> flags)
    {
        _flags[territoryId] = new HashSet<FeatureFlag>(flags);
    }
}
