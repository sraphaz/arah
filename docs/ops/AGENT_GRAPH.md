# Agent Graph (Grafo Operacional)

**Versão**: 1.0
**Data**: 2026-07-02
**Status**: primeiro passo incremental (documentação estruturada + artefato gerado)

---

## O que é

O **Agent Graph** é uma camada leve que **formaliza o grafo operacional que já
existe** no repositório `sraphaz/arah`. Ele não introduz um novo runtime nem
substitui o orquestrador: apenas torna explícitas, num único artefato auditável,
as relações que hoje estão espalhadas por `.agents/`, `.skills/`, `docs/specs/`,
`scripts/` e `.github/workflows/`.

Concretamente, o Agent Graph responde a perguntas do tipo:

- *"Por que este agente foi acionado neste PR?"*
- *"Quais skills uma mudança neste path pode disparar?"*
- *"Qual domínio precisa dar parecer quando mexo em `Financial/`?"*
- *"Qual guardrail bloqueia merge automático, e qual workflow o impõe?"*
- *"Qual spec/harness valida esta fase?"*

Fonte inicial de verdade: [`.agents/choreography.yaml`](../../.agents/choreography.yaml),
cruzada com os manifests de agentes, skills, specs e workflows.

## Por que existe

A operação por agentes do Arah cresceu de forma orgânica e correta, mas o
conhecimento sobre *como as peças se conectam* ficou implícito no código dos
scripts e na cabeça de quem escreveu os manifests. Isso dificulta:

- **auditar** decisões de roteamento/co-ativação;
- **explicar** para novos contribuidores (humanos ou agentes) o encadeamento;
- **validar** que rules críticas têm gates coerentes;
- **evoluir** a operação sem regressões silenciosas.

O Agent Graph resolve isso reaproveitando o que já existe, sem custo de contexto
recorrente (é um artefato gerado sob demanda, não algo injetado a cada request).

## O que ele NÃO é

- **Não** é Neo4j, LangGraph, MCP Graph server, nem qualquer banco de grafos.
- **Não** é um novo orquestrador — o roteamento continua em
  [`orchestrator.agent.yaml`](../../.agents/orchestrator.agent.yaml) +
  [`choreograph-agents.ps1`](../../scripts/agents/choreograph-agents.ps1).
- **Não** executa agentes nem skills; é descritivo/validador.
- **Não** adiciona dependências pesadas: os scripts são PowerShell 5.1+ com parse
  por regex, no mesmo estilo dos scripts existentes.
- **Não** implementa MCP agora (ver [Futuro: exposição via MCP](#futuro-exposição-via-mcp)).

## Problemas que resolve

| Problema | Como o graph ajuda |
|---|---|
| Roteamento opaco | Torna explícitas as arestas `matches_rule` / `activates_agent` / `consults_domain_agent` |
| Skills "mágicas" | `requires_skill` / `may_invoke_skill` mostram origem (rule ou manifest) via campo `via` |
| Gates críticos sem cobertura | Validação verifica rules críticas (identity-privacy, monetization, infra-deploy, core-control-plane, federation-handoff) |
| Deriva entre docs e realidade | Artefato é **gerado** dos arquivos-fonte; validação avisa quando defasado |
| Onboarding | Um único JSON + este doc explicam a operação inteira |

---

## Nós

Definidos formalmente em [`.agents/agent-graph.schema.yaml`](../../.agents/agent-graph.schema.yaml).
No JSON gerado, cada nó tem um id com namespace (`agent:`, `skill:`, `rule:` …).

| Nó | Namespace | Fonte |
|---|---|---|
| **Agent** (operational / domain / specialist) | `agent:` | `.agents/**/*.agent.yaml` |
| **Skill** | `skill:` | `.skills/*.skill.yaml` |
| **ChoreographyRule** | `rule:` | `.agents/choreography.yaml` |
| **PathPattern** | `path:` | rules[].paths + scope.paths |
| **Domain** | `domain:` | `.agents/domain/*` + rules de type `domain` |
| **Spec** | `spec:` | `docs/specs/**/*.spec.yaml` |
| **Harness** | `harness:` | bloco `harness` das specs + `scripts/harness/**` |
| **Guardrail** | `guardrail:` | manifests, specs e `run-harness.ps1` (`Test-Guardrail`) |
| **Workflow** | `workflow:` | `.github/workflows/*.yml` |
| **ReviewGate** | `gate:` | `pr-always`, `pr-steward`, workflows |

## Relações

| Aresta | De → Para | Significado |
|---|---|---|
| `matches_rule` | PathPattern → ChoreographyRule | mudança no path casa com a regra |
| `activates_agent` | ChoreographyRule → Agent | ativa agente operacional |
| `consults_domain_agent` | ChoreographyRule/Agent → Agent | aciona parecer de domínio |
| `may_invoke_skill` | Agent → Skill | agente pode invocar skill |
| `requires_skill` | ChoreographyRule → Skill | skill declarada na rule (autonomy) |
| `requires_spec` | ChoreographyRule → Spec | paths SDD exigem spec válida |
| `requires_harness` | Spec → Harness | spec validada por harness |
| `validated_by` | Harness → Agent | agente responsável no harness |
| `requires_human_review` | ReviewGate → ReviewGate | encaminha ao merge humano |
| `blocked_by_guardrail` | Agent/Spec → Guardrail | opera sob o guardrail |
| `enforced_by_workflow` | Guardrail → Workflow | guardrail imposto por CI |

O campo `via` de cada aresta registra a origem (`rule:<id>` ou `manifest`),
permitindo reconstruir a cadeia causal de uma ativação.

---

## Como se conecta à coreografia atual

O Agent Graph é um **espelho estruturado** da coreografia, não um substituto:

```
.agents/choreography.yaml  ──(export)──►  docs/_meta/agent-graph.generated.json
        │                                              │
        ▼                                              ▼
choreograph-agents.ps1 (runtime, decide)      validate-agent-graph.ps1 (audita)
```

- O **runtime** de roteamento continua em `choreograph-agents.ps1` / `orchestrate`.
- O **graph** é gerado a partir das mesmas fontes e serve para inspeção/validação.
- Os dois usam o mesmo parser regex (mesma leitura de `paths`/`agents`/`skills`),
  então não divergem por interpretação.

## Como se conecta ao SDD + Harness

Ver [`docs/_meta/SDD_AND_HARNESS.md`](../_meta/SDD_AND_HARNESS.md).

- Cada **Spec** (`docs/specs/**`) vira nó `spec:` com aresta `requires_harness`.
- O bloco `harness.agents` da spec gera arestas `validated_by` para os agentes.
- Os **guardrails** executáveis (`clean-architecture`,
  `territory-data-stays-on-instance`, `no-merge-automatic`, …) verificados em
  `run-harness.ps1` viram arestas `enforced_by_workflow` → `spec-harness.yml`.
- A rule `specs-sdd` gera `requires_spec`, materializando *spec-before-code*.

## Como melhora os guardrails

- Deixa **auditável** quais agentes operam sob quais guardrails
  (`blocked_by_guardrail`), ex.: `no_merge=true` no `pr-steward` e no `backend`.
- Liga cada guardrail executável ao workflow que o impõe.
- A validação garante que rules críticas nunca fiquem **sem gate** (domain
  consult, agente operacional ou skill coerente), com erro no CI.
- Reforça, de forma explícita e verificável, os princípios do repositório:
  humano comanda; tudo via PR; sem commit direto em `main`; escopo mínimo;
  spec-before-code; comunicação passiva; CI + PR Steward como gates; merge humano.

---

## Uso

```powershell
# Gerar/atualizar o artefato
./scripts/agents/export-agent-graph.ps1
./scripts/agents/arah-agents.ps1 export-graph

# Validar consistência (erros = crítico; warnings = não bloqueia)
./scripts/harness/validate-agent-graph.ps1
./scripts/harness/validate-agent-graph.ps1 -Strict   # warnings viram erro
./scripts/agents/arah-agents.ps1 validate-graph
```

O artefato gerado fica em
[`docs/_meta/agent-graph.generated.json`](../_meta/agent-graph.generated.json) e
**deve ser commitado no mesmo PR** que altera coreografia/manifests/skills
(doc como código). A validação avisa quando o JSON está defasado.

### Exemplo de leitura ("por quê?")

Para saber por que o `backend` foi ativado ao mudar `backend/Arah.Core/Foo.cs`:

1. `path:backend/Arah.Core/**` → `matches_rule` → `rule:core-control-plane`
2. `rule:core-control-plane` → `activates_agent` → `agent:backend`
3. `rule:core-control-plane` → `consults_domain_agent` → `agent:control-plane`
4. `rule:core-control-plane` → `requires_skill` (via) → `skill:architecture-review`
5. `spec:FASE53-arah-core` → `requires_harness` → `harness:FASE53-arah-core`

---

## Futuro: exposição via MCP

Este passo **não** implementa MCP. Porém, o desenho já deixa o caminho pronto:

- O artefato `agent-graph.generated.json` tem esquema estável (nós + arestas),
  fácil de servir como *resource* MCP (`arah://agent-graph`).
- Consultas do tipo *"quais agentes/skills um path dispara"* podem virar *tools*
  MCP finas por cima do JSON já gerado (ou re-executando o export).
- Como o graph é derivado e read-only, expô-lo via MCP não muda guardrails: o MCP
  seria apenas uma janela de leitura/explicação, mantendo o merge humano.
- Próximo incremento natural: um pequeno servidor MCP que carrega o JSON e
  responde `explain(path)` / `agents_for(path)` / `skills_for(agent)`.

## Limitações conhecidas (v1)

- Parsing por regex (não YAML completo): assume a formatação atual dos arquivos.
- `generated_at` muda a cada export (o restante do JSON é estável/ordenado).
- Workflows são anotados por um mapa curado de papéis; não há análise semântica
  profunda de cada `.yml`.
- Arestas de spec cobrem `harness.agents` e `guardrails`; não modelam ainda
  cada acceptance criterion individual.

## Próximos passos recomendados

1. Tornar a validação um **gate duro** no harness/CI quando estável (hoje é soft).
2. Gerar um diagrama (Mermaid/LikeC4) a partir do JSON para navegação visual.
3. Expor via MCP como resource/tools read-only.
4. Estender o graph para arestas por acceptance criterion (rastreabilidade fina).

## Referências

- [`.agents/agent-graph.schema.yaml`](../../.agents/agent-graph.schema.yaml)
- [`docs/_meta/agent-graph.generated.json`](../_meta/agent-graph.generated.json)
- [`scripts/agents/export-agent-graph.ps1`](../../scripts/agents/export-agent-graph.ps1)
- [`scripts/harness/validate-agent-graph.ps1`](../../scripts/harness/validate-agent-graph.ps1)
- [AGENTS.md](../../AGENTS.md) · [AGENT_OPERATION.md](AGENT_OPERATION.md) · [SDD_AND_HARNESS.md](../_meta/SDD_AND_HARNESS.md)
