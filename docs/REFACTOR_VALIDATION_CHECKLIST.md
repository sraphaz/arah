# Checklist de Validação - Refatoração User-Centric

Este documento lista todos os pontos que devem ser validados antes e durante a implementação da refatoração.

---

## ✅ Validação Conceitual

### 1. Separação de Responsabilidades

- [ ] **User** contém apenas:
  - Identidade pessoal (displayName, email, cpf, etc.)
  - Autenticação (2FA, provider, externalId)
  - Verificação de identidade global (`UserIdentityVerificationStatus`)
  - Papel técnico global (Admin, se existir)

- [ ] **TerritoryMembership** contém apenas:
  - Vínculo User ↔ Territory
  - Papel territorial (Visitor/Resident)
  - Verificação de residência (Unverified/GeoVerified/DocumentVerified)

- [ ] **MembershipSettings** contém apenas:
  - Configurações e opt-ins do membro
  - MarketplaceOptIn
  - Futuras configurações

- [ ] **MembershipCapability** contém apenas:
  - Capacidades operacionais (Curator, Moderator)
  - Metadados de concessão/revogação

### 2. Verificações

- [ ] `UserIdentityVerificationStatus` é global e pertence ao User
- [ ] `ResidencyVerificationStatus` é territorial e pertence ao Membership
- [ ] Não há mistura entre verificação global e territorial

### 3. Regras de Negócio

- [ ] Regra de 1 Resident por User está clara e documentada
- [ ] Validação de 1 Resident por User será feita na camada de aplicação
- [ ] Marketplace usa regras compostas (não é papel)
- [ ] Capacidades são territoriais (não globais)
- [ ] Capacidades são empilháveis

---

## ✅ Validação de Modelo de Dados

### 1. Entidades de Domínio

- [ ] `User` tem `IdentityVerificationStatus` e `IdentityVerifiedAtUtc`
- [ ] `TerritoryMembership` mantém `ResidencyVerification` (renomear internamente para `ResidencyVerificationStatus`)
- [ ] `MembershipSettings` existe e tem relacionamento 1:1 com Membership
- [ ] `MembershipCapability` existe e tem relacionamento N:1 com Membership
- [ ] `MembershipCapabilityType` enum existe (Curator, Moderator)

### 2. Enums

- [ ] `UserIdentityVerificationStatus`: Unverified, Pending, Verified, Rejected
- [ ] `ResidencyVerification`: Unverified, GeoVerified, DocumentVerified (mantém nome atual)
- [ ] `MembershipCapabilityType`: Curator, Moderator
- [ ] `FeatureFlag`: inclui MarketplaceEnabled

### 3. Relacionamentos

- [ ] User 1:N TerritoryMembership
- [ ] TerritoryMembership 1:1 MembershipSettings
- [ ] TerritoryMembership 1:N MembershipCapability
- [ ] Territory 1:N FeatureFlag

---

## ✅ Validação de Infraestrutura

### 1. Banco de Dados

- [ ] Tabela `users` tem colunas:
  - `identity_verification_status` (integer)
  - `identity_verified_at_utc` (timestamp nullable)

- [ ] Tabela `membership_settings` existe com:
  - `membership_id` (PK, FK para territory_memberships, unique)
  - `marketplace_opt_in` (boolean)
  - `created_at_utc` (timestamp)
  - `updated_at_utc` (timestamp)

- [ ] Tabela `membership_capabilities` existe com:
  - `id` (PK)
  - `membership_id` (FK para territory_memberships)
  - `capability_type` (integer)
  - `granted_at_utc` (timestamp)
  - `revoked_at_utc` (timestamp nullable)
  - `granted_by_user_id` (uuid nullable)
  - `granted_by_membership_id` (uuid nullable)
  - `reason` (text nullable)

- [ ] Tabela `feature_flags` tem registro para `MarketplaceEnabled`

- [ ] Índices apropriados:
  - `membership_settings.membership_id` (unique)
  - `membership_capabilities.membership_id`
  - `membership_capabilities.capability_type`

### 2. Repositórios

- [ ] `IMembershipSettingsRepository` existe
- [ ] `IMembershipCapabilityRepository` existe
- [ ] Implementações Postgres existem
- [ ] Mappers atualizados

---

## ✅ Validação de Aplicação

### 1. Services

- [ ] `MembershipService` cria `MembershipSettings` automaticamente
- [ ] `MembershipAccessRules` usa novo modelo para marketplace
- [ ] `AccessEvaluator` usa `MembershipCapability` ao invés de `UserRole.Curator`
- [ ] `StoreService` usa novas regras de marketplace

### 2. Regras de Marketplace

- [ ] Criar Store/Item verifica:
  - Territory.FeatureFlags.MarketplaceEnabled
  - MembershipSettings.MarketplaceOptIn
  - Membership.Role == Resident
  - Membership.ResidencyVerificationStatus != Unverified

- [ ] Operar plenamente verifica tudo acima +:
  - User.IdentityVerificationStatus == Verified

### 3. Autorização

- [ ] `AccessEvaluator.IsCurator()` substituído por `HasCapability()`
- [ ] Todas as verificações de Curator atualizadas
- [ ] Verificações baseadas em Membership ativo

---

## ✅ Validação de API

### 1. Endpoints

- [ ] `PUT /api/v1/memberships/{id}/settings` existe
- [ ] `POST /api/v1/memberships/{id}/capabilities` existe
- [ ] `DELETE /api/v1/memberships/{id}/capabilities/{capabilityId}` existe
- [ ] Endpoints de marketplace retornam HTTP 403 explícito quando negado

### 2. Contratos

- [ ] `MembershipSettingsResponse` existe
- [ ] `MembershipCapabilityResponse` existe
- [ ] Contratos de marketplace atualizados

---

## ✅ Validação de Testes

### 1. Testes Unitários

- [ ] Testes para `User.UpdateIdentityVerification()`
- [ ] Testes para `MembershipSettings`
- [ ] Testes para `MembershipCapability`
- [ ] Testes para regras de marketplace

### 2. Testes de Integração

- [ ] Teste: criar Membership cria MembershipSettings automaticamente
- [ ] Teste: regra de 1 Resident por User
- [ ] Teste: regras compostas de marketplace
- [ ] Teste: capacidades territoriais

### 3. Testes de API

- [ ] Teste: atualizar MembershipSettings
- [ ] Teste: conceder/remover capabilities
- [ ] Teste: marketplace com todas as regras

---

## ✅ Validação de Migração

### 1. Dados Existentes

- [ ] Migração de dados existentes:
  - Users sem `IdentityVerificationStatus` → `Unverified`
  - Criar `MembershipSettings` para Memberships existentes
  - Migrar `UserRole.Curator` para `MembershipCapability`

### 2. Compatibilidade

- [ ] Código antigo continua funcionando durante transição
- [ ] Rollback possível se necessário

---

## ✅ Validação Final

### 1. Build e Testes

- [ ] Build passa sem erros
- [ ] Todos os testes passam
- [ ] Não há warnings críticos

### 2. Documentação

- [ ] Documentação atualizada
- [ ] Comentários no código explicam o modelo
- [ ] ADRs atualizados se necessário

### 3. Revisão

- [ ] Código revisado
- [ ] Modelo validado com stakeholders
- [ ] Pronto para merge

---

## 📝 Notas

- Este checklist deve ser preenchido durante a implementação
- Marque cada item conforme for validado
- Documente qualquer desvio ou decisão alternativa
