# System Config, Work Queue e Evidências (Admin)

Este documento descreve os componentes “admin” introduzidos para suportar **configurações calibráveis**, **filas de revisão humana** e **evidências documentais**, preservando o modelo atual (User-Centric Membership) e o princípio de governança mínima por território.

## Objetivos (P0)

- Centralizar configurações calibráveis do sistema em **SystemConfig** (auditável).
- Padronizar filas de revisão humana em um modelo genérico **WorkItem** (Work Queue).
- Implementar verificação documental:
  - **Identidade (global)** com fallback para **SystemAdmin**
  - **Residência (territorial)** com fallback para **Curator** do território
- Permitir armazenamento de evidências fora do banco (arquivo) + metadados no banco (**DocumentEvidence**).
- Habilitar **download por proxy** (API faz o streaming do storage), evitando URLs públicas ou pré-assinadas neste estágio.

---

## Papéis e permissões

- **SystemAdmin** (via `SystemPermissionType.SystemAdmin`)
  - Gerencia `SystemConfig`
  - Decide verificação de **identidade**
  - Pode acessar/baixar evidências via endpoints admin
- **Curator** (via `MembershipCapabilityType.Curator`, territorial)
  - Decide verificação de **residência** no território
  - Decide curadoria de **assets** sugeridos no território
  - Pode listar/completar WorkItems territoriais
- **Moderator** (via `MembershipCapabilityType.Moderator`, territorial)
  - Decide casos de moderação (WorkItem `MODERATIONCASE`)
  - Pode listar/completar WorkItems territoriais (quando aplicável)
- **Resident validado**
  - Continua habilitado para validações comunitárias (onde o modelo prevê), mas a “decisão final” do fluxo é do Curator quando existir curadoria.

---

## SystemConfig

### O que é
`SystemConfig` é uma configuração **global** (não territorial) composta por:
- `Key` (string única)
- `Value` (string)
- `Category` (ex.: Providers, Security, Moderation, Validation, etc.)
- `Description` (opcional)
- metadados de auditoria (created/updated por quem e quando)

### Endpoints (SystemAdmin)
- `GET /api/v1/admin/system-config`
- `GET /api/v1/admin/system-config/{key}`
- `PUT /api/v1/admin/system-config`

> Todas as alterações são auditadas (`IAuditLogger`).

---

## Work Queue (WorkItem)

### O que é
`WorkItem` representa um trabalho pendente de revisão/ação humana, com:
- `Type` (ex.: `IdentityVerification`, `ResidencyVerification`, `AssetCuration`, `ModerationCase`)
- `Status` (`Open`, `RequiresHumanReview`, `Completed`, `Cancelled`)
- `Outcome` (`Approved`, `Rejected`, `NoAction`, `None`)
- `TerritoryId` (opcional, para itens globais fica `null`)
- requisitos de autorização:
  - `RequiredSystemPermission` (ex.: SystemAdmin)
  - `RequiredCapability` (ex.: Curator/Moderator)
- `SubjectType` / `SubjectId` (ex.: USER, MEMBERSHIP, REPORT, ASSET)
- `PayloadJson` (campo livre para anexar contexto mínimo)

### Endpoints
**Globais (SystemAdmin):**
- `GET /api/v1/admin/work-items`
- `POST /api/v1/admin/work-items/{workItemId}/complete`

**Por território (Curator/Moderator):**
- `GET /api/v1/territories/{territoryId}/work-items`
- `POST /api/v1/territories/{territoryId}/work-items/{workItemId}/complete`

---

## Evidências documentais (DocumentEvidence)

### O que é
`DocumentEvidence` armazena apenas **metadados**:
- `Id`
- `UserId`
- `TerritoryId` (obrigatório para residência; nulo para identidade)
- `Kind` (`Identity` | `Residency`)
- `StorageProvider` (`Local` | `S3`)
- `StorageKey` (chave/caminho no storage)
- `ContentType`, `SizeBytes`, `Sha256`, `OriginalFileName`, `CreatedAtUtc`

O conteúdo do arquivo fica em `IFileStorage` (local ou S3/MinIO).

### Upload (multipart/form-data)
- **Identidade (global)**:
  - `POST /api/v1/verification/identity/document/upload`
- **Residência (territorial)**:
  - `POST /api/v1/memberships/{territoryId}/verify-residency/document/upload`

### Download por proxy (stream via API)
- **Admin (SystemAdmin)**:
  - `GET /api/v1/admin/evidences/{evidenceId}/download`
- **Território (Curator/Moderator)**:
  - `GET /api/v1/territories/{territoryId}/evidences/{evidenceId}/download`

> O download por proxy permite aplicar autorização/auditoria e não expõe URL pública do storage.

---

## Fluxos implementados

### 1) Verificação de Identidade (global)
1. Usuário faz upload (`/verification/identity/document/upload`)
2. Sistema cria `DocumentEvidence (Kind=Identity)`
3. Sistema cria `WorkItem (Type=IdentityVerification, RequiredSystemPermission=SystemAdmin)`
4. SystemAdmin decide:
   - `POST /api/v1/admin/verifications/identity/{workItemId}/decide` com outcome `APPROVED|REJECTED`

### 2) Verificação de Residência (territorial)
1. Usuário (Resident) faz upload (`/memberships/{territoryId}/verify-residency/document/upload`)
2. Sistema cria `DocumentEvidence (Kind=Residency, TerritoryId=territoryId)`
3. Sistema cria `WorkItem (Type=ResidencyVerification, RequiredCapability=Curator)`
4. Curator decide:
   - `POST /api/v1/territories/{territoryId}/verification/residency/{workItemId}/decide`

### 3) Curadoria de Assets (territorial)
1. Asset é criado como `Suggested`
2. Sistema cria `WorkItem (Type=AssetCuration, RequiredCapability=Curator)`
3. Curator aprova/rejeita via endpoint de curadoria

### 4) Moderação por Reports (territorial)
1. Um report é criado (post/user)
2. Um `WorkItem (Type=ModerationCase)` é enfileirado
3. Curator/Moderator decide:
   - `POST /api/v1/territories/{territoryId}/moderation/cases/{workItemId}/decide`

---

## Storage (Local vs S3/MinIO)

### Interface
`IFileStorage` define operações de:
- `SaveAsync(stream, fileName, contentType)`
- `OpenReadAsync(storageKey)`
- `Provider` (`Local` ou `S3`)

### Configuração S3/MinIO
Para habilitar:
- `Storage:Provider = S3`
- `Storage:S3:Bucket`
- `Storage:S3:Region`
- `Storage:S3:ServiceUrl` (MinIO)
- `Storage:S3:ForcePathStyle = true` (MinIO recomendado)
- `Storage:S3:AccessKey` / `Storage:S3:SecretKey`

---

## Observações

- Este P0 não inclui OCR/IA/provedores de e-mail/SMS — apenas estrutura para calibrar via `SystemConfig` e evoluir.
- A abordagem de download adotada é **proxy** (mais simples, mais controlada). URL pré-assinada fica para uma fase futura.

