# Fase 54: Infraestrutura como Código e 1ª Instância

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🔴 P0  
**Onda**: S0 — Fundação técnica  
**Depende de**: FASE52, FASE53  
**Estimativa Total**: 112 horas  
**Status**: 🟡 Código entregue — config manual pendente ([PILOT_STAGING_CONFIG_TODO.md](../ops/PILOT_STAGING_CONFIG_TODO.md))
**Spec SDD**: [docs/specs/phases/FASE54-iac.spec.yaml](../specs/phases/FASE54-iac.spec.yaml)  
**Handoff**: [Plano Operacional P3–P7](../handoff/arquitetura-c4/Plano%20Operacional%20e%20Pipelines%20-%20Arah.dc.html)

---

## Objetivo

Provisionar a **primeira instância gerenciada** (território-piloto) via IaC: Terraform + Helm, com stack completo (API, PostgreSQL, storage, realtime, workers, PSP sandbox).

---

## Repositórios (organização alvo)

| Repo | Stack | Responsabilidade |
|------|-------|------------------|
| `arah.api` | .NET | API REST /v1, billing, carteira |
| `arah.app` | Flutter | App mobile |
| `arah.cockpit` | Web | Painel implementador |
| `arah.core` | serviço | Registro, federação, ledger |
| `arah.infra` | IaC | Terraform + Helm |
| `arah.portal` | Next.js | Site institucional |

*Nota: monorepo atual (`sraphaz/arah`) evolui para esta organização gradualmente.*

---

## Pipeline: provisionar instância

1. Solicitação no cockpit (futuro) ou `./scripts/provision/provision-pilot-instance.ps1`
2. `docker compose` piloto ou staging (GHCR) — ver [infrastructure/pilot/README.md](../../infrastructure/pilot/README.md)
3. Registrar no Core (FASE53) — automático em `verify-pilot-instance.ps1`
4. Migrar schema + seed território (`Persistence__ApplyMigrations=true`)
5. Health check verde → publicar no app

### Scripts (`scripts/provision/`)

| Script | Função |
|--------|--------|
| `provision-pilot-instance.ps1` | Sobe compose + verificação completa |
| `verify-pilot-instance.ps1` | Health → JWT → Core → heartbeat → Online |
| `get-pilot-admin-token.ps1` | JWT SystemAdmin (social login piloto) |
| `register-core-instance.ps1` | Registro manual no Core |
| `backup-pilot-db.ps1` | `pg_dump` inicial (RPO ≤ 15 min doc) |
| `verify-stripe-sandbox.ps1` | Balance API Stripe (`STRIPE__SECRETKEY`) |

### Variáveis de ambiente (piloto/staging)

| Variável | Uso |
|----------|-----|
| `Pilot__BootstrapAdminEnabled` | `true` — cria admin Postgres na 1ª subida |
| `JWT__SIGNINGKEY` | Assinatura JWT (≥32 chars em prod) |
| `STRIPE__SECRETKEY` | PSP sandbox (opcional no CI) |
| `Persistence__Provider` | `Postgres` no piloto |
| `Persistence__ApplyMigrations` | `true` — aplica migrações no start |

### CI (deploy staging)

Workflow [deploy-staging.yml](../../.github/workflows/deploy-staging.yml): após health API+BFF, executa `verify-pilot-instance.ps1` (passo **FASE54**).

---

## Modos de hospedagem

| Modo | Quem opera infra | Caminho |
|------|------------------|---------|
| **Gerenciada** | Plataforma | Recomendado — piloto |
| **Soberana** | Implementador | Onda S3 (FASE59) |

---

## Critérios de aceite

- [x] Compose piloto reproduz stack (`infrastructure/pilot/docker-compose.pilot.yml`)
- [x] Script registra instância no Core (`scripts/provision/register-core-instance.ps1`)
- [x] Verificação piloto (`scripts/provision/verify-pilot-instance.ps1`) integrada ao deploy staging
- [x] Bootstrap SystemAdmin Postgres (`Pilot__BootstrapAdminEnabled`)
- [x] Backup inicial documentado (`scripts/provision/backup-pilot-db.ps1`, RPO ≤ 15 min)
- [x] JWT funcional em staging (verify-pilot-instance + CI)
- [x] HTTPS opcional via Caddy (`docker compose --profile https`)
- [ ] PSP em sandbox conectado → ver [PILOT_STAGING_CONFIG_TODO.md](../ops/PILOT_STAGING_CONFIG_TODO.md)

---

## Configuração manual (TODO ops)

Toda configuração externa (secrets GitHub, Stripe, backup agendado, fechar issue) está centralizada em **[docs/ops/PILOT_STAGING_CONFIG_TODO.md](../ops/PILOT_STAGING_CONFIG_TODO.md)**.

---

## Referências

- [FASE58](./FASE58.md) · [scripts/run-local-stack.ps1](../../scripts/run-local-stack.ps1)
