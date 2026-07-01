# Fase 52: Fundação Técnica e CI/CD

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🔴 P0 (Bloqueador de produção)  
**Onda**: S0 — Fundação técnica  
**Depende de**: Fases 1–4 (observabilidade, testes)  
**Estimativa Total**: 112 horas  
**Status**: 🟡 Em progresso  
**Handoff**: [Plano Operacional](../handoff/arquitetura-c4/Plano%20Operacional%20e%20Pipelines%20-%20Arah.dc.html)

---

## Objetivo

Estabelecer a cadeia de entrega do código ao ambiente executável: pipelines CI/CD, registry de imagens, ambientes dev/staging e gestão de segredos — pré-requisito para qualquer território em produção.

---

## Entregas

### Pipeline de aplicação (app · api · cockpit)

1. PR → lint + testes automatizados
2. Build → imagem Docker → GHCR
3. Deploy automático em **staging**
4. Gate manual → promoção **prod**

### Ambientes

| Ambiente | Uso | Promoção |
|----------|-----|----------|
| dev | Branch/PR, dados sintéticos | Automático por PR |
| staging | Integração, e2e, sandbox PSP | Verde nos testes |
| prod | Multi-instância por território | Aprovação + gate backup |

### Ferramentas (referência)

- GitHub Actions, GHCR, GitHub Secrets
- Prometheus + Grafana + OpenTelemetry (baseline)
- PostgreSQL gerenciado, Object Storage + CDN

---

## Critérios de aceite

- [x] Todo PR dispara build + testes; merge bloqueado se falhar (CI + branch protection recomendado)
- [x] Imagem versionada publicada no GHCR a cada merge em main
- [x] Staging recebe deploy automático (`deploy-staging.yml`); prod exige gate (`deploy-production.yml`)
- [x] Secrets não aparecem em logs nem no repositório — ver [CI_CD_PIPELINE.md](../ops/CI_CD_PIPELINE.md)
- [x] Health check `/health` respondendo em staging (API + BFF)
- [ ] Observabilidade baseline (Prometheus/OTel) — incremento restante

---

## Referências

- [REALINHAMENTO_SUSTENTACAO_OPERACIONAL](./REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)
- [FASE53](./FASE53.md) · [FASE54](./FASE54.md)
