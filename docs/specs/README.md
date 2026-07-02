# Spec-Driven Design (SDD) — Arah

**Versão**: 1.0  
**Dono**: Spec Steward + agentes de código  
**Regra**: **Spec antes de código** — todo épico/fase com spec em `docs/specs/` antes de PR de implementação.

---

## Estrutura

```
docs/specs/
├── README.md                 ← este arquivo
├── _template.spec.yaml       ← copiar para nova spec
├── platform/                 ← operabilidade transversal
├── phases/                   ← FASE52–61 e épicos
├── features/                 ← features comunitárias (por domínio)
└── harness/                  ← contratos do harness de agentes/CI
```

---

## Formato (`*.spec.yaml`)

| Campo | Obrigatório | Descrição |
|-------|-------------|-----------|
| `id` | sim | kebab-case único |
| `version` | sim | semver da spec |
| `title` | sim | título legível |
| `owner` | sim | backend \| flutter \| web \| ops \| docs-steward |
| `status` | sim | draft \| active \| deprecated |
| `phase` | não | ex. FASE53 |
| `acceptance` | sim | critérios verificáveis (id + then) |
| `harness` | não | comandos/scripts/testes ligados |
| `links` | não | docs, ADRs, handoff |

---

## Fluxo SDD

```
Spec (YAML) → review → harness verde → implementação → PR → merge
```

1. Criar ou atualizar `*.spec.yaml` no mesmo PR ou PR anterior (docs-only).
2. Rodar `./scripts/agents/arah-agents.ps1 harness -SpecId <id>`.
3. Agente implementa **apenas** o que a spec descreve.
4. PR referencia `Spec-Id: <id>` no corpo.

---

## Harness

Ver [SDD_AND_HARNESS.md](../_meta/SDD_AND_HARNESS.md) e `scripts/harness/run-harness.ps1`.

---

## Specs existentes

| Spec | Fase | Status |
|------|------|--------|
| [FASE52-cicd.spec.yaml](./phases/FASE52-cicd.spec.yaml) | FASE52 | completed |
| [FASE53-arah-core.spec.yaml](./phases/FASE53-arah-core.spec.yaml) | FASE53 | completed |
| [FASE54-iac.spec.yaml](./phases/FASE54-iac.spec.yaml) | FASE54 | active |
| [FASE55-monetization.spec.yaml](./phases/FASE55-monetization.spec.yaml) | FASE55 | active |
| [FASE56-transparency.spec.yaml](./phases/FASE56-transparency.spec.yaml) | FASE56 | draft |
| [FASE57-cockpit.spec.yaml](./phases/FASE57-cockpit.spec.yaml) | FASE57 | draft |
| [FASE58-multi-instance.spec.yaml](./phases/FASE58-multi-instance.spec.yaml) | FASE58 | draft |
| [FASE59-federation.spec.yaml](./phases/FASE59-federation.spec.yaml) | FASE59 | draft |
| [FASE60-implementer-app.spec.yaml](./phases/FASE60-implementer-app.spec.yaml) | FASE60 | draft |
| [FASE61-territorial-capital.spec.yaml](./phases/FASE61-territorial-capital.spec.yaml) | FASE61 | draft |
| [operability.spec.yaml](./platform/operability.spec.yaml) | plataforma | active |
| [agent-operation.spec.yaml](./harness/agent-operation.spec.yaml) | harness | active |

> Specs `draft` (FASE56–61) satisfazem o spec-before-code: passam pelo **clarify**
> (ambiguidades resolvidas com o humano) e viram `active` na abertura da fase.

## Rastreabilidade AC ↔ teste (gate automático)

- Acceptance em formato **EARS**: `when` (gatilho) + `then` (resposta verificável).
- `status: covered` **exige** `covered_by` — referência ao teste que cobre o AC no runner
  da stack do owner (ex.: filtro `dotnet test` no backend, `flutter test` no app,
  `npm test` na web); `status: manual` **exige** `evidence`.
  O `validate-specs.ps1` falha o CI se a regra for violada (DOD-09).

---

## Referências

- [PLATFORM_STATE.md](../ops/PLATFORM_STATE.md)
- [AGENTS.md](../../AGENTS.md)
- [PHASE_QUEUE.yaml](../_meta/PHASE_QUEUE.yaml)
