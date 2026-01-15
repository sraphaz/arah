using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Application.Events;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.FileStorage;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Outbox;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Araponga.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core services
        services.AddScoped<MembershipAccessRules>();
        services.AddScoped<AccessEvaluator>();
        services.AddScoped<CurrentUserAccessor>();

        // Cache services
        services.AddScoped<TerritoryCacheService>();
        services.AddScoped<FeatureFlagCacheService>();
        services.AddScoped<UserBlockCacheService>();
        services.AddScoped<MapEntityCacheService>();
        services.AddScoped<EventCacheService>();
        services.AddScoped<AlertCacheService>();

        // Feature flags guards (territory-scoped)
        services.AddScoped<TerritoryFeatureFlagGuard>();

        // Feed services (refactored)
        services.AddScoped<PostCreationService>();
        services.AddScoped<PostInteractionService>();
        services.AddScoped<PostFilterService>();
        services.AddScoped<FeedService>();

        // Other services
        services.AddScoped<TerritoryService>();
        services.AddScoped<AuthService>();
        services.AddScoped<MembershipService>();
        services.AddScoped<ResidencyRequestService>();
        services.AddScoped<JoinRequestService>();
        services.AddScoped<EventsService>();
        services.AddScoped<MapService>();
        services.AddScoped<ActiveTerritoryService>();
        services.AddScoped<HealthService>();
        services.AddScoped<TerritoryAssetService>();
        services.AddScoped<ReportService>();
        services.AddScoped<UserBlockService>();
        services.AddScoped<FeatureFlagService>();
        services.AddScoped<StoreService>();
        services.AddScoped<StoreItemService>();
        services.AddScoped<InquiryService>();
        services.AddScoped<PlatformFeeService>();
        services.AddScoped<CartService>();
        services.AddScoped<UserPreferencesService>();
        services.AddScoped<UserProfileService>();
        services.AddScoped<SystemPermissionService>();
        services.AddScoped<MembershipCapabilityService>();
        services.AddScoped<SystemConfigCacheService>();
        services.AddScoped<SystemConfigService>();
        services.AddScoped<WorkQueueService>();
        services.AddScoped<VerificationQueueService>();
        services.AddScoped<DocumentEvidenceService>();
        services.AddScoped<ModerationCaseService>();
        services.AddScoped<ChatService>();

        return services;
    }

    public static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IEventHandler<PostCreatedEvent>, PostCreatedNotificationHandler>();
        services.AddScoped<IEventHandler<ReportCreatedEvent>, ReportCreatedNotificationHandler>();
        services.AddScoped<IEventHandler<ReportCreatedEvent>, ReportCreatedWorkItemHandler>();
        services.AddScoped<IEventHandler<SystemPermissionRevokedEvent>, SystemPermissionRevokedCacheHandler>();
        services.AddScoped<IEventHandler<MembershipCapabilityRevokedEvent>, MembershipCapabilityRevokedCacheHandler>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var persistenceProvider = configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";

        if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<ArapongaDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres"), npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(30);
                }));

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ArapongaDbContext>());
            services.AddPostgresRepositories();
            services.AddHostedService<OutboxDispatcherWorker>();
        }
        else
        {
            services.AddSingleton<InMemoryDataStore>();
            services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
            services.AddInMemoryRepositories();
        }

        services.AddSingleton<Araponga.Application.Interfaces.IObservabilityLogger, InMemoryObservabilityLogger>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        
        var storageProvider = configuration.GetValue<string>("Storage:Provider") ?? "Local";
        if (string.Equals(storageProvider, "S3", StringComparison.OrdinalIgnoreCase))
        {
            var options = configuration.GetSection("Storage:S3").Get<S3StorageOptions>() ?? new S3StorageOptions();
            services.AddSingleton<IFileStorage>(_ => new S3FileStorage(options));
        }
        else
        {
            services.AddSingleton<IFileStorage>(_ => new LocalFileStorage(Path.Combine(AppContext.BaseDirectory, "app_data", "uploads")));
        }

        return services;
    }

    private static IServiceCollection AddPostgresRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITerritoryRepository, PostgresTerritoryRepository>();
        services.AddScoped<IUserRepository, PostgresUserRepository>();
        services.AddScoped<ITerritoryMembershipRepository, PostgresTerritoryMembershipRepository>();
        services.AddScoped<ITerritoryJoinRequestRepository, PostgresTerritoryJoinRequestRepository>();
        services.AddScoped<IFeedRepository, PostgresFeedRepository>();
        services.AddScoped<ITerritoryEventRepository, PostgresTerritoryEventRepository>();
        services.AddScoped<IEventParticipationRepository, PostgresEventParticipationRepository>();
        services.AddScoped<IMapRepository, PostgresMapRepository>();
        services.AddScoped<IMapEntityRelationRepository, PostgresMapEntityRelationRepository>();
        services.AddScoped<IPostGeoAnchorRepository, PostgresPostGeoAnchorRepository>();
        services.AddScoped<ITerritoryAssetRepository, PostgresAssetRepository>();
        services.AddScoped<IAssetGeoAnchorRepository, PostgresAssetGeoAnchorRepository>();
        services.AddScoped<IAssetValidationRepository, PostgresAssetValidationRepository>();
        services.AddScoped<IPostAssetRepository, PostgresPostAssetRepository>();
        services.AddScoped<IActiveTerritoryStore, PostgresActiveTerritoryStore>();
        services.AddScoped<IHealthAlertRepository, PostgresHealthAlertRepository>();
        services.AddScoped<IFeatureFlagService, PostgresFeatureFlagService>();
        services.AddScoped<IAuditLogger, PostgresAuditLogger>();
        services.AddScoped<IReportRepository, PostgresReportRepository>();
        services.AddScoped<IUserBlockRepository, PostgresUserBlockRepository>();
        services.AddScoped<ISanctionRepository, PostgresSanctionRepository>();
        services.AddScoped<IOutbox, PostgresOutbox>();
        services.AddScoped<INotificationInboxRepository, PostgresNotificationInboxRepository>();
        services.AddScoped<IStoreRepository, PostgresStoreRepository>();
        services.AddScoped<IStoreItemRepository, PostgresStoreItemRepository>();
        services.AddScoped<IInquiryRepository, PostgresInquiryRepository>();
        services.AddScoped<ICartRepository, PostgresCartRepository>();
        services.AddScoped<ICartItemRepository, PostgresCartItemRepository>();
        services.AddScoped<ICheckoutRepository, PostgresCheckoutRepository>();
        services.AddScoped<ICheckoutItemRepository, PostgresCheckoutItemRepository>();
        services.AddScoped<IPlatformFeeConfigRepository, PostgresPlatformFeeConfigRepository>();
        services.AddScoped<IUserPreferencesRepository, PostgresUserPreferencesRepository>();
        services.AddScoped<IMembershipSettingsRepository, PostgresMembershipSettingsRepository>();
        services.AddScoped<IMembershipCapabilityRepository, PostgresMembershipCapabilityRepository>();
        services.AddScoped<ISystemPermissionRepository, PostgresSystemPermissionRepository>();
        services.AddScoped<ISystemConfigRepository, PostgresSystemConfigRepository>();
        services.AddScoped<IWorkItemRepository, PostgresWorkItemRepository>();
        services.AddScoped<IDocumentEvidenceRepository, PostgresDocumentEvidenceRepository>();

        // Chat
        services.AddScoped<IChatConversationRepository, PostgresChatConversationRepository>();
        services.AddScoped<IChatConversationParticipantRepository, PostgresChatConversationParticipantRepository>();
        services.AddScoped<IChatMessageRepository, PostgresChatMessageRepository>();
        services.AddScoped<IChatConversationStatsRepository, PostgresChatConversationStatsRepository>();

        return services;
    }

    private static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ITerritoryRepository, InMemoryTerritoryRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddSingleton<ITerritoryJoinRequestRepository, InMemoryTerritoryJoinRequestRepository>();
        services.AddSingleton<IFeedRepository, InMemoryFeedRepository>();
        services.AddSingleton<ITerritoryEventRepository, InMemoryTerritoryEventRepository>();
        services.AddSingleton<IEventParticipationRepository, InMemoryEventParticipationRepository>();
        services.AddSingleton<IMapRepository, InMemoryMapRepository>();
        services.AddSingleton<IMapEntityRelationRepository, InMemoryMapEntityRelationRepository>();
        services.AddSingleton<IPostGeoAnchorRepository, InMemoryPostGeoAnchorRepository>();
        services.AddSingleton<ITerritoryAssetRepository, InMemoryAssetRepository>();
        services.AddSingleton<IAssetGeoAnchorRepository, InMemoryAssetGeoAnchorRepository>();
        services.AddSingleton<IAssetValidationRepository, InMemoryAssetValidationRepository>();
        services.AddSingleton<IPostAssetRepository, InMemoryPostAssetRepository>();
        services.AddSingleton<IActiveTerritoryStore, InMemoryActiveTerritoryStore>();
        services.AddSingleton<IHealthAlertRepository, InMemoryHealthAlertRepository>();
        services.AddSingleton<IFeatureFlagService, InMemoryFeatureFlagService>();
        services.AddSingleton<IAuditLogger, InMemoryAuditLogger>();
        services.AddSingleton<IReportRepository, InMemoryReportRepository>();
        services.AddSingleton<IUserBlockRepository, InMemoryUserBlockRepository>();
        services.AddSingleton<ISanctionRepository, InMemorySanctionRepository>();
        services.AddSingleton<IOutbox, InMemoryOutbox>();
        services.AddSingleton<INotificationInboxRepository, InMemoryNotificationInboxRepository>();
        services.AddSingleton<IStoreRepository, InMemoryStoreRepository>();
        services.AddSingleton<IStoreItemRepository, InMemoryStoreItemRepository>();
        services.AddSingleton<IInquiryRepository, InMemoryInquiryRepository>();
        services.AddSingleton<ICartRepository, InMemoryCartRepository>();
        services.AddSingleton<ICartItemRepository, InMemoryCartItemRepository>();
        services.AddSingleton<ICheckoutRepository, InMemoryCheckoutRepository>();
        services.AddSingleton<ICheckoutItemRepository, InMemoryCheckoutItemRepository>();
        services.AddSingleton<IPlatformFeeConfigRepository, InMemoryPlatformFeeConfigRepository>();
        services.AddSingleton<IUserPreferencesRepository, InMemoryUserPreferencesRepository>();
        services.AddSingleton<IMembershipSettingsRepository, InMemoryMembershipSettingsRepository>();
        services.AddSingleton<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddSingleton<ISystemPermissionRepository, InMemorySystemPermissionRepository>();
        services.AddSingleton<ISystemConfigRepository, InMemorySystemConfigRepository>();
        services.AddSingleton<IWorkItemRepository, InMemoryWorkItemRepository>();
        services.AddSingleton<IDocumentEvidenceRepository, InMemoryDocumentEvidenceRepository>();

        // Chat
        services.AddSingleton<IChatConversationRepository, InMemoryChatConversationRepository>();
        services.AddSingleton<IChatConversationParticipantRepository, InMemoryChatConversationParticipantRepository>();
        services.AddSingleton<IChatMessageRepository, InMemoryChatMessageRepository>();
        services.AddSingleton<IChatConversationStatsRepository, InMemoryChatConversationStatsRepository>();

        return services;
    }
}
