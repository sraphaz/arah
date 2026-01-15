using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;

namespace Araponga.Tests.Application;

public static class FeedServiceTestHelper
{
    public static FeedService CreateFeedService(
        InMemoryDataStore dataStore,
        IEventBus? eventBus = null)
    {
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var membershipAccessRules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlags);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            membershipAccessRules,
            cache);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var mapRepository = new InMemoryMapRepository(dataStore);
        var geoAnchorRepository = new InMemoryPostGeoAnchorRepository(dataStore);
        var postAssetRepository = new InMemoryPostAssetRepository(dataStore);
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var sanctionRepository = new InMemorySanctionRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var eventBusInstance = eventBus ?? new NoOpEventBus();

        // Create the specialized services
        var postCreationService = new PostCreationService(
            feedRepository,
            mapRepository,
            assetRepository,
            geoAnchorRepository,
            postAssetRepository,
            sanctionRepository,
            featureFlags,
            auditLogger,
            eventBusInstance,
            unitOfWork);

        var postInteractionService = new PostInteractionService(
            feedRepository,
            accessEvaluator,
            sanctionRepository,
            auditLogger,
            unitOfWork);

        var postFilterService = new PostFilterService(
            accessEvaluator,
            blockRepository,
            postAssetRepository);

        // Create FeedService with the new structure
        return new FeedService(
            feedRepository,
            postCreationService,
            postInteractionService,
            postFilterService);
    }
}
