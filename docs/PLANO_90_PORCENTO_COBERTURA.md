# 🎯 Plano para Atingir 90% de Cobertura em Todas as Camadas

**Última Atualização**: 2026-01-25  
**Status Atual** (medido após Fase 4 + Fase 2 lotes 1–8 + Fase 3 branches): 
- Domain Layer: **~84–85% linhas, ~76% branches**
- Application Layer: **~70–72% linhas, ~54% branches**
- **Suíte**: 2021 passando, 20 skipped, 0 failed.

**Meta**: 90%+ em todas as camadas  
**Gap**: 
- Domain: ~5% linhas, ~14% branches
- Application: ~20% linhas, ~36% branches

**Estimativa**: 360-500 testes adicionais (ver plano detalhado)

**Progresso 90% (2026-01-24)**:
- ✅ **Fase 4 Domain**: +55 testes (TerritoryMembership, MapEntityCategory, MapEntity, TerritoryCharacterization, Event Update title). Domain: 82%→84.5% linhas, 74%→76% branches.
- ✅ **Fase 2 (lote 1)**: UserProfileServiceEdgeCasesTests (7), AnalyticsServiceEdgeCasesTests (5). Application: 66%→70% linhas, 50%→54% branches.
- ✅ **Fase 2 (lote 2)**: DataExportServiceEdgeCasesTests (3), UserBlockServiceEdgeCasesTests (6), VotingServiceEdgeCasesTests (7), AuditServiceEdgeCasesTests (3), CacheInvalidationServiceEdgeCasesTests (9). +28 testes.
- ✅ **Fase 2 (lote 3)**: InputSanitizationServiceEdgeCasesTests (16), DocumentEvidenceServiceEdgeCasesTests (7), HealthServiceEdgeCasesTests (6), NotificationConfigServiceEdgeCasesTests (5). +32 testes.
- ✅ **Fase 2 (lote 4)**: InterestFilterServiceEdgeCasesTests (2), UserInterestServiceEdgeCasesTests (5), SystemConfigServiceEdgeCasesTests (4), ActiveTerritoryServiceEdgeCasesTests (4). +15 testes.
- ✅ **Fase 2 (lote 5)**: TerritoryServiceEdgeCasesTests (8), PostFilterServiceEdgeCasesTests (3), PostEditServiceEdgeCasesTests (3). +14 testes.
- ✅ **Fase 2 (lote 6)**: PostInteractionServiceEdgeCasesTests (6), MapServiceEdgeCasesTests (3). +9 testes.
- ✅ **Fase 2 (lote 7)**: PushNotificationServiceEdgeCasesTests (5), MarketplaceSearchServiceEdgeCasesTests (3), RatingServiceEdgeCasesTests (6), ReportServiceEdgeCasesTests (2), WorkQueueServiceEdgeCasesTests (3). +19 testes.
- ✅ **Fase 2 (lote 8)**: InquiryServiceEdgeCasesTests (3), CartServiceEdgeCasesTests (2), TermsAcceptanceServiceEdgeCasesTests (3), PolicyRequirementServiceEdgeCasesTests (2). +10 testes.
- ✅ **Fase 3 (branches)**: PostFilterService mapEntityId filter (+1), MapService resident vs public (+1).
- ✅ **Domain (branches)**: EventParticipation UpdateStatus FromConfirmedToInterested (+1), ChatMessage Edit empty newText (+1).
- **Total de testes**: 2021 passando, 20 skipped.

---

## 📊 Situação Atual (Medida em 2026-01-25)

| Camada | Cobertura Linhas | Cobertura Branches | Meta | Gap Linhas | Gap Branches | Prioridade |
|--------|------------------|-------------------|------|------------|--------------|------------|
| Domain Layer | ~84–85% | ~76% | 90%+ | ~5% | ~14% | 🟡 Média |
| Application Layer | ~70–72% | ~54% | 90%+ | ~20% | ~36% | 🔴 Alta |
| Infrastructure Layer | ~75% (estimada) | ~75% (estimada) | 90%+ | ~15% | ~15% | 🔴 Alta |
| API Layer | ~80% (estimada) | ~80% (estimada) | 90%+ | ~10% | ~10% | 🟡 Média |

**Nota**: Application Layer tem gap significativo em branches. Prioridade máxima. **Coverage**: `./scripts/run-coverage.ps1` localmente; CI envia ao Codecov e exibe resumo no Job Summary.

---

## 🎯 Phase 6: Domain Layer - Entidades Restantes

**Objetivo**: ~85% → 90%+  
**Estimativa**: 20-30 testes  
**Prioridade**: 🟡 Média

### Entidades Identificadas para Cobertura

#### 1. Media Entities (5-8 testes)
- ✅ `MediaAsset` - Já tem testes básicos
- ⚠️ `MediaAttachment` - Já tem testes básicos
- ❌ **Faltam edge cases**:
  - Unicode em nomes de arquivo
  - Tamanhos de arquivo extremos
  - Tipos MIME inválidos
  - URLs malformadas

**Arquivo**: `backend/Araponga.Tests/Domain/Media/MediaEdgeCasesTests.cs` (NOVO)

#### 2. Events Entities (5-8 testes)
- ❌ `TerritoryEvent` - Sem testes de edge cases
- ❌ `EventParticipation` - Sem testes de edge cases
- **Edge cases necessários**:
  - Coordenadas geográficas inválidas
  - Datas no passado/futuro extremo
  - Capacidade zero/negativa
  - Status transitions
  - Unicode em títulos/descrições

**Arquivo**: `backend/Araponga.Tests/Domain/Events/EventEdgeCasesTests.cs` (NOVO)

#### 3. Chat Entities (4-6 testes)
- ❌ `ChatConversation` - Sem testes de edge cases
- ❌ `ChatMessage` - Sem testes de edge cases
- **Edge cases necessários**:
  - Mensagens vazias/null
  - Unicode em mensagens
  - Timestamps inválidos
  - Status transitions

**Arquivo**: `backend/Araponga.Tests/Domain/Chat/ChatEdgeCasesTests.cs` (NOVO)

#### 4. Assets Entities (3-5 testes)
- ✅ `TerritoryAsset` - Já tem testes básicos
- ❌ **Faltam edge cases**:
  - GeoAnchors inválidos
  - Status transitions
  - Unicode em nomes/descrições

**Arquivo**: `backend/Araponga.Tests/Domain/Assets/AssetEdgeCasesTests.cs` (NOVO)

#### 5. Financial Entities (3-5 testes)
- ✅ `SellerBalance`, `SellerTransaction` - Já tem testes básicos
- ❌ **Faltam edge cases**:
  - Valores negativos/zero
  - Transições de status
  - Moedas inválidas

**Arquivo**: `backend/Araponga.Tests/Domain/Financial/FinancialEdgeCasesTests.cs` (NOVO)

**Total Phase 6**: ~20-30 testes

---

## 🎯 Phase 7: Application Layer - Serviços Adicionais

**Objetivo**: ~75% → 90%+  
**Estimativa**: 50-70 testes  
**Prioridade**: 🔴 Alta

### Serviços Identificados para Cobertura

#### 1. MediaService (10-15 testes)
- ❌ Upload de arquivos com edge cases
- ❌ Validação de tipos MIME
- ❌ Limites de tamanho
- ❌ Unicode em nomes de arquivo
- ❌ Error handling

**Arquivo**: `backend/Araponga.Tests/Application/MediaServiceEdgeCasesTests.cs` (NOVO)

#### 2. EventService (8-12 testes)
- ❌ Criação com coordenadas inválidas
- ❌ Datas no passado/futuro
- ❌ Capacidade zero/negativa
- ❌ Participação edge cases
- ❌ Error handling

**Arquivo**: `backend/Araponga.Tests/Application/EventServiceEdgeCasesTests.cs` (NOVO)

#### 3. ChatService (8-12 testes)
- ❌ Mensagens vazias/null
- ❌ Unicode em mensagens
- ❌ Timestamps inválidos
- ❌ Conversas com participantes inválidos
- ❌ Error handling

**Arquivo**: `backend/Araponga.Tests/Application/ChatServiceEdgeCasesTests.cs` (NOVO)

#### 4. AssetService (6-10 testes)
- ❌ GeoAnchors inválidos
- ❌ Status transitions
- ❌ Validação de território
- ❌ Error handling

**Arquivo**: `backend/Araponga.Tests/Application/AssetServiceEdgeCasesTests.cs` (NOVO)

#### 5. FinancialService (8-12 testes)
- ❌ Valores negativos/zero
- ❌ Transações inválidas
- ❌ Moedas inválidas
- ❌ Error handling

**Arquivo**: `backend/Araponga.Tests/Application/FinancialServiceEdgeCasesTests.cs` (NOVO)

#### 6. VerificationService (5-8 testes)
- ❌ Documentos inválidos
- ❌ Status transitions
- ❌ Error handling

**Arquivo**: `backend/Araponga.Tests/Application/VerificationServiceEdgeCasesTests.cs` (NOVO)

#### 7. JoinRequestService (5-8 testes)
- ✅ Já tem testes básicos
- ❌ **Faltam edge cases**:
  - Status transitions
  - Validação de território
  - Error handling

**Arquivo**: `backend/Araponga.Tests/Application/JoinRequestServiceEdgeCasesTests.cs` (NOVO)

**Total Phase 7**: ~50-70 testes

---

## 🎯 Phase 8: Infrastructure Layer - Repositórios e Storage

**Objetivo**: ~75% → 90%+  
**Estimativa**: 30-40 testes  
**Prioridade**: 🔴 Alta

### Componentes Identificados para Cobertura

#### 1. Postgres Repositories (15-20 testes)
- ❌ Testes de integração com banco real
- ❌ Transações e rollback
- ❌ Concorrência
- ❌ Performance com grandes volumes
- ❌ Error handling de conexão

**Arquivo**: `backend/Araponga.Tests/Infrastructure/Postgres/PostgresRepositoryIntegrationTests.cs` (NOVO)

#### 2. File Storage (8-12 testes)
- ❌ `LocalFileStorage` edge cases
- ❌ `S3FileStorage` edge cases
- ❌ Upload de arquivos grandes
- ❌ Unicode em nomes de arquivo
- ❌ Error handling

**Arquivo**: `backend/Araponga.Tests/Infrastructure/FileStorage/FileStorageEdgeCasesTests.cs` (NOVO)

#### 3. Email Services (5-8 testes)
- ❌ `SmtpEmailSender` edge cases
- ❌ `LoggingEmailSender` edge cases
- ❌ Unicode em emails
- ❌ Error handling

**Arquivo**: `backend/Araponga.Tests/Infrastructure/Email/EmailServiceEdgeCasesTests.cs` (NOVO)

#### 4. Event Bus (3-5 testes)
- ❌ `InMemoryEventBus` edge cases
- ❌ Error handling
- ❌ Concorrência

**Arquivo**: `backend/Araponga.Tests/Infrastructure/Eventing/EventBusEdgeCasesTests.cs` (NOVO)

**Total Phase 8**: ~30-40 testes

---

## 🎯 Phase 9: API Layer - Endpoints e Autenticação

**Objetivo**: ~80% → 90%+  
**Estimativa**: 40-50 testes  
**Prioridade**: 🟡 Média

### Componentes Identificados para Cobertura

#### 1. Controller Integration Tests (20-25 testes)
- ❌ Testes de integração E2E para endpoints críticos
- ❌ Validação de autorização
- ❌ Rate limiting
- ❌ Error responses
- ❌ Status codes corretos

**Arquivo**: `backend/Araponga.Tests/Api/ControllerIntegrationEdgeCasesTests.cs` (NOVO)

#### 2. Authentication & Authorization (10-15 testes)
- ❌ JWT token inválido/expirado
- ❌ Permissões insuficientes
- ❌ Rate limiting
- ❌ Error handling

**Arquivo**: `backend/Araponga.Tests/Api/AuthEdgeCasesTests.cs` (NOVO)

#### 3. Request Validation (10-12 testes)
- ✅ Já tem `ControllerValidationEdgeCasesTests`
- ❌ **Faltam**:
  - Validação de headers
  - Validação de query parameters
  - Validação de route parameters

**Arquivo**: `backend/Araponga.Tests/Api/RequestValidationEdgeCasesTests.cs` (NOVO)

**Total Phase 9**: ~40-50 testes

---

## 📋 Resumo do Plano

| Phase | Camada | Testes | Prioridade | Estimativa |
|-------|--------|--------|------------|------------|
| Phase 6 | Domain | 20-30 | 🟡 Média | 1-2 semanas |
| Phase 7 | Application | 50-70 | 🔴 Alta | 2-3 semanas |
| Phase 8 | Infrastructure | 30-40 | 🔴 Alta | 1-2 semanas |
| Phase 9 | API | 40-50 | 🟡 Média | 2-3 semanas |
| **Total** | **Todas** | **140-190** | - | **6-10 semanas** |

---

## 🎯 Priorização Recomendada

### 🔴 Alta Prioridade (Fases 7 e 8)
**Razão**: Maior gap (15%) e impacto direto na qualidade do código

1. **Phase 7: Application Layer** (50-70 testes)
   - Serviços críticos (Media, Events, Chat, Financial)
   - Error handling abrangente
   - **Impacto**: +15% cobertura

2. **Phase 8: Infrastructure Layer** (30-40 testes)
   - Repositórios Postgres
   - File storage
   - **Impacto**: +15% cobertura

### 🟡 Média Prioridade (Fases 6 e 9)
**Razão**: Gap menor (5-10%) mas importante para completude

3. **Phase 6: Domain Layer** (20-30 testes)
   - Entidades restantes
   - **Impacto**: +5% cobertura

4. **Phase 9: API Layer** (40-50 testes)
   - Integração E2E
   - Autenticação
   - **Impacto**: +10% cobertura

---

## 📊 Métricas de Sucesso

### Por Fase
- ✅ **Phase 6**: Domain Layer ≥ 90%
- ✅ **Phase 7**: Application Layer ≥ 90%
- ✅ **Phase 8**: Infrastructure Layer ≥ 90%
- ✅ **Phase 9**: API Layer ≥ 90%

### Geral
- ✅ **Média Geral**: ≥ 90%
- ✅ **Taxa de Sucesso**: 100%
- ✅ **Zero Regressions**: Mantido

---

## 🚀 Estratégia de Implementação

### Abordagem Incremental
1. **Fase por Fase**: Implementar uma fase por vez
2. **Testes Passando**: Garantir 100% de sucesso antes de avançar
3. **Code Review**: Revisar cada fase antes de merge
4. **Documentação**: Documentar cada fase conforme implementada

### Padrões Estabelecidos
- ✅ Padrão AAA (Arrange-Act-Assert)
- ✅ Nomenclatura: `MethodName_Scenario_ExpectedBehavior`
- ✅ Comentários descritivos
- ✅ Testes isolados e independentes

---

## 📝 Checklist de Implementação

### Para Cada Fase
- [ ] Identificar entidades/serviços faltantes
- [ ] Criar arquivo de testes
- [ ] Implementar edge cases
- [ ] Garantir 100% de sucesso
- [ ] Documentar a fase
- [ ] Code review
- [ ] Merge

### Geral
- [ ] Todas as fases completas
- [ ] 90%+ em todas as camadas
- [ ] Documentação atualizada
- [ ] Métricas validadas

---

## 💡 Dicas de Implementação

### 1. Focar em Edge Cases Críticos
- Unicode e internacionalização
- Boundary conditions
- Null safety
- Error handling

### 2. Reutilizar Padrões
- Usar os padrões estabelecidos nas fases 1-5
- Seguir estrutura de arquivos existente
- Manter consistência de nomenclatura

### 3. Priorizar Impacto
- Começar pelos serviços mais críticos
- Focar em áreas com maior gap
- Validar com métricas reais

---

## 📈 Progresso Esperado

### Timeline Estimada

| Semana | Fase | Testes | Cobertura Esperada |
|--------|------|--------|-------------------|
| 1-2 | Phase 6 | 20-30 | Domain: 85% → 90% |
| 3-5 | Phase 7 | 50-70 | Application: 75% → 90% |
| 6-7 | Phase 8 | 30-40 | Infrastructure: 75% → 90% |
| 8-10 | Phase 9 | 40-50 | API: 80% → 90% |

**Total**: 6-10 semanas para 90%+ em todas as camadas

---

## ✅ Critérios de Aceitação

### Para Cada Fase
- ✅ Todos os testes passando (100%)
- ✅ Zero regressions
- ✅ Build succeeds
- ✅ Documentação atualizada
- ✅ Code review aprovado

### Final
- ✅ 90%+ cobertura em todas as camadas
- ✅ Média geral ≥ 90%
- ✅ 100% taxa de sucesso
- ✅ Documentação completa

---

**Status**: 📋 Planejado  
**Próxima Fase**: Phase 6 (Domain Layer - Entidades Restantes)  
**Estimativa Total**: 140-190 testes adicionais em 6-10 semanas
