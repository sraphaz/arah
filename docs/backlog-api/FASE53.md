# Fase 53: Arah Core — Registro, Identidade e Releases

**Duração**: 5 semanas (35 dias úteis)  
**Prioridade**: 🔴 P0  
**Onda**: S0 — Fundação técnica  
**Depende de**: FASE52  
**Estimativa Total**: 140 horas  
**Status**: ✅ Completo (2026-07-01)  
**Spec SDD**: [docs/specs/phases/FASE53-arah-core.spec.yaml](../specs/phases/FASE53-arah-core.spec.yaml)
**Handoff**: [Anexo Operação O1–O7](../handoff/arquitetura-c4/Anexo%20Handoff%20-%20Operacao%20Instancias%20e%20Federacao.dc.html)

---

## Objetivo

Implementar o **Arah Core** mínimo: hub que coordena instâncias sem possuir dados de território — registro, identidade federada, catálogo de releases e agregação de telemetria/ledger.

---

## Responsabilidades do Core

- Registro e descoberta de instâncias e territórios (diretório)
- Identidade federada (`globalUserId`) — login entre territórios
- Catálogo de releases e canais (stable/beta)
- Agregação de health/telemetria das instâncias
- Consolidação do ledger global de splits e payouts

---

## Entidades (control plane)

| Entidade | Escopo | Campos principais |
|----------|--------|-------------------|
| `Instance` | Core + instância | id, mode, baseUrl, publicKey, version, territoryIds[], status |
| `Release` | Core | id, version, channel, schemaVersion, publishedAt |
| `HealthCheck` | Core + instância | instanceId, services{}, uptime, reportedAt |

---

## Endpoints (mínimo)

```
POST /core/instances              — provisionar/registrar instância
GET  /core/instances              — listar instâncias (telemetria agregada)
POST /core/instances/{id}/heartbeat — telemetria
GET  /core/releases?channel=stable — releases disponíveis
POST /core/directory/territories  — publicar território no diretório
GET  /federation/identity/{globalUserId} — resolver identidade (base)
```

Autenticação: token de instância (`X-Arah-Instance-Token` no heartbeat) + SystemAdmin para operações admin.

---

## Critérios de aceite

- [x] Instância registra-se e recebe id + par de chaves RSA (private key só na resposta de registro)
- [x] Heartbeat a cada 30s; status visível no Core (listagem + telemetria via GET /core/instances)
- [x] Release publicada via `POST /core/releases` (tag/versão); instâncias consultam canal stable
- [x] Território publicado no diretório global (`POST /core/directory/territories`)
- [x] Identidade federada resolvível (`GET /federation/identity/{globalUserId}`)
- [x] Token de instância emitido no registro; heartbeat autenticado sem JWT admin
- [x] Cache local (`ICoreAvailabilityCache`) para releases/registro — base para modo autônomo (FASE54)

---

## Referências

- [FASE54](./FASE54.md) · [FASE59](./FASE59.md)
