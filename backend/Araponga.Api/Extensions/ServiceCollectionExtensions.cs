using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Application.Events;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Outbox;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core services
        services.AddScoped<AccessEvaluator>();
        services.AddScoped<CurrentUserAccessor>();

        // Feed services (refactored)
        services.AddScoped<PostCreationService>();
        services.AddScoped<PostInteractionService>();
        services.AddScoped<PostFilterService>();
        services.AddScoped<FeedService>();

        // Other services
        services.AddScoped<TerritoryService>();
        services.AddScoped<AuthService>();
        services.AddScoped<MembershipService>();
        services.AddScoped<JoinRequestService>();
        services.AddScoped<EventsService>();
        services.AddScoped<MapService>();
        services.AddScoped<ActiveTerritoryService>();
        services.AddScoped<HealthService>();
        services.AddScoped<AssetService>();
        services.AddScoped<ReportService>();
        services.AddScoped<UserBlockService>();
        services.AddScoped<FeatureFlagService>();
        services.AddScoped<StoreService>();
        services.AddScoped<ListingService>();
        services.AddScoped<InquiryService>();
        services.AddScoped<PlatformFeeService>();
        services.AddScoped<CartService>();

        return services;
    }

    public static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IEventHandler<PostCreatedEvent>, PostCreatedNotificationHandler>();
        services.AddScoped<IEventHandler<ReportCreatedEvent>, ReportCreatedNotificationHandler>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var persistenceProvider = configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";

        if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<ArapongaDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres")));

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

        return services;
    }

    private static IServiceCollection AddPostgresRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITerritoryRepository, PostgresTerritoryRepository>();
        services.AddScoped<IUserRepository, PostgresUserRepository>();
        services.AddScoped<ITerritoryMembershipRepository, PostgresTerritoryMembershipRepository>();
        services.AddScoped<IUserTerritoryRepository, PostgresUserTerritoryRepository>();
        services.AddScoped<ITerritoryJoinRequestRepository, PostgresTerritoryJoinRequestRepository>();
        services.AddScoped<IFeedRepository, PostgresFeedRepository>();
        services.AddScoped<ITerritoryEventRepository, PostgresTerritoryEventRepository>();
        services.AddScoped<IEventParticipationRepository, PostgresEventParticipationRepository>();
        services.AddScoped<IMapRepository, PostgresMapRepository>();
        services.AddScoped<IMapEntityRelationRepository, PostgresMapEntityRelationRepository>();
        services.AddScoped<IPostGeoAnchorRepository, PostgresPostGeoAnchorRepository>();
        services.AddScoped<IAssetRepository, PostgresAssetRepository>();
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
        services.AddScoped<IListingRepository, PostgresListingRepository>();
        services.AddScoped<IInquiryRepository, PostgresInquiryRepository>();
        services.AddScoped<ICartRepository, PostgresCartRepository>();
        services.AddScoped<ICartItemRepository, PostgresCartItemRepository>();
        services.AddScoped<ICheckoutRepository, PostgresCheckoutRepository>();
        services.AddScoped<ICheckoutItemRepository, PostgresCheckoutItemRepository>();
        services.AddScoped<IPlatformFeeConfigRepository, PostgresPlatformFeeConfigRepository>();

        return services;
    }

    private static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ITerritoryRepository, InMemoryTerritoryRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddSingleton<IUserTerritoryRepository, InMemoryUserTerritoryRepository>();
        services.AddSingleton<ITerritoryJoinRequestRepository, InMemoryTerritoryJoinRequestRepository>();
        services.AddSingleton<IFeedRepository, InMemoryFeedRepository>();
        services.AddSingleton<ITerritoryEventRepository, InMemoryTerritoryEventRepository>();
        services.AddSingleton<IEventParticipationRepository, InMemoryEventParticipationRepository>();
        services.AddSingleton<IMapRepository, InMemoryMapRepository>();
        services.AddSingleton<IMapEntityRelationRepository, InMemoryMapEntityRelationRepository>();
        services.AddSingleton<IPostGeoAnchorRepository, InMemoryPostGeoAnchorRepository>();
        services.AddSingleton<IAssetRepository, InMemoryAssetRepository>();
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
        services.AddSingleton<IListingRepository, InMemoryListingRepository>();
        services.AddSingleton<IInquiryRepository, InMemoryInquiryRepository>();
        services.AddSingleton<ICartRepository, InMemoryCartRepository>();
        services.AddSingleton<ICartItemRepository, InMemoryCartItemRepository>();
        services.AddSingleton<ICheckoutRepository, InMemoryCheckoutRepository>();
        services.AddSingleton<ICheckoutItemRepository, InMemoryCheckoutItemRepository>();
        services.AddSingleton<IPlatformFeeConfigRepository, InMemoryPlatformFeeConfigRepository>();

        return services;
    }
}
