# Resumo Executivo - Modelo Refatorado User-Centric

## 🎯 Objetivo

Refatorar o modelo de domínio para separar claramente:
- **Identidade pessoal** (User)
- **Vínculo territorial** (Membership)
- **Verificação** (global vs territorial)
- **Configurações** (Settings)
- **Capacidades operacionais** (Capabilities)

---

## 📊 Modelo Validado

### 1. User (Pessoa Global)

```
User
├── Identidade pessoal (displayName, email, cpf, etc.)
├── Autenticação (2FA, provider, externalId)
├── UserIdentityVerificationStatus (Unverified, Pending, Verified, Rejected)
└── IdentityVerifiedAtUtc (timestamp)
```

**Responsabilidade**: Pessoa única e global, verificação de identidade global.

---

### 2. Territory (Território)

```
Territory
└── FeatureFlags
    ├── AlertPosts
    ├── EventPosts
    └── MarketplaceEnabled (novo)
```

**Responsabilidade**: Define funcionalidades disponíveis no território.

---

### 3. TerritoryMembership (Vínculo Territorial)

```
TerritoryMembership
├── UserId
├── TerritoryId
├── MembershipRole (Visitor, Resident)
├── ResidencyVerificationStatus (Unverified, GeoVerified, DocumentVerified)
├── LastGeoVerifiedAtUtc
└── LastDocumentVerifiedAtUtc
```

**Responsabilidade**: Vínculo User ↔ Territory, papel e verificação de residência.

**Regra**: 1 Resident por User (máximo) em todo o sistema.

---

### 4. MembershipSettings (Escolhas do Membro)

```
MembershipSettings
├── MembershipId (1:1)
├── MarketplaceOptIn (bool)
├── CreatedAtUtc
└── UpdatedAtUtc
```

**Responsabilidade**: Configurações e opt-ins do membro no território.

**Regra**: Criado automaticamente com o Membership.

---

### 5. MembershipCapability (Poderes Operacionais)

```
MembershipCapability
├── MembershipId
├── CapabilityType (Curator, Moderator)
├── GrantedAtUtc
├── RevokedAtUtc (nullable)
├── GrantedByUserId (nullable)
├── GrantedByMembershipId (nullable)
└── Reason (nullable)
```

**Responsabilidade**: Capacidades operacionais territoriais.

**Regra**: Empilháveis, territoriais, não alteram papel social.

---

## 🔗 Relacionamentos

```
User (1) ──< (N) TerritoryMembership (1) ──< (1) MembershipSettings
User (1) ──< (N) TerritoryMembership (1) ──< (N) MembershipCapability
Territory (1) ──< (N) TerritoryMembership
Territory (1) ──< (N) FeatureFlag
```

---

## 🛒 Marketplace - Regras Compostas

Marketplace **não é papel, identidade ou verificação**. É uma **regra composta**.

### Criar Store / Item
Requer **todas**:
1. ✅ `Territory.FeatureFlags.MarketplaceEnabled == true`
2. ✅ `MembershipSettings.MarketplaceOptIn == true`
3. ✅ `Membership.Role == Resident`
4. ✅ `Membership.ResidencyVerificationStatus != Unverified`

### Operar Plenamente
Requer tudo acima **+**:
5. ✅ `User.IdentityVerificationStatus == Verified`

---

## ✅ Validação do Modelo

### Separação de Responsabilidades
- ✅ User: identidade global
- ✅ Membership: vínculo territorial
- ✅ Settings: escolhas do membro
- ✅ Capabilities: poderes operacionais

### Verificações
- ✅ Verificação de identidade: global, no User
- ✅ Verificação de residência: territorial, no Membership

### Extensibilidade
- ✅ Novas configurações em MembershipSettings
- ✅ Novas capacidades em MembershipCapability
- ✅ Novos feature flags em Territory

### Regras de Negócio
- ✅ 1 Resident por User (validado na aplicação)
- ✅ Marketplace por composição de regras
- ✅ Capacidades empilháveis e territoriais

---

## 📋 Próximos Passos

1. ✅ **Documentação criada e validada**
2. ⏳ Implementar entidades de domínio
3. ⏳ Criar migrations
4. ⏳ Atualizar services
5. ⏳ Atualizar API
6. ⏳ Testes

---

## 📚 Documentação Relacionada

- [REFACTOR_USER_CENTRIC_MEMBERSHIP.md](./REFACTOR_USER_CENTRIC_MEMBERSHIP.md) - Documentação completa
- [REFACTOR_VALIDATION_CHECKLIST.md](./REFACTOR_VALIDATION_CHECKLIST.md) - Checklist de validação
- [12_DOMAIN_MODEL.md](./12_DOMAIN_MODEL.md) - Modelo de domínio atualizado
