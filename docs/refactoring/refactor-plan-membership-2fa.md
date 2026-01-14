# Plano de Refatoração: Membership e Autenticação (2FA)

**Data**: 2026-01-13  
**Status**: 📋 Em Planejamento

---

## 📋 Objetivo

Refatorar o modelo de Membership para eliminar ambiguidades entre papel territorial e nível de verificação, e implementar Autenticação de Dois Fatores (2FA) corretamente posicionada no modelo de identidade.

**Mudanças principais**:
- **Separar Role de ResidencyVerification** (eliminar ambiguidade)
- **Regra estrutural**: 1 Resident por User (máximo)
- **Múltiplos Visitors**: User pode ter múltiplos Memberships como Visitor
- **2FA no User/Auth**: Isolado na identidade, sem interferir em permissões
- **Visualização Multi-Território**: User pode visualizar informações de múltiplos territórios no mapa

---

## 🎯 Regras de Negócio Confirmadas

### 1. Membership - Dimensões Separadas

**Papel no território (MembershipRole)**:
- `Visitor`: Usuário com vínculo básico no território
- `Resident`: Usuário morador do território (máximo 1 por User em todo o sistema)

**Verificação de residência (ResidencyVerification)**:
- `Unverified`: Sem verificação
- `GeoVerified`: Verificado por geolocalização
- `DocumentVerified`: Verificado por comprovante documental

**Habilitação econômica (Marketplace)**:
- `MarketplaceIdentityVerifiedAtUtc?`: Timestamp de verificação de identidade para operações econômicas
- Armazenado no User, não no Membership

### 2. Regra de Cardinalidade (1 Resident por User)

**Restrição estrutural**:
- Um User pode ter múltiplos Memberships como Visitor (em territórios diferentes)
- Um User pode ter no máximo 1 Membership como Resident em todo o sistema
- Garantia: Índice único parcial no banco (Role = Resident)
- Validação: No serviço de aplicação antes de promover a Resident

### 3. Visualização Multi-Território no Mapa

**Comportamento atual**:
- User seleciona um território ativo por sessão
- Visualização de conteúdo filtrado pelo papel (Visitor/Resident) no território ativo

**Comportamento novo**:
- User pode selecionar múltiplos territórios para visualização no mapa
- Para cada território, o conteúdo é filtrado pelo MembershipRole do usuário naquele território específico
- Visitor em território A vê apenas conteúdo público de A
- Resident em território B vê todo conteúdo de B
- Mapa pode mostrar pins de múltiplos territórios simultaneamente, cada um com seu nível de acesso

### 4. Autenticação de Dois Fatores (2FA)

**Posicionamento**:
- 2FA pertence exclusivamente ao modelo de identidade do User (Auth)
- Não participa de Membership, Território, Papéis ou Permissões funcionais
- É avaliado apenas durante o login
- Após autenticação, o sistema trata o usuário apenas como "autenticado"

**Modelo mínimo no User**:
- `TwoFactorEnabled` (bool)
- `TwoFactorSecret` (string?) - criptografado
- `TwoFactorRecoveryCodesHash` (string?) - hash dos recovery codes
- `TwoFactorVerifiedAtUtc?` (timestamp)

---

## 📊 Análise de Impacto

### Estado Atual → Novo Modelo

#### VerificationStatus → ResidencyVerification

**Mapeamento de dados existentes**:
- `VerificationStatus.Pending` + `Role=Resident` → `ResidencyVerification.Unverified`
- `VerificationStatus.Validated` + `Role=Resident` → `ResidencyVerification.GeoVerified` (assumir geo como padrão)
- `VerificationStatus.Rejected` + `Role=Resident` → `ResidencyVerification.Unverified`
- `VerificationStatus.*` + `Role=Visitor` → `ResidencyVerification.Unverified` (visitor não precisa verificação)

**Estratégia de migração**:
1. Adicionar novas colunas (ResidencyVerification, timestamps)
2. Migrar dados existentes conforme mapeamento acima
3. Manter coluna antiga temporariamente (deprecated)
4. Remover coluna antiga após validação completa

#### MembershipRole (manter, mas com nova regra)

**Mudanças**:
- Enum mantém: `Visitor`, `Resident`
- Nova regra: Validação de exclusividade de Resident
- Novo método no repositório: `HasResidentMembershipAsync(userId)`

#### TerritoryMembership (adicionar campos)

**Novos campos**:
- `ResidencyVerification` (enum) - substitui `VerificationStatus`
- `LastGeoVerifiedAtUtc?` (DateTime?)
- `LastDocumentVerifiedAtUtc?` (DateTime?)
- Manter `Role` (MembershipRole)
- Manter `CreatedAtUtc`

**Campos a remover**:
- `VerificationStatus` (deprecated, remover após migração)

#### User (adicionar 2FA)

**Novos campos**:
- `TwoFactorEnabled` (bool)
- `TwoFactorSecret` (string?) - criptografado
- `TwoFactorRecoveryCodesHash` (string?) - hash dos recovery codes
- `TwoFactorVerifiedAtUtc?` (DateTime?)

---

## 🔧 Componentes Impactados

### Domain

**Novos**:
- `ResidencyVerification` (enum)
- Campos em `TerritoryMembership`
- Campos em `User` (2FA)

**Modificados**:
- `TerritoryMembership`: Adicionar campos, remover `VerificationStatus`
- `User`: Adicionar campos 2FA

**Obsoletos**:
- `VerificationStatus` (enum) - remover após migração

### Application

**Novos**:
- `MembershipAccessRules` (helper para centralizar regras)
- Métodos 2FA em `AuthService`

**Modificados**:
- `MembershipService`: 
  - Validação de exclusividade de Resident
  - Atualizar para usar `ResidencyVerification`
  - Métodos de verificação de residência (geo/document)
  - Transferência de residência
- `AccessEvaluator`: 
  - Atualizar para usar `ResidencyVerification`
  - Centralizar regras em `MembershipAccessRules`
- `AuthService`: 
  - Implementar fluxo 2FA
  - Login em duas etapas
  - Setup/confirmação 2FA
  - Recovery codes

**Impactados indiretamente**:
- `StoreService`: Usar `MembershipAccessRules`
- `StoreItemService`: Usar `MembershipAccessRules`
- `MapService`: Suportar múltiplos territórios
- `FeedService`: Filtros por múltiplos territórios

### Infrastructure

**Repositórios - Novos métodos**:
- `ITerritoryMembershipRepository.HasResidentMembershipAsync(userId)`
- `ITerritoryMembershipRepository.GetResidentMembershipAsync(userId)`
- `ITerritoryMembershipRepository.ListByUserAsync(userId)`
- `IUserRepository`: Métodos 2FA

**Migration**:
- Adicionar colunas: `ResidencyVerification`, `LastGeoVerifiedAtUtc`, `LastDocumentVerifiedAtUtc`
- Adicionar índice único parcial: `UNIQUE (UserId) WHERE Role = Resident`
- Migração de dados: `VerificationStatus` → `ResidencyVerification`
- User: Adicionar colunas 2FA
- Remover coluna `VerificationStatus` (após período de transição)

**Records (Entities)**:
- `TerritoryMembershipRecord`: Adicionar campos
- `UserRecord`: Adicionar campos 2FA

### API

**Novos endpoints**:
- `POST /api/v1/territories/{territoryId}/enter` - Entrar como Visitor
- `POST /api/v1/memberships/{territoryId}/become-resident` - Solicitar ser Resident
- `POST /api/v1/memberships/transfer-residency` - Transferir residência
- `POST /api/v1/memberships/{territoryId}/verify-residency/geo` - Verificação geo
- `POST /api/v1/memberships/{territoryId}/verify-residency/document` - Verificação documental
- `GET /api/v1/memberships/{territoryId}/me` - Consultar meu estado
- `GET /api/v1/memberships/me` - Listar meus memberships
- `POST /api/v1/auth/2fa/setup` - Setup 2FA
- `POST /api/v1/auth/2fa/confirm` - Confirmar 2FA
- `POST /api/v1/auth/login` - Login (etapa 1, pode retornar 2FA_REQUIRED)
- `POST /api/v1/auth/2fa/verify` - Verificar 2FA (etapa 2)
- `POST /api/v1/auth/2fa/recover` - Usar recovery code
- `POST /api/v1/auth/2fa/disable` - Desabilitar 2FA
- `GET /api/v1/map/pins?territoryIds=...` - Pins de múltiplos territórios

**Modificados**:
- `POST /api/v1/territories/{territoryId}/membership` - Adaptar para novo modelo
- `GET /api/v1/territories/{territoryId}/membership/me` - Retornar `ResidencyVerification`

**Contracts**:
- `MembershipResponse`: Adicionar `ResidencyVerification`, remover `VerificationStatus`
- `MembershipStatusResponse`: Adicionar `ResidencyVerification`
- Novos contracts para 2FA e verificação de residência

### Testes

#### Testes Existentes a Ajustar

**Domain Tests (`DomainValidationTests.cs`)**:
- `TerritoryMembership` construtor e validações
- Atualizar para usar `ResidencyVerification` em vez de `VerificationStatus`
- Adicionar testes para novos métodos (`UpdateResidencyVerification`, `UpdateGeoVerification`, `UpdateDocumentVerification`)
- Testes de 2FA no `User` (EnableTwoFactor, DisableTwoFactor)

**Application Tests (`ApplicationServiceTests.cs`)**:
- `MembershipService_ReturnsStatusAndValidates`: Atualizar para `ResidencyVerification`
- `MembershipService_AllowsVisitorUpgradeToResident`: Atualizar para novo modelo e validação de exclusividade
- Testes de `AccessEvaluator`: Atualizar para usar `MembershipAccessRules` e `ResidencyVerification`

**Marketplace Tests (`MarketplaceServiceTests.cs`)**:
- Testes de criação de Store: Validar uso de `MembershipAccessRules`
- Testes de criação de Item: Validar regras baseadas em `ResidencyVerification`
- Testes de publicação: Validar `MarketplaceIdentityVerifiedAtUtc` quando implementado

**API Tests (`ApiScenariosTests.cs`)**:
- `Memberships_RequireAuthAndTerritory`: Atualizar para novos endpoints
- `Memberships_CreatePendingAndReuse`: Atualizar para `ResidencyVerification`
- Testes de endpoints de membership: Adaptar para novos contratos

**Repository Tests (`RepositoryTests.cs`)**:
- Testes de `ITerritoryMembershipRepository`: Adicionar testes para novos métodos
  - `HasResidentMembershipAsync`
  - `GetResidentMembershipAsync`
  - `ListByUserAsync`
  - `UpdateResidencyVerificationAsync`
  - `UpdateGeoVerificationAsync`
  - `UpdateDocumentVerificationAsync`

#### Novos Testes de Domínio

**TerritoryMembership**:
- ✅ `ResidencyVerification_Initialized_AsUnverified` - Verificar inicialização padrão
- ✅ `UpdateResidencyVerification_ChangesState` - Verificar atualização de verificação
- ✅ `UpdateGeoVerification_SetsTimestamp` - Verificar timestamp de verificação geo
- ✅ `UpdateDocumentVerification_SetsTimestamp` - Verificar timestamp de verificação documental
- ✅ `ResidencyVerification_DocumentVerified_HasHighestPriority` - Verificar que DocumentVerified sobrescreve GeoVerified
- ✅ `ConvertVerificationStatus_ToResidencyVerification` - Testar conversão de dados legados

**User (2FA)**:
- ✅ `EnableTwoFactor_SetsProperties` - Verificar habitação de 2FA
- ✅ `DisableTwoFactor_ClearsSecrets` - Verificar desabilitação e limpeza
- ✅ `TwoFactorSecret_IsRequired_WhenEnabled` - Validar que secret é obrigatório
- ✅ `RecoveryCodes_AreHashed` - Validar que recovery codes são hasheados

#### Novos Testes de Application

**MembershipService**:
- ✅ `EnterAsVisitorAsync_CreatesNewMembership` - Criar membership como Visitor
- ✅ `EnterAsVisitorAsync_ReturnsExisting_IfAlreadyVisitor` - Retornar membership existente
- ✅ `BecomeResidentAsync_Succeeds_WhenNoExistingResident` - Permitir tornar-se Resident quando não há outro
- ✅ `BecomeResidentAsync_Fails_WhenHasResidentInAnotherTerritory` - Validar regra "1 Resident por User"
- ✅ `BecomeResidentAsync_Returns409Conflict_OnConflict` - Retornar erro apropriado (409)
- ✅ `BecomeResidentAsync_AutoVerifies_FirstResident` - Primeiro residente é auto-verificado
- ✅ `BecomeResidentAsync_SetsUnverified_WhenOtherResidentsExist` - Novos residents são Unverified
- ✅ `TransferResidencyAsync_DemotesCurrentResident` - Demover Resident atual
- ✅ `TransferResidencyAsync_PromotesNewTerritory` - Promover novo território
- ✅ `TransferResidencyAsync_RollbackOnFailure` - Reverter mudanças em caso de falha
- ✅ `VerifyResidencyByGeoAsync_UpdatesVerification` - Atualizar verificação geo
- ✅ `VerifyResidencyByGeoAsync_Fails_IfNotResident` - Falhar se não for Resident
- ✅ `VerifyResidencyByDocumentAsync_UpdatesVerification` - Atualizar verificação documental
- ✅ `VerifyResidencyByDocumentAsync_Fails_IfNotResident` - Falhar se não for Resident
- ✅ `ListMyMembershipsAsync_ReturnsAllMemberships` - Listar todos os memberships do usuário
- ✅ `ListMyMembershipsAsync_IncludesMultipleVisitors` - Suportar múltiplos Visitors

**MembershipAccessRules**:
- ✅ `CanCreateStoreAsync_RequiresResidentAndVerified` - Validar regra de criação de Store
- ✅ `CanCreateStoreAsync_Fails_ForVisitor` - Visitor não pode criar Store
- ✅ `CanCreateStoreAsync_Fails_ForUnverifiedResident` - Resident não verificado não pode criar Store
- ✅ `CanCreateItemAsync_SameAsStoreRule` - Regra igual a criação de Store
- ✅ `CanPublishItemAsync_RequiresMarketplaceVerification` - Validar regra de publicação (quando implementado)
- ✅ `IsVerifiedResidentAsync_ChecksRoleAndVerification` - Verificar se é Resident validado

**AuthService (2FA)**:
- ✅ `Setup2FAAsync_GeneratesSecretAndQR` - Gerar secret e QR code
- ✅ `Setup2FAAsync_Fails_IfAlreadyEnabled` - Falhar se já habilitado
- ✅ `Confirm2FAAsync_ValidatesCode` - Validar código TOTP
- ✅ `Confirm2FAAsync_Fails_OnInvalidCode` - Falhar com código inválido
- ✅ `Confirm2FAAsync_Enables2FA_OnSuccess` - Habilitar 2FA após confirmação
- ✅ `Confirm2FAAsync_GeneratesRecoveryCodes` - Gerar recovery codes
- ✅ `LoginAsync_Returns2FARequired_WhenEnabled` - Retornar 2FA_REQUIRED quando habilitado
- ✅ `LoginAsync_ReturnsJWT_When2FADisabled` - Retornar JWT quando 2FA desabilitado
- ✅ `Verify2FAAsync_ReturnsJWT_OnValidCode` - Retornar JWT após verificação 2FA
- ✅ `Verify2FAAsync_Fails_OnInvalidCode` - Falhar com código inválido
- ✅ `Recover2FAAsync_ReturnsJWT_OnValidRecoveryCode` - Usar recovery code
- ✅ `Recover2FAAsync_InvalidatesUsedCode` - Invalidar código usado
- ✅ `Recover2FAAsync_Fails_OnInvalidCode` - Falhar com recovery code inválido
- ✅ `Disable2FAAsync_RequiresPasswordOr2FA` - Exigir senha ou 2FA para desabilitar
- ✅ `Disable2FAAsync_ClearsSecrets` - Limpar secrets ao desabilitar

#### Novos Testes de API

**Membership Endpoints**:
- ✅ `POST /territories/{id}/enter` - Entrar como Visitor
  - Retorna 200 com membership
  - Cria membership se não existir
  - Retorna membership existente se já for Visitor
- ✅ `POST /memberships/{territoryId}/become-resident` - Tornar-se Resident
  - Retorna 200 quando bem-sucedido
  - Retorna 409 quando já tem Resident em outro território
  - Retorna 404 se território não existe
- ✅ `POST /memberships/transfer-residency` - Transferir residência
  - Retorna 200 quando bem-sucedido
  - Retorna 400 se não tem Resident atual
  - Retorna 404 se território destino não existe
  - Retorna 409 se violar regras
- ✅ `POST /memberships/{territoryId}/verify-residency/geo` - Verificar geo
  - Retorna 200 quando bem-sucedido
  - Retorna 400 se não for Resident
  - Retorna 404 se membership não existe
- ✅ `POST /memberships/{territoryId}/verify-residency/document` - Verificar documental
  - Retorna 200 quando bem-sucedido
  - Retorna 400 se não for Resident
  - Retorna 404 se membership não existe
- ✅ `GET /memberships/{territoryId}/me` - Consultar estado
  - Retorna 200 com detalhes do membership
  - Retorna 404 se não tem membership
  - Retorna ResidencyVerification no response
- ✅ `GET /memberships/me` - Listar meus memberships
  - Retorna 200 com lista de memberships
  - Suporta múltiplos Visitors

**2FA Endpoints**:
- ✅ `POST /auth/2fa/setup` - Setup 2FA
  - Retorna QR code e secret
  - Retorna 400 se já habilitado
- ✅ `POST /auth/2fa/confirm` - Confirmar 2FA
  - Retorna recovery codes quando bem-sucedido
  - Retorna 400 com código inválido
- ✅ `POST /auth/login` - Login (etapa 1)
  - Retorna JWT quando 2FA desabilitado
  - Retorna 2FA_REQUIRED quando 2FA habilitado
- ✅ `POST /auth/2fa/verify` - Verificar 2FA (etapa 2)
  - Retorna JWT quando código válido
  - Retorna 400 com código inválido
- ✅ `POST /auth/2fa/recover` - Recovery code
  - Retorna JWT quando código válido
  - Retorna 400 com código inválido
  - Invalida código usado
- ✅ `POST /auth/2fa/disable` - Desabilitar 2FA
  - Retorna 200 quando bem-sucedido
  - Exige senha ou código 2FA

**Multi-Território**:
- ✅ `GET /map/pins?territoryIds=...` - Pins múltiplos territórios
  - Retorna pins de múltiplos territórios
  - Filtra conteúdo por role em cada território
  - Visitor vê apenas conteúdo público
  - Resident vê todo conteúdo

#### Novos Testes de Integração

**Fluxos Completos**:
- ✅ `UserCanHaveMultipleVisitors_ButOnlyOneResident` - Validar regra estrutural completa
- ✅ `TransferResidency_CompleteFlow` - Fluxo completo de transferência
- ✅ `ResidencyVerification_CompleteFlow` - Fluxo completo de verificação
- ✅ `2FA_CompleteLoginFlow` - Fluxo completo de login com 2FA
- ✅ `StoreCreation_RespectsResidencyVerification` - Criar Store respeitando verificação
- ✅ `ItemCreation_RespectsResidencyVerification` - Criar Item respeitando verificação
- ✅ `MapVisualization_MultipleTerritories` - Visualização no mapa de múltiplos territórios

---

## 📝 Plano de Execução Detalhado

### Fase 1: Planejamento e Preparação
- [x] Criar plano de refatoração
- [ ] Revisar e validar mapeamento de dados
- [ ] Criar branch de refatoração
- [ ] Documentar estratégia de rollback

### Fase 2: Domain - Novo Modelo

#### 2.1 Criar ResidencyVerification
1. Criar enum `ResidencyVerification` (Unverified, GeoVerified, DocumentVerified)
2. Documentar enum

#### 2.2 Atualizar TerritoryMembership
1. Adicionar propriedade `ResidencyVerification`
2. Adicionar `LastGeoVerifiedAtUtc?`
3. Adicionar `LastDocumentVerifiedAtUtc?`
4. Adicionar métodos `UpdateResidencyVerification*`
5. Marcar `VerificationStatus` como obsoleto (mantém temporariamente)
6. Atualizar construtor
7. Atualizar testes de domínio

#### 2.3 Adicionar 2FA ao User
1. Adicionar propriedades 2FA
2. Adicionar métodos de gerenciamento 2FA
3. Atualizar testes de domínio

### Fase 3: Application - Lógica de Negócio

#### 3.1 Criar MembershipAccessRules
1. Criar helper `MembershipAccessRules`
2. Centralizar regras:
   - `CanCreateStore(userId, territoryId)`
   - `CanCreateItem(userId, territoryId)`
   - `CanPublishItem(userId, territoryId)`
3. Atualizar `AccessEvaluator` para usar helper

#### 3.2 Atualizar MembershipService
1. Adicionar validação de exclusividade de Resident
2. Adicionar método `HasResidentMembershipAsync`
3. Atualizar `DeclareMembershipAsync` para usar `ResidencyVerification`
4. Adicionar método `BecomeResidentAsync` (com validação)
5. Adicionar método `TransferResidencyAsync`
6. Adicionar métodos de verificação (geo/document)
7. Atualizar testes

#### 3.3 Implementar 2FA no AuthService
1. Adicionar método `Setup2FAAsync`
2. Adicionar método `Confirm2FAAsync`
3. Atualizar `LoginSocialAsync` para suportar 2FA
4. Adicionar método `Verify2FAAsync`
5. Adicionar método `Recover2FAAsync`
6. Adicionar método `Disable2FAAsync`
7. Criar testes

### Fase 4: Infrastructure - Repositórios e Migration

#### 4.1 Atualizar Interfaces
1. `ITerritoryMembershipRepository`: Adicionar novos métodos
2. `IUserRepository`: Adicionar métodos 2FA

#### 4.2 Atualizar Repositórios (Postgres)
1. Implementar novos métodos
2. Atualizar mappers
3. Atualizar `TerritoryMembershipRecord`

#### 4.3 Atualizar Repositórios (InMemory)
1. Implementar novos métodos
2. Atualizar testes

#### 4.4 Migration
1. Criar migration: Adicionar colunas `ResidencyVerification`, timestamps
2. Criar migration: Índice único parcial (Resident)
3. Criar migration: Migração de dados (`VerificationStatus` → `ResidencyVerification`)
4. Criar migration: User 2FA (adicionar colunas)
5. Criar migration: Remover `VerificationStatus` (após período de transição)
6. Testar migrations em ambiente de desenvolvimento

### Fase 5: API - Endpoints

#### 5.1 Novos Endpoints de Membership
1. `POST /api/v1/territories/{territoryId}/enter`
2. `POST /api/v1/memberships/{territoryId}/become-resident`
3. `POST /api/v1/memberships/transfer-residency`
4. `POST /api/v1/memberships/{territoryId}/verify-residency/geo`
5. `POST /api/v1/memberships/{territoryId}/verify-residency/document`
6. `GET /api/v1/memberships/{territoryId}/me`
7. `GET /api/v1/memberships/me`

#### 5.2 Novos Endpoints de 2FA
1. `POST /api/v1/auth/2fa/setup`
2. `POST /api/v1/auth/2fa/confirm`
3. `POST /api/v1/auth/2fa/verify`
4. `POST /api/v1/auth/2fa/recover`
5. `POST /api/v1/auth/2fa/disable`
6. Atualizar `POST /api/v1/auth/social` (login)

#### 5.3 Atualizar Endpoints Existentes
1. `POST /api/v1/territories/{territoryId}/membership`: Adaptar
2. `GET /api/v1/territories/{territoryId}/membership/me`: Atualizar response

#### 5.4 Visualização Multi-Território
1. `GET /api/v1/map/pins?territoryIds=...`: Suportar múltiplos territórios
2. Atualizar `MapService` para múltiplos territórios
3. Filtros de conteúdo por território e role

#### 5.5 Contracts
1. Atualizar `MembershipResponse`
2. Atualizar `MembershipStatusResponse`
3. Criar novos contracts (2FA, verificação)
4. Atualizar OpenAPI/Swagger

### Fase 6: Testes

#### 6.1 Testes de Domínio
1. Atualizar `DomainValidationTests.cs`:
   - `TerritoryMembership`: Atualizar para `ResidencyVerification`
   - Adicionar testes para novos métodos (`UpdateResidencyVerification*`)
   - Testes de conversão de `VerificationStatus` → `ResidencyVerification`
2. Novos testes para `User` (2FA):
   - `EnableTwoFactor`, `DisableTwoFactor`
   - Validações de propriedades 2FA

#### 6.2 Testes de Application
1. Atualizar `ApplicationServiceTests.cs`:
   - `MembershipService_ReturnsStatusAndValidates`: Adaptar para `ResidencyVerification`
   - `MembershipService_AllowsVisitorUpgradeToResident`: Atualizar com validação de exclusividade
2. Novos testes em `MembershipServiceTests.cs` (criar arquivo):
   - Regra "1 Resident por User" (8 testes)
   - Transferência de residência (3 testes)
   - Múltiplos Visitors (2 testes)
   - Verificação geo/documental (4 testes)
3. Testes de `MembershipAccessRules`:
   - Regras de criação de Store/Item
   - Validação de publicação (quando implementado)
4. Testes de `AuthService` (2FA):
   - Setup e confirmação (4 testes)
   - Login em duas etapas (3 testes)
   - Recovery codes (3 testes)
   - Desabilitação (2 testes)
5. Atualizar `AccessEvaluator`:
   - Usar `MembershipAccessRules`
   - Validar `ResidencyVerification`

#### 6.3 Testes de API
1. Atualizar `ApiScenariosTests.cs`:
   - Endpoints existentes de membership
   - Adaptar para novos contratos
2. Novos testes de Membership Endpoints (7 endpoints):
   - Entrar como Visitor
   - Tornar-se Resident
   - Transferir residência
   - Verificações (geo/document)
   - Consultar/listar memberships
3. Novos testes de 2FA Endpoints (6 endpoints):
   - Setup, confirmação, login, verificação, recovery, disable
4. Testes de Multi-Território:
   - Visualização no mapa com múltiplos territórios
   - Filtros por role em cada território

#### 6.4 Testes de Integração
1. Testes de fluxos completos:
   - Regra estrutural completa
   - Transferência de residência
   - Verificação completa
   - Login com 2FA completo
   - Marketplace respeitando verificação
   - Visualização multi-território

### Fase 7: Validação e Cleanup

#### 7.1 Validação
1. Executar todos os testes
2. Validar migrations
3. Validar API (Swagger)
4. Teste manual de fluxos principais

#### 7.2 Cleanup
1. Remover código obsoleto (`VerificationStatus`)
2. Atualizar documentação
3. Atualizar comentários XML

---

## ⚠️ Considerações Importantes

### 1. Migração de Dados

**Estratégia**:
- Fase 1: Adicionar novas colunas (nullable)
- Fase 2: Migrar dados existentes
- Fase 3: Tornar colunas NOT NULL
- Fase 4: Remover colunas antigas (após período de transição)

**Rollback**:
- Manter coluna `VerificationStatus` temporariamente
- Script de rollback preparado
- Validação em ambiente de staging antes de produção

### 2. Regra "1 Resident por User"

**Implementação**:
- Índice único parcial no banco: `CREATE UNIQUE INDEX ... WHERE Role = Resident`
- Validação no serviço antes de promover
- Tratamento de erro: HTTP 409 Conflict

**Casos especiais**:
- Usuários existentes com múltiplos Residents: Resolver manualmente ou escolher o mais recente
- Transferência de residência: Demover Resident atual antes de promover novo

### 3. Visualização Multi-Território

**Impacto**:
- `MapService` precisa suportar múltiplos `territoryIds`
- Filtros de conteúdo por território
- Performance: Otimizar consultas para múltiplos territórios
- Cache: Considerar cache por território

**API**:
- Query parameter: `?territoryIds=id1,id2,id3`
- Response: Agrupar pins por território ou unificar com metadata

### 4. Breaking Changes

**API**:
- Response de membership muda (adiciona `ResidencyVerification`, remove `VerificationStatus`)
- Novos endpoints (não quebram, mas podem conflitar se já existirem)
- Login com 2FA muda formato de resposta (pode retornar `2FA_REQUIRED`)

**Contratos**:
- `MembershipResponse`: Mudança de estrutura
- Versão da API: Considerar versionamento (v1 vs v2) se necessário
- OpenAPI/Swagger: Atualizar documentação de contratos

**Repositórios**:
- Novos métodos em `ITerritoryMembershipRepository` (não quebram, mas implementações precisam atualizar)
- Métodos obsoletos mantidos temporariamente para compatibilidade

### 5. Segurança (2FA)

**Armazenamento**:
- `TwoFactorSecret`: Criptografado
- `TwoFactorRecoveryCodesHash`: Hash (não armazenar plain text)
- Rotação de segredos: Considerar política

**Fluxo**:
- Setup: Gerar secret, retornar QR code
- Confirmação: Validar código antes de habilitar
- Login: Emitir JWT apenas após verificação 2FA
- Recovery: Invalidar código usado

---

## 📊 Estatísticas Estimadas

- **Arquivos a modificar**: ~80-100 arquivos
- **Classes/Enums a criar**: ~3-5
- **Classes a modificar**: ~15-20
- **Novos métodos**: ~30-40
- **Novos endpoints**: ~12-15
- **Migrations**: 5-6
- **Testes a criar/atualizar**: ~40-50

---

## 🔄 Sequência de Commits Sugerida

### Commit 1: Domain - Novo Modelo
- Criar `ResidencyVerification` enum
- Atualizar `TerritoryMembership`
- Adicionar 2FA ao `User`

### Commit 2: Application - MembershipAccessRules
- Criar `MembershipAccessRules`
- Atualizar `AccessEvaluator`

### Commit 3: Application - MembershipService (Parte 1)
- Validação de exclusividade
- Atualizar para `ResidencyVerification`

### Commit 4: Application - MembershipService (Parte 2)
- Métodos de verificação
- Transferência de residência

### Commit 5: Application - AuthService 2FA
- Implementar 2FA completo

### Commit 6: Infrastructure - Interfaces
- Atualizar interfaces de repositório

### Commit 7: Infrastructure - Repositórios
- Implementar novos métodos

### Commit 8: Infrastructure - Migration (Parte 1)
- Adicionar colunas
- Índice único parcial

### Commit 9: Infrastructure - Migration (Parte 2)
- Migração de dados
- User 2FA

### Commit 10: API - Membership Endpoints
- Novos endpoints de membership

### Commit 11: API - 2FA Endpoints
- Endpoints de 2FA

### Commit 12: API - Multi-Território
- Visualização multi-território no mapa

### Commit 13: Testes
- Atualizar e adicionar testes

### Commit 14: Cleanup
- Remover código obsoleto
- Documentação

---

---

## 📚 Impacto nas Documentações

### Documentações Técnicas

#### 1. `docs/60_API_LÓGICA_NEGÓCIO.md`
**Mudanças necessárias**:
- Adicionar seção sobre novos endpoints de Membership
- Documentar fluxos de verificação (geo/document)
- Documentar transferência de residência
- Adicionar seção sobre 2FA (setup, login, recovery)
- Documentar regra "1 Resident por User"
- Atualizar exemplos de contratos de Membership

#### 2. `docs/22_COHESION_AND_TESTS.md`
**Mudanças necessárias**:
- Atualizar exemplos de testes de Membership
- Adicionar exemplos de testes de 2FA
- Documentar novos padrões de teste para regras de acesso
- Adicionar exemplos de testes de integração para Membership

#### 3. `docs/23_IMPLEMENTATION_RECOMMENDATIONS.md`
**Mudanças necessárias**:
- Atualizar recomendações sobre Membership
- Adicionar recomendações sobre 2FA
- Documentar padrões de validação de exclusividade
- Adicionar recomendações sobre visualização multi-território

#### 4. OpenAPI/Swagger (gerado automaticamente)
**Mudanças necessárias**:
- Atualizar schemas de `MembershipResponse`
- Adicionar novos endpoints na documentação
- Adicionar exemplos de requisições/respostas
- Documentar códigos de erro (409 Conflict para exclusividade)

### Documentações de Arquitetura

#### 5. `design/Archtecture/C4_Components.md`
**Mudanças necessárias**:
- Atualizar diagrama de componentes com novos serviços
- Documentar `MembershipAccessRules` como componente
- Adicionar fluxos de 2FA
- Documentar serviços de verificação de residência

#### 6. `design/Archtecture/C4_Containers.md`
**Mudanças necessárias**:
- Atualizar containers com novos endpoints
- Documentar integração de 2FA no container de autenticação
- Atualizar fluxos de comunicação entre containers

### Documentações de Modelo de Dados

#### 7. Diagramas ER ou Documentação de Schema
**Mudanças necessárias**:
- Atualizar diagrama de `TerritoryMembership`:
  - Adicionar `ResidencyVerification` (enum)
  - Adicionar `LastGeoVerifiedAtUtc` (nullable timestamp)
  - Adicionar `LastDocumentVerifiedAtUtc` (nullable timestamp)
  - Documentar índice único parcial para Resident
- Atualizar diagrama de `User`:
  - Adicionar campos 2FA
  - Documentar relacionamentos
- Adicionar notas sobre migração de dados

### Documentações de Planejamento

#### 8. `docs/refactor-plan-membership-2fa.md` (este arquivo)
**Status**: ✅ Em atualização
- Adicionar seção de testes detalhados
- Adicionar seção de impacto em documentações
- Atualizar progresso conforme implementação

#### 9. README.md (se existir seção de arquitetura)
**Mudanças necessárias**:
- Atualizar diagramas de fluxo
- Adicionar informações sobre 2FA
- Documentar novas regras de Membership

### Documentações de Usuário (se existirem)

#### 10. Guias de Usuário/Frontend
**Mudanças necessárias**:
- Documentar novo fluxo de "Entrar como Visitor"
- Documentar processo de tornar-se Resident
- Documentar transferência de residência
- Documentar verificação de residência (geo/document)
- Adicionar guia de setup de 2FA
- Documentar uso de recovery codes

### Checklist de Atualização de Documentações

- [ ] `docs/60_API_LÓGICA_NEGÓCIO.md` - Atualizar endpoints e contratos
- [ ] `docs/22_COHESION_AND_TESTS.md` - Adicionar exemplos de testes
- [ ] `docs/23_IMPLEMENTATION_RECOMMENDATIONS.md` - Atualizar recomendações
- [ ] OpenAPI/Swagger - Atualizar schemas e endpoints
- [ ] `design/Archtecture/C4_Components.md` - Atualizar componentes
- [ ] `design/Archtecture/C4_Containers.md` - Atualizar containers
- [ ] Diagramas ER/Schema - Atualizar modelos de dados
- [ ] README.md - Atualizar arquitetura (se aplicável)
- [ ] Guias de usuário - Atualizar fluxos (se existirem)
- [ ] CHANGELOG.md - Adicionar entrada para esta refatoração

---

## 📈 Estatísticas Atualizadas

### Código
- **Arquivos a modificar**: ~80-100 arquivos
- **Classes/Enums a criar**: ~3-5
- **Classes a modificar**: ~15-20
- **Novos métodos**: ~30-40
- **Novos endpoints**: ~12-15
- **Migrations**: 5-6

### Testes
- **Testes a atualizar**: ~15-20
- **Novos testes de domínio**: ~8-10
- **Novos testes de application**: ~25-30
- **Novos testes de API**: ~15-20
- **Novos testes de integração**: ~6-8
- **Total de testes novos/atualizados**: ~60-80

### Documentações
- **Documentações técnicas a atualizar**: ~6-8
- **Documentações de arquitetura**: ~2-3
- **Diagramas a atualizar**: ~3-5
- **Guias de usuário**: ~1-2 (se existirem)

---

**Status**: Em execução - Branch `refactor/membership-2fa` criada e parcialmente implementada
