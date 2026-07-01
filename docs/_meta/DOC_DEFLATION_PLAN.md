# Plano de desinflação — `docs/`

**Problema**: ~246 arquivos `.md` na raiz de `docs/` (510 no total). Dificulta navegação, duplica fontes de verdade e polui busca/wiki.

**Meta**: raiz de `docs/` com **≤80** arquivos ativos (índices + númerados canônicos + hubs); resto em subpastas semânticas ou `_archive/`.

---

## Três ondas (sem quebrar wiki)

| Onda | O quê | Risco | Comando |
|------|--------|-------|---------|
| **1** | PR, RESUMO, ANALISE, AVALIACAO, planos de sessão → `_archive/` + **stub** no path antigo | Baixo | `./scripts/agents/migrate-docs-deflate.ps1 -Wave 1` |
| **2** | `NN_*.md` numerados → pastas semânticas (`product/`, `architecture/`, …) + stub | Médio | Wave 2 (futuro) |
| **3** | Consolidar duplicatas (CHANGELOG, STRUCTURE vs README) | Médio | Manual + Docs Steward |

**Stub**: arquivo curto no path original com front-matter `redirect:` e link — wiki e links antigos continuam resolvendo.

---

## Estrutura `_archive/`

```
docs/_archive/
├── README.md
├── pr-summaries/      # PR_*.md da raiz
├── sessions/          # RESUMO_*
├── analyses/          # ANALISE_*
├── evaluations/       # AVALIACAO_*
└── plans/             # IMPLEMENTACAO_*, REESTRUTURACAO_*, PROPOSAL_*, etc.
```

**Não arquivar**: `00_INDEX.md`, `CHANGELOG.md`, `FASE*.md` (ficam em `backlog-api/`), `ops/`, `handoff/`, `api/`, docs numerados ainda referenciados no índice.

---

## Parar inflação (gate)

Novos arquivos na raiz de `docs/` matching `PR_*`, `RESUMO_*`, `ANALISE_*`, `AVALIACAO_*` → **rejeitados** por `sync-docs-check.ps1`.

**Onde colocar novo conteúdo**:

| Tipo | Destino |
|------|---------|
| Fase / épico | `docs/backlog-api/FASE*.md` |
| ADR | `docs/decisions/` ou `10_ARCHITECTURE_DECISIONS.md` |
| Runbook / ops | `docs/ops/` |
| Análise pontual | Issue/PR GitHub — **não** novo `.md` na raiz |
| Handoff interativo | `docs/handoff/*.html` |

---

## Navegação enxuta (hub)

Entrada única para humanos:

1. [00_INDEX.md](../00_INDEX.md) — mapa curado (≤50 links ativos)
2. [INDEX.generated.md](../INDEX.generated.md) — inventário completo gerado
3. [backlog-api/README.md](../backlog-api/README.md) — fases
4. [ops/AGENT_OPERATION.md](../ops/AGENT_OPERATION.md) — agentes
5. [handoff/README.md](../handoff/README.md) — C4 / sustentação

Regenerar índice: `./scripts/agents/arah-agents.ps1 doc-index`

---

## Referências

- [DOC_TAXONOMY.md](./DOC_TAXONOMY.md)
- [PLANO_REORGANIZACAO_FEDERAL.md](../PLANO_REORGANIZACAO_FEDERAL.md)
- [STRUCTURE.md](../STRUCTURE.md)
