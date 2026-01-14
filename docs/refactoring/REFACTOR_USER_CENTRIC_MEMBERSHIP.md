# Refatoração User-Centric com MembershipSettings

**Data**: 2026-01-XX  
**Status**: 📋 Em Planejamento

---

## 📋 Contexto

No Araponga, o modelo atual mistura identidade pessoal, vínculo territorial, verificação, participação econômica e permissões operacionais. Isso gerou ambiguidades e dificuldade de evolução.

Este trabalho refatora o modelo para recentralizar o **User** e separar corretamente:
- Identidade pessoal
- Vínculo territorial
- Processos de verificação
- Configurações do membro no território
- Capacidades operacionais

**As decisões abaixo são definitivas e não devem ser reinterpretadas.**

---

## 🎯 Princípio Estrutural (Não Violar)

```
User é a pessoa.
Membership é o vínculo territorial.
Verificação é confiança.
Settings são escolhas.
Capacidades são poderes operacionais.
```

---

## 📐 MODELO FINAL DE DOMÍNIO

### 1) User — Núcleo do Sistema

O **User** representa uma pessoa única e global.

#### Responsabilidades do User
- Identidade pessoal
- Confiabilidade global
- Autenticação e segurança
- Papel técnico global (Admin, se existir)
- **Identidade verificada (global, obrigatória para features sensíveis)**

#### Criar ou ajustar no User

**UserIdentityVerificationStatus** (enum)
- `Unverified` - Identidade não verificada
- `Pending` - Verificação pendente
- `Verified` - Identidade verificada
- `Rejected` - Verificação rejeitada (opcional)

**Campos auxiliares:**
- `IdentityVerifiedAtUtc?` - Timestamp da verificação

#### Regra
**Identidade verificada pertence exclusivamente ao User.**
- Nunca pertence ao Membership nem ao Marketplace.
- É global e única por pessoa.

---

### 2) Territory — Disponibilidade de Funcionalidades

O **Territory** define o que existe naquele território.

#### Feature Flags
Usar feature flags existentes (ou criar se não houver):

```csharp
Territory.FeatureFlags.MarketplaceEnabled = true|false
```

**FeatureFlag enum** deve incluir:
- `AlertPosts` (existente)
- `EventPosts` (existente)
- `MarketplaceEnabled` (novo)

---

### 3) Membership — Vínculo Territorial (Contrato)

**Membership** representa o vínculo `User ↔ Territory`.

#### 3.1 Papel Territorial

**MembershipRole** (enum existente)
- `Visitor` - Visitante do território
- `Resident` - Morador do território

#### 3.2 Verificação de Residência (Processo Territorial)

**ResidencyVerificationStatus** (renomear de `ResidencyVerification`)
- `Unverified` - Sem verificação
- `GeoVerified` - Verificado por geolocalização
- `DocumentVerified` - Verificado por comprovante documental

**Campos auxiliares:**
- `LastGeoVerifiedAtUtc?`
- `LastDocumentVerifiedAtUtc?`

#### 3.3 Regra Estrutural de Residência

**Um User comum pode ter apenas 1 Membership como Resident em todo o sistema.**

- Admin global é exceção.
- Essa regra deve ser validada na camada de aplicação, não por constraint de banco.
- Um User pode ter múltiplos Memberships como Visitor.

---

### 4) MembershipSettings — Escolhas do Membro no Território

Esta entidade é **obrigatória** no modelo.

**MembershipSettings** representa configurações e opt-ins do membro dentro de um território.

#### Estrutura

```csharp
public sealed class MembershipSettings
{
    public Guid MembershipId { get; } // 1:1 com Membership
    public bool MarketplaceOptIn { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }
}
```

#### Regras
- Criado automaticamente ao criar um Membership
- Relacionamento 1:1 com Membership
- Futuras configurações devem ser adicionadas aqui, não no Membership

---

### 5) Marketplace — Regra Composta (Sem Papel Novo)

**Marketplace não é papel, não é identidade e não é verificação.**

#### Condições para permitir ações de marketplace

**Criar Store / Item:**
Exigir **todas**:
1. `Territory.FeatureFlags.MarketplaceEnabled == true`
2. `MembershipSettings.MarketplaceOptIn == true`
3. `Membership.Role == Resident`
4. `Membership.ResidencyVerificationStatus != Unverified`

**Operar plenamente** (publicar / responder interesse, se aplicável):
- Tudo acima **+**
5. `User.IdentityVerificationStatus == Verified`

#### Regra de Erro
- Negação por regra de domínio retorna **HTTP 403 explícito**.
- Não usar `Forbid()` para regras de negócio.

---

### 6) Curador e Moderador — Capacidades Territoriais

**Curador e Moderador não são papéis sociais nem verificações.**

São **capacidades operacionais**, atribuídas ao Membership.

#### Modelo Fixo

Criar entidade:

```csharp
public sealed class MembershipCapability
{
    public Guid Id { get; }
    public Guid MembershipId { get; }
    public MembershipCapabilityType CapabilityType { get; }
    public DateTime GrantedAtUtc { get; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? GrantedByUserId { get; }
    public Guid? GrantedByMembershipId { get; }
    public string? Reason { get; }
}
```

**MembershipCapabilityType** (enum)
- `Curator` - Capacidade de curadoria
- `Moderator` - Capacidade de moderação

#### Regras
- Curador/Moderator atuam apenas no território do Membership
- Não alteram papel
- Não criam regras
- Não governam o território
- São capacidades empilháveis (um Membership pode ter múltiplas)

---

## 🔄 OPERAÇÕES MÍNIMAS A IMPLEMENTAR / AJUSTAR

### 1. Criar automaticamente MembershipSettings
- Ao criar um Membership, criar automaticamente MembershipSettings com valores padrão

### 2. Endpoint para atualizar MembershipSettings
- Ex.: `PUT /api/v1/memberships/{id}/settings`
- Permitir atualizar `MarketplaceOptIn`

### 3. Ajustar endpoints de marketplace
- Usar:
  - Territory feature flag
  - Membership
  - MembershipSettings
  - User identity verification

### 4. Criar operações administrativas
- Conceder/remover capabilities
- `POST /api/v1/memberships/{id}/capabilities`
- `DELETE /api/v1/memberships/{id}/capabilities/{capabilityId}`

### 5. Garantir autorização
- Sempre baseada no Membership ativo
- Ajustar `AccessEvaluator` para usar `MembershipCapability` ao invés de `UserRole.Curator`

### 6. Ajustar testes e retornos HTTP
- HTTP 403 explícito para regras de negócio negadas

---

## ✅ Critérios de Sucesso

- [ ] User é fonte única de identidade verificada
- [ ] Membership contém apenas vínculo e residência
- [ ] MembershipSettings concentra escolhas do membro
- [ ] Marketplace funciona por composição de regras
- [ ] Curador/Moderator são capacidades empilháveis
- [ ] Modelo permanece simples, extensível e sem ambiguidades
- [ ] Build e testes passam

---

## 📊 Diagrama de Relacionamentos

```
User (1) ──< (N) Membership (1) ──< (1) MembershipSettings
User (1) ──< (N) Membership (1) ──< (N) MembershipCapability
Territory (1) ──< (N) Membership
Territory (1) ──< (N) FeatureFlag
```

---

## 🔍 Validação do Modelo

### Pontos de Validação

1. **Separação de Responsabilidades**
   - ✅ User: identidade global
   - ✅ Membership: vínculo territorial
   - ✅ MembershipSettings: escolhas do membro
   - ✅ MembershipCapability: poderes operacionais

2. **Verificações**
   - ✅ UserIdentityVerificationStatus: global, no User
   - ✅ ResidencyVerificationStatus: territorial, no Membership

3. **Marketplace**
   - ✅ Regras compostas (não é papel)
   - ✅ Depende de Territory, Membership, MembershipSettings e User

4. **Capacidades**
   - ✅ Territoriais (não globais)
   - ✅ Empilháveis
   - ✅ Não alteram papel social

5. **Extensibilidade**
   - ✅ Novas configurações em MembershipSettings
   - ✅ Novas capacidades em MembershipCapability
   - ✅ Novos feature flags em Territory

---

## 📝 Mudanças no Código

### Domínio

1. **User.cs**
   - Adicionar `UserIdentityVerificationStatus IdentityVerificationStatus`
   - Adicionar `DateTime? IdentityVerifiedAtUtc`
   - Método `UpdateIdentityVerification()`

2. **TerritoryMembership.cs**
   - Renomear `ResidencyVerification` para `ResidencyVerificationStatus` (apenas nome interno, enum permanece)

3. **Novas Entidades**
   - `MembershipSettings.cs`
   - `MembershipCapability.cs`
   - `MembershipCapabilityType.cs` (enum)

4. **FeatureFlag.cs**
   - Adicionar `MarketplaceEnabled = 3`

### Infraestrutura

1. **Tabelas**
   - `membership_settings` (1:1 com `territory_memberships`)
   - `membership_capabilities` (N:1 com `territory_memberships`)
   - Adicionar colunas em `users`: `identity_verification_status`, `identity_verified_at_utc`

2. **Repositórios**
   - `IMembershipSettingsRepository`
   - `IMembershipCapabilityRepository`

### Aplicação

1. **MembershipService**
   - Criar `MembershipSettings` automaticamente ao criar `Membership`

2. **MembershipAccessRules**
   - Atualizar regras de marketplace para usar novo modelo

3. **AccessEvaluator**
   - Substituir `IsCurator(User)` por `HasCapability(Membership, CapabilityType)`

4. **StoreService**
   - Usar novas regras de marketplace

---

## 🚀 Plano de Implementação

### Fase 1: Domínio
1. Criar enums e entidades de domínio
2. Atualizar User
3. Validar modelo conceitual

### Fase 2: Infraestrutura
1. Criar migrations
2. Criar repositórios
3. Atualizar mappers

### Fase 3: Aplicação
1. Atualizar services
2. Atualizar regras de acesso
3. Criar endpoints de settings

### Fase 4: Testes
1. Testes unitários
2. Testes de integração
3. Validação de regras de negócio

---

## ⚠️ Notas Importantes

- **Não violar o princípio estrutural**: User é pessoa, Membership é vínculo, Settings são escolhas, Capacidades são poderes.
- **Regra de 1 Resident por User**: Validar na aplicação, não no banco.
- **HTTP 403 explícito**: Para regras de negócio negadas.
- **Extensibilidade**: Novas configurações em MembershipSettings, novas capacidades em MembershipCapability.

## 🔄 Pontos de Atenção na Migração

### 1. UserRole.Curator → MembershipCapability

**Impacto**: 29 ocorrências encontradas no código.

**Locais que precisam ser atualizados:**
- `AccessEvaluator.IsCurator()` → `HasCapability(Membership, CapabilityType)`
- `UserRepository.ListUserIdsByRoleAsync(UserRole.Curator)` → Buscar por capabilities
- Controllers: AlertsController, AssetsController, FeaturesController, MapController, ModerationController, PlatformFeesController, JoinRequestsController
- Services: StoreService, StoreItemService, EventsService, JoinRequestService
- Handlers: ReportCreatedNotificationHandler

**Estratégia de migração:**
1. Criar `MembershipCapability` para cada User com `UserRole.Curator` em todos os seus Memberships
2. Manter `UserRole.Curator` temporariamente para compatibilidade
3. Atualizar código gradualmente
4. Remover `UserRole.Curator` após migração completa

### 2. ResidencyVerification

**Status**: Enum já existe e está correto.

**Nota**: O enum `ResidencyVerification` permanece com o nome atual. Internamente, pode ser referenciado como "status de verificação de residência" mas o nome do enum não precisa mudar.

### 3. Verificação de Identidade Global

**Novo**: `UserIdentityVerificationStatus` é completamente novo.

**Migração**: Todos os Users existentes devem iniciar com `Unverified`.

### 4. MembershipSettings

**Novo**: Entidade completamente nova.

**Migração**: Criar `MembershipSettings` para todos os `TerritoryMembership` existentes com valores padrão:
- `MarketplaceOptIn = false` (padrão conservador)
