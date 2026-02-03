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
    ChatMediaEnabled = 9,

    // Media module (territory-scoped)
    MediaImagesEnabled = 10,
    MediaVideosEnabled = 11,
    MediaAudioEnabled = 12,
    ChatMediaImagesEnabled = 13,
    ChatMediaAudioEnabled = 14,

    // Connections / Círculo de Amigos (territory-scoped)
    ConnectionsEnabled = 15,
    ConnectionsFeedPrioritize = 16
}
