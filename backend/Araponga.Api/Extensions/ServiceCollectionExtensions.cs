using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Media;
using Araponga.Application.Interfaces.Users;
using Araponga.Application.Services;
using Araponga.Application.Events;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.Email;
using Araponga.Infrastructure.FileStorage;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Media;
using Araponga.Infrastructure.Outbox;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Araponga.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core services
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

        // Feed services (refactored)
        services.AddScoped<PostCreationService>();
        services.AddScoped<PostEditService>();
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
        services.AddScoped<DataExportService>();
        services.AddScoped<AccountDeletionService>();
        services.AddScoped<AnalyticsService>();
        services.AddScoped<PushNotificationService>();
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
            services.AddPostgresRepositories(configuration);
            services.AddHostedService<OutboxDispatcherWorker>();
            services.AddHostedService<Araponga.Infrastructure.Background.PayoutProcessingWorker>();
            services.AddHostedService<Araponga.Infrastructure.Email.EmailQueueWorker>();
            services.AddHostedService<Araponga.Infrastructure.Email.EventReminderWorker>();
        }
        else
        {
            services.AddSingleton<InMemoryDataStore>();
            services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
            services.AddInMemoryRepositories();
        }

        services.AddSingleton<Araponga.Application.Interfaces.IObservabilityLogger, InMemoryObservabilityLogger>();
        services.AddSingleton<ITokenService, JwtTokenService>();
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
        services.AddScoped<IStoreRatingRepository, PostgresStoreRatingRepository>();
        services.AddScoped<IStoreItemRatingRepository, PostgresStoreItemRatingRepository>();
        services.AddScoped<IStoreRatingResponseRepository, PostgresStoreRatingResponseRepository>();
        services.AddScoped<ICartRepository, PostgresCartRepository>();
        services.AddScoped<ICartItemRepository, PostgresCartItemRepository>();
        services.AddScoped<ICheckoutRepository, PostgresCheckoutRepository>();
        services.AddScoped<ICheckoutItemRepository, PostgresCheckoutItemRepository>();
        services.AddScoped<IPlatformFeeConfigRepository, PostgresPlatformFeeConfigRepository>();
        services.AddScoped<ITerritoryPayoutConfigRepository, PostgresTerritoryPayoutConfigRepository>();

        // Financial
        services.AddScoped<IFinancialTransactionRepository, PostgresFinancialTransactionRepository>();
        services.AddScoped<ITransactionStatusHistoryRepository, PostgresTransactionStatusHistoryRepository>();
        services.AddScoped<ISellerBalanceRepository, PostgresSellerBalanceRepository>();
        services.AddScoped<ISellerTransactionRepository, PostgresSellerTransactionRepository>();
        services.AddScoped<IPlatformFinancialBalanceRepository, PostgresPlatformFinancialBalanceRepository>();
        services.AddScoped<IPlatformRevenueTransactionRepository, PostgresPlatformRevenueTransactionRepository>();
        services.AddScoped<IPlatformExpenseTransactionRepository, PostgresPlatformExpenseTransactionRepository>();
        services.AddScoped<IReconciliationRecordRepository, PostgresReconciliationRecordRepository>();

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

        // Media
        services.AddScoped<IMediaAssetRepository, PostgresMediaAssetRepository>();
        services.AddScoped<IMediaAttachmentRepository, PostgresMediaAttachmentRepository>();
        // TODO: Implementar PostgresTerritoryMediaConfigRepository e PostgresUserMediaPreferencesRepository
        // services.AddScoped<ITerritoryMediaConfigRepository, Araponga.Infrastructure.Postgres.PostgresTerritoryMediaConfigRepository>();
        // services.AddScoped<IUserMediaPreferencesRepository, Araponga.Infrastructure.Postgres.PostgresUserMediaPreferencesRepository>();

        // Policies
        services.AddScoped<ITermsOfServiceRepository, PostgresTermsOfServiceRepository>();
        services.AddScoped<ITermsAcceptanceRepository, PostgresTermsAcceptanceRepository>();
        services.AddScoped<IPrivacyPolicyRepository, PostgresPrivacyPolicyRepository>();
        services.AddScoped<IPrivacyPolicyAcceptanceRepository, PostgresPrivacyPolicyAcceptanceRepository>();

        // Push Notifications
        services.AddScoped<IUserDeviceRepository, PostgresUserDeviceRepository>();

        // Push Notification Provider (opcional - configurar Firebase:ServerKey para habilitar)
        var firebaseServerKey = configuration["Firebase:ServerKey"];
        if (!string.IsNullOrWhiteSpace(firebaseServerKey))
        {
            services.AddScoped<IPushNotificationProvider, Araponga.Infrastructure.Notifications.FirebasePushNotificationProvider>();
        }

        // Email Configuration
        services.Configure<EmailConfiguration>(configuration.GetSection("Email"));
        services.AddScoped<IEmailSender, SmtpEmailSender>();

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

        // Media
        services.AddSingleton<IMediaAssetRepository, InMemoryMediaAssetRepository>();
        services.AddSingleton<IMediaAttachmentRepository, InMemoryMediaAttachmentRepository>();
        services.AddSingleton<ITerritoryMediaConfigRepository, InMemoryTerritoryMediaConfigRepository>();
        services.AddSingleton<IUserMediaPreferencesRepository, InMemoryUserMediaPreferencesRepository>();
        services.AddSingleton<IMediaStorageConfigRepository, InMemoryMediaStorageConfigRepository>();

        // Policies
        services.AddSingleton<ITermsOfServiceRepository, InMemoryTermsOfServiceRepository>();
        services.AddSingleton<ITermsAcceptanceRepository, InMemoryTermsAcceptanceRepository>();
        services.AddSingleton<IPrivacyPolicyRepository, InMemoryPrivacyPolicyRepository>();
        services.AddSingleton<IPrivacyPolicyAcceptanceRepository, InMemoryPrivacyPolicyAcceptanceRepository>();

        // Push Notifications
        services.AddSingleton<IUserDeviceRepository, InMemoryUserDeviceRepository>();

        return services;
    }
}
