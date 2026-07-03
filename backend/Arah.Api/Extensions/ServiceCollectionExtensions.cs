using Arah.Api.Security;
using Arah.Api.Services.Journeys;
using Arah.Api.Services.Journeys.Backend;
using Arah.Application.Interfaces;
using Arah.Application.Interfaces.Connections;
using Arah.Application.Interfaces.Geo;
using Arah.Modules.Assets.Application.Interfaces;
using Arah.Modules.Map.Application.Interfaces;
using Arah.Modules.Marketplace.Application.Interfaces;
using Arah.Modules.Moderation.Application.Interfaces;
using Arah.Application.Interfaces.Media;
using Arah.Application.Interfaces.Users;
using Arah.Application.Services.Connections;
using Arah.Domain.Connections;
using Arah.Application.Services;
using Arah.Application.Events;
using Arah.Infrastructure;
using Arah.Infrastructure.Eventing;
using Arah.Infrastructure.Email;
using Arah.Infrastructure.FileStorage;
using Arah.Infrastructure.InMemory;
using Arah.Infrastructure.Media;
using Arah.Infrastructure.Outbox;
using Arah.Infrastructure.Postgres;
using Arah.Infrastructure.Geo;
using Arah.Infrastructure.Security;
using Arah.Infrastructure.Shared.Postgres;
using Arah.Application;
using Arah.Infrastructure.Shared;
using Arah.Modules.Feed;
using Arah.Modules.Marketplace;
using Arah.Modules.Events;
using Arah.Modules.Map;
using Arah.Modules.Chat;
using Arah.Modules.Subscriptions;
using Arah.Modules.Moderation;
using Arah.Modules.Notifications;
using Arah.Modules.Alerts;
using Arah.Modules.Assets;
using Arah.Modules.Admin;
using Arah.Modules.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arah.Api.Extensions;

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
        services.AddScoped<IMunicipalityTerritoryProvisioningService, MunicipalityTerritoryProvisioningService>();
        services.AddScoped<ITerritoryProposalService, TerritoryProposalService>();
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
        services.AddScoped<CommercialStoreGateService>();
        services.AddScoped<TransactionQuoteService>();
        services.AddScoped<RefundService>();
        services.AddScoped<PaymentService>();
        services.AddScoped<PayoutConsolidationService>();
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
        services.AddScoped<Arah.Application.Interfaces.ISubscriptionGatewayFactory, Arah.Infrastructure.Payments.SubscriptionGatewayFactory>();

        // Subscription gateways (registrar ambos como ISubscriptionGateway)
        services.AddScoped<Arah.Application.Interfaces.ISubscriptionGateway, Arah.Infrastructure.Payments.StripeSubscriptionService>();
        services.AddScoped<Arah.Application.Interfaces.ISubscriptionGateway, Arah.Infrastructure.Payments.MercadoPagoSubscriptionService>();

        // Manter compatibilidade com IStripeSubscriptionService
        services.AddScoped<Arah.Application.Interfaces.IStripeSubscriptionService, Arah.Infrastructure.Payments.StripeSubscriptionService>();
        services.AddScoped<UserInterestService>();
        services.AddScoped<VotingService>();
        services.AddScoped<TerritoryModerationService>();
        services.AddScoped<InterestFilterService>();
        services.AddScoped<TerritoryCharacterizationService>();
        services.AddScoped<DataExportService>();
        services.AddScoped<AccountDeletionService>();
        services.AddScoped<AnalyticsService>();
        services.AddScoped<PushNotificationService>();
        services.AddScoped<Arah.Application.Services.Notifications.NotificationConfigService>();
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
        services.AddScoped<Arah.Application.Interfaces.Media.IGlobalMediaLimits, Arah.Api.Services.GlobalMediaLimitsService>();
        services.AddScoped<Arah.Application.Services.Media.TerritoryMediaConfigService>();
        services.AddScoped<Arah.Application.Services.Users.UserMediaPreferencesService>();
        services.AddScoped<Arah.Application.Services.Media.MediaStorageConfigService>();
        services.AddScoped<Arah.Application.Interfaces.IEmailTemplateService>(sp =>
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var templatesPath = Path.Combine(env.ContentRootPath, "Templates", "Email");
            var logger = sp.GetRequiredService<ILogger<Arah.Application.Services.EmailTemplateService>>();
            return new Arah.Application.Services.EmailTemplateService(logger, templatesPath);
        });
        services.AddScoped<Arah.Application.Services.EmailQueueService>();

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
        services.AddScoped<IPayoutGateway, Arah.Infrastructure.Payments.MockPayoutGateway>();
        // IPaymentGateway é registrado por ambiente em Program.cs (mock apenas fora de produção).

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
        var isPostgres = string.Equals(persistenceProvider, "Postgres", StringComparison.OrdinalIgnoreCase);

        // Registrar repositórios de mídia logo no início quando Postgres (usados por TerritoryMediaConfigService e vários serviços).
        // Assim garantimos registro antes de módulos/DbContext que possam falhar.
        if (isPostgres)
        {
            services.AddScoped<ITerritoryMediaConfigRepository, PostgresTerritoryMediaConfigRepository>();
            services.AddScoped<IUserMediaPreferencesRepository, PostgresUserMediaPreferencesRepository>();
            services.AddScoped<IMediaStorageConfigRepository, PostgresMediaStorageConfigRepository>();
        }

        if (isPostgres)
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
            // Nota: A maioria dos repositórios ainda está em Arah.Infrastructure.Postgres
            // e será migrada gradualmente para os módulos apropriados
            services.AddPostgresRepositories(configuration);

            // Connection Pool Metrics Service (usa IServiceScopeFactory para resolver DbContext por chamada; evita Singleton+Scoped)
            services.AddSingleton<ConnectionPoolMetricsService>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var logger = sp.GetRequiredService<ILogger<ConnectionPoolMetricsService>>();
                return new ConnectionPoolMetricsService(scopeFactory, logger);
            });

            // Workers e serviços de background
            services.AddHostedService<OutboxDispatcherWorker>();
            services.AddHostedService<Arah.Infrastructure.Background.PayoutProcessingWorker>();
            services.AddHostedService<Arah.Infrastructure.Background.SubscriptionRenewalWorker>();
            services.AddHostedService<Arah.Infrastructure.Email.EmailQueueWorker>();
            services.AddHostedService<Arah.Infrastructure.Email.EventReminderWorker>();

            // Manter ArahDbContext temporariamente para compatibilidade (será removido na Fase 6)
            services.AddDbContext<ArahDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres"), npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(30);
                }));

            // Participantes do UoW composto: contexto principal e shared (módulos registram os deles no próprio Apply)
            services.AddScoped<IUnitOfWorkParticipant>(sp => new DbContextUnitOfWorkParticipant(sp.GetRequiredService<ArahDbContext>()));
            services.AddScoped<IUnitOfWorkParticipant>(sp => new DbContextUnitOfWorkParticipant(sp.GetRequiredService<SharedDbContext>()));

            services.AddScoped<DbContextTransactionScopeAdapter>();
            // IUnitOfWork = CompositeUnitOfWork: CommitAsync persiste todos os participantes; transação explícita delega ao adapter do contexto principal (ADR-017).
            services.AddScoped<IUnitOfWork>(sp =>
            {
                var participants = sp.GetServices<IUnitOfWorkParticipant>().ToList();
                var transactionScope = sp.GetRequiredService<DbContextTransactionScopeAdapter>();
                return new CompositeUnitOfWork(participants, transactionScope);
            });

            if (configuration.GetValue<bool>("Pilot:BootstrapAdminEnabled"))
            {
                services.AddHostedService<Arah.Infrastructure.Hosting.PilotAdminBootstrapHostedService>();
            }

            services.AddScoped<IFeeSplitRuleRepository, PostgresFeeSplitRuleRepository>();
            services.AddHostedService<Arah.Infrastructure.Hosting.FeeSplitRuleBootstrapHostedService>();
        }
        else
        {
            // Shared InMemory (core: Territory, User, Membership, JoinRequest, UserPreferences, etc.)
            services.AddSharedInMemoryRepositories();
            services.AddSingleton<InMemoryDataStore>();
            services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
            services.AddInMemoryRepositories();
            services.AddHostedService<Arah.Infrastructure.Hosting.FeeSplitRuleBootstrapHostedService>();
            services.AddSingleton<IEmailQueueRepository>(sp =>
                new InMemoryEmailQueueRepository(sp.GetRequiredService<InMemoryDataStore>()));
        }

        // Fallback: garantir repositórios de mídia quando Postgres (evita falha de DI se o bloco inicial não tiver sido executado)
        var persistenceAgain = configuration.GetValue<string>("Persistence:Provider") ?? "InMemory";
        if (string.Equals(persistenceAgain, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            services.TryAddScoped<ITerritoryMediaConfigRepository, PostgresTerritoryMediaConfigRepository>();
            services.TryAddScoped<IUserMediaPreferencesRepository, PostgresUserMediaPreferencesRepository>();
            services.TryAddScoped<IMediaStorageConfigRepository, PostgresMediaStorageConfigRepository>();
        }

        // Email Configuration (aplicável a ambos InMemory e Postgres)
        services.Configure<EmailConfiguration>(configuration.GetSection("Email"));

        // Push Notification Provider (aplicável a ambos InMemory e Postgres; Firebase:ServerKey para habilitar FCM, senão no-op)
        var firebaseServerKey = configuration["Firebase:ServerKey"];
        if (!string.IsNullOrWhiteSpace(firebaseServerKey))
        {
            services.AddScoped<IPushNotificationProvider, Arah.Infrastructure.Notifications.FirebasePushNotificationProvider>();
        }
        else
        {
            services.AddScoped<IPushNotificationProvider, Arah.Infrastructure.Notifications.NoOpPushNotificationProvider>();
        }

        services.AddSingleton<Arah.Application.Interfaces.IObservabilityLogger, InMemoryObservabilityLogger>();
        services.AddGeoIntegrationServices();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.Configure<Arah.Infrastructure.InMemory.RefreshTokenOptions>(configuration.GetSection("RefreshToken"));
        services.AddSingleton<Arah.Application.Interfaces.IRefreshTokenStore, Arah.Infrastructure.InMemory.InMemoryRefreshTokenStore>();
        services.AddSingleton<Arah.Infrastructure.Security.ISecretsService, Arah.Infrastructure.Security.EnvironmentSecretsService>();

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
                options.InstanceName = "Arah:";
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
        // Media (necessário para TerritoryMediaConfigService -> EventJourneyService e outros)
        services.AddScoped<ITerritoryMediaConfigRepository, PostgresTerritoryMediaConfigRepository>();
        services.AddScoped<IUserMediaPreferencesRepository, PostgresUserMediaPreferencesRepository>();
        services.AddScoped<IMediaStorageConfigRepository, PostgresMediaStorageConfigRepository>();

        // Repositórios core (Territory, User, Membership, JoinRequest, UserPreferences, UserInterest, Voting, Vote,
        // TerritoryCharacterization, MembershipSettings, MembershipCapability, SystemPermission, SystemConfig,
        // TermsOfService, TermsAcceptance, PrivacyPolicy, PrivacyPolicyAcceptance, UserDevice): AddSharedCrossCuttingServices (Shared)
        // IFeedRepository, IPostGeoAnchorRepository, IPostAssetRepository: registrados em Arah.Modules.Feed.Infrastructure.FeedModule
        // Events: registrado em Arah.Modules.Events.Infrastructure.EventsModule
        // Map: registrado em Arah.Modules.Map.Infrastructure.MapModule
        // Assets: registrado em Arah.Modules.Assets.Infrastructure.AssetsModule
        services.AddScoped<IActiveTerritoryStore, PostgresActiveTerritoryStore>();
        // Alerts: registrado em Arah.Modules.Alerts.Infrastructure.AlertsModule
        services.AddScoped<IFeatureFlagService, PostgresFeatureFlagService>();
        services.AddScoped<IAuditLogger, PostgresAuditLogger>();
        // IReportRepository, ISanctionRepository registrados em Arah.Modules.Moderation.Infrastructure.ModerationModule
        services.AddScoped<IUserBlockRepository, PostgresUserBlockRepository>();
        services.AddScoped<IOutbox, PostgresOutbox>();
        // INotificationInboxRepository registrado em Arah.Modules.Notifications.Infrastructure.NotificationsModule
        // Marketplace (Store, StoreItem, Inquiry, Ratings, Cart, Checkout, PlatformFee, TerritoryPayout): registrado em Arah.Modules.Marketplace.Infrastructure.MarketplaceModule

        // Financial
        services.AddScoped<IFinancialTransactionRepository, PostgresFinancialTransactionRepository>();
        services.AddScoped<ITransactionStatusHistoryRepository, PostgresTransactionStatusHistoryRepository>();
        services.AddScoped<ISellerBalanceRepository, PostgresSellerBalanceRepository>();
        services.AddScoped<ISellerTransactionRepository, PostgresSellerTransactionRepository>();
        services.AddScoped<IPlatformFinancialBalanceRepository, PostgresPlatformFinancialBalanceRepository>();
        services.AddScoped<IPlatformRevenueTransactionRepository, PostgresPlatformRevenueTransactionRepository>();
        services.AddScoped<IPlatformExpenseTransactionRepository, PostgresPlatformExpenseTransactionRepository>();
        services.AddScoped<IReconciliationRecordRepository, PostgresReconciliationRecordRepository>();

        // Chat: registrado em Arah.Modules.Chat.Infrastructure.ChatModule

        // Media (ITerritoryMediaConfigRepository e IUserMediaPreferencesRepository já registrados no início do método)
        services.AddScoped<IMediaAssetRepository, PostgresMediaAssetRepository>();
        services.AddScoped<IMediaAttachmentRepository, PostgresMediaAttachmentRepository>();

        // Subscriptions: registrado em Arah.Modules.Subscriptions.Infrastructure.SubscriptionsModule

        // Connections (Círculo de Amigos): repositórios registrados em Arah.Modules.Connections.Infrastructure.ConnectionsModule
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
        // Media (necessário para TerritoryMediaConfigService -> EventJourneyService e outros)
        services.AddSingleton<ITerritoryMediaConfigRepository, InMemoryTerritoryMediaConfigRepository>();
        services.AddSingleton<IUserMediaPreferencesRepository, InMemoryUserMediaPreferencesRepository>();

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
        services.AddSingleton<Arah.Modules.Marketplace.Application.Interfaces.IStoreRatingResponseRepository, InMemoryStoreRatingResponseRepository>();
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
        services.AddSingleton<IFeeSplitRuleRepository, InMemoryFeeSplitRuleRepository>();

        // Subscriptions
        services.AddSingleton<ISubscriptionPlanRepository, InMemorySubscriptionPlanRepository>();
        services.AddSingleton<ISubscriptionRepository, InMemorySubscriptionRepository>();
        services.AddSingleton<ISubscriptionPaymentRepository, InMemorySubscriptionPaymentRepository>();
        services.AddSingleton<ICouponRepository, InMemoryCouponRepository>();
        services.AddSingleton<ISubscriptionCouponRepository, InMemorySubscriptionCouponRepository>();
        services.AddSingleton<ISubscriptionPlanHistoryRepository, InMemorySubscriptionPlanHistoryRepository>();
        services.AddSingleton<ITerritoryModerationRuleRepository, InMemoryTerritoryModerationRuleRepository>();
        services.AddSingleton<Arah.Application.Interfaces.Notifications.INotificationConfigRepository, InMemoryNotificationConfigRepository>();
        services.AddSingleton<IWorkItemRepository, InMemoryWorkItemRepository>();
        services.AddSingleton<IDocumentEvidenceRepository, InMemoryDocumentEvidenceRepository>();

        // Chat
        services.AddSingleton<IChatConversationRepository, InMemoryChatConversationRepository>();
        services.AddSingleton<IChatConversationParticipantRepository, InMemoryChatConversationParticipantRepository>();
        services.AddSingleton<IChatMessageRepository, InMemoryChatMessageRepository>();
        services.AddSingleton<IChatConversationStatsRepository, InMemoryChatConversationStatsRepository>();

        // Media (ITerritoryMediaConfigRepository e IUserMediaPreferencesRepository já registrados no início do método)
        services.AddSingleton<IMediaAssetRepository, InMemoryMediaAssetRepository>();
        services.AddSingleton<IMediaAttachmentRepository, InMemoryMediaAttachmentRepository>();
        services.AddSingleton<IMediaStorageConfigRepository, InMemoryMediaStorageConfigRepository>();

        return services;
    }
}
