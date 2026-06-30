# Fase 54: Infraestrutura como Código e 1ª Instância

**Duração**: 4 semanas (28 dias úteis)  
**Prioridade**: 🔴 P0  
**Onda**: S0 — Fundação técnica  
**Depende de**: FASE52, FASE53  
**Estimativa Total**: 112 horas  
**Status**: ⏳ Pendente  
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

1. Solicitação no cockpit (futuro) ou script manual
2. `terraform apply` — recursos cloud
3. Registrar no Core (FASE53)
4. Migrar schema + seed território
5. Health check verde → publicar no app

---

## Modos de hospedagem

| Modo | Quem opera infra | Caminho |
|------|------------------|---------|
| **Gerenciada** | Plataforma | Recomendado — piloto |
| **Soberana** | Implementador | Onda S3 (FASE59) |

---

## Critérios de aceite

- [ ] IaC reproduz instância do zero em staging
- [ ] 1ª instância piloto verde no Core
- [ ] Backup inicial verificado (RPO ≤ 15 min)
- [ ] API acessível via HTTPS; JWT funcional
- [ ] PSP em sandbox conectado

---

## Referências

- [FASE58](./FASE58.md) · [scripts/run-local-stack.ps1](../../scripts/run-local-stack.ps1)
