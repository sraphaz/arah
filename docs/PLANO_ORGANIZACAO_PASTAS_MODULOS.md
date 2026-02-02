# Plano: Organização em pastas por módulo (Application e API)

**Data**: 2026-02-02  
**Objetivo**: Fronteiras prontas para split — Application e API organizadas por domínio em pastas, sem alterar namespaces (apenas estrutura de pastas).

---

## 1. Escopo

- **Application/Services**: criar pastas por módulo e mover serviços (manter `namespace Araponga.Application.Services`).
- **Application/Interfaces**: criar pastas por módulo e mover interfaces (manter `namespace Araponga.Application.Interfaces` ou subnamespaces existentes).
- **Api/Controllers**: criar pastas por módulo e mover controllers (manter namespace atual).

Namespaces **não** serão alterados para evitar quebra de usings em todo o código; apenas a organização em pastas.

---

## 2. Mapeamento Services → pasta

| Pasta | Serviços |
|-------|----------|
| **Feed** | FeedService, PostCreationService, PostEditService, PostFilterService, PostInteractionService |
| **Events** | EventsService, EventCacheService |
| **Map** | MapService, MapEntityCacheService |
| **Chat** | ChatService |
| **Alerts** | HealthService, AlertCacheService |
| **Assets** | TerritoryAssetService |
| **Marketplace** | StoreService, StoreItemService, CartService, InquiryService, RatingService, PlatformFeeService, MarketplaceSearchService, SellerPayoutService, TerritoryPayoutConfigService |
| **Moderation** | ReportService, TerritoryModerationService, ModerationCaseService, DocumentEvidenceService, WorkQueueService, VerificationQueueService, UserBlockService, UserBlockCacheService |
| **Notifications** | PushNotificationService; manter subpasta Notifications/ (NotificationConfigService) |
| **Subscriptions** | SubscriptionService, CouponService, SubscriptionPlanAdminService, SubscriptionCapabilityService, SubscriptionPlanSeedService, SubscriptionTrialService, SubscriptionRenewalService, SubscriptionAnalyticsService, StripeWebhookService, MercadoPagoWebhookService |
| **Media** | MediaService; manter subpasta Media/ (MediaStorageConfigService, TerritoryMediaConfigService) |
| **Users** | UserPreferencesService, UserProfileService, UserProfileStatsService, UserActivityService, UserInterestService; manter subpasta Users/ (UserMediaPreferencesService) |
| **Core** | TerritoryService, TerritoryCacheService, TerritoryCharacterizationService, TerritoryFeatureFlagGuard, ActiveTerritoryService, MembershipService, MembershipAccessRules, MembershipCapabilityService, AuthService, PasswordResetService, JoinRequestService, ResidencyRequestService, AccountDeletionService, DataExportService, FeatureFlagService, FeatureFlagCacheService, SystemConfigService, SystemConfigCacheService, SystemPermissionService, AccessEvaluator, PolicyRequirementService, TermsOfServiceService, TermsAcceptanceService, PrivacyPolicyService, PrivacyPolicyAcceptanceService, VotingService, AuditService, CacheInvalidationService, CacheMetricsService, EmailQueueService, EmailTemplateService, EmailNotificationMapper, InputSanitizationService, AnalyticsService |

---

## 3. Mapeamento Interfaces → pasta

| Pasta | Interfaces (repositórios e serviços de aplicação) |
|-------|---------------------------------------------------|
| **Feed** | IFeedRepository, IPostGeoAnchorRepository, IPostAssetRepository |
| **Events** | ITerritoryEventRepository, IEventParticipationRepository |
| **Map** | IMapRepository, IMapEntityRelationRepository |
| **Chat** | IChatConversationRepository, IChatConversationParticipantRepository, IChatMessageRepository, IChatConversationStatsRepository |
| **Alerts** | IHealthAlertRepository |
| **Assets** | ITerritoryAssetRepository, IAssetGeoAnchorRepository, IAssetValidationRepository |
| **Marketplace** | IStoreRepository, IStoreItemRepository, IInquiryRepository, IStoreRatingRepository, IStoreItemRatingRepository, IStoreRatingResponseRepository, ICartRepository, ICartItemRepository, ICheckoutRepository, ICheckoutItemRepository, IPlatformFeeConfigRepository, ITerritoryPayoutConfigRepository, IFinancialTransactionRepository, ITransactionStatusHistoryRepository, ISellerBalanceRepository, ISellerTransactionRepository, IPlatformFinancialBalanceRepository, IPlatformRevenueTransactionRepository, IPlatformExpenseTransactionRepository, IReconciliationRecordRepository |
| **Moderation** | IReportRepository, ISanctionRepository, IWorkItemRepository, IDocumentEvidenceRepository, ITerritoryModerationRuleRepository |
| **Notifications** | INotificationInboxRepository; manter Notifications/ (INotificationConfigRepository) |
| **Subscriptions** | ISubscriptionPlanRepository, ISubscriptionRepository, ISubscriptionPaymentRepository, ICouponRepository, ISubscriptionCouponRepository, ISubscriptionPlanHistoryRepository, ISubscriptionGateway, ISubscriptionGatewayFactory, IStripeSubscriptionService |
| **Media** | Manter pasta Media/ (IMediaAssetRepository, IMediaAttachmentRepository, etc.) |
| **Users** | Manter pasta Users/ (IUserMediaPreferencesRepository); interfaces de User/Territory/Membership em Core |
| **Core** | ITerritoryRepository, IUserRepository, ITerritoryMembershipRepository, ITerritoryJoinRequestRepository, IActiveTerritoryStore, IFeatureFlagService, IAuditLogger, IOutbox, IUserBlockRepository, IUserPreferencesRepository, IUserInterestRepository, IVotingRepository, IVoteRepository, ITerritoryCharacterizationRepository, IMembershipSettingsRepository, IMembershipCapabilityRepository, ISystemPermissionRepository, ISystemConfigRepository, IUserDeviceRepository, IEmailQueueRepository, ITermsOfServiceRepository, ITermsAcceptanceRepository, IPrivacyPolicyRepository, IPrivacyPolicyAcceptanceRepository, IModule, IModuleRegistry, IUnitOfWork, ITokenService, IEmailSender, IEmailTemplateService, IFileStorage, IPayoutGateway, IDistributedCacheService, IObservabilityLogger, IPushNotificationProvider, IAuditRepository |

---

## 4. Mapeamento Controllers → pasta

| Pasta | Controllers |
|-------|-------------|
| **Feed** | FeedController |
| **Events** | EventsController |
| **Map** | MapController |
| **Chat** | ChatController, TerritoryChatController |
| **Alerts** | AlertsController |
| **Assets** | AssetsController |
| **Marketplace** | StoresController, ItemsController, CartController, InquiriesController, RatingController, PlatformFeesController, TerritoryPayoutConfigController, MarketplaceSearchController, SellerBalanceController, PlatformFinancialController |
| **Moderation** | ModerationController, TerritoryModerationCasesController, AdminWorkItemsController, AdminEvidencesController, TerritoryWorkItemsController, TerritoryEvidencesController |
| **Notifications** | NotificationsController, NotificationConfigController |
| **Subscriptions** | SubscriptionsController, SubscriptionPlansController, TerritorySubscriptionPlansController, CouponsController, AdminCouponsController, AdminSubscriptionPlansController, SubscriptionCapabilitiesController, SubscriptionAnalyticsController, StripeWebhookController, MercadoPagoWebhookController |
| **Media** | MediaController, MediaConfigController, MediaStorageConfigController, UserMediaPreferencesController |
| **Users** | UserPreferencesController, UserProfileController, UserPublicProfileController, UserInterestsController, UserActivityController |
| **Core** | AuthController, TerritoriesController, MembershipsController, JoinRequestsController, FeaturesController, DevicesController, DataExportController, CacheMetricsController, TermsOfServiceController, PrivacyPolicyController, AnalyticsController, VerificationController, VerificationUploadController, AdminVerificationController, TerritoryVerificationController, VotingsController, AdminSystemConfigController, AdminSeedController |

---

## 5. Ordem de execução

1. Branch `feat/organizacao-pastas-modulos` e commit do estado atual.
2. Application/Services: criar pastas e mover arquivos (namespaces inalterados).
3. Application/Interfaces: criar pastas e mover arquivos (namespaces inalterados).
4. Api/Controllers: criar pastas e mover arquivos (namespaces inalterados).
5. Build e testes; ajustes se necessário.

---

## 6. Observações

- **Namespaces**: não mudam; evita centenas de `using` em toda a solução.
- **.csproj**: projetos .NET incluem recursivamente `**\*.cs`; não é necessário alterar .csproj ao mover para subpastas.
- **Core**: serviços e interfaces compartilhados (Territory, User, Auth, Membership, Policies, etc.) ficam em `Core/` para não duplicar módulos inexistentes.
