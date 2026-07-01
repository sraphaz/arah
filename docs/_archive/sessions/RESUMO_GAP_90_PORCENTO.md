# 📊 Resumo: O que falta para 90% de Cobertura

**Data**: 2026-01-25  
**Status Atual Medido** (após Fase 4 + Fase 2 lotes 1–8 + Fase 3 branches):
- **Domain Layer**: ~84–85% linhas, ~76% branches
- **Application Layer**: ~70–72% linhas, ~54% branches
- **Suíte**: 2021 passando, 20 skipped, 0 failed.

---

## 🎯 Gap para 90%

### Domain Layer
- **Gap Linhas**: ~5% (~84–85% → 90%)
- **Gap Branches**: ~14% (~76% → 90%)
- **Prioridade**: 🟡 Média
- **Testes Necessários**: ~20-30 testes focados em branches (Fase 4 parcialmente feita)

### Application Layer
- **Gap Linhas**: ~20% (~70–72% → 90%)
- **Gap Branches**: ~36% (~54% → 90%) ⚠️ **CRÍTICO**
- **Prioridade**: 🔴 Alta
- **Testes Necessários**: ~300-450 testes (Fase 2 em progresso)

---

## 🔴 Application Layer - O que falta (Prioridade Alta)

### Serviços sem Testes de Edge Cases (60-90 testes)

1. **ResidencyRequestService** - 8-12 testes
2. **AccountDeletionService** - 8-12 testes
3. **TerritoryModerationService** - 8-12 testes
4. **PlatformFeeService** - 6-10 testes
5. **SellerPayoutService** - 8-12 testes
6. **StoreService** - 8-12 testes (edge cases adicionais)
7. **PostCreationService** - 8-12 testes (edge cases adicionais)
8. **MembershipService** - 8-12 testes (edge cases adicionais)

**Impacto**: +14% cobertura (66% → 80%)

### Serviços com Testes Básicos (Precisam Edge Cases) (~220-300 testes)

**Serviços de Usuário**:
- UserProfileService, UserProfileStatsService, UserPreferencesService
- UserInterestService, InterestFilterService
- UserBlockService

**Serviços de Marketplace**:
- StoreItemService, **CartService** ✅ (lote 8), **InquiryService** ✅ (lote 8), MarketplaceSearchService

**Serviços de Sistema**:
- SystemPermissionService, MembershipCapabilityService
- WorkQueueService, VerificationQueueService
- TerritoryPayoutConfigService

**Serviços de Conteúdo**:
- PostInteractionService, PostEditService, PostFilterService
- TerritoryService, MapService

**Serviços de Política**:
- TermsAcceptanceService, PrivacyPolicyAcceptanceService
- TermsOfServiceService, PrivacyPolicyService
- PolicyRequirementService

**Outros Serviços**:
- AnalyticsService, DataExportService
- PushNotificationService, NotificationConfigService
- DocumentEvidenceService, HealthService
- ActiveTerritoryService, SystemConfigService
- InputSanitizationService, AuditService
- CacheInvalidationService, VotingService
- MediaStorageConfigService, TerritoryMediaConfigService

**Impacto**: +5% cobertura (80% → 85%)

### Cobertura de Branches (50-70 testes)

**Foco**: Testar todos os caminhos condicionais, casos de erro, validações e transições de estado

**Impacto**: +5% cobertura (85% → 90%+)

---

## 🟡 Domain Layer - O que falta (Prioridade Média)

### Cobertura de Branches (30-40 testes)

**Foco**:
- Testar todas as validações de entidades
- Testar todas as transições de estado
- Testar todos os edge cases de value objects
- Testar todos os métodos de domínio

**Entidades com Baixa Cobertura**:
- TerritoryEvent, EventParticipation
- ChatConversation, ChatMessage
- MediaAttachment (edge cases)
- Entidades financeiras (edge cases)

**Impacto**: +8% linhas, +16% branches (82% → 90%+)

---

## 📋 Plano de Ação Resumido

### Fase 1: Application Layer - Serviços Críticos (2-3 semanas)
- **8 serviços prioritários**
- **60-90 testes**
- **Objetivo**: 66% → 80%

### Fase 2: Application Layer - Serviços Médios (2-3 semanas)
- **~40 serviços**
- **220-300 testes**
- **Objetivo**: 80% → 85%

### Fase 3: Application Layer - Branches (1-2 semanas)
- **50-70 testes focados em branches**
- **Objetivo**: 85% → 90%+

### Fase 4: Domain Layer - Branches (1 semana)
- **30-40 testes focados em branches**
- **Objetivo**: 74% → 90%+

---

## 📊 Resumo Executivo

| Item | Quantidade |
|------|------------|
| **Total de Testes Necessários** | 360-500 testes |
| **Tempo Estimado** | 6-9 semanas |
| **Prioridade Máxima** | Application Layer (branches: 50.39%) |
| **Maior Gap** | Application Layer branches (~40%) |

---

## 🎯 Recomendação Imediata

**Começar pela Fase 1** (Application Layer - Serviços Críticos):
1. ResidencyRequestService
2. AccountDeletionService
3. TerritoryModerationService
4. PlatformFeeService
5. SellerPayoutService
6. StoreService (edge cases)
7. PostCreationService (edge cases)
8. MembershipService (edge cases)

**Impacto Esperado**: +14% cobertura (66% → 80%)

---

**Ver plano detalhado**: [`PLANO_90_PORCENTO_COBERTURA_DETALHADO.md`](./PLANO_90_PORCENTO_COBERTURA_DETALHADO.md)
