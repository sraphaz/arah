# TODO — Configuração manual (piloto / staging)

**Dono**: humano (ops)  
**Código relacionado**: FASE54 — IaC e 1ª instância · FASE55 — PSP/comercial (quando aplicável)  
**Quando fizer**: antes de fechar issue FASE54 (#389); Stripe antes de validar PSP no CI

> Tudo abaixo é **configuração externa** — não commitar secrets no repo.  
> Documentação técnica: [FASE54.md](../backlog-api/FASE54.md) · [CI_CD_PIPELINE.md](./CI_CD_PIPELINE.md) · [infrastructure/pilot/README.md](../../infrastructure/pilot/README.md)
---

## GitHub — environment `staging`

| Item | Secret / variável | Onde | Status |
|------|-------------------|------|--------|
| JWT assinatura | `JWT__SIGNINGKEY` | Settings → Environments → staging → Secrets | ⬜ |
| Stripe sandbox | `STRIPE__SECRETKEY` (`sk_test_...`) | idem | ⬜ |
| Stripe webhook (opcional staging) | `Stripe__WebhookSecret` (`whsec_...`) | idem ou env no compose | ⬜ |

**Validação após configurar:**

```powershell
# Disparar workflow CD → Deploy Staging (ou push main)
# Confirmar passo "FASE54 — verify pilot instance" verde
# Com Stripe: remover -SkipStripeCheck no CI (já automático se secret existir)
```

---

## Stripe — conta sandbox

| Item | Ação | Status |
|------|------|--------|
| Conta test mode | [dashboard.stripe.com](https://dashboard.stripe.com/test/apikeys) | ⬜ |
| API key | Copiar `sk_test_...` → secret `STRIPE__SECRETKEY` | ⬜ |
| Verificação local | `$env:STRIPE__SECRETKEY='sk_test_...'; ./scripts/provision/verify-stripe-sandbox.ps1` | ⬜ |
| Webhook endpoint (futuro) | `POST /api/v1/webhooks/stripe` apontando para staging público | ⬜ |

---

## Piloto local (Docker)

| Item | Comando / nota | Status |
|------|----------------|--------|
| Subir stack | `docker compose -f infrastructure/pilot/docker-compose.pilot.yml up -d --build` | ⬜ |
| Verificar Core + JWT | `./scripts/provision/provision-pilot-instance.ps1 -SkipStripeCheck` | ⬜ |
| HTTPS (opcional) | `docker compose -f infrastructure/pilot/docker-compose.pilot.yml --profile https up -d` | ⬜ |
| Backup inicial | `./scripts/provision/backup-pilot-db.ps1 -ContainerName pilot-postgres-1` | ⬜ |

---

## Produção / RPO ≤ 15 min

| Item | Ação | Status |
|------|------|--------|
| Agendar backup | CronJob K8s ou cron host: `pg_dump` a cada **≤ 15 min** | ⬜ |
| Retenção | Política de retenção (ex.: 7 dias rolling) | ⬜ |
| Restore test | Restaurar dump em ambiente efêmero e validar | ⬜ |

---

## GitHub Project (#3)

| Item | Comando | Status |
|------|---------|--------|
| Sync Status | `./scripts/agents/github-project.ps1 sync-status` | ⬜ |
| Export snapshot | `./scripts/agents/export-phase-status.ps1` | ⬜ |
| Fechar FASE54 | Após CI verde + checklist acima → `PHASE_ROADMAP_META.yaml` + `close-phases` | ⬜ |

---

## Fechar FASE54 (checklist final)

- [ ] Deploy Staging CI verde (health + `verify-pilot-instance`)
- [ ] `STRIPE__SECRETKEY` configurado e passo Stripe verde (ou aceite explícito de adiar PSP)
- [ ] Backup manual testado uma vez (`backup-pilot-db.ps1`)
- [ ] Issue **#389** fechada
- [ ] `docs/_meta/PHASE_ROADMAP_META.yaml` → adicionar `FASE54` em `completed`
- [ ] Spec `FASE54-iac.spec.yaml` → `status: completed`

---

## Referências

- [infrastructure/pilot/README.md](../../infrastructure/pilot/README.md)
- [FASE54.md](../backlog-api/FASE54.md)
- [CI_CD_PIPELINE.md](./CI_CD_PIPELINE.md)
