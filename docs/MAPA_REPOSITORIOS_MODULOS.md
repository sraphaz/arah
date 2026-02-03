# Mapeamento de Repositórios para Módulos

**Data**: 2026-01-28  
**Status**: 📋 Em Progresso  
**Tipo**: Documentação Técnica - Migração

---

## 📋 Objetivo

Este documento mapeia quais repositórios devem ser migrados de `Araponga.Infrastructure.Postgres` para os módulos correspondentes.

---

## 🗺️ Mapeamento Completo

### ✅ Feed Module (Já Migrado)
- ✅ `IFeedRepository` → `PostgresFeedRepository` (já em `Araponga.Modules.Feed.Infrastructure`)

### ✅ Chat Module (Migrado)
- ✅ `IChatConversationRepository` → `PostgresChatConversationRepository` (em Araponga.Modules.Chat.Infrastructure)
- ✅ `IChatConversationParticipantRepository` → `PostgresChatConversationParticipantRepository`
- ✅ `IChatMessageRepository` → `PostgresChatMessageRepository`
- ✅ `IChatConversationStatsRepository` → `PostgresChatConversationStatsRepository`

### ✅ Events Module (Migrado)
- ✅ `ITerritoryEventRepository` → `PostgresTerritoryEventRepository` (em Araponga.Modules.Events.Infrastructure)
- ✅ `IEventParticipationRepository` → `PostgresEventParticipationRepository`

### ✅ Map Module (Migrado)
- ✅ `IMapRepository` → `PostgresMapRepository` (em Araponga.Modules.Map.Infrastructure)
- ✅ `IMapEntityRelationRepository` → `PostgresMapEntityRelationRepository`

### ✅ Marketplace Module (Migrado)
- ✅ `IStoreRepository` → `PostgresStoreRepository` (em Araponga.Modules.Marketplace.Infrastructure)
- ✅ `IStoreItemRepository` → `PostgresStoreItemRepository`
- ✅ `IInquiryRepository` → `PostgresInquiryRepository`
- ✅ `IStoreRatingRepository` → `PostgresStoreRatingRepository`
- ✅ `IStoreItemRatingRepository` → `PostgresStoreItemRatingRepository`
- ✅ `IStoreRatingResponseRepository` → `PostgresStoreRatingResponseRepository`
- ✅ `ICartRepository` → `PostgresCartRepository`
- ✅ `ICartItemRepository` → `PostgresCartItemRepository`
- ✅ `ICheckoutRepository` → `PostgresCheckoutRepository`
- ✅ `ICheckoutItemRepository` → `PostgresCheckoutItemRepository`
- ✅ `IPlatformFeeConfigRepository` → `PostgresPlatformFeeConfigRepository`
- ✅ `ITerritoryPayoutConfigRepository` → `PostgresTerritoryPayoutConfigRepository`

### ✅ Subscriptions Module (Migrado)
- ✅ `ISubscriptionPlanRepository` → `PostgresSubscriptionPlanRepository` (em Araponga.Modules.Subscriptions.Infrastructure)
- ✅ `ISubscriptionRepository` → `PostgresSubscriptionRepository`
- ✅ `ISubscriptionPaymentRepository` → `PostgresSubscriptionPaymentRepository`
- ✅ `ICouponRepository` → `PostgresCouponRepository`
- ✅ `ISubscriptionCouponRepository` → `PostgresSubscriptionCouponRepository`
- ✅ `ISubscriptionPlanHistoryRepository` → `PostgresSubscriptionPlanHistoryRepository`

### ✅ Moderation Module (Migrado)
- ✅ `IReportRepository` → `PostgresReportRepository` (em Araponga.Modules.Moderation.Infrastructure)
- ✅ `ISanctionRepository` → `PostgresSanctionRepository`
- ✅ `IWorkItemRepository` → `PostgresWorkItemRepository`
- ✅ `IDocumentEvidenceRepository` → `PostgresDocumentEvidenceRepository`
- ✅ `ITerritoryModerationRuleRepository` → `PostgresTerritoryModerationRuleRepository`

### ✅ Notifications Module (Migrado)
- ✅ `INotificationInboxRepository` → `PostgresNotificationInboxRepository` (em Araponga.Modules.Notifications.Infrastructure)
- ✅ `INotificationConfigRepository` → `PostgresNotificationConfigRepository`

### ✅ Alerts Module (Migrado)
- ✅ `IHealthAlertRepository` → `PostgresHealthAlertRepository` (em Araponga.Modules.Alerts.Infrastructure)

### ✅ Assets Module (Migrado)
- ✅ `ITerritoryAssetRepository` → `PostgresAssetRepository` (em Araponga.Modules.Assets.Infrastructure)
- ✅ `IAssetGeoAnchorRepository` → `PostgresAssetGeoAnchorRepository`
- ✅ `IAssetValidationRepository` → `PostgresAssetValidationRepository`

### 📍 Shared/Infrastructure (Permanecer)
Estes repositórios devem permanecer em `Araponga.Infrastructure.Postgres` ou `Araponga.Infrastructure.Shared`:

- ✅ `ITerritoryRepository` → `PostgresTerritoryRepository` (Shared - core)
- ✅ `IUserRepository` → `PostgresUserRepository` (Shared - core)
- ✅ `ITerritoryMembershipRepository` → `PostgresTerritoryMembershipRepository` (Shared - core)
- ✅ `ITerritoryJoinRequestRepository` → `PostgresTerritoryJoinRequestRepository` (Shared - core)
- ✅ `IUserPreferencesRepository` → `PostgresUserPreferencesRepository` (Shared)
- ✅ `IUserInterestRepository` → `PostgresUserInterestRepository` (Shared)
- ✅ `IUserBlockRepository` → `PostgresUserBlockRepository` (Shared)
- ✅ `IUserDeviceRepository` → `PostgresUserDeviceRepository` (Shared)
- ✅ `IMembershipSettingsRepository` → `PostgresMembershipSettingsRepository` (Shared)
- ✅ `IMembershipCapabilityRepository` → `PostgresMembershipCapabilityRepository` (Shared)
- ✅ `ISystemPermissionRepository` → `PostgresSystemPermissionRepository` (Shared)
- ✅ `ISystemConfigRepository` → `PostgresSystemConfigRepository` (Shared)
- ✅ `IVotingRepository` → `PostgresVotingRepository` (Shared - Governança)
- ✅ `IVoteRepository` → `PostgresVoteRepository` (Shared - Governança)
- ✅ `ITerritoryCharacterizationRepository` → `PostgresTerritoryCharacterizationRepository` (Shared)
- ✅ `IFeatureFlagService` → `PostgresFeatureFlagService` (Shared)
- ✅ `IAuditLogger` → `PostgresAuditLogger` (Shared)
- ✅ `IOutbox` → `PostgresOutbox` (Shared)
- ✅ `IPostGeoAnchorRepository` → `PostgresPostGeoAnchorRepository` (Feed - mas pode ficar em Feed)
- ✅ `IPostAssetRepository` → `PostgresPostAssetRepository` (Feed - mas pode ficar em Feed)
- ✅ `IActiveTerritoryStore` → `PostgresActiveTerritoryStore` (Shared)
- ✅ `IMediaAssetRepository` → `PostgresMediaAssetRepository` (Shared - cross-cutting)
- ✅ `IMediaAttachmentRepository` → `PostgresMediaAttachmentRepository` (Shared - cross-cutting)
- ✅ `ITermsOfServiceRepository` → `PostgresTermsOfServiceRepository` (Shared - Policies)
- ✅ `ITermsAcceptanceRepository` → `PostgresTermsAcceptanceRepository` (Shared - Policies)
- ✅ `IPrivacyPolicyRepository` → `PostgresPrivacyPolicyRepository` (Shared - Policies)
- ✅ `IPrivacyPolicyAcceptanceRepository` → `PostgresPrivacyPolicyAcceptanceRepository` (Shared - Policies)
- ✅ `IEmailQueueRepository` → `PostgresEmailQueueRepository` (Shared - Email)

### 💰 Financial (A Decidir)
Estes repositórios financeiros podem ficar em Marketplace ou em um módulo Finance separado:

- ⚠️ `IFinancialTransactionRepository` → `PostgresFinancialTransactionRepository`
- ⚠️ `ITransactionStatusHistoryRepository` → `PostgresTransactionStatusHistoryRepository`
- ⚠️ `ISellerBalanceRepository` → `PostgresSellerBalanceRepository`
- ⚠️ `ISellerTransactionRepository` → `PostgresSellerTransactionRepository`
- ⚠️ `IPlatformFinancialBalanceRepository` → `PostgresPlatformFinancialBalanceRepository`
- ⚠️ `IPlatformRevenueTransactionRepository` → `PostgresPlatformRevenueTransactionRepository`
- ⚠️ `IPlatformExpenseTransactionRepository` → `PostgresPlatformExpenseTransactionRepository`
- ⚠️ `IReconciliationRecordRepository` → `PostgresReconciliationRecordRepository`

---

## 📊 Estatísticas

| Status | Quantidade | Percentual |
|--------|------------|------------|
| ✅ Já Migrado | Todos os módulos (Feed, Chat, Events, Map, Marketplace, Subscriptions, Moderation, Notifications, Alerts, Assets) | 100% dos módulos |
| 📍 Permanecer | ~25 (Shared/Infrastructure) | ~37% |
| ⚠️ A Decidir | 8 (Financial) | ~12% |

---

## 🎯 Prioridade de Migração

### Alta Prioridade (Módulos com Stubs)
1. **Chat Module** (4 repositórios) - Módulo já referenciado, precisa de implementação
2. **Events Module** (2 repositórios) - Módulo já referenciado, precisa de implementação
3. **Map Module** (2 repositórios) - Módulo já referenciado, precisa de implementação

### Média Prioridade
4. **Subscriptions Module** (6 repositórios) - Módulo já referenciado
5. **Moderation Module** (5 repositórios) - Módulo já referenciado
6. **Alerts Module** (1 repositório) - Módulo já referenciado
7. **Assets Module** (3 repositórios) - Módulo já referenciado
8. **Notifications Module** (2 repositórios) - Módulo já referenciado

### Baixa Prioridade
9. **Marketplace Module** - Já tem DbContext, migração pode ser gradual
10. **Financial** - Decidir se fica em Marketplace ou módulo separado

---

## 📝 Notas

- A migração deve ser feita **gradualmente**, testando após cada módulo
- Cada módulo deve ter seu próprio `DbContext` quando necessário
- Repositórios que dependem de `ArapongaDbContext` precisarão ser refatorados para usar o `DbContext` do módulo ou `SharedDbContext`
- Manter compatibilidade durante a migração (registrar em ambos os lugares temporariamente)

---

**Última atualização**: 2026-02-02
