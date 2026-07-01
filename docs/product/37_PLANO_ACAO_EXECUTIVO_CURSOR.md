# Plano de Ação Executivo - Agente Cursor Arah

**Versão**: 1.0  
**Data**: 2025-01-20  
**Status**: 🚀 Plano de Ação Pronto para Execução  
**Tipo**: Guia Sequencial Completo para Agente de IA

---

## 📋 Índice

1. [Visão Geral do Plano](#visão-geral-do-plano)
2. [Contexto e Pré-requisitos](#contexto-e-pré-requisitos)
3. [Ordem de Execução Otimizada](#ordem-de-execução-otimizada)
4. [Fase 1: Bloqueadores Críticos do Frontend](#fase-1-bloqueadores-críticos-do-frontend)
5. [Fase 2: Mídias e Sincronização Offline](#fase-2-mídias-e-sincronização-offline)
6. [Fase 3: Funcionalidades Core](#fase-3-funcionalidades-core)
7. [Validação e Qualidade](#validação-e-qualidade)
8. [Próximas Fases](#próximas-fases)
9. [Checklist de Progresso](#checklist-de-progresso)

---

## 🎯 Visão Geral do Plano

### Objetivo

Este documento fornece um **plano de ação executivo sequencial** para um agente Cursor implementar as funcionalidades críticas do backend Arah, seguindo a priorização estratégica otimizada por prosperidade.

### Princípios de Execução

1. **Prioridade Crítica Primeiro**: Implementar bloqueadores do frontend primeiro
2. **Desenvolvimento Incremental**: Uma funcionalidade por vez, testada e validada
3. **Padrões Elevados**: Seguir todas as diretrizes do projeto (Clean Architecture, testes, documentação)
4. **Validação Contínua**: Testar e validar cada etapa antes de prosseguir
5. **Documentação Atualizada**: Atualizar documentação a cada mudança

### Estrutura do Plano

Este plano está dividido em **3 Fases Críticas** que devem ser executadas sequencialmente:

- **Fase 1**: Bloqueadores Críticos do Frontend (21 dias)
- **Fase 2**: Mídias e Sincronização Offline (25 dias)
- **Fase 3**: Funcionalidades Core (15 dias)

**Total**: 61 dias de desenvolvimento backend crítico

---

## 📚 Contexto e Pré-requisitos

### Documentação de Referência

**Documentos Obrigatórios (ler primeiro)**:
1. **[34_FLUTTER_API_STRATEGIC_ALIGNMENT.md](./34_FLUTTER_API_STRATEGIC_ALIGNMENT.md)** - Gaps identificados e ajustes necessários
2. **[35_PRIORIZACAO_ESTRATEGICA_API_FRONTEND.md](./35_PRIORIZACAO_ESTRATEGICA_API_FRONTEND.md)** - Priorização otimizada
3. **[backlog-api/FASE9.md](./backlog-api/FASE9.md)** - Detalhes da Fase 9 expandida
4. **[backlog-api/FASE10.md](./backlog-api/FASE10.md)** - Detalhes da Fase 10 expandida
5. **[60_API_LÓGICA_NEGÓCIO.md](./60_API_LÓGICA_NEGÓCIO.md)** - Lógica de negócio completa

### Estado Atual do Projeto

**Backend**:
- ✅ Fases 1-8 implementadas e funcionando
- ✅ Base sólida de segurança, performance, observabilidade
- ✅ Infraestrutura de mídia completa (Fase 8)
- ⏳ Fase 9 expandida: **PENDENTE** (este plano)
- ⏳ Fase 10 expandida: **PENDENTE** (este plano)

**Frontend**:
- ⏳ Aguardando implementação das Fases 9 e 10 expandidas
- ✅ Documentação completa e pronta

### Tecnologias e Padrões

- **.NET 8** / **C# 12**
- **ASP.NET Core** / **Minimal APIs**
- **PostgreSQL** / **PostGIS**
- **EF Core** / **Migrations**
- **Clean Architecture**: Domain → Application → Infrastructure → API
- **Result Pattern**: `Result<T>` e `OperationResult`
- **FluentValidation**: Validação de requests
- **Serilog**: Logging estruturado
- **OpenTelemetry**: Observabilidade

---

## 🚀 Ordem de Execução Otimizada

### Sequência Recomendada

```
FASE 1 (21 dias): Bloqueadores Críticos
  ├── Perfil Completo (6 dias)
  ├── Segurança e Dispositivos (5 dias) ⭐ NOVO
  ├── Recuperação de Conta (5 dias) ⭐ NOVO
  └── Exclusão de Conta (5 dias) ⭐ NOVO

FASE 2 (25 dias): Mídias + Sync Offline
  ├── Mídias em Posts (8 dias)
  ├── Mídias em Eventos (5 dias)
  ├── Mídias em Marketplace (5 dias)
  ├── Mídias em Chat (4 dias)
  └── Sincronização Offline (3 dias) ⭐ NOVO

FASE 3 (15 dias): Funcionalidades Core
  ├── Edição de Posts (5 dias)
  ├── Edição de Eventos (5 dias)
  └── Gestão de Conteúdo (5 dias)
```

**Total**: 61 dias sequencial (~12 semanas)

**Com Paralelização**: ~46 dias (~9 semanas) - Ver Fase 13 (Email) em paralelo com Fase 2

---

## 📦 Fase 1: Bloqueadores Críticos do Frontend (21 dias)

### Objetivo

Implementar todas as funcionalidades que bloqueiam o desenvolvimento do frontend Flutter, incluindo perfil completo, segurança, recuperação e exclusão de conta.

### Duração

**21 dias úteis** (~4 semanas)

### Tarefas Sequenciais

---

#### TAREFA 1.1: Avatar e Bio no Perfil (6 dias)

**Objetivo**: Adicionar avatar e bio ao perfil de usuário

**Documentação de Referência**: `docs/backlog-api/FASE9.md` - Seção 32.1 e 32.2

**Passos Detalhados**:

1. **Modelo de Domínio** (1 dia)
   - [ ] Ler `backend/Arah.Domain/Users/User.cs`
   - [ ] Adicionar propriedades:
     ```csharp
     public Guid? AvatarMediaAssetId { get; private set; }
     public string? Bio { get; private set; }
     ```
   - [ ] Adicionar métodos:
     ```csharp
     public void UpdateAvatar(Guid? mediaAssetId)
     public void UpdateBio(string? bio)
     ```
   - [ ] Adicionar validações:
     - Bio: máximo 500 caracteres, trim
     - Avatar: deve existir como `MediaAsset`
   - [ ] Atualizar `UserRecord.cs` no Infrastructure
   - [ ] Criar migration: `YYYYMMDDHHMMSS_AddUserAvatarAndBio.cs`

2. **Serviço de Aplicação** (2 dias)
   - [ ] Ler `backend/Arah.Application/Services/UserProfileService.cs`
   - [ ] Adicionar método `UpdateAvatarAsync(Guid userId, Guid mediaAssetId, CancellationToken)`
     - Validar que `mediaAssetId` existe e pertence ao usuário
     - Validar que é imagem (não vídeo)
     - Atualizar `User.AvatarMediaAssetId`
   - [ ] Adicionar método `UpdateBioAsync(Guid userId, string? bio, CancellationToken)`
     - Validar tamanho (máx. 500 caracteres)
     - Atualizar `User.Bio`
   - [ ] Atualizar método `GetProfileAsync` para incluir avatar e bio
     - Respeitar preferências de privacidade
     - Incluir URL do avatar se disponível

3. **Controllers e DTOs** (1.5 dias)
   - [ ] Criar/atualizar `backend/Arah.Api/Controllers/ProfileController.cs`
   - [ ] Adicionar endpoint `PUT /api/v1/users/profile/avatar`
     - Request: `{ mediaAssetId: Guid }`
     - Response: `{ success: bool, avatarUrl?: string }`
   - [ ] Adicionar endpoint `PUT /api/v1/users/profile/bio`
     - Request: `{ bio?: string }`
     - Response: `{ success: bool }`
   - [ ] Atualizar endpoint `GET /api/v1/users/profile` para incluir avatar e bio
   - [ ] Criar DTOs:
     - `UpdateAvatarRequest.cs`
     - `UpdateBioRequest.cs`
     - `UserProfileResponse.cs` (atualizado)

4. **Validações** (0.5 dia)
   - [ ] Criar `UpdateAvatarRequestValidator.cs`
     - Validar que `mediaAssetId` não é vazio
   - [ ] Criar `UpdateBioRequestValidator.cs`
     - Validar que bio tem no máximo 500 caracteres

5. **Testes** (1 dia)
   - [ ] Testes unitários do modelo `User` (avatar e bio)
   - [ ] Testes unitários do `UserProfileService`
   - [ ] Testes de integração dos endpoints
   - [ ] Testes de validação

**Critérios de Sucesso**:
- ✅ Avatar pode ser atualizado via API
- ✅ Bio pode ser atualizada via API
- ✅ Avatar e bio aparecem no GET /api/v1/users/profile
- ✅ Validações funcionando corretamente
- ✅ Testes passando (>80% cobertura)

**Arquivos a Criar/Modificar**:
- `backend/Arah.Domain/Users/User.cs` (modificar)
- `backend/Arah.Infrastructure/Postgres/Entities/UserRecord.cs` (modificar)
- `backend/Arah.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddUserAvatarAndBio.cs` (criar)
- `backend/Arah.Application/Services/UserProfileService.cs` (modificar)
- `backend/Arah.Api/Controllers/ProfileController.cs` (criar/atualizar)
- `backend/Arah.Api/Contracts/Profile/*.cs` (criar)

---

#### TAREFA 1.2: Segurança e Dispositivos - Push Tokens (5 dias) ⭐ NOVO

**Objetivo**: Implementar registro e gerenciamento de dispositivos para push notifications

**Documentação de Referência**: `docs/34_FLUTTER_API_STRATEGIC_ALIGNMENT.md` - Gap 1

**Passos Detalhados**:

1. **Modelo de Domínio** (1 dia)
   - [ ] Ler `design/Archtecture/MER.md` - entidade `USER_DEVICE`
   - [ ] Criar `backend/Arah.Domain/Devices/Device.cs`
     ```csharp
     public class Device
     {
         public Guid Id { get; }
         public Guid UserId { get; }
         public string Platform { get; } // IOS, ANDROID, WEB
         public string DeviceToken { get; private set; } // FCM/APNs token
         public string? DeviceLabel { get; private set; }
         public string? DeviceFingerprint { get; private set; }
         public string? AppVersion { get; private set; }
         public bool IsTrusted { get; private set; }
         public bool IsRevoked { get; private set; }
         public DateTime CreatedAtUtc { get; }
         public DateTime LastSeenAtUtc { get; private set; }
     }
     ```
   - [ ] Criar métodos:
     ```csharp
     public void UpdateToken(string token)
     public void UpdateLastSeen()
     public void Revoke()
     public void Trust()
     ```
   - [ ] Criar `DeviceRecord.cs` no Infrastructure
   - [ ] Criar migration: `YYYYMMDDHHMMSS_AddUserDevice.cs`

2. **Repositórios** (1 dia)
   - [ ] Criar `backend/Arah.Application/Interfaces/IDeviceRepository.cs`
     - `CreateAsync(Device, CancellationToken)`
     - `GetByIdAsync(Guid, CancellationToken)`
     - `GetByUserIdAsync(Guid, CancellationToken)`
     - `GetByTokenAsync(string, CancellationToken)`
     - `UpdateAsync(Device, CancellationToken)`
     - `DeleteAsync(Guid, CancellationToken)`
   - [ ] Implementar `PostgresDeviceRepository.cs`
   - [ ] Implementar `InMemoryDeviceRepository.cs` (para testes)

3. **Serviço de Aplicação** (1.5 dias)
   - [ ] Criar `backend/Arah.Application/Services/DeviceService.cs`
     - `RegisterDeviceAsync(Guid userId, string platform, string token, string? label, string? appVersion, CancellationToken)`
       - Validar token único por usuário
       - Criar ou atualizar device existente
     - `UpdateDeviceTokenAsync(Guid deviceId, string token, CancellationToken)`
       - Validar device pertence ao usuário
       - Atualizar token e lastSeenAt
     - `GetUserDevicesAsync(Guid userId, CancellationToken)`
     - `RevokeDeviceAsync(Guid deviceId, Guid userId, CancellationToken)`
       - Validar device pertence ao usuário
       - Marcar como revoked

4. **Controllers e DTOs** (1 dia)
   - [ ] Criar `backend/Arah.Api/Controllers/DevicesController.cs`
   - [ ] Endpoint `POST /api/v1/users/devices/register`:
     - Request: `{ platform: string, deviceToken: string, deviceLabel?: string, appVersion?: string }`
     - Response: `{ deviceId: Guid }`
   - [ ] Endpoint `PUT /api/v1/users/devices/{deviceId}/token`:
     - Request: `{ deviceToken: string }`
     - Response: `{ success: bool }`
   - [ ] Endpoint `GET /api/v1/users/devices`:
     - Response: `{ devices: DeviceInfo[] }`
   - [ ] Endpoint `DELETE /api/v1/users/devices/{deviceId}`:
     - Response: `{ success: bool }`
   - [ ] Criar DTOs:
     - `RegisterDeviceRequest.cs`
     - `UpdateDeviceTokenRequest.cs`
     - `DeviceInfoResponse.cs`

5. **Validações** (0.25 dia)
   - [ ] `RegisterDeviceRequestValidator.cs`
     - Platform: IOS, ANDROID, WEB
     - DeviceToken: obrigatório, mínimo 10 caracteres
   - [ ] `UpdateDeviceTokenRequestValidator.cs`

6. **Testes** (0.25 dia)
   - [ ] Testes unitários do modelo `Device`
   - [ ] Testes unitários do `DeviceService`
   - [ ] Testes de integração dos endpoints

**Critérios de Sucesso**:
- ✅ Dispositivos podem ser registrados via API
- ✅ Tokens podem ser atualizados via API
- ✅ Dispositivos podem ser listados e revogados
- ✅ Validações funcionando
- ✅ Testes passando

**Arquivos a Criar**:
- `backend/Arah.Domain/Devices/Device.cs`
- `backend/Arah.Infrastructure/Postgres/Entities/DeviceRecord.cs`
- `backend/Arah.Application/Interfaces/IDeviceRepository.cs`
- `backend/Arah.Application/Services/DeviceService.cs`
- `backend/Arah.Api/Controllers/DevicesController.cs`
- `backend/Arah.Api/Contracts/Devices/*.cs`

---

#### TAREFA 1.3: Segurança e Dispositivos - Preferências de Segurança (2 dias) ⭐ NOVO

**Objetivo**: Expor preferências de segurança (biometria) via API

**Documentação de Referência**: `docs/34_FLUTTER_API_STRATEGIC_ALIGNMENT.md` - Gap 6

**Passos Detalhados**:

1. **Modelo de Domínio** (0.5 dia)
   - [ ] Ler `design/Archtecture/MER.md` - entidade `USER_SECURITY_SETTINGS`
   - [ ] Verificar se existe `UserSecuritySettings` no Domain
   - [ ] Se não existe, criar `backend/Arah.Domain/Users/UserSecuritySettings.cs`
     ```csharp
     public class UserSecuritySettings
     {
         public Guid UserId { get; }
         public bool BiometricEnabled { get; private set; }
         public DateTime? LastStrongAuthAtUtc { get; private set; }
         public DateTime UpdatedAtUtc { get; private set; }
     }
     ```
   - [ ] Criar migration se necessário

2. **Serviço de Aplicação** (0.5 dia)
   - [ ] Criar `backend/Arah.Application/Services/SecuritySettingsService.cs`
     - `GetSecuritySettingsAsync(Guid userId, CancellationToken)`
     - `UpdateBiometricEnabledAsync(Guid userId, bool enabled, CancellationToken)`
     - `UpdateLastStrongAuthAsync(Guid userId, CancellationToken)`

3. **Controllers e DTOs** (0.75 dia)
   - [ ] Criar/atualizar `backend/Arah.Api/Controllers/SecurityController.cs`
   - [ ] Endpoint `GET /api/v1/users/security-settings`:
     - Response: `{ biometricEnabled: bool, lastStrongAuthAt?: DateTime }`
   - [ ] Endpoint `PUT /api/v1/users/security-settings`:
     - Request: `{ biometricEnabled: bool }`
     - Response: `{ success: bool }`
   - [ ] Criar DTOs:
     - `SecuritySettingsResponse.cs`
     - `UpdateSecuritySettingsRequest.cs`

4. **Validações** (0.1 dia)
   - [ ] `UpdateSecuritySettingsRequestValidator.cs`

5. **Testes** (0.15 dia)
   - [ ] Testes unitários do serviço
   - [ ] Testes de integração dos endpoints

**Critérios de Sucesso**:
- ✅ Preferências de segurança podem ser consultadas e atualizadas
- ✅ Validações funcionando
- ✅ Testes passando

---

#### TAREFA 1.4: Recuperação de Conta (5 dias) ⭐ NOVO

**Objetivo**: Implementar recuperação de conta via email/telefone e recuperação de 2FA

**Documentação de Referência**: `docs/34_FLUTTER_API_STRATEGIC_ALIGNMENT.md` - Gap 2

**Passos Detalhados**:

1. **Modelo de Domínio** (1 dia)
   - [ ] Criar `backend/Arah.Domain/Recovery/RecoveryRequest.cs`
     ```csharp
     public class RecoveryRequest
     {
         public Guid Id { get; }
         public Guid? UserId { get; private set; } // null até validação
         public string EmailOrPhone { get; }
         public string Code { get; } // 6 dígitos
         public RecoveryType Type { get; } // ACCOUNT, TWO_FACTOR
         public RecoveryStatus Status { get; private set; }
         public DateTime CreatedAtUtc { get; }
         public DateTime ExpiresAtUtc { get; }
         public DateTime? VerifiedAtUtc { get; private set; }
         public int Attempts { get; private set; }
     }
     ```
   - [ ] Criar enums:
     - `RecoveryType`: ACCOUNT, TWO_FACTOR
     - `RecoveryStatus`: PENDING, VERIFIED, EXPIRED, USED
   - [ ] Criar migration: `YYYYMMDDHHMMSS_AddRecoveryRequest.cs`

2. **Serviço de Recuperação** (2 dias)
   - [ ] Criar `backend/Arah.Application/Services/RecoveryService.cs`
     - `RequestRecoveryAsync(string emailOrPhone, RecoveryType type, CancellationToken)`
       - Validar email/telefone existe no sistema
       - Gerar código de 6 dígitos
       - Criar `RecoveryRequest` (expira em 15 minutos)
       - Enviar código por email/SMS (integrar com Fase 13 quando disponível)
       - Retornar `recoveryId`
     - `VerifyRecoveryCodeAsync(Guid recoveryId, string code, CancellationToken)`
       - Validar código
       - Validar não expirado
       - Validar tentativas < 3
       - Marcar como VERIFIED
     - `Reset2FAAsync(Guid recoveryId, string new2FASecret, CancellationToken)`
       - Validar recovery VERIFIED e tipo TWO_FACTOR
       - Atualizar 2FA do usuário
       - Marcar recovery como USED
     - `ResetAuthMethodAsync(Guid recoveryId, string newProvider, CancellationToken)`
       - Validar recovery VERIFIED e tipo ACCOUNT
       - Atualizar método de autenticação do usuário
       - Marcar recovery como USED

3. **Controllers e DTOs** (1 dia)
   - [ ] Criar `backend/Arah.Api/Controllers/RecoveryController.cs`
   - [ ] Endpoint `POST /api/v1/auth/recover`:
     - Request: `{ emailOrPhone: string, type: string }` // ACCOUNT ou TWO_FACTOR
     - Response: `{ recoveryId: Guid, expiresAt: DateTime }`
   - [ ] Endpoint `POST /api/v1/auth/recover/verify`:
     - Request: `{ recoveryId: Guid, code: string }`
     - Response: `{ userId: Guid, canReset2FA: bool }`
   - [ ] Endpoint `POST /api/v1/auth/recover/reset-2fa`:
     - Request: `{ recoveryId: Guid, new2FASecret: string }`
     - Response: `{ success: bool }`
   - [ ] Endpoint `POST /api/v1/auth/recover/reset-auth-method`:
     - Request: `{ recoveryId: Guid, newProvider: string }`
     - Response: `{ success: bool }`
   - [ ] Criar DTOs:
     - `RecoveryRequest.cs`
     - `VerifyRecoveryRequest.cs`
     - `Reset2FARequest.cs`
     - `ResetAuthMethodRequest.cs`

4. **Validações** (0.5 dia)
   - [ ] `RecoveryRequestValidator.cs`
     - EmailOrPhone: obrigatório, formato válido
     - Type: ACCOUNT ou TWO_FACTOR
   - [ ] `VerifyRecoveryRequestValidator.cs`
     - Code: obrigatório, 6 dígitos
   - [ ] `Reset2FARequestValidator.cs`
     - New2FASecret: obrigatório, formato válido
   - [ ] `ResetAuthMethodRequestValidator.cs`
     - NewProvider: GOOGLE, APPLE, MICROSOFT

5. **Integração com Email/SMS** (0.3 dia)
   - [ ] Criar interface `IEmailService` ou `ISmsService` (mock para agora)
   - [ ] Implementar mock que loga código (integração real na Fase 13)

6. **Testes** (0.2 dia)
   - [ ] Testes unitários do modelo
   - [ ] Testes unitários do serviço
   - [ ] Testes de integração dos endpoints

**Critérios de Sucesso**:
- ✅ Recuperação de conta pode ser solicitada
- ✅ Código de recuperação pode ser verificado
- ✅ 2FA pode ser resetado
- ✅ Método de autenticação pode ser resetado
- ✅ Validações funcionando (tentativas, expiração)
- ✅ Testes passando

**Nota**: Integração real com email/SMS será feita na Fase 13. Por enquanto, usar mock que loga o código.

---

#### TAREFA 1.5: Exclusão de Conta (LGPD/GDPR) (5 dias) ⭐ NOVO

**Objetivo**: Implementar exclusão de conta com exportação de dados e período de graça

**Documentação de Referência**: `docs/34_FLUTTER_API_STRATEGIC_ALIGNMENT.md` - Gap 3

**Passos Detalhados**:

1. **Modelo de Domínio** (1 dia)
   - [ ] Adicionar propriedades ao `User`:
     ```csharp
     public DateTime? DeletionScheduledAtUtc { get; private set; }
     public DateTime? DeletionGracePeriodEndsAtUtc { get; private set; }
     ```
   - [ ] Adicionar métodos:
     ```csharp
     public void ScheduleDeletion(DateTime gracePeriodEndsAt)
     public void CancelDeletion()
     public bool IsDeletionPending => DeletionScheduledAtUtc.HasValue && DateTime.UtcNow < DeletionGracePeriodEndsAtUtc
     ```
   - [ ] Criar migration: `YYYYMMDDHHMMSS_AddUserDeletion.cs`

2. **Serviço de Exportação** (1.5 dias)
   - [ ] Criar `backend/Arah.Application/Services/DataExportService.cs`
     - `ExportUserDataAsync(Guid userId, CancellationToken)`
       - Coletar todos os dados do usuário:
         - Perfil completo
         - Posts criados
         - Comentários
         - Eventos criados/participados
         - Vínculos territoriais
         - Preferências
         - Histórico de atividades
       - Gerar arquivo JSON
       - Upload para storage temporário (7 dias de expiração)
       - Retornar URL de download
   - [ ] Criar modelo `UserDataExport.cs` com estrutura completa

3. **Serviço de Exclusão** (1 dia)
   - [ ] Criar `backend/Arah.Application/Services/AccountDeletionService.cs`
     - `ScheduleDeletionAsync(Guid userId, CancellationToken)`
       - Validar confirmação dupla (via request)
       - Agendar exclusão (período de graça: 7 dias)
       - Marcar `DeletionScheduledAtUtc` e `DeletionGracePeriodEndsAtUtc`
       - Publicar evento de exclusão agendada
     - `CancelDeletionAsync(Guid userId, CancellationToken)`
       - Validar exclusão pendente
       - Cancelar exclusão
       - Limpar campos de deleção
     - `GetDeletionStatusAsync(Guid userId, CancellationToken)`
       - Retornar status de exclusão
     - `ProcessScheduledDeletionsAsync(CancellationToken)` (background job)
       - Buscar usuários com `DeletionGracePeriodEndsAtUtc < DateTime.UtcNow`
       - Anonimizar ou deletar dados
       - Processar em batch

4. **Controllers e DTOs** (1 dia)
   - [ ] Criar `backend/Arah.Api/Controllers/AccountController.cs`
   - [ ] Endpoint `GET /api/v1/users/export-data`:
     - Response: `{ dataUrl: string, expiresAt: DateTime }`
   - [ ] Endpoint `POST /api/v1/users/delete-account`:
     - Request: `{ confirmation: string }` // deve ser "EXCLUIR"
     - Response: `{ deletionScheduledAt: DateTime, gracePeriodEndsAt: DateTime }`
   - [ ] Endpoint `DELETE /api/v1/users/delete-account/cancel`:
     - Response: `{ success: bool }`
   - [ ] Endpoint `GET /api/v1/users/delete-account/status`:
     - Response: `{ status: string, scheduledAt?: DateTime, gracePeriodEndsAt?: DateTime }`
   - [ ] Criar DTOs:
     - `ExportDataResponse.cs`
     - `DeleteAccountRequest.cs`
     - `DeleteAccountResponse.cs`
     - `DeleteAccountStatusResponse.cs`

5. **Background Worker** (0.3 dia)
   - [ ] Criar `backend/Arah.Infrastructure/Background/AccountDeletionWorker.cs`
     - Processar exclusões agendadas periodicamente (diário)
     - Chamar `ProcessScheduledDeletionsAsync`

6. **Validações** (0.1 dia)
   - [ ] `DeleteAccountRequestValidator.cs`
     - Confirmation: deve ser exatamente "EXCLUIR"

7. **Testes** (0.1 dia)
   - [ ] Testes unitários do modelo
   - [ ] Testes unitários dos serviços
   - [ ] Testes de integração dos endpoints

**Critérios de Sucesso**:
- ✅ Dados podem ser exportados (JSON completo)
- ✅ Exclusão pode ser agendada com período de graça
- ✅ Exclusão pode ser cancelada durante período de graça
- ✅ Status de exclusão pode ser consultado
- ✅ Background worker processa exclusões expiradas
- ✅ Testes passando

**Nota**: Anonimização vs deleção completa deve seguir política de dados do projeto.

---

### Validação da Fase 1

**Checklist Completo**:
- [ ] Todos os endpoints funcionando e testados
- [ ] Migrations criadas e aplicadas
- [ ] Testes unitários > 80% cobertura
- [ ] Testes de integração passando
- [ ] Documentação atualizada
- [ ] Swagger/OpenAPI atualizado
- [ ] Compilação sem erros
- [ ] Sem erros de linter

**Entregável**: Backend desbloqueia desenvolvimento frontend ✅

---

## 📦 Fase 2: Mídias e Sincronização Offline (25 dias)

### Objetivo

Implementar integração de mídias em todos os conteúdos e sincronização offline para suportar modo offline do frontend.

### Duração

**25 dias úteis** (~5 semanas)

### Tarefas Sequenciais

---

#### TAREFA 2.1: Mídias em Posts (8 dias)

**Objetivo**: Adicionar suporte a múltiplas imagens por post

**Documentação de Referência**: `docs/backlog-api/FASE10.md` - Seção 10.1

**Passos Detalhados**:

1. **Modelo de Domínio** (1 dia)
   - [ ] Ler `backend/Arah.Domain/Posts/Post.cs`
   - [ ] Verificar se `MediaAttachment` já existe (Fase 8)
   - [ ] Se não existe, criar modelo de relacionamento Post ↔ MediaAsset
   - [ ] Adicionar propriedade ao Post:
     ```csharp
     public IReadOnlyList<MediaAttachment> MediaAttachments { get; }
     ```
   - [ ] Criar `MediaAttachment` (se não existe):
     ```csharp
     public class MediaAttachment
     {
         public Guid PostId { get; }
         public Guid MediaAssetId { get; }
         public int DisplayOrder { get; }
     }
     ```
   - [ ] Criar migration se necessário

2. **Serviço de Criação de Posts** (2 dias)
   - [ ] Ler `backend/Arah.Application/Services/PostCreationService.cs`
   - [ ] Atualizar `CreatePostAsync` para aceitar `mediaIds`:
     - Validar que mídias pertencem ao usuário
     - Validar máximo 10 mídias por post
     - Criar `MediaAttachment` para cada mídia
     - Definir `DisplayOrder` (ordem de envio)

3. **Controllers e DTOs** (1 dia)
   - [ ] Ler `backend/Arah.Api/Controllers/FeedController.cs`
   - [ ] Atualizar `POST /api/v1/feed/posts`:
     - Request adicionar `mediaIds?: Guid[]`
   - [ ] Atualizar `PostResponse`:
     - Adicionar `mediaUrls: string[]`
     - Adicionar `mediaCount: int`
   - [ ] Atualizar `CreatePostRequest.cs`

4. **Exclusão de Posts com Mídias** (1 dia)
   - [ ] Atualizar exclusão de posts:
     - Deletar `MediaAttachment` quando post é deletado
     - Soft delete de `MediaAsset` (se não usado em outros lugares)

5. **Validações** (0.5 dia)
   - [ ] Atualizar `CreatePostRequestValidator.cs`
     - MediaIds: máximo 10 itens
     - Validar que mídias existem

6. **Testes** (2.5 dias)
   - [ ] Testes unitários do serviço
   - [ ] Testes de integração dos endpoints
   - [ ] Testes de exclusão de mídias

**Critérios de Sucesso**:
- ✅ Posts podem ter múltiplas imagens (até 10)
- ✅ Ordem de exibição funcionando
- ✅ Exclusão de posts deleta mídias associadas
- ✅ Validações funcionando
- ✅ Testes passando

---

#### TAREFA 2.2: Mídias em Eventos (5 dias)

**Objetivo**: Adicionar imagem de capa e mídias adicionais em eventos

**Documentação de Referência**: `docs/backlog-api/FASE10.md` - Seção 10.2

**Passos Detalhados**:

1. **Modelo de Domínio** (1 dia)
   - [ ] Ler `backend/Arah.Domain/Events/Event.cs`
   - [ ] Adicionar propriedades:
     ```csharp
     public Guid? CoverImageMediaAssetId { get; private set; }
     public IReadOnlyList<MediaAttachment> MediaAttachments { get; }
     ```
   - [ ] Criar migration

2. **Serviço de Eventos** (1.5 dias)
   - [ ] Ler `backend/Arah.Application/Services/EventsService.cs`
   - [ ] Atualizar `CreateEventAsync` para aceitar `coverImageId` e `mediaIds`
   - [ ] Atualizar `UpdateEventAsync` para permitir atualizar mídias

3. **Controllers e DTOs** (1 dia)
   - [ ] Ler `backend/Arah.Api/Controllers/EventsController.cs`
   - [ ] Atualizar `POST /api/v1/events`:
     - Adicionar `coverImageId?: Guid` e `mediaIds?: Guid[]`
   - [ ] Atualizar `EventResponse`:
     - Adicionar `coverImageUrl?: string` e `mediaUrls: string[]`

4. **Exclusão de Eventos** (0.5 dia)
   - [ ] Atualizar exclusão para deletar mídias associadas

5. **Testes** (1 dia)
   - [ ] Testes unitários e de integração

**Critérios de Sucesso**:
- ✅ Eventos podem ter imagem de capa
- ✅ Eventos podem ter mídias adicionais
- ✅ Exclusão deleta mídias
- ✅ Testes passando

---

#### TAREFA 2.3: Mídias em Marketplace (5 dias)

**Objetivo**: Adicionar múltiplas imagens por item no marketplace

**Documentação de Referência**: `docs/backlog-api/FASE10.md` - Seção 10.3

**Passos Detalhados**:

1. **Modelo de Domínio** (1 dia)
   - [ ] Ler `backend/Arah.Domain/Marketplace/Item.cs`
   - [ ] Adicionar suporte a mídias (similar a posts)

2. **Serviço de Marketplace** (1.5 dias)
   - [ ] Atualizar criação/atualização de items para incluir mídias
   - [ ] Definir primeira imagem como principal

3. **Controllers e DTOs** (1 dia)
   - [ ] Atualizar endpoints de items para incluir mídias

4. **Exclusão** (0.5 dia)
   - [ ] Atualizar exclusão de items

5. **Testes** (1 dia)
   - [ ] Testes completos

**Critérios de Sucesso**:
- ✅ Items podem ter múltiplas imagens
- ✅ Imagem principal identificada
- ✅ Testes passando

---

#### TAREFA 2.4: Mídias em Chat (4 dias)

**Objetivo**: Permitir envio de imagens em mensagens de chat

**Documentação de Referência**: `docs/backlog-api/FASE10.md` - Seção 10.4

**Passos Detalhados**:

1. **Modelo de Domínio** (1 dia)
   - [ ] Ler `backend/Arah.Domain/Chat/Message.cs`
   - [ ] Adicionar suporte a `MediaAssetId` opcional

2. **Serviço de Chat** (1.5 dias)
   - [ ] Atualizar criação de mensagens para incluir mídia
   - [ ] Validar tamanho e tipo de mídia

3. **Controllers e DTOs** (1 dia)
   - [ ] Atualizar endpoints de mensagens

4. **Testes** (0.5 dia)
   - [ ] Testes básicos

**Critérios de Sucesso**:
- ✅ Mensagens podem ter imagens
- ✅ Validações funcionando
- ✅ Testes passando

---

#### TAREFA 2.5: Sincronização Offline (3 dias) ⭐ NOVO

**Objetivo**: Implementar endpoints para sincronização batch de ações offline

**Documentação de Referência**: `docs/34_FLUTTER_API_STRATEGIC_ALIGNMENT.md` - Gap 4

**Passos Detalhados**:

1. **Modelo de Dados** (0.5 dia)
   - [ ] Criar `backend/Arah.Application/Models/OfflineAction.cs`
     ```csharp
     public class OfflineAction
     {
         public Guid ClientId { get; set; }
         public string Type { get; set; } // CREATE_POST, CREATE_COMMENT, LIKE, etc.
         public Dictionary<string, object> Data { get; set; }
         public DateTime ClientTimestamp { get; set; }
     }
     ```
   - [ ] Criar `OfflineActionResult.cs`:
     ```csharp
     public class OfflineActionResult
     {
         public Guid ClientId { get; set; }
         public bool Success { get; set; }
         public Guid? ServerId { get; set; }
         public string? Error { get; set; }
         public ConflictInfo? Conflict { get; set; }
     }
     ```

2. **Serviço de Sincronização** (1.5 dias)
   - [ ] Criar `backend/Arah.Application/Services/OfflineSyncService.cs`
     - `SyncBatchAsync(Guid userId, List<OfflineAction> actions, CancellationToken)`
       - Processar cada ação na ordem
       - Validar e executar ação
       - Detectar conflitos (se item foi alterado/deletado online)
       - Retornar resultados com conflitos
     - `ResolveConflictAsync(Guid userId, Guid clientId, string resolution, Dictionary<string, object>? data, CancellationToken)`
       - Resolução: SERVER_WINS, CLIENT_WINS, MERGE
       - Aplicar resolução

3. **Controllers e DTOs** (0.75 dia)
   - [ ] Criar `backend/Arah.Api/Controllers/SyncController.cs`
   - [ ] Endpoint `POST /api/v1/sync/batch`:
     - Request: `{ actions: OfflineAction[] }`
     - Response: `{ results: OfflineActionResult[] }`
   - [ ] Endpoint `GET /api/v1/sync/conflicts/{clientId}`:
     - Response: `{ conflictType: string, serverData: object, clientData: object }`
   - [ ] Endpoint `POST /api/v1/sync/resolve-conflict`:
     - Request: `{ clientId: Guid, resolution: string, data?: object }`
     - Response: `{ success: bool, serverId?: Guid }`
   - [ ] Endpoint `GET /api/v1/sync/status`:
     - Response: `{ lastSyncAt?: DateTime, pendingActionsCount: int }`

4. **Validações** (0.1 dia)
   - [ ] Validar tipos de ações suportados
   - [ ] Validar formato de dados

5. **Testes** (0.15 dia)
   - [ ] Testes unitários do serviço
   - [ ] Testes de integração

**Critérios de Sucesso**:
- ✅ Múltiplas ações offline podem ser sincronizadas
- ✅ Conflitos podem ser detectados
- ✅ Conflitos podem ser resolvidos
- ✅ Status de sincronização pode ser consultado
- ✅ Testes passando

**Tipos de Ações Suportadas Inicialmente**:
- `CREATE_POST`
- `CREATE_COMMENT`
- `LIKE_POST`
- `CREATE_EVENT`

---

### Validação da Fase 2

**Checklist Completo**:
- [ ] Mídias integradas em todos os conteúdos
- [ ] Sincronização offline funcionando
- [ ] Todos os endpoints testados
- [ ] Migrations aplicadas
- [ ] Testes passando (>80% cobertura)
- [ ] Documentação atualizada
- [ ] Swagger atualizado

**Entregável**: Mídias completas + Modo offline funcionando ✅

---

## 📦 Fase 3: Funcionalidades Core (15 dias)

### Objetivo

Implementar funcionalidades de edição e gestão de conteúdo.

### Duração

**15 dias úteis** (~3 semanas)

### Tarefas Sequenciais

---

#### TAREFA 3.1: Edição de Posts (5 dias)

**Documentação de Referência**: `docs/backlog-api/FASE11.md` - Seção 11.1

**Passos**:
1. Atualizar `Post` para permitir edição
2. Criar `PostUpdateService`
3. Endpoint `PUT /api/v1/feed/posts/{id}`
4. Validações e testes

---

#### TAREFA 3.2: Edição de Eventos (5 dias)

**Documentação de Referência**: `docs/backlog-api/FASE11.md` - Seção 11.2

**Passos**:
1. Atualizar `Event` para permitir edição
2. Criar `EventUpdateService`
3. Endpoint `PUT /api/v1/events/{id}`
4. Validações e testes

---

#### TAREFA 3.3: Gestão de Conteúdo (5 dias)

**Documentação de Referência**: `docs/backlog-api/FASE11.md` - Seção 11.3

**Passos**:
1. Gestão de mídias (reordenar, deletar)
2. Histórico de edições (opcional)
3. Versões de conteúdo (opcional)
4. Testes completos

---

### Validação da Fase 3

**Checklist Completo**:
- [ ] Edição de posts funcionando
- [ ] Edição de eventos funcionando
- [ ] Gestão de mídias funcionando
- [ ] Testes passando
- [ ] Documentação atualizada

**Entregável**: Funcionalidades core completas ✅

---

## ✅ Validação e Qualidade

### Critérios de Qualidade para Cada Tarefa

1. **Código**:
   - ✅ Compila sem erros
   - ✅ Sem warnings críticos
   - ✅ Segue padrões do projeto (Clean Architecture)
   - ✅ Documentação XML em classes públicas

2. **Testes**:
   - ✅ Cobertura > 80% (unitários)
   - ✅ Todos os testes passando
   - ✅ Testes de integração para endpoints

3. **Validações**:
   - ✅ FluentValidation implementado
   - ✅ Validações de domínio implementadas
   - ✅ Tratamento de erros adequado

4. **Documentação**:
   - ✅ Swagger/OpenAPI atualizado
   - ✅ Comentários XML atualizados
   - ✅ README atualizado se necessário

5. **Migrations**:
   - ✅ Migrations criadas
   - ✅ Migrations aplicadas sem erros
   - ✅ Rollback testado

---

## 🚀 Próximas Fases

### Após Completar Fase 1-3

**Próximas prioridades** (conforme [35_PRIORIZACAO_ESTRATEGICA_API_FRONTEND.md](./35_PRIORIZACAO_ESTRATEGICA_API_FRONTEND.md)):

1. **Fase 13**: Email Connector (14 dias) - Pode ser feito em paralelo com Fase 2
2. **Fase 14**: Governança e Votação (21 dias)
3. **Fase 29**: Suporte Mobile Avançado (14 dias) - Após Fase 1 e 2

---

## 📋 Checklist de Progresso

### Fase 1: Bloqueadores Críticos (21 dias)

- [ ] TAREFA 1.1: Avatar e Bio (6 dias)
- [ ] TAREFA 1.2: Push Tokens (5 dias)
- [ ] TAREFA 1.3: Preferências de Segurança (2 dias)
- [ ] TAREFA 1.4: Recuperação de Conta (5 dias)
- [ ] TAREFA 1.5: Exclusão de Conta (5 dias)

**Status**: ⏳ **PENDENTE**

---

### Fase 2: Mídias + Sync Offline (25 dias)

- [ ] TAREFA 2.1: Mídias em Posts (8 dias)
- [ ] TAREFA 2.2: Mídias em Eventos (5 dias)
- [ ] TAREFA 2.3: Mídias em Marketplace (5 dias)
- [ ] TAREFA 2.4: Mídias em Chat (4 dias)
- [ ] TAREFA 2.5: Sincronização Offline (3 dias)

**Status**: ⏳ **PENDENTE**

---

### Fase 3: Funcionalidades Core (15 dias)

- [ ] TAREFA 3.1: Edição de Posts (5 dias)
- [ ] TAREFA 3.2: Edição de Eventos (5 dias)
- [ ] TAREFA 3.3: Gestão de Conteúdo (5 dias)

**Status**: ⏳ **PENDENTE**

---

## 🎯 Instruções para Agente Cursor

### Como Usar Este Plano

1. **Começar pela Fase 1, Tarefa 1.1**
2. **Seguir sequencialmente**: Não pular tarefas
3. **Validar cada tarefa**: Completar checklist antes de prosseguir
4. **Atualizar documentação**: Após cada tarefa concluída
5. **Testar tudo**: Antes de marcar como completo

### Comandos Úteis

```bash
# Criar migration
dotnet ef migrations add NomeDaMigration --project backend/Arah.Infrastructure --startup-project backend/Arah.Api

# Aplicar migration
dotnet ef database update --project backend/Arah.Infrastructure --startup-project backend/Arah.Api

# Rodar testes
dotnet test

# Gerar documentação Swagger
# Já configurado em /swagger

# Verificar compilação
dotnet build
```

### Padrões a Seguir

- **Clean Architecture**: Respeitar camadas (Domain → Application → Infrastructure → API)
- **Result Pattern**: Usar `Result<T>` para operações que podem falhar
- **FluentValidation**: Validações em validators separados
- **Testes**: Criar testes unitários e de integração
- **Migrations**: Sempre criar migrations para mudanças no banco
- **Documentação**: Atualizar Swagger e comentários XML

---

## 📊 Status do Plano

### Progresso Geral

**Fase 1**: 0% ⏳  
**Fase 2**: 0% ⏳  
**Fase 3**: 0% ⏳  

**Total**: 0/61 dias completados

---

## 🎯 Próximo Passo Imediato

**INICIAR**: Fase 1, Tarefa 1.1 - Avatar e Bio no Perfil

**Ação**: Ler `docs/backlog-api/FASE9.md` seção 32.1 e começar implementação do modelo de domínio.

---

**Versão**: 1.0  
**Criado em**: 2025-01-20  
**Status**: ✅ **PLANO PRONTO PARA EXECUÇÃO**

**Boa sorte! 🚀**
