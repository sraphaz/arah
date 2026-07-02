# Piloto — 1ª instância gerenciada (FASE54)

Stack mínimo para reproduzir instância do zero em ambiente local/staging.

## Configuração manual

Secrets, Stripe, backup agendado e fechamento da issue: **[docs/ops/PILOT_STAGING_CONFIG_TODO.md](../../docs/ops/PILOT_STAGING_CONFIG_TODO.md)**

## Uso rápido

```powershell
./scripts/provision/provision-pilot-instance.ps1 -SkipStripeCheck
```

Ou passo a passo:

```powershell
docker compose -f infrastructure/pilot/docker-compose.pilot.yml up -d --build
./scripts/provision/verify-pilot-instance.ps1 `
  -ApiBaseUrl http://localhost:8080 `
  -InstanceBaseUrl http://localhost:8080 `
  -SkipStripeCheck
```

Com Stripe sandbox (secret fora do repo):

```powershell
$env:STRIPE__SECRETKEY = 'sk_test_...'
docker compose -f infrastructure/pilot/docker-compose.pilot.yml up -d --build
./scripts/provision/verify-pilot-instance.ps1 -ApiBaseUrl http://localhost:8080 -InstanceBaseUrl http://localhost:8080
```

## HTTPS (opcional)

Perfil `https` com Caddy + certificado interno:

```powershell
docker compose -f infrastructure/pilot/docker-compose.pilot.yml --profile https up -d --build
./scripts/provision/verify-pilot-instance.ps1 `
  -ApiBaseUrl http://localhost:8080 `
  -InstanceBaseUrl https://localhost:8443 `
  -RequireHttps `
  -HttpsBaseUrl https://localhost:8443 `
  -SkipStripeCheck
```

## Bootstrap SystemAdmin (Postgres)

Com `Pilot__BootstrapAdminEnabled=true`, a API cria `google/admin-external` com `SystemAdmin` na primeira subida — necessário para registrar instância no Core fora do InMemory.

## Backup (RPO ≤ 15 min)

```powershell
./scripts/provision/backup-pilot-db.ps1 -ContainerName pilot-postgres-1
```

Em produção: agendar `pg_dump` a cada 15 minutos (CronJob / cron).

## Pipeline (ordem de dependência)

1. **FASE52** — CI/CD + imagens GHCR ✅
2. **FASE53** — registro no Core ✅
3. **FASE54** — compose + verify-pilot-instance + deploy-staging CI
4. Seed território Camburi (`POST /api/v1/admin/seed/territories/camburi`)
5. Health check verde → publicar no app

## Scripts

| Script | Função |
|--------|--------|
| `provision-pilot-instance.ps1` | Orquestra compose + verificação completa |
| `get-pilot-admin-token.ps1` | JWT via social login piloto |
| `register-core-instance.ps1` | Registro manual no Core |
| `verify-pilot-instance.ps1` | Health + JWT + Core + heartbeat |
| `verify-stripe-sandbox.ps1` | Balance API Stripe sandbox |
| `backup-pilot-db.ps1` | pg_dump inicial |

## Referências

- [FASE54.md](../../docs/backlog-api/FASE54.md)
- [FASE53 — Arah Core](../../docs/backlog-api/FASE53.md)
- [PILOT_STAGING_CONFIG_TODO.md](../../docs/ops/PILOT_STAGING_CONFIG_TODO.md)
- [CI_CD_PIPELINE.md](../../docs/ops/CI_CD_PIPELINE.md)
