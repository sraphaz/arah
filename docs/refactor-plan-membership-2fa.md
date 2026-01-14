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

**Ajustar testes existentes**:
- `ApplicationServiceTests.MembershipService_*`: Atualizar para `ResidencyVerification`
- `ApplicationServiceTests.MarketplaceServiceTests`: Atualizar regras de acesso
- `ApiScenariosTests`: Atualizar fluxos de membership

**Novos testes**:
- Regra "1 Resident por User" (tentativa de criar segundo Resident falha)
- Transferência de residência
- Múltiplos Visitors (User pode ter vários)
- Verificação geo/documental
- 2FA: Setup, login, recovery codes
- Visualização multi-território no mapa
- Filtros de conteúdo por múltiplos territórios

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
1. Atualizar testes existentes
2. Novos testes para `ResidencyVerification`

#### 6.2 Testes de Application
1. Atualizar testes de `MembershipService`
2. Teste: Regra "1 Resident por User"
3. Teste: Transferência de residência
4. Teste: Múltiplos Visitors
5. Teste: Verificação geo/documental
6. Teste: 2FA (setup, login, recovery)
7. Teste: `MembershipAccessRules`

#### 6.3 Testes de API
1. Atualizar testes de endpoints existentes
2. Testes de novos endpoints
3. Teste: Visualização multi-território

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
- Login com 2FA muda formato de resposta

**Contratos**:
- `MembershipResponse`: Mudança de estrutura
- Versão da API: Considerar versionamento (v1 vs v2)

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

**Status**: Aguardando aprovação para iniciar execução
