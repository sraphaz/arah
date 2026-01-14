# System Config e Work Queue (P0)

Este documento descreve a base **P0** implementada para:

- **SystemConfig**: configurações globais calibráveis gerenciadas por `SystemAdmin`.
- **Work Queue**: fila genérica de itens de trabalho para suportar **automação + fallback humano** (curadoria/moderação/verificações).

> Nota: **SystemConfig não armazena segredos** (senhas, API keys). Apenas valores calibráveis e seleção de provider.
> Segredos devem permanecer em variáveis de ambiente/secret manager.

---

## 1) SystemConfig

### Objetivo
Centralizar configurações globais que hoje ficam espalhadas (thresholds, janelas, toggles, providers, etc.), com:

- auditoria (`system_config.created` / `system_config.updated`)
- cache leve (5 min) com invalidação em update

### Endpoints

- **Listar configs** (filtro opcional por categoria):
  - `GET /api/v1/admin/system-configs?category=MODERATION|VALIDATION|ASSETS|PROVIDERS|OBSERVABILITY|SECURITY|OTHER`

- **Obter por key**:
  - `GET /api/v1/admin/system-configs/{key}`

- **Criar/atualizar (upsert)**:
  - `PUT /api/v1/admin/system-configs/{key}`
  - Body:
    - `value` (string)
    - `category` (string)
    - `description` (string, opcional)

### Permissão
- Requer `SystemPermissionType.SystemAdmin`.

---

## 2) Work Queue (Work Items)

### Objetivo
Representar uma fila genérica para tarefas que podem começar automáticas e, quando necessário, cair em revisão humana:

- verificação de identidade (global)
- verificação de residência (territorial)
- curadoria de assets (territorial)
- casos de moderação (territorial)

Nesta fase P0, o foco é a **infra**: persistência, listagem e conclusão manual com auditoria.

### Endpoints (Admin)

- **Listar work items (global)**:
  - `GET /api/v1/admin/work-items?type=&status=`
  - `type`: `IDENTITYVERIFICATION|RESIDENCYVERIFICATION|ASSETCURATION|MODERATIONCASE|OTHER`
  - `status`: `PENDING|AUTOPROCESSED|REQUIRESHUMANREVIEW|COMPLETED|CANCELLED`

- **Completar work item (global)**:
  - `POST /api/v1/admin/work-items/{workItemId}/complete`
  - Body:
    - `outcome` (string): `APPROVED|REJECTED|NOACTION`
    - `notes` (string, opcional)

### Endpoints (Território)

- **Listar work items do território**:
  - `GET /api/v1/territories/{territoryId}/work-items?type=&status=`
  - Permissão: `Curator` **ou** `Moderator` no território (SystemAdmin também passa).

- **Completar work item do território**:
  - `POST /api/v1/territories/{territoryId}/work-items/{workItemId}/complete`
  - Permissão:
    - se o item exige `RequiredSystemPermission`: precisa dessa `SystemPermission`
    - se o item exige `RequiredCapability`: precisa dessa `MembershipCapability`
    - caso contrário: `Curator` **ou** `Moderator`

### Auditoria
- Criação: `work_item.created`
- Conclusão: `work_item.completed`

---

## Próximos passos (P1/P2)

- **P1 (Verificações)**: usar Work Items para fila de identidade e residência com evidências (upload/asset).
- **P1 (Assets)**: manter validação comunitária por Resident, e adicionar decisão final por Curator (estado/curation status).
- **P2 (Moderação)**: alimentar Work Items a partir de reports/blocks, com fallback para Moderator e (futuro) triagem por IA.

---

## 3) Verificações (P1 - já com endpoints iniciais)

### Submissão (usuário)
- **Identidade (global)**:
  - `POST /api/v1/verification/identity/document`
  - Body: `{"documentRef":"..."}`
  - Resultado: retorna `workItemId` (vai para revisão humana por SystemAdmin).

- **Identidade (global) com upload**:
  - `POST /api/v1/verification/identity/document/upload` (multipart/form-data)
  - Form: `file`
  - Resultado: retorna `evidenceId` e `workItemId`.

- **Residência (territorial)**:
  - `POST /api/v1/memberships/{territoryId}/verify-residency/document`
  - Body: `{"documentRef":"..."}`
  - Resultado: retorna `workItemId` (vai para revisão humana por Curator do território).

- **Residência (territorial) com upload**:
  - `POST /api/v1/memberships/{territoryId}/verify-residency/document/upload` (multipart/form-data)
  - Form: `file`
  - Resultado: retorna `evidenceId` e `workItemId`.

### Decisão (humano)
- **Identidade (SystemAdmin)**:
  - `POST /api/v1/admin/verifications/identity/{workItemId}/decide`
  - Body: `{"outcome":"APPROVED|REJECTED","notes":"..."}`

- **Residência (Curator)**:
  - `POST /api/v1/territories/{territoryId}/verifications/residency/{workItemId}/decide`
  - Body: `{"outcome":"APPROVED|REJECTED","notes":"..."}`

---

## 4) Assets (P1 - curadoria + fila)

### Comportamento
- Ao criar um `TerritoryAsset`, o status inicial passa a ser **`SUGGESTED`** e um Work Item de **`ASSETCURATION`** é criado para curadoria humana.
- **Resident validado** continua podendo registrar validações (`/api/v1/assets/{assetId}/validate`), como sinal comunitário.
- **Curator** faz a decisão final de curadoria.

### Endpoint de curadoria (Curator)
- `POST /api/v1/assets/{assetId}/curate?territoryId=...`
- Body: `{"outcome":"APPROVED|REJECTED","notes":"..."}`

