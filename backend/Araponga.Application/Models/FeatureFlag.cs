namespace Araponga.Application.Models;

public enum FeatureFlag
{
    AlertPosts = 1,
    EventPosts = 2,
    MarketplaceEnabled = 3,

    // Chat module (territory-scoped)
    ChatEnabled = 4,
    ChatTerritoryPublicChannel = 5,
    ChatTerritoryResidentsChannel = 6,
    ChatGroups = 7,
    ChatDmEnabled = 8,
    ChatMediaEnabled = 9
}
