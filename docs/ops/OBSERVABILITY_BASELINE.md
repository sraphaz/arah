# Observabilidade baseline (FASE52)

**Versão**: 1.0  
**Data**: 2026-07-01

Baseline mínimo já presente na API — este doc fixa contrato operacional e smoke CI.

---

## Endpoints

| Path | Auth | Uso |
|------|------|-----|
| `/liveness` | Público | Smoke CD — processo vivo |
| `/health` | Público | Readiness agregado (JSON) |
| `/health/ready` | Público | Checks tag `ready` |
| `/health/live` | Público | Liveness probes |
| `/metrics` | Público | Prometheus text exposition |

---

## Stack

- **Prometheus** — `prometheus-net` (`UseMetricServer`, `UseHttpMetrics`)
- **OpenTelemetry** — traces + metrics; export OTLP se `OpenTelemetry:Otlp:Endpoint` configurado
- **Serilog** — request logging estruturado

Config: `backend/Arah.Api/appsettings.json` → seções `OpenTelemetry`, `Metrics:Prometheus`.

---

## CI/CD

Job `smoke-test-api` em `.github/workflows/cd.yml`:

1. Pull imagem GHCR publicada no merge em `main`
2. Valida `/liveness`
3. Verifica `/metrics` expõe formato Prometheus (`# HELP` ou `process_`)

---

## Próximo incremento (pós-baseline)

- Grafana dashboard + scrape em staging
- Alertas mínimos (health down, error rate)
- BFF expor `/metrics` alinhado à API
- Remover exporter Jaeger legado (migrar 100% OTLP)

---

## Referências

- [CI_CD_PIPELINE.md](./CI_CD_PIPELINE.md)
- [FASE52.md](../backlog-api/FASE52.md)
