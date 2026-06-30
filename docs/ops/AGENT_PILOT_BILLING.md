# Piloto assistido — Billing / Split (PR 7)

**Versão**: 1.0  
**Data**: 2026-06-30  
**Modo**: Fase 1 Assistido → Fase 2 Copiloto  
**Épico**: FASE15 (monetização) + FASE55 (transparência)

---

## Objetivo

Validar a operação por agentes ponta a ponta em um épico real: **quote com split visível antes do checkout PIX**.

---

## Papéis

| Papel | Agente | Skills |
|-------|--------|--------|
| Orquestração | orchestrator | backlog-to-issue |
| Backend | backend | run-tests, register-bff-journey, sync-docs, open-pr |
| Flutter | flutter | run-tests, gen-l10n, sync-docs, open-pr |
| Docs | docs-steward | sync-docs, doc-taxonomy |
| QA | qa | code-review, run-tests |
| Security | security | dep-audit |
| Consulta domínio | carteira-arata, monetizacao-split | — |

---

## Sequência do piloto

1. Abrir issue com template **Agent Pilot — Billing** (`.github/ISSUE_TEMPLATE/agent-pilot-billing.yml`)
2. Orquestrador roteia → `area/backend` primeiro (contrato quote/split)
3. Backend agent abre PR → gates (`agents-gates.yml`) + QA comment
4. Issue filha ou checklist → `area/flutter` (UI split no checkout)
5. docs-steward valida CHANGELOG + FASE15 + API doc
6. Humano aprova merge de cada PR

---

## Critérios de aceite (épico)

- [ ] Morador vê split (plataforma / território / prestador) **antes** de confirmar PIX
- [ ] Endpoint documentado em `docs/60_API_LÓGICA_NEGÓCIO.md`
- [ ] BFF journey registrada se exposta via `/bff/journeys`
- [ ] Testes backend + widget test Flutter
- [ ] `sync-docs-check` sem avisos críticos
- [ ] Métricas registradas (ver abaixo)

---

## Métricas a medir

| Métrica | Como medir |
|---------|------------|
| Lead time issue → PR | Timestamps GitHub |
| % PRs abertos por agente | Labels + corpo PR |
| Retrabalho pós-review | Comments "changes requested" |
| Doc-freshness | `sync-docs-check` warnings = 0 |

---

## Comandos locais

```powershell
./scripts/agents/arah-agents.ps1 orchestrate -Labels area/backend
./scripts/agents/invoke-skill.ps1 -Skill run-tests -Area backend
./scripts/agents/run-gates.ps1 -Gate all
./scripts/agents/open-pr.ps1 -Agent backend -Title "feat(billing): quote split preview" -Issue <N>
```

---

## Referências

- [AGENT_OPERATION.md](AGENT_OPERATION.md)
- [FASE15.md](../backlog-api/FASE15.md)
- [FASE55.md](../backlog-api/FASE55.md)
- [.agents/domain/monetizacao-split.agent.yaml](../../.agents/domain/monetizacao-split.agent.yaml)
