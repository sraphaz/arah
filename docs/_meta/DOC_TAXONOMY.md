# Taxonomia de documentação (alvo)

**Status**: migração gradual (PR 3 do handoff agentes)  
**Dono**: Docs Steward  
**Regra**: uma fonte de verdade por tema; legado numérico → `docs/_archive/`

---

## Estrutura alvo

```
docs/
├── INDEX.generated.md      # gerado por scripts/agents/generate-docs-index.ps1
├── product/                # visão, roadmap, backlog resumido
├── architecture/           # C4, ADRs, serviços
├── backend/                # API, domínio, BFF
├── frontend/
│   ├── flutter/
│   └── web/
├── ops/                    # CI, agentes, runbooks
├── governance/             # moderação, votação, LGPD
├── decisions/              # ADRs (links ou cópias)
├── handoff/                # HTML interativos de handoff
├── backlog-api/            # FASE*.md (mantido até migração completa)
├── _archive/               # docs legados numerados
└── _meta/                  # este arquivo, mapas, config fases
```

---

## Front-matter obrigatório (source of truth)

```yaml
---
title: Título legível
owner: backend | flutter | web | docs-steward | platform
status: active | draft | deprecated
updated: YYYY-MM-DD
source_of_truth: true
---
```

---

## Regras

1. **kebab-case** semântico — sem prefixos `30_`, `38_` que colidem.
2. Duplicata → link ou `_archive/`; nunca duas fontes de verdade.
3. `INDEX.generated.md` regenerado pelo Docs Steward (skill `doc-taxonomy`).
4. Wiki/devportal continuam consumindo `docs/` — paths estáveis durante migração.

---

## Mapeamento inicial (não destrutivo)

| Legado | Alvo futuro |
|--------|-------------|
| `docs/01_PRODUCT_VISION.md` | `docs/product/vision.md` |
| `docs/02_ROADMAP.md` | `docs/product/roadmap.md` |
| `docs/10_ARCHITECTURE_DECISIONS.md` | `docs/decisions/` |
| `docs/12_DOMAIN_MODEL.md` | `docs/backend/domain-model.md` |
| `docs/backlog-api/` | mantido + links em `docs/product/backlog.md` |
| Handoff C4 | `docs/handoff/` ✅ |

**Não mover em massa sem PR dedicado do Docs Steward.**
