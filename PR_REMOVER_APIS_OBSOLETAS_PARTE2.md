# PR: Remover APIs Obsoletas - Parte 2

**Data**: 2025-01-14  
**Status**: ✅ Implementado e Testado  
**Branch**: `refactor/remover-apis-obsoletas-parte2`

---

## 📋 Resumo

Remoção completa de código obsoleto relacionado a `VerificationStatus` e atualização de todos os testes de API para usar os novos endpoints e contratos. O projeto agora está livre de código obsoleto, conforme solicitado.

---

## 🎯 Objetivos

- ✅ Remover completamente `VerificationStatus` do código-fonte ativo
- ✅ Atualizar todos os testes de API para usar novos endpoints
- ✅ Garantir que o projeto não nasça com código obsoleto
- ✅ Todos os testes passando

---

## 🗑️ Remoções Realizadas

### Domain Layer
- ❌ Removido `VerificationStatus` property de `TerritoryMembership`
- ❌ Removido construtor obsoleto `TerritoryMembership(Guid, Guid, Guid, MembershipRole, VerificationStatus, DateTime)`
- ❌ Removido método `UpdateVerificationStatus(VerificationStatus)`
- ❌ Removidos métodos estáticos de conversão:
  - `ConvertVerificationStatusToResidencyVerification`
  - `ConvertResidencyVerificationToVerificationStatus`

### Infrastructure Layer
- ❌ Removido `VerificationStatus` de `TerritoryMembershipRecord`
- ❌ Removida configuração EF Core para `VerificationStatus`
- ❌ Atualizado `PostgresMappers` para não usar `VerificationStatus`
- ❌ Atualizado `ArapongaDbContextModelSnapshot` (removido manualmente)
- ❌ Atualizado `InMemoryDataStore` para usar `ResidencyVerification`

### Testes
- ✅ Atualizado `DomainValidationTests` para usar novo construtor
- ✅ Atualizados todos os testes de API (47 testes)
- ✅ Todos os 164 testes passando

---

## 🔄 Atualizações nos Testes de API

### Endpoints Atualizados
- `POST /api/v1/territories/{territoryId}/membership` → `POST /api/v1/territories/{territoryId}/enter` (EnterAsVisitor)
- `POST /api/v1/territories/{territoryId}/membership` → `POST /api/v1/memberships/{territoryId}/become-resident` (BecomeResident)
- `GET /api/v1/territories/{territoryId}/membership/me` → `GET /api/v1/memberships/{territoryId}/me` (GetMyMembership)

### Contratos Atualizados
- ❌ Removido uso de `MembershipResponse`
- ❌ Removido uso de `MembershipStatusResponse`
- ✅ `EnterTerritoryResponse` para Visitor
- ✅ `MembershipDetailResponse` para Resident e consultas

### Asserções Atualizadas
- `VerificationStatus.PENDING` → `ResidencyVerification.UNVERIFIED`
- `VerificationStatus.VALIDATED` → `ResidencyVerification.GEOVERIFIED` ou `DOCUMENTVERIFIED`
- `VerificationStatus.NONE` → `HttpStatusCode.NotFound` (quando não há membership)

---

## 📁 Arquivos Modificados

### Domain
- `backend/Araponga.Domain/Social/TerritoryMembership.cs` - Removido `VerificationStatus` completamente

### Infrastructure
- `backend/Araponga.Infrastructure/Postgres/Entities/TerritoryMembershipRecord.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresMappers.cs`
- `backend/Araponga.Infrastructure/Postgres/ArapongaDbContext.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresTerritoryMembershipRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/ArapongaDbContextModelSnapshot.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryDataStore.cs`

### Testes
- `backend/Araponga.Tests/Domain/DomainValidationTests.cs`
- `backend/Araponga.Tests/Api/EndToEndTests.cs`
- `backend/Araponga.Tests/Api/ApiScenariosTests.cs`

---

## ✅ Testes

### Resultados
- ✅ **164 testes passando** (0 falhas)
- ✅ **47 testes de API** atualizados e passando
- ✅ Build compila sem erros ou warnings

### Testes Atualizados
- `CompleteUserFlow_CadastroToFeed`
- `CompleteResidentFlow_CadastroToPost`
- `Memberships_RequireAuthAndTerritory`
- `Memberships_CreatePendingAndReuse`
- `Memberships_UpgradeVisitorToResident`
- `Memberships_UpgradeRequiresGeo`
- `MembershipStatus_ReturnsNoneAndValidated`
- `MembershipValidation_RequiresValidatedResident` (reescrito para usar verify-residency/geo)
- `JoinRequests_ApprovePromotesToResident`
- `JoinRequests_RejectDoesNotPromoteMembership`
- E outros...

---

## 📝 Notas Importantes

### VerificationStatus Enum
O enum `VerificationStatus` ainda existe em `backend/Araponga.Domain/Social/VerificationStatus.cs` **apenas para migrations SQL históricas**. Não é mais usado no código-fonte ativo.

### Compatibilidade
Como o projeto ainda não foi lançado ("nao tem nada implementado ainda"), não há preocupação com compatibilidade retroativa. Todas as mudanças são quebra de compatibilidade, mas isso é aceitável dado o estado atual do projeto.

---

## 🔐 Build e Validação

- ✅ Build compila sem erros
- ✅ Sem warnings CS0618 relacionados a `VerificationStatus`
- ✅ Todos os testes passando
- ✅ Sem código obsoleto no projeto

---

## 🚀 Commits

1. `ecf35a1` - Remover VerificationStatus completamente do código-fonte
2. `f8dbaea` - Atualizar testes de API para usar novos endpoints e contratos

---

## ✅ Checklist

- [x] Remover `VerificationStatus` do Domain
- [x] Remover `VerificationStatus` da Infrastructure
- [x] Atualizar mappers e repositórios
- [x] Atualizar ModelSnapshot
- [x] Atualizar dados de teste
- [x] Atualizar testes de domínio
- [x] Atualizar todos os testes de API
- [x] Build compila sem erros
- [x] Todos os testes passando
- [x] Commit e push realizados

---

**Status**: ✅ **PRONTO PARA REVIEW E MERGE**
