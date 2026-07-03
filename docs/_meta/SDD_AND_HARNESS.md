# SDD + Harness Engineering

**Versão**: 1.0  
**Data**: 2026-07-01

---

## Objetivo

Tornar o repositório **governável** (specs como contrato) e **produtivo** (harness reproduzível para humanos, CI e agentes).

| Pilar | Artefato |
|-------|----------|
| **Spec-Driven Design** | `docs/specs/*.spec.yaml` |
| **Harness Engineering** | `scripts/harness/run-harness.ps1` + workflow `spec-harness.yml` |
| **Agentes** | Spec Steward, checklists, `harness` CLI |

---

## Harness layers

```
┌─────────────────────────────────────────┐
│  CI (spec-harness.yml)                  │
├─────────────────────────────────────────┤
│  run-harness.ps1 → validate-specs       │
│                 → agent manifests       │
│                 → spec-linked tests     │
├─────────────────────────────────────────┤
│  Agentes (Cursor + agents.yml)          │
├─────────────────────────────────────────┤
│  Specs YAML (fonte de aceite)           │
└─────────────────────────────────────────┘
```

---

## Comandos

```powershell
./scripts/harness/validate-specs.ps1
./scripts/agents/arah-agents.ps1 harness
./scripts/agents/arah-agents.ps1 harness -SpecId platform-operability
./scripts/agents/arah-agents.ps1 spec-validate
```

---

## Gate em PR

- Alteração em `docs/specs/` → `validate-specs` obrigatório
- Alteração de código sem spec `active` para a fase → warning em `sync-docs-check` (features críticas)
- Specs `draft` (spec-before-code): o harness valida estrutura (agents, guardrails, links),
  mas **pula** `scripts`/`commands` — passam a executar quando a spec vira `active`

---

## Agent Graph

O grafo operacional ([AGENT_GRAPH.md](../ops/AGENT_GRAPH.md)) espelha as specs e
o harness: cada spec vira nó `spec:` com `requires_harness`; `harness.agents`
gera `validated_by`; e os guardrails executáveis de `run-harness.ps1`
(`clean-architecture`, `territory-data-stays-on-instance`, …) viram
`enforced_by_workflow` → `spec-harness.yml`. Gerar/validar:

```powershell
./scripts/agents/export-agent-graph.ps1
./scripts/harness/validate-agent-graph.ps1
```

---

## Referências

- [docs/specs/README.md](../specs/README.md)
- [PLATFORM_STATE.md](../ops/PLATFORM_STATE.md)
- [AGENT_OPERATION.md](../ops/AGENT_OPERATION.md)
- [AGENT_GRAPH.md](../ops/AGENT_GRAPH.md)
