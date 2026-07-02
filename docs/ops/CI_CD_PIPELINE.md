# Pipeline CI/CD — FASE52 + FASE54

**Status**: FASE52 ✅ · FASE54 código entregue (config ops: [PILOT_STAGING_CONFIG_TODO.md](./PILOT_STAGING_CONFIG_TODO.md))  
**Dono**: Release / DevOps Agent  
**Handoff**: [Plano Operacional](../handoff/arquitetura-c4/Plano%20Operacional%20e%20Pipelines%20-%20Arah.dc.html)

---

## Fluxo

```
PR → CI (build + test) → merge main
  → CD (Docker → GHCR + smoke /health)
  → Deploy Staging (automático: health + verify-pilot-instance FASE54)
  → Deploy Production (manual, environment production + confirm DEPLOY)
```

| Workflow | Gatilho | Entrega |
|----------|---------|---------|
| [ci.yml](../../.github/workflows/ci.yml) | PR + push main | Build, testes, Trivy, Codecov |
| [cd.yml](../../.github/workflows/cd.yml) | push main, tags | Imagens GHCR `arah-api`, `arah-bff`, app artifacts |
| [deploy-staging.yml](../../.github/workflows/deploy-staging.yml) | CD concluído | Stack staging + `/health` + **FASE54** `verify-pilot-instance.ps1` |
| [deploy-production.yml](../../.github/workflows/deploy-production.yml) | `workflow_dispatch` | Gate humano; verificação antes de prod real (FASE54) |

---

## Imagens GHCR

| Imagem | Tag |
|--------|-----|
| `ghcr.io/sraphaz/arah/arah-api` | `latest`, `{sha}` |
| `ghcr.io/sraphaz/arah/arah-bff` | `latest`, `{sha}` |

---

## Ambientes GitHub

Configure em **Settings → Environments**:

| Environment | Uso | Proteção sugerida |
|-------------|-----|-------------------|
| `staging` | Deploy automático pós-CD | Nenhuma (CI) |
| `production` | Promoção manual | Required reviewers |

---

## Secrets

| Secret | Onde | Obrigatório |
|--------|------|-------------|
| `JWT__SIGNINGKEY` | staging/production | Recomendado (≥32 chars prod) |
| `STRIPE__SECRETKEY` | staging | Opcional — PSP sandbox FASE54 (`sk_test_...`) |
| `GITHUB_TOKEN` | CI/CD | Automático |
| `CODECOV_TOKEN` | CI | Opcional |

Detalhes e checklist: [PILOT_STAGING_CONFIG_TODO.md](./PILOT_STAGING_CONFIG_TODO.md).

**Nunca** commitar secrets. Fallback apenas em CI de smoke quando secret ausente.

---

## Staging local

```powershell
$env:ARAH_API_IMAGE = "ghcr.io/sraphaz/arah/arah-api:latest"
$env:ARAH_BFF_IMAGE = "ghcr.io/sraphaz/arah/arah-bff:latest"
$env:JWT__SIGNINGKEY = "dev-only-change-me-min-32-chars"
docker compose -f docker-compose.staging.yml up -d
curl http://localhost:8080/health
curl http://localhost:5001/health
./scripts/provision/verify-pilot-instance.ps1 -SkipStripeCheck
```

### Piloto local (compose dedicado)

```powershell
./scripts/provision/provision-pilot-instance.ps1 -SkipStripeCheck
# ou com HTTPS:
./scripts/provision/provision-pilot-instance.ps1 -WithHttps -SkipStripeCheck
```

Ver [infrastructure/pilot/README.md](../../infrastructure/pilot/README.md).

---

## Critérios FASE52

- [x] PR → build + testes (CI)
- [x] Merge main → imagem GHCR versionada
- [x] Staging deploy automático + health
- [x] FASE54 — verify piloto no CI (`verify-pilot-instance.ps1`)
- [x] Prod → gate manual (`Deploy Production`, confirm=`DEPLOY`)
- [x] Secrets documentados (este arquivo + PILOT_STAGING_CONFIG_TODO)
- [ ] Observabilidade baseline (Prometheus/OTel) — próximo incremento FASE52
- [ ] Deploy físico multi-instância cloud — pós-piloto (Terraform/Helm alvo)

---

## Referências

- [FASE52.md](../backlog-api/FASE52.md)
- [FASE54.md](../backlog-api/FASE54.md)
- [PILOT_STAGING_CONFIG_TODO.md](./PILOT_STAGING_CONFIG_TODO.md)
- [AGENT_OPERATION.md](./AGENT_OPERATION.md)
- [release.checklist.md](../../.agents/checklists/release.checklist.md)
