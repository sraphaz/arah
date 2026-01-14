# Plano de Migração - Refatoração User-Centric

Este documento detalha o plano de migração dos dados existentes para o novo modelo.

---

## 📊 Análise do Estado Atual

### Dados Existentes

1. **Users**
   - Sem `IdentityVerificationStatus`
   - Sem `IdentityVerifiedAtUtc`
   - Alguns com `UserRole.Curator` (29 ocorrências no código)

2. **TerritoryMemberships**
   - Com `ResidencyVerification` (já correto)
   - Sem `MembershipSettings` associado
   - Sem `MembershipCapability` associado

3. **FeatureFlags**
   - `AlertPosts` (existente)
   - `EventPosts` (existente)
   - `MarketplaceEnabled` (não existe)

---

## 🔄 Estratégia de Migração

### Fase 1: Preparação (Sem Breaking Changes)

1. **Adicionar novos campos/tabelas**
   - Adicionar colunas em `users`: `identity_verification_status`, `identity_verified_at_utc`
   - Criar tabela `membership_settings`
   - Criar tabela `membership_capabilities`
   - Adicionar `MarketplaceEnabled` ao enum `FeatureFlag`

2. **Valores padrão**
   - `users.identity_verification_status` → `Unverified` (1)
   - `users.identity_verified_at_utc` → `NULL`
   - `membership_settings.marketplace_opt_in` → `false` (padrão conservador)

3. **Migrar dados existentes**
   - Criar `MembershipSettings` para todos os `TerritoryMembership` existentes
   - Criar `MembershipCapability` (Curator) para Users com `UserRole.Curator` em todos os seus Memberships

### Fase 2: Migração de Código (Gradual)

1. **Criar novos métodos mantendo antigos**
   - `AccessEvaluator.HasCapability()` (novo)
   - Manter `AccessEvaluator.IsCurator()` temporariamente

2. **Atualizar código gradualmente**
   - Services primeiro
   - Controllers depois
   - Remover métodos antigos no final

### Fase 3: Limpeza

1. **Remover código obsoleto**
   - `UserRole.Curator` (deprecar primeiro, remover depois)
   - `AccessEvaluator.IsCurator()` (após migração completa)

---

## 📝 Scripts de Migração

### Migration 1: Adicionar Campos e Tabelas

```sql
-- Adicionar campos em users
ALTER TABLE users
ADD COLUMN identity_verification_status INTEGER NOT NULL DEFAULT 1, -- Unverified
ADD COLUMN identity_verified_at_utc TIMESTAMP WITH TIME ZONE NULL;

-- Criar tabela membership_settings
CREATE TABLE membership_settings (
    membership_id UUID PRIMARY KEY,
    marketplace_opt_in BOOLEAN NOT NULL DEFAULT false,
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    CONSTRAINT fk_membership_settings_membership
        FOREIGN KEY (membership_id)
        REFERENCES territory_memberships(id)
        ON DELETE CASCADE
);

CREATE UNIQUE INDEX idx_membership_settings_membership_id
ON membership_settings(membership_id);

-- Criar tabela membership_capabilities
CREATE TABLE membership_capabilities (
    id UUID PRIMARY KEY,
    membership_id UUID NOT NULL,
    capability_type INTEGER NOT NULL,
    granted_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    revoked_at_utc TIMESTAMP WITH TIME ZONE NULL,
    granted_by_user_id UUID NULL,
    granted_by_membership_id UUID NULL,
    reason TEXT NULL,
    CONSTRAINT fk_membership_capabilities_membership
        FOREIGN KEY (membership_id)
        REFERENCES territory_memberships(id)
        ON DELETE CASCADE
);

CREATE INDEX idx_membership_capabilities_membership_id
ON membership_capabilities(membership_id);

CREATE INDEX idx_membership_capabilities_type
ON membership_capabilities(capability_type);

-- Adicionar MarketplaceEnabled ao enum FeatureFlag (via código)
-- Não há enum SQL, é gerenciado no código
```

### Migration 2: Migrar Dados Existentes

```sql
-- Criar MembershipSettings para todos os Memberships existentes
INSERT INTO membership_settings (membership_id, marketplace_opt_in, created_at_utc, updated_at_utc)
SELECT 
    id AS membership_id,
    false AS marketplace_opt_in,
    created_at_utc AS created_at_utc,
    created_at_utc AS updated_at_utc
FROM territory_memberships
WHERE id NOT IN (SELECT membership_id FROM membership_settings);

-- Criar MembershipCapability (Curator) para Users com UserRole.Curator
-- Nota: Assumindo que Curator = 3 no enum UserRole
INSERT INTO membership_capabilities (
    id,
    membership_id,
    capability_type,
    granted_at_utc,
    revoked_at_utc,
    granted_by_user_id,
    granted_by_membership_id,
    reason
)
SELECT 
    gen_random_uuid() AS id,
    tm.id AS membership_id,
    1 AS capability_type, -- Assumindo Curator = 1 no enum MembershipCapabilityType
    tm.created_at_utc AS granted_at_utc,
    NULL AS revoked_at_utc,
    NULL AS granted_by_user_id,
    NULL AS granted_by_membership_id,
    'Migrated from UserRole.Curator' AS reason
FROM territory_memberships tm
INNER JOIN users u ON tm.user_id = u.id
WHERE u.role = 3 -- UserRole.Curator
AND NOT EXISTS (
    SELECT 1 FROM membership_capabilities mc
    WHERE mc.membership_id = tm.id
    AND mc.capability_type = 1
    AND mc.revoked_at_utc IS NULL
);
```

---

## ✅ Checklist de Migração

### Pré-Migração

- [ ] Backup do banco de dados
- [ ] Documentação atualizada
- [ ] Testes de unidade atualizados
- [ ] Testes de integração preparados

### Migração

- [ ] Migration 1 executada (campos e tabelas)
- [ ] Migration 2 executada (dados existentes)
- [ ] Verificar integridade dos dados
- [ ] Testes passando

### Pós-Migração

- [ ] Código atualizado gradualmente
- [ ] Métodos antigos deprecados
- [ ] Métodos antigos removidos
- [ ] Documentação final atualizada

---

## ⚠️ Riscos e Mitigações

### Risco 1: Perda de dados durante migração

**Mitigação**: 
- Backup completo antes da migração
- Transações atômicas
- Rollback plan preparado

### Risco 2: Breaking changes em produção

**Mitigação**:
- Migração gradual
- Manter métodos antigos temporariamente
- Feature flags para ativar novo código

### Risco 3: Performance durante migração

**Mitigação**:
- Migração em horários de baixo tráfego
- Índices criados antes da migração
- Monitoramento durante migração

---

## 📅 Timeline Sugerido

1. **Semana 1**: Preparação e documentação
2. **Semana 2**: Implementação de entidades e migrations
3. **Semana 3**: Migração de dados e testes
4. **Semana 4**: Atualização gradual de código
5. **Semana 5**: Limpeza e remoção de código obsoleto

---

## 🔍 Validação Pós-Migração

### Verificações

- [ ] Todos os Users têm `IdentityVerificationStatus` (Unverified por padrão)
- [ ] Todos os Memberships têm `MembershipSettings`
- [ ] Todos os Users com `UserRole.Curator` têm `MembershipCapability` correspondente
- [ ] Nenhum dado perdido
- [ ] Performance mantida ou melhorada

### Testes

- [ ] Testes unitários passando
- [ ] Testes de integração passando
- [ ] Testes de API passando
- [ ] Testes de regressão passando
