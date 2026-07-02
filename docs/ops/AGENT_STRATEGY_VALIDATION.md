# Validação da estratégia de agentes — Arah vs mercado

**Versão**: 1.0
**Data**: 2026-07-02
**Escopo**: `.agents/`, `.skills/`, coreografia, harness SDD, guardrails, workflows CI
**Complementa**: [AGENTS.md](../../AGENTS.md), [AGENT_OPERATION.md](./AGENT_OPERATION.md), [AGENT_QUICKSTART.md](./AGENT_QUICKSTART.md)

---

## 1. Resumo executivo

A estratégia de agentes do Arah está **alinhada com o estado da arte de 2026** e, em alguns
pontos (coreografia path-based, pareceres de domínio autônomos, DoD com evidência), está
**à frente** do que ferramentas genéricas oferecem. As referências de mercado usadas:

- **GitHub Spec Kit** (SDD open-source, agent-agnostic): Constitution → Specify → Plan → Tasks
- **AWS Kiro** (IDE agentic): EARS para requisitos + *hooks* como guardrails automáticos
- **Padrão `AGENTS.md`** (AAIF): manual de operação versionado, lido nativamente por Cursor, Copilot, Codex, Claude Code etc.
- **Práticas multi-agente consolidadas**: orquestrador + especialistas em 1 nível, permissão mínima por agente, quality gates antes do merge, merge humano

### Veredito por pilar

| Pilar | Mercado (prática consolidada) | Arah | Avaliação |
|-------|-------------------------------|------|-----------|
| Manual central versionado | `AGENTS.md` padrão AAIF | `AGENTS.md` + `.cursorrules` + rules | ✅ Conforme (equivale à "Constitution" do Spec Kit) |
| Spec antes de código | Spec Kit: Specify→Plan→Tasks; Kiro: requirements/design/tasks | `docs/specs/*.spec.yaml` + `Spec-Id` no PR + `spec-gate-check` | ✅ Conforme; specs YAML são até mais "machine-readable" que markdown |
| Guardrails automáticos | Kiro Hooks (test/lint/scan pós-ação) | Hook Cursor `stop` (domain-review) + `run-gates.ps1` (QA/Security/Design/SDD) + CI | ✅ Conforme e mais amplo (gates de design e nomenclatura de domínio) |
| Orquestração 1 nível | Orquestrador → especialistas, sem hierarquia profunda | `orchestrator` roteia; coreografia co-ativa; consultivos não abrem PR | ✅ Conforme |
| Permissão mínima | `tools`/paths restritos por subagente | `scope.paths` + `may_code` + `max_diff_files` por manifest | ✅ Conforme |
| Quality gates pré-merge | CI verde + review humano + bots resolvidos | `pr-steward` exige CI + zero apontamentos de bot → `ready-for-merge`; merge humano | ✅ Conforme |
| Rastreabilidade AC ↔ teste | EARS + testes ligados ao requisito | DoD §2: `covered_by`/naming + harness `dotnet test --filter` + gate automático em `validate-specs.ps1` (DOD-09 ✅) | ✅ Conforme |
| Specs como docs duráveis | "Specs outlive the code" | Specs permanecem em `docs/specs/` com status | ✅ Conforme (Arah é *spec-anchored*, não apenas *spec-first*) |
| Cobertura de domínios | Especialista por domínio de negócio | 7 → **11 agentes de domínio** após esta revisão | ✅ Corrigido nesta revisão (ver §3) |

---

## 2. O que o mercado faz e o Arah já pratica

1. **Constitution first** — O Spec Kit recomenda comprometer princípios imutáveis antes da
   primeira spec. No Arah isso é `.cursorrules` (princípios) + `AGENTS.md` (operação) +
   `docs/governance/DEFINITION_OF_DONE.md` (rigor). Já versionados e carregados em toda sessão.
2. **Review em fronteiras de fase** — nunca pular de spec para código. No Arah:
   `spec-gate-check.ps1` bloqueia PR de fase S0+ sem `Spec-Id`, e a `PHASE_QUEUE.yaml`
   controla desbloqueio por dependência (`blocked_by`).
3. **Hooks como guardrail** (Kiro) — o hook `stop` do Cursor roda `domain-autoreview.ps1`
   a cada interação e o CI publica pareceres em todo PR. Equivalente funcional aos Kiro Hooks,
   com a vantagem de os pareceres serem **de domínio de negócio**, não só lint/test.
4. **Merge sempre humano** — consenso do mercado para produção; `guardrails.no_merge: true`
   em todos os manifests.
5. **Checklists por etapa** — o Spec Kit usa checklists como "DoD por etapa do workflow";
   o Arah tem `.agents/checklists/` + templates de QA/Security/bots.
6. **Specs curtas e verificáveis** — os `acceptance` com `id` + `then` mensurável seguem o
   espírito do EARS (comportamento verificável), e o harness executa os comandos ligados.

---

## 3. Gaps encontrados e ajustes aplicados (nesta revisão)

### 3.1 Cobertura de domínios — 4 novos agentes de domínio

O backend tem 12+ módulos de domínio, mas só 7 tinham agente consultivo. Domínios inteiros
(feed, eventos, chat, notificações, mapa, mídia, identidade/LGPD) não recebiam parecer.
`flutter.agent.yaml` inclusive referenciava `feed-conteudo` e `mapa-lugares` **que não existiam**.

| Novo agente | Cobre | Manifest |
|-------------|-------|----------|
| `feed-conteudo` | Feed, Events, Media — presença sem engajamento infinito | `.agents/domain/feed-conteudo.agent.yaml` |
| `mapa-lugares` | Map, Assets, Geo — mapa vivo sem tracking de pessoas | `.agents/domain/mapa-lugares.agent.yaml` |
| `comunidade-conexoes` | Chat, Connections, Notifications, Alerts — silêncio funcional | `.agents/domain/comunidade-conexoes.agent.yaml` |
| `identidade-privacidade` | Users, Auth, Policies, LGPD — privacidade por padrão | `.agents/domain/identidade-privacidade.agent.yaml` |

Coreografia (`.agents/choreography.yaml` v2) ganhou as regras `feed-content`, `map-places`,
`community-connections` e `identity-privacy` (esta co-ativa o agente `security`).

### 3.2 Referências quebradas corrigidas

| Problema | Correção |
|----------|----------|
| Specialist `postgresql` citado em `backend.agent.yaml` mas inexistente | Criado `.agents/specialists/postgresql.agent.yaml` |
| Domínios `feed-conteudo`/`mapa-lugares` citados em `flutter.agent.yaml` mas inexistentes | Criados (§3.1) |
| `docs-steward` com `CHANGELOG.md` na raiz (não existe; regra proíbe) | Removido do scope; mantido `docs/CHANGELOG.md` |
| `agent-operation.spec.yaml` AC-AG-1 com contagem hardcoded (22/17, real era 24/21) | AC reescrito para contagem dinâmica (regra: nunca hardcode) |
| `control-plane` não cobria código de federação em runtime | Scope + coreografia incluem `FederationController` e `Arah.Core/Application/Federation*` |
| `.agents/README.md` listava só 9 agentes | Catálogo completo (operacionais, domínio, especialistas) |
| `validate-manifests.ps1` não checava referências cruzadas | Agora falha se `consult.domain`/`consult.specialists`/`skills` não resolverem (LIC-003) |

### 3.3 Documentação de operação

- Este documento (validação vs mercado).
- [AGENT_QUICKSTART.md](./AGENT_QUICKSTART.md) — guia passo a passo para novos devs.
- `docs/specs/README.md` atualizado com todas as specs e a pasta `features/` criada.

---

## 4. Recomendações — status de implementação

Priorizadas por impacto sobre autonomia e qualidade. **P0/P1 e a maior parte de P2 foram
implementadas em 2026-07-02** (mesmo PR desta validação):

### P0 — antes de escalar as fases 56–61

1. ✅ **Spec antes de código para FASE56–61**: autoradas as 6 specs em
   `docs/specs/phases/` (`FASE56-transparency`, `FASE57-cockpit`, `FASE58-multi-instance`,
   `FASE59-federation`, `FASE60-implementer-app`, `FASE61-territorial-capital`) com
   acceptance em EARS e `status: draft` (viram `active` após clarify na abertura da fase).
   `PHASE_QUEUE.yaml` ganhou `spec_id` em todas as fases 54–61.
2. ✅ **Gate automático de `covered_by`** (DOD-09): `scripts/harness/validate-specs.ps1`
   falha AC `status: covered` sem `covered_by` e `status: manual` sem `evidence`.
   Regressão de rigor agora quebra o CI.

### P1 — qualidade das specs

3. ✅ **Sintaxe EARS nos `acceptance`**: `docs/specs/_template.spec.yaml` documenta
   `when`/`then` + `status`/`covered_by`/`evidence`; as 6 novas specs já seguem o formato.
4. ✅ **Passo clarify explícito**: item obrigatório nos checklists do `spec-steward`
   ("ambiguidades resolvidas antes de ativar") e do `backend` ("clarify antes de implementar").
5. ✅ **Specs curtas (1–3 páginas)**: mantido; overflow vai para `docs/specs/features/`.

### P2 — operação e medição

6. ✅ **Métricas de efetividade dos agentes**: skill `agent-metrics`
   (`scripts/agents/agent-metrics.ps1`) — PRs por origem, tempo abertura→`ready-for-merge`,
   apontamentos de bot por PR, pareceres de domínio. Rodar mensalmente para calibrar autonomia.
7. ✅ **Worktrees para tentativas paralelas**: skill `parallel-attempt`
   (`scripts/agents/parallel-attempt.ps1`) — best-of-N (2–5) em worktrees isolados;
   humano escolhe o PR vencedor. Usar só em tarefas exploratórias.
8. ✅ **Consolidar sistemas de skills**: `.skills/` é a fonte de verdade; as skills Cursor
   (`.cursor/skills/arah-*`) já apenas referenciam os YAML correspondentes — regra registrada
   no `AGENT_QUICKSTART.md` (nova skill = YAML primeiro, Cursor só aponta).
9. ✅ **Contexto sob demanda (consumo de API)**: PR 14 — `.cursorrules` enxuto (~4 KB),
   regras escopadas por glob, hook `stop` passivo (sem `followup_message`), skills globais
   do usuário via `skill-router` + índice em `skills-backup/` (modelo pull, não push).
10. ⏳ **Revisão periódica de conteúdo dos checklists**: contínua — o
   `validate-manifests.ps1` verifica existência e integridade referencial, não conteúdo;
   revisar checklists quando módulos novos surgirem.

---

## 5. O que **não** adotar do mercado (decisão consciente)

- **Tessl "spec as source"** (código 100% gerado da spec): não determinístico demais para um
  backend financeiro/territorial; o Arah mantém código como artefato revisado.
- **Merge automático com CI verde**: contraria o princípio "humano comanda, agente executa".
- **Hierarquia multi-nível de agentes** (agente que cria agentes): o mercado converge para
  1 nível de delegação; profundidade adicional degrada contexto e auditabilidade.

---

## 6. Rastreabilidade

- Inventário completo do sistema: ver [AGENT_OPERATION.md](./AGENT_OPERATION.md)
- Definition of Done: [../governance/DEFINITION_OF_DONE.md](../governance/DEFINITION_OF_DONE.md)
- SDD e harness: [../_meta/SDD_AND_HARNESS.md](../_meta/SDD_AND_HARNESS.md)
- Fontes de mercado: GitHub Spec Kit (github.github.com/spec-kit), AWS Kiro (EARS + Hooks),
  padrão AGENTS.md (AAIF), análises de SDD de Birgitta Böckeler (martinfowler.com, 2025–2026)
