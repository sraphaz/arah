using Araponga.Api.Security;
using Araponga.Api.Services.Journeys;
using Araponga.Api.Services.Journeys.Backend;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Connections;
using Araponga.Modules.Assets.Application.Interfaces;
using Araponga.Modules.Map.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Interfaces.Users;
using Araponga.Application.Services.Connections;
using Araponga.Domain.Connections;
using Araponga.Application.Services;
using Araponga.Application.Events;
using Araponga.Infrastructure;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.Email;
using Araponga.Infrastructure.FileStorage;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Media;
using Araponga.Infrastructure.Outbox;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Security;
using Araponga.Infrastructure.Shared.Postgres;
using Araponga.Application;
using Araponga.Infrastructure.Shared;
using Araponga.Modules.Feed;
using Araponga.Modules.Marketplace;
using Araponga.Modules.Events;
using Araponga.Modules.Map;
using Araponga.Modules.Chat;
using Araponga.Modules.Subscriptions;
using Araponga.Modules.Moderation;
using Araponga.Modules.Notifications;
using Araponga.Modules.Alerts;
using Araponga.Modules.Assets;
using Araponga.Modules.Admin;
using Araponga.Modules.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Araponga.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Platform services (cross-cutting)
        services.AddScoped<MembershipAccessRules>();
        // AccessEvaluator será registrado depois para permitir injeção dos serviços de políticas
        services.AddScoped<CurrentUserAccessor>();

        // Cache services
        services.AddScoped<TerritoryCacheService>();
        services.AddScoped<FeatureFlagCacheService>();
        services.AddScoped<UserBlockCacheService>();
        services.AddScoped<MapEntityCacheService>();
        services.AddScoped<EventCacheService>();
        services.AddScoped<AlertCacheService>();
        services.AddScoped<CacheInvalidationService>();

        // Feature flags guards (territory-scoped)
        services.AddScoped<TerritoryFeatureFlagGuard>();
        services.AddScoped<IGeoConvergenceBypassService, GeoConvergenceBypassService>();

        // Feed services (refactored)
        services.AddScoped<PostCreationService>();
        services.AddScoped<PostEditService>();
        services.AddScoped<PostInteractionService>();
        services.AddScoped<PostFilterService>();
        services.AddScoped<FeedService>();

        // Connections (Círculo de Amigos)
        services.AddScoped<ConnectionService>();
        services.AddScoped<ConnectionPrivacyService>();

        // Other services
        services.AddScoped<TerritoryService>();
        services.AddScoped<AuthService>();
        services.AddScoped<PasswordResetService>();
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
        services.AddScoped<UserProfileStatsService>();

        // Subscription services
        services.AddScoped<SubscriptionService>();
        services.AddScoped<CouponService>();
        services.AddScoped<SubscriptionPlanAdminService>();
        services.AddScoped<SubscriptionCapabilityService>();
        services.AddScoped<SubscriptionPlanSeedService>();
        services.AddScoped<SubscriptionTrialService>();
        services.AddScoped<SubscriptionRenewalService>();
        services.AddScoped<SubscriptionAnalyticsService>();
        services.AddScoped<StripeWebhookService>();
        services.AddScoped<MercadoPagoWebhookService>();
        services.AddScoped<Araponga.Application.Interfaces.ISubscriptionGatewayFactory, Araponga.Infrastructure.Payments.SubscriptionGatewayFactory>();

        // Subscription gateways (registrar ambos como ISubscriptionGateway)
        services.AddScoped<Araponga.Application.Interfaces.ISubscriptionGateway, Araponga.Infrastructure.Payments.StripeSubscriptionService>();
        services.AddScoped<Araponga.Application.Interfaces.ISubscriptionGateway, Araponga.Infrastructure.Payments.MercadoPagoSubscriptionService>();

        // Manter compatibilidade com IStripeSubscriptionService
        services.AddScoped<Araponga.Application.Interfaces.IStripeSubscriptionService, Araponga.Infrastructure.Payments.StripeSubscriptionService>();
        services.AddScoped<UserInterestService>();
        services.AddScoped<VotingService>();
        services.AddScoped<TerritoryModerationService>();
        services.AddScoped<InterestFilterService>();
        services.AddScoped<TerritoryCharacterizationService>();
        services.AddScoped<DataExportService>();
        services.AddScoped<AccountDeletionService>();
        services.AddScoped<AnalyticsService>();
        services.AddScoped<PushNotificationService>();
        services.AddScoped<Araponga.Application.Services.Notifications.NotificationConfigService>();
        services.AddScoped<SystemPermissionService>();
        services.AddScoped<MembershipCapabilityService>();
        services.AddScoped<SystemConfigCacheService>();
        services.AddScoped<SystemConfigService>();
        services.AddScoped<WorkQueueService>();
        services.AddScoped<VerificationQueueService>();
        services.AddScoped<DocumentEvidenceService>();
        services.AddScoped<ModerationCaseService>();
        services.AddScoped<ChatService>();
        services.AddScoped<InputSanitizationService>();
        services.AddScoped<SellerPayoutService>();
        services.AddScoped<TerritoryPayoutConfigService>();
        services.AddScoped<MediaService>();
        services.AddScoped<RatingService>();
        services.AddScoped<MarketplaceSearchService>();
        services.AddScoped<UserActivityService>();
        services.AddScoped<Araponga.Application.Interfaces.Media.IGlobalMediaLimits, Araponga.Api.Services.GlobalMediaLimitsService>();
        services.AddScoped<Araponga.Application.Services.Media.TerritoryMediaConfigService>();
        services.AddScoped<Araponga.Application.Services.Users.UserMediaPreferencesService>();
        services.AddScoped<Araponga.Application.Services.Media.MediaStorageConfigService>();
        services.AddScoped<Araponga.Application.Interfaces.IEmailTemplateService>(sp =>
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var templatesPath = Path.Combine(env.ContentRootPath, "Templates", "Email");
            var logger = sp.GetRequiredService<ILogger<Araponga.Application.Services.EmailTemplateService>>();
            return new Araponga.Application.Services.EmailTemplateService(logger, templatesPath);
        });
        services.AddScoped<Araponga.Application.Services.EmailQueueService>();

        // BFF Backends (usados pela API que ainda expõe /api/v2/journeys; o BFF é app separada que faz proxy para estes endpoints)
        services.AddScoped<IFeedJourneyBackend, InProcessFeedJourneyBackend>();
        services.AddScoped<IOnboardingJourneyBackend, InProcessOnboardingJourneyBackend>();
        services.AddScoped<IEventsJourneyBackend, InProcessEventsJourneyBackend>();
        // BFF Journey services (v2 - expostos pela API; o app BFF encaminha para aqui)
        services.AddScoped<IFeedJourneyService, FeedJourneyService>();
        services.AddScoped<IOnboardingJourneyService, OnboardingJourneyService>();
        services.AddScoped<IEventJourneyService, EventJourneyService>();

        // Policies services
        services.AddScoped<TermsOfServiceService>();
        services.AddScoped<TermsAcceptanceService>();
        services.AddScoped<PrivacyPolicyService>();
        services.AddScoped<PrivacyPolicyAcceptanceService>();
        services.AddScoped<PolicyRequirementService>();

        // AccessEvaluator (registrado depois para permitir injeção dos serviços de políticas)
        services.AddScoped<AccessEvaluator>(sp =>
        {
            return new AccessEvaluator(
                sp.GetRequiredService<ITerritoryMembershipRepository>(),
                sp.GetRequiredService<IMembershipCapabilityRepository>(),
                sp.GetRequiredService<ISystemPermissionRepository>(),
                sp.GetRequiredService<MembershipAccessRules>(),
                sp.GetRequiredService<IDistributedCacheService>(),
                sp.GetService<CacheMetricsService>(),
                sp.GetService<PolicyRequirementService>(),
                sp.GetService<TermsAcceptanceService>(),
                sp.GetService<PrivacyPolicyAcceptanceService>());
        });

        // Payout Gateway
        services.AddScoped<IPayoutGateway, Araponga.Infrastructure.Payments.MockPayoutGateway>();

        // Email Configuration (aplicável a ambos InMemory e Postgres)
        // Deve ser registrado aqui, não em AddPostgresRepositories, pois precisa estar disponível em testes
        services.AddScoped<IEmailSender, SmtpEmailSender>();

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
        services.AddScoped<IEventHandler<TermsOfServicePublishedEvent>, TermsOfServicePublishedNotificationHandler>();
        services.AddScoped<IEventHandler<PrivacyPolicyPublishedEvent>, PrivacyPolicyPublishedNotificationHandler>();
        services.AddScoped<IEventHandler<ConnectionRequestedEvent>, ConnectionRequestedNotificationHandler>();
        services.AddScoped<IEventHandler<ConnectionAcceptedEvent>, ConnectionAcceptedNotificationHandler>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var persistenceProvider = configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";

        if (string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            // Registrar infraestrutura compartilhada primeiro (SharedDbContext e repositórios compartilhados)
            services.AddSharedInfrastructure(configuration);
            services.AddSharedCrossCuttingServices(configuration);

            // Registrar módulos (que registram suas próprias infraestruturas)
            var modules = new IModule[]
            {
                new FeedModule(),
                new MarketplaceModule(),
                new EventsModule(),
                new MapModule(),
                new ChatModule(),
                new SubscriptionsModule(),
                new ModerationModule(),
                new NotificationsModule(),
                new AlertsModule(),
                new AssetsModule(),
                new AdminModule(),
                new ConnectionsModule()
            };

            // Criar logger temporário para ModuleRegistry
            using var loggerFactory = LoggerFactory.Create(b => b.AddConsole().SetMinimumLevel(LogLevel.Information));
            var moduleRegistryLogger = loggerFactory.CreateLogger<ModuleRegistry>();
            var moduleRegistry = new ModuleRegistry(modules, moduleRegistryLogger);
            moduleRegistry.Apply(services, configuration);
            services.AddSingleton<IModuleRegistry>(moduleRegistry);

            // Registrar repositórios compartilhados adicionais (que não estão em módulos nem em Shared)
            // Nota: A maioria dos repositórios ainda está em Araponga.Infrastructure.Postgres
            // e será migrada gradualmente para os módulos apropriados
            services.AddPostgresRepositories(configuration);

            // Connection Pool Metrics Service (usando ArapongaDbContext temporariamente)
            // TODO: Atualizar ConnectionPoolMetricsService para aceitar DbContext genérico
            services.AddSingleton<ConnectionPoolMetricsService>(sp =>
            {
                var dbContext = sp.GetRequiredService<ArapongaDbContext>();
                var logger = sp.GetRequiredService<ILogger<ConnectionPoolMetricsService>>();
                return new ConnectionPoolMetricsService(dbContext, logger);
            });

            // Workers e serviços de background
            services.AddHostedService<OutboxDispatcherWorker>();
            services.AddHostedService<Araponga.Infrastructure.Background.PayoutProcessingWorker>();
            services.AddHostedService<Araponga.Infrastructure.Background.SubscriptionRenewalWorker>();
            services.AddHostedService<Araponga.Infrastructure.Email.EmailQueueWorker>();
            services.AddHostedService<Araponga.Infrastructure.Email.EventReminderWorker>();

            // Manter ArapongaDbContext temporariamente para compatibilidade (será removido na Fase 6)
            services.AddDbContext<ArapongaDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres"), npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(30);
                }));

            // Participantes do UoW composto: contexto principal e shared (módulos registram os deles no próprio Apply)
            services.AddScoped<IUnitOfWorkParticipant>(sp => new DbContextUnitOfWorkParticipant(sp.GetRequiredService<ArapongaDbContext>()));
            services.AddScoped<IUnitOfWorkParticipant>(sp => new DbContextUnitOfWorkParticipant(sp.GetRequiredService<SharedDbContext>()));

            services.AddScoped<DbContextTransactionScopeAdapter>();
            // IUnitOfWork = CompositeUnitOfWork: CommitAsync persiste todos os participantes; transação explícita delega ao adapter do contexto principal (ADR-017).
            services.AddScoped<IUnitOfWork>(sp =>
            {
                var participants = sp.GetServices<IUnitOfWorkParticipant>().ToList();
                var transactionScope = sp.GetRequiredService<DbContextTransactionScopeAdapter>();
                return new CompositeUnitOfWork(participants, transactionScope);
            });
        }
        else
        {
            // Shared InMemory (core: Territory, User, Membership, JoinRequest, UserPreferences, etc.)
            services.AddSharedInMemoryRepositories();
            services.AddSingleton<InMemoryDataStore>();
            services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
            services.AddInMemoryRepositories();
        }

        // Email Configuration (aplicável a ambos InMemory e Postgres)
        services.Configure<EmailConfiguration>(configuration.GetSection("Email"));

        // Push Notification Provider (aplicável a ambos InMemory e Postgres; Firebase:ServerKey para habilitar FCM, senão no-op)
        var firebaseServerKey = configuration["Firebase:ServerKey"];
        if (!string.IsNullOrWhiteSpace(firebaseServerKey))
        {
            services.AddScoped<IPushNotificationProvider, Araponga.Infrastructure.Notifications.FirebasePushNotificationProvider>();
        }
        else
        {
            services.AddScoped<IPushNotificationProvider, Araponga.Infrastructure.Notifications.NoOpPushNotificationProvider>();
        }

        services.AddSingleton<Araponga.Application.Interfaces.IObservabilityLogger, InMemoryObservabilityLogger>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.Configure<Araponga.Infrastructure.InMemory.RefreshTokenOptions>(configuration.GetSection("RefreshToken"));
        services.AddSingleton<Araponga.Application.Interfaces.IRefreshTokenStore, Araponga.Infrastructure.InMemory.InMemoryRefreshTokenStore>();
        services.AddSingleton<Araponga.Infrastructure.Security.ISecretsService, Araponga.Infrastructure.Security.EnvironmentSecretsService>();

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

        // Media Storage Configuration
        services.Configure<MediaStorageOptions>(configuration.GetSection("MediaStorage"));

        // Media Storage Factory
        services.AddSingleton<MediaStorageFactory>();

        // Media Storage Service (criado via factory para suportar cache)
        services.AddScoped<IMediaStorageService>(sp =>
        {
            var factory = sp.GetRequiredService<MediaStorageFactory>();
            return factory.CreateStorageService();
        });

        // Media Processing Service
        services.AddScoped<IMediaProcessingService, LocalMediaProcessingService>();
        services.AddScoped<IMediaValidator, MediaValidator>();

        // Async Media Processing Background Service (opcional)
        var mediaStorageOptions = configuration.GetSection("MediaStorage").Get<MediaStorageOptions>() ?? new MediaStorageOptions();
        if (mediaStorageOptions.EnableAsyncProcessing)
        {
            services.AddSingleton<AsyncMediaProcessingBackgroundService>();
            services.AddHostedService(sp => sp.GetRequiredService<AsyncMediaProcessingBackgroundService>());
            services.AddSingleton<IAsyncMediaProcessor>(sp => sp.GetRequiredService<AsyncMediaProcessingBackgroundService>());
        }
        else
        {
            // NoOp se desabilitado
            services.AddSingleton<IAsyncMediaProcessor, NoOpAsyncMediaProcessor>();
        }

        // Distributed Cache para URLs de mídia (se Redis estiver configurado)
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "Araponga:";
            });
        }
        else
        {
            // Usar cache em memória se Redis não estiver disponível
            services.AddMemoryCache();
            services.AddSingleton<IDistributedCache, MemoryDistributedCache>();
        }

        return services;
    }

    private static IServiceCollection AddPostgresRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        // Repositórios core (Territory, User, Membership, JoinRequest, UserPreferences, UserInterest, Voting, Vote,
        // TerritoryCharacterization, MembershipSettings, MembershipCapability, SystemPermission, SystemConfig,
        // TermsOfService, TermsAcceptance, PrivacyPolicy, PrivacyPolicyAcceptance, UserDevice): AddSharedCrossCuttingServices (Shared)
        // IFeedRepository, IPostGeoAnchorRepository, IPostAssetRepository: registrados em Araponga.Modules.Feed.Infrastructure.FeedModule
        // Events: registrado em Araponga.Modules.Events.Infrastructure.EventsModule
        // Map: registrado em Araponga.Modules.Map.Infrastructure.MapModule
        // Assets: registrado em Araponga.Modules.Assets.Infrastructure.AssetsModule
        services.AddScoped<IActiveTerritoryStore, PostgresActiveTerritoryStore>();
        // Alerts: registrado em Araponga.Modules.Alerts.Infrastructure.AlertsModule
        services.AddScoped<IFeatureFlagService, PostgresFeatureFlagService>();
        services.AddScoped<IAuditLogger, PostgresAuditLogger>();
        // IReportRepository, ISanctionRepository registrados em Araponga.Modules.Moderation.Infrastructure.ModerationModule
        services.AddScoped<IUserBlockRepository, PostgresUserBlockRepository>();
        services.AddScoped<IOutbox, PostgresOutbox>();
        // INotificationInboxRepository registrado em Araponga.Modules.Notifications.Infrastructure.NotificationsModule
        // Marketplace (Store, StoreItem, Inquiry, Ratings, Cart, Checkout, PlatformFee, TerritoryPayout): registrado em Araponga.Modules.Marketplace.Infrastructure.MarketplaceModule

        // Financial
        services.AddScoped<IFinancialTransactionRepository, PostgresFinancialTransactionRepository>();
        services.AddScoped<ITransactionStatusHistoryRepository, PostgresTransactionStatusHistoryRepository>();
        services.AddScoped<ISellerBalanceRepository, PostgresSellerBalanceRepository>();
        services.AddScoped<ISellerTransactionRepository, PostgresSellerTransactionRepository>();
        services.AddScoped<IPlatformFinancialBalanceRepository, PostgresPlatformFinancialBalanceRepository>();
        services.AddScoped<IPlatformRevenueTransactionRepository, PostgresPlatformRevenueTransactionRepository>();
        services.AddScoped<IPlatformExpenseTransactionRepository, PostgresPlatformExpenseTransactionRepository>();
        services.AddScoped<IReconciliationRecordRepository, PostgresReconciliationRecordRepository>();

        // Chat: registrado em Araponga.Modules.Chat.Infrastructure.ChatModule

        // Media
        services.AddScoped<IMediaAssetRepository, PostgresMediaAssetRepository>();
        services.AddScoped<IMediaAttachmentRepository, PostgresMediaAttachmentRepository>();
        // TODO: Implementar PostgresTerritoryMediaConfigRepository e PostgresUserMediaPreferencesRepository
        // services.AddScoped<ITerritoryMediaConfigRepository, Araponga.Infrastructure.Postgres.PostgresTerritoryMediaConfigRepository>();
        // services.AddScoped<IUserMediaPreferencesRepository, Araponga.Infrastructure.Postgres.PostgresUserMediaPreferencesRepository>();

        // Subscriptions: registrado em Araponga.Modules.Subscriptions.Infrastructure.SubscriptionsModule

        // Connections (Círculo de Amigos): repositórios registrados em Araponga.Modules.Connections.Infrastructure.ConnectionsModule
        services.AddScoped<IAcceptedConnectionsProvider, AcceptedConnectionsProvider>();

        // Email Queue
        var emailPersistenceProvider = configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";
        if (string.Equals(emailPersistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IEmailQueueRepository, PostgresEmailQueueRepository>();
        }
        else
        {
            services.AddSingleton<IEmailQueueRepository>(sp =>
                new InMemoryEmailQueueRepository(sp.GetRequiredService<InMemoryDataStore>()));
        }

        return services;
    }

    private static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
    {
        // Shared/platform repos (Territory, User, Membership, JoinRequest, UserPreferences, UserInterest, Voting, Vote,
        // TerritoryCharacterization, MembershipSettings, MembershipCapability, SystemPermission, SystemConfig,
        // TermsOfService, TermsAcceptance, PrivacyPolicy, PrivacyPolicyAcceptance, UserDevice): AddSharedInMemoryRepositories
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
        services.AddSingleton<IUserConnectionRepository, InMemoryUserConnectionRepository>();
        services.AddSingleton<IConnectionPrivacySettingsRepository, InMemoryConnectionPrivacySettingsRepository>();
        services.AddSingleton<IAcceptedConnectionsProvider, AcceptedConnectionsProvider>();
        services.AddSingleton<ISanctionRepository, InMemorySanctionRepository>();
        services.AddSingleton<IOutbox, InMemoryOutbox>();
        services.AddSingleton<INotificationInboxRepository, InMemoryNotificationInboxRepository>();
        services.AddSingleton<IStoreRepository, InMemoryStoreRepository>();
        services.AddSingleton<IStoreItemRepository, InMemoryStoreItemRepository>();
        services.AddSingleton<IInquiryRepository, InMemoryInquiryRepository>();
        services.AddSingleton<IStoreRatingRepository, InMemoryStoreRatingRepository>();
        services.AddSingleton<IStoreItemRatingRepository, InMemoryStoreItemRatingRepository>();
        services.AddSingleton<IStoreRatingResponseRepository, InMemoryStoreRatingResponseRepository>();
        services.AddSingleton<ICartRepository, InMemoryCartRepository>();
        services.AddSingleton<ICartItemRepository, InMemoryCartItemRepository>();
        services.AddSingleton<ICheckoutRepository, InMemoryCheckoutRepository>();
        services.AddSingleton<ICheckoutItemRepository, InMemoryCheckoutItemRepository>();
        services.AddSingleton<IPlatformFeeConfigRepository, InMemoryPlatformFeeConfigRepository>();
        services.AddSingleton<ITerritoryPayoutConfigRepository, InMemoryTerritoryPayoutConfigRepository>();

        // Financial
        services.AddSingleton<IFinancialTransactionRepository, InMemoryFinancialTransactionRepository>();
        services.AddSingleton<ITransactionStatusHistoryRepository, InMemoryTransactionStatusHistoryRepository>();
        services.AddSingleton<ISellerBalanceRepository, InMemorySellerBalanceRepository>();
        services.AddSingleton<ISellerTransactionRepository, InMemorySellerTransactionRepository>();
        services.AddSingleton<IPlatformFinancialBalanceRepository, InMemoryPlatformFinancialBalanceRepository>();
        services.AddSingleton<IPlatformRevenueTransactionRepository, InMemoryPlatformRevenueTransactionRepository>();
        services.AddSingleton<IPlatformExpenseTransactionRepository, InMemoryPlatformExpenseTransactionRepository>();
        services.AddSingleton<IReconciliationRecordRepository, InMemoryReconciliationRecordRepository>();

        // Subscriptions
        services.AddSingleton<ISubscriptionPlanRepository, InMemorySubscriptionPlanRepository>();
        services.AddSingleton<ISubscriptionRepository, InMemorySubscriptionRepository>();
        services.AddSingleton<ISubscriptionPaymentRepository, InMemorySubscriptionPaymentRepository>();
        services.AddSingleton<ICouponRepository, InMemoryCouponRepository>();
        services.AddSingleton<ISubscriptionCouponRepository, InMemorySubscriptionCouponRepository>();
        services.AddSingleton<ISubscriptionPlanHistoryRepository, InMemorySubscriptionPlanHistoryRepository>();
        services.AddSingleton<ITerritoryModerationRuleRepository, InMemoryTerritoryModerationRuleRepository>();
        services.AddSingleton<Araponga.Application.Interfaces.Notifications.INotificationConfigRepository, InMemoryNotificationConfigRepository>();
        services.AddSingleton<IWorkItemRepository, InMemoryWorkItemRepository>();
        services.AddSingleton<IDocumentEvidenceRepository, InMemoryDocumentEvidenceRepository>();

        // Chat
        services.AddSingleton<IChatConversationRepository, InMemoryChatConversationRepository>();
        services.AddSingleton<IChatConversationParticipantRepository, InMemoryChatConversationParticipantRepository>();
        services.AddSingleton<IChatMessageRepository, InMemoryChatMessageRepository>();
        services.AddSingleton<IChatConversationStatsRepository, InMemoryChatConversationStatsRepository>();

        // Media
        services.AddSingleton<IMediaAssetRepository, InMemoryMediaAssetRepository>();
        services.AddSingleton<IMediaAttachmentRepository, InMemoryMediaAttachmentRepository>();
        services.AddSingleton<ITerritoryMediaConfigRepository, InMemoryTerritoryMediaConfigRepository>();
        services.AddSingleton<IUserMediaPreferencesRepository, InMemoryUserMediaPreferencesRepository>();
        services.AddSingleton<IMediaStorageConfigRepository, InMemoryMediaStorageConfigRepository>();

        return services;
    }
}
