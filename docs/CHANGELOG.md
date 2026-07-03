# Changelog - Arah

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/lang/pt-BR/).

---

## [Unreleased]

### Corrigido — Craftsmanship no tooling do Agent Graph (aplicando as próprias skills de review) (2026-07-03)

- **DRY**: helpers de leitura YAML duplicados entre `export-agent-graph.ps1` e `validate-agent-graph.ps1` extraídos para `scripts/agents/yaml-lite.ps1` (fonte única, ambos dot-source)
- **Falsos positivos de órfã**: `agent-activate`, `agent-metrics`, `domain-consult` e `parallel-attempt` declaram `activation:` (orchestrator/workflow/cli); o validador deixa de marcá-las como órfãs (acionadas fora da coreografia de paths)
- **Dogfooding**: `craft-review-check.ps1` passa a reconhecer `.ps1/.psm1/.mjs/.cjs/.py` como código — antes ignorava a própria linguagem do tooling do Ará
- **Lacuna de testes fechada**: `scripts/agents/tests/agent-graph.tests.ps1` (PS puro, sem Pester) cobre `yaml-lite`, `choreography-parser` e a detecção do `craft-review`; `craft-review-check.ps1` agora é dot-sourceável (execução principal guardada)
- Agent graph regenerado e validado (`-Strict`: 0 erro, 0 aviso)

### Corrigido — Apontamentos de bots no PR do Agent Graph (#432) (2026-07-03)

- **MCP `globToRegex`** (bug real pego por teste novo): `*`→`[^/]*` corrompia o `.*` inserido por `**`, então `backend/**` não casava subpaths; corrigido com sentinelas (runtime PS já estava correto via `[regex]::Escape`)
- **MCP ignora regras de evento**: `matchRulesForPath` pula `when: pull_request` (ex.: `pr-always`), não reporta mais `qa`/`pr-steward` para paths locais arbitrários
- **CodeQL (mjs)**: `globToRegex` passa a escapar `\` no conjunto de metacaracteres
- **`.mmd` puro**: `Build-Mermaid` não embrulha mais em cercas ```` ```mermaid ````; arquivo renderiza no Mermaid Live/como `.mmd`
- **Validador — integridade referencial sempre roda** (não só quando não há erros); **catálogo de skills pelo campo `id:`** do manifest (não pelo filename)
- **`choreography-parser`**: regras sem `paths` deixam de sumir silenciosamente — entram na lista e o check "regra sem paths" do validador as flagra
- **`craft-review-check`**: `git fetch` segue `$BaseRef` (não `origin/main` fixo), evitando diff vazio que mascararia o aviso de "mudança sem teste"
- **CI `agents-validate.yml`**: `persist-credentials: false` nos checkouts; paths de `push` incluem `CODEOWNERS`/`agents-validate.yml`/`agents.yml`; novo job roda os testes PS + Node
- **Docs**: link `arah-craft/SKILL.md` corrigido (`../../../` para a raiz do repo)
- Testes novos: `scripts/agents/tests/mcp.test.mjs` (Node) e MCP tornado importável (loop stdio só em execução direta)

### Adicionado — Skill de craftsmanship (Uncle Bob) acionada pelos agentes no ciclo projetar→desenvolver→testar (2026-07-03)

- **Nova skill `craft-review`** (`.skills/craft-review.skill.yaml`): disciplina Clean Code/Clean Architecture/SOLID/TDD em três fases (projetar, desenvolver, testar), complementando `architecture-review` (fronteiras/ADR) e `code-review` (checklist de PR)
- **Script `scripts/agents/craft-review-check.ps1`** (soft): imprime o checklist por fase e avisa sobre mudança de código sem teste; `-Strict` promove a gate duro; wired em `invoke-skill.ps1`
- **Skill de descoberta do Cursor** `.cursor/skills/arah-craft/` para o agente interativo carregar sob demanda ao projetar/codar/testar
- **Fiação nos agentes**: `craft-review` adicionada a `solutions-architect`, `backend`, `flutter`, `web` e `qa`
- **Coreografia**: novas rules `craft-backend`/`craft-flutter`/`craft-web` (invocam `craft-review` no desenvolvimento; `solutions-architect` co-ativado no backend); `pr-always` passa a invocar `craft-review` no `qa`; `core-control-plane`/`architecture-docs` incluem `craft-review` no `solutions-architect`
- **`choreograph-agents.ps1`**: `skill_invocations` agora é sempre registrado (plano auditável), independente de `-ExecuteAutonomy` (que passa a controlar só a execução)
- Agent graph regenerado (25 skills, 18 rules); `AGENTS.md` e `docs/ops/AGENT_OPERATION.md` atualizados

### Adicionado — FASE55 monetização: fluxo de pagamento + ledger unificado + FeeSplitRule Postgres (DOD-05/06/07) (2026-07-02)

- **DOD-05 — `PaymentService`**: transição de pagamento `Created → AwaitingPayment → Paid`; após confirmar, delega o ledger a `SellerPayoutService.ProcessPaidCheckoutAsync`
- **`IPaymentGateway`** + **`MockPaymentGateway`**: abstração de cobrança (PIX/Stripe) espelhando o padrão de `IPayoutGateway`
- **Endpoints**: `POST /api/v1/transactions/{id}/pay`, `POST /api/v1/transactions/{id}/confirm-payment`; webhook mock `POST /api/v1/webhooks/checkout/mock-approved` (dev/testes)
- **Domínio**: `Checkout.MarkAsAwaitingPayment()` e `Checkout.MarkAsPaid()` com guardas de transição; AC-55-8 na spec FASE55
- **DOD-06 — estorno no ledger**: `SellerPayoutService.ReversePaidCheckoutAsync` persiste `FinancialTransaction` tipo `Refund` (append-only), cancela `SellerTransaction` pendente e reverte `SellerBalance`/`PlatformFinancialBalance`; `RefundService` delega ao ledger antes de marcar `Refunded`
- **DOD-07 — FeeSplitRule Postgres**: `FeeSplitRuleRecord`, `PostgresFeeSplitRuleRepository`, migration `AddFeeSplitRules`; branch Postgres passa a usar persistência real
- **Testes**: `PaymentServiceTests` (unit), `RefundLedgerIntegrationTests` (pay → confirm → refund → ledger) e casos HTTP em `TransactionsControllerTests`
- **Hardening (revisão de bots #431)**: confirmação valida que o pagamento pertence ao checkout (`PaymentStatusResult.CheckoutId`); `InitiatePaymentAsync` faz commit da transição `AwaitingPayment` (persistência no Postgres); confirmação de checkout já pago reprocessa o ledger de forma idempotente (auto-recuperação); estorno marca `Refunded` no mesmo commit da reversão do ledger (atomicidade/idempotência) e persiste status `Completed` das transações de estorno antes do `AddAsync`; `MockPaymentGateway` singleton e thread-safe (estado sobrevive entre requisições); `IPaymentGateway` mock só fora de produção (`UnavailablePaymentGateway` em produção); `PostgresFeeSplitRuleRepository` compara `RevenueType` case-insensitive

### Corrigido — Design quality epic #427: DSG-03 devportal tokens + DSG-05 portal a11y (2026-07-02)

- **DSG-03**: tokens forest/surface/dark-text em `frontend/shared/styles/design-tokens.css`; devportal deriva `--bg`, `--text`, `--accent` etc. dos tokens compartilhados; `semantic-colors.css` e `color-depth-system.css` reescritos sem hex e importados em `devportal.css`
- **DSG-05**: portal landing — skip link (`skip-to-content`), landmark `main`, `aria-label` na navegação e links externos, `role="banner"` no header, alt descritivo na imagem da hero
- `docs/design/AUDITORIA_DESIGN.md` atualizado (DSG-03/05 ✅)

### Alterado — Contexto em camadas + comunicação passiva entre agentes (otimização de consumo de API) (2026-07-02)

- **`.cursorrules` v2.0**: de ~60 KB always-apply para núcleo de ~4 KB (princípios, guardrails, ponteiros) — redução de ~95% do contexto fixo injetado em cada requisição do Cursor
- **Regras escopadas por glob** em `.cursor/rules/`: `backend-standards.mdc` (`backend/**`), `frontend-design.mdc` (`frontend/**`), `docs-organization.mdc` (`docs/**`, `*.md`) — só entram no contexto quando arquivos correspondentes são tocados
- **Hook `stop` passivo** (`.cursor/hooks/domain-review.ps1`): continua gerando os pareceres em `.cursor/domain-review.md`, mas não injeta mais `followup_message` — elimina o turno extra de modelo (rodada completa de API) a cada interação; CI (`agents.yml`) permanece a instância autoritativa publicando pareceres no PR
- **`domain-agents-autonomy.mdc`**: de `alwaysApply: true` para escopo `backend/**`,`frontend/**`; instrui leitura do parecer por arquivo (comunicação passiva)
- **Novas skills Cursor** (descoberta sob demanda): `arah-open-pr` e `arah-domain-consult` em `.cursor/skills/`
- **`AGENTS.md` v2.0**: enxugado de ~15 KB para ~5 KB; tabelas de SDD, coreografia e skills movidas para `docs/ops/AGENT_OPERATION.md` (PR 14)
- **Lição LIC-004** em `docs/LICOES_APRENDIDAS.md` — contexto fixo e comunicação ativa entre agentes inflam consumo de API

### Corrigido — Harness local com stack Arah em execução (2026-07-02)

- **`scripts/harness/stop-arah-local-processes.ps1`**: encerra `Arah.Bff`/`Arah.Api` órfãos antes de `dotnet build` no harness — evita falso `MSB3027` (exe locked) que cascateava em falhas de teste
- **`run-harness.ps1`**: invoca o script automaticamente antes de comandos `dotnet build`

### Corrigido — Apontamentos de revisão do PR 426 (2026-07-02)

- **Harness**: filtros `dotnet test` com `|` nas specs FASE56/58/61 agora entre aspas simples — o PowerShell interpretava o `|` como pipe e quebrava o comando
- **Harness (spec-before-code)**: specs `draft` validam estrutura mas **pulam** `scripts`/`commands` — fases sem implementação (FASE56–61) não falham mais o CI com "no test matches filter", `flutter test` ausente ou health check de instância não provisionada
- **`validate-specs.ps1`**: regex do bloco `acceptance` sem modo singleline — o capture não vaza mais para `harness:`/`guardrails:`, evitando falso `covered_by`/`evidence` no último AC
- **Coreografia**: regra `community-connections` cobre os paths de frontend do manifest (`features/{chat,connections,notifications}`); `design-ux.agent.yaml` sincronizado com `frontend/**/tailwind.config.ts`
- **Docs**: Estatísticas de `LICOES_APRENDIDAS.md` atualizadas (LIC-002/003); `specs/README.md` generaliza o runner de cobertura por stack; `AGENT_QUICKSTART.md` inclui suítes `Arah.Tests.Modules.*`; checklist backend menciona caminho `manual`/`evidence`
- **`agent-metrics.ps1`**: regravado com BOM UTF-8 (PS 5.1 renderizava acentos como mojibake)

### Adicionado — Validação da estratégia de agentes vs mercado + cobertura completa de domínios (2026-07-02)

- **`docs/ops/AGENT_STRATEGY_VALIDATION.md`** — benchmark da malha de agentes contra práticas de mercado 2026 (GitHub Spec Kit, AWS Kiro/EARS, padrão AGENTS.md/AAIF, multi-agente com permissão mínima e merge humano): estratégia conforme em todos os pilares; gaps e recomendações priorizadas (P0: specs FASE56–61 antes de código, gate `covered_by`; P1: EARS nos acceptance, passo clarify; P2: métricas de efetividade, worktrees paralelos)
- **`docs/ops/AGENT_QUICKSTART.md`** — guia de operação por agentes para novos devs: fluxo dia a dia, CLI, DoD resumido, como estender a malha (novo agente/skill/fase), guardrails
- **4 novos agentes de domínio** (fechando cobertura dos módulos do backend): `feed-conteudo` (Feed/Events/Media), `mapa-lugares` (Map/Assets/Geo), `comunidade-conexoes` (Chat/Connections/Notifications/Alerts), `identidade-privacidade` (Users/Auth/Policies/LGPD — co-ativa agente `security`)
- **Coreografia v2** (`.agents/choreography.yaml`): regras `feed-content`, `map-places`, `community-connections`, `identity-privacy`; regra `federation-handoff` agora cobre código de federação em runtime (`FederationController`, `Arah.Core/Application/Federation*`)
- **Referências quebradas corrigidas**: criado specialist `postgresql` (citado por `backend.agent.yaml`); `docs-steward` sem `CHANGELOG.md` de raiz; `agent-operation.spec.yaml` sem contagem hardcoded de agentes/skills
- **Integridade referencial de manifests** (LIC-003): `validate-manifests.ps1` agora falha se `consult.domain`, `consult.specialists` ou `skills` de um agente referenciarem manifests/skills inexistentes — referência fantasma quebra o CI
- **Specs FASE56–61 autoradas** (spec-before-code, P0 da validação): `FASE56-transparency`, `FASE57-cockpit`, `FASE58-multi-instance`, `FASE59-federation`, `FASE60-implementer-app`, `FASE61-territorial-capital` em `docs/specs/phases/` — acceptance em EARS, `status: draft`; `PHASE_QUEUE.yaml` com `spec_id` nas fases 54–61
- **Gate DOD-09 implementado**: `validate-specs.ps1` falha AC `status: covered` sem `covered_by` e `status: manual` sem `evidence` (retrofit: AC-55-7 com `covered_by`, AC-52-5 corrigido para `manual`); retrospectiva DoD atualizada
- **Template de spec em EARS**: `_template.spec.yaml` documenta `when`/`then` + `status`/`covered_by`/`evidence`; passo **clarify** obrigatório nos checklists de `spec-steward` e `backend`
- **Novas skills de operação**: `agent-metrics` (`scripts/agents/agent-metrics.ps1` — PRs, tempo→ready-for-merge, apontamentos de bot/PR, pareceres de domínio) e `parallel-attempt` (`scripts/agents/parallel-attempt.ps1` — best-of-N em git worktrees isolados)
- Catálogos atualizados: `.agents/README.md` completo (12 operacionais + 11 domínio + 6 especialistas), `AGENTS.md` (tabelas consultivos/coreografia/referências), `.cursor/rules/domain-agents-autonomy.mdc` (mapeamento), `docs/ops/PLATFORM_STATE.md` (29 agentes + 22 skills)
- `docs/specs/README.md` — lista todas as specs (inclui platform/harness) e sinaliza pendência spec-before-code de FASE56–61; pasta `docs/specs/features/` criada

### Adicionado — Agente autônomo de Design (UX/UI) (2026-07-02)

- **Novo agente de domínio `design-ux`** (`.agents/domain/design-ux.agent.yaml`): especialista consultivo que garante identidade high-premium, acessibilidade (WCAG AA), mobile-first e uso de tokens (nunca cor hardcoded), em web e Flutter
- **Coreografia** (`.agents/choreography.yaml`): regra `design-ux` aciona o agente automaticamente ao tocar em `frontend/**`, tokens compartilhados, `docs/design/**` e `docs/**/design*` — parecer local (hook `stop`) e no PR (workflow `agents.yml`/Agents Orchestrate; o gate roda em `agents-gates.yml` via `run-gates.ps1`)
- Catálogo atualizado: `AGENTS.md` (consultivos + tabela de coreografia), `.cursor/rules/domain-agents-autonomy.mdc` (mapeamento), `docs/ops/PLATFORM_STATE.md` (24 agentes)
- **Auditoria** `docs/design/AUDITORIA_DESIGN.md` — 8 gaps mapeados (DSG-01..08) com evidência, severidade e ordem de correção
- **Corrigido DSG-01 (wiki)**: 5 cores arbitrárias `bg-[#...]` no Mermaid fullscreen → classes configuradas `bg-accent`/`bg-link` (`frontend/wiki/app/mermaid/fullscreen/page.tsx`)
- **Corrigido DSG-01b (wiki)**: 22 hex nos `themeVariables` do Mermaid (2 componentes) → adapter `lib/mermaid-theme.ts` derivando o tema dos design tokens (descoberto pelo próprio gate DSG-08)
- **Corrigido DSG-02 (Flutter)**: `Color(0xFF228B22)`/`Colors.orange` fora do tema no onboarding → `AppDesignTokens.territoryBoundary`/`locationPin` (`propose_territory_sheet.dart`, `onboarding_screen.dart`)
- **Gate DSG-08**: `scripts/agents/design-gate-check.ps1` integrado ao `run-gates.ps1` (QA) — falha o PR se arquivo de frontend alterado tiver cor hardcoded (Tailwind `[#...]`, hex inline, CSS fora de `--token:`, `Color(0x...)`/`Colors.*` fora do tema)
- Backlog `docs/_meta/PHASE_QUEUE.yaml` — entrada `design-quality` (P1) para DSG-03..07 (devportal→tokens, espaçamento 8px, a11y landing, unificar glass, contraste syntax)
- Lição `docs/LICOES_APRENDIDAS.md` LIC-002 — regressão de cores exige agente + gate de design, não só diretriz

### Retrospectiva DoD — gaps de fases anteriores (2026-07-02)

- `docs/backlog-api/RETROSPECTIVA_DOD_GAPS.md` — auditoria das entregas anteriores à nova Definition of Done, com 10 itens acionáveis (DOD-01..10) priorizados
- **DOD-01**: `scripts/harness/validate-specs.ps1` passa a aceitar root `status: completed` (antes rejeitava FASE52/53 concluídas)
- **DOD-02**: `FASE53-arah-core.spec.yaml` — 10 ACs anotados com `covered_by` (unit + integração Core/Federation já existentes)
- **DOD-03**: `FASE54-iac.spec.yaml` — ACs de script com `status: manual` + `evidence`; `AC-54-4` com `covered_by`
- **DOD-04**: `FASE52-cicd.spec.yaml` — ACs de workflow com `status: manual` + `evidence`
- Fila `docs/_meta/PHASE_QUEUE.yaml` — entrada `dod-retrofit` (P1) para os itens abertos (DOD-05..10: PaymentService, unificação de ledger, Postgres, backfill de spec por risco, gate `covered_by`, evidência CI)
- `spec-validate` verde (6 specs)

### Adicionado — FASE55 payout consolidado + estorno (2026-07-02)

- **Estorno (AC-55-6)**: `POST /api/v1/transactions/{id}/refund` — reverte fee e split proporcionalmente; idempotente (só `Paid` → `Refunded`, segundo estorno retorna 409). `RefundService` + `CheckoutStatus.Refunded`
- **Payout consolidado (AC-55-5)**: `GET /api/v1/territories/{id}/payouts/consolidated?from=&to=` — agrega o split das transações pagas (exclui estornadas e fora do período) por destinatário. `PayoutConsolidationService` (read-model sobre checkouts)
- **DRY**: matemática de split extraída para `FeeSplitCalculator` (fonte única, banker's rounding + reconciliação); `TransactionQuoteService` e os novos serviços passam a usá-la
- Testes: `TransactionsControllerTests` (+4 refund: reverte, idempotente, não-pago 400, inexistente 404), `TerritoryPayoutsControllerTests` (+3: consolida/exclui refunded+fora do período, vazio, `from>to` 400)
- Spec `FASE55-monetization` — AC-55-5/6 `covered`; harness roda filtro `TerritoryPayoutsController`
- 22/22 testes FASE55 verdes
- **Dívida sinalizada**: nenhum código de produção leva `Checkout` a `Paid` (sem `PaymentService`/webhook); payout/refund unificar futuramente com `SellerPayoutService`/ledger `FinancialTransaction` (ver backlog retrospectiva DoD)

### Testes — FASE55 fechando ACs com evidência (2026-07-02)

- `TransactionsControllerTests` — integração HTTP de `POST /transactions/{id}/quote` (AC-55-2) e `GET .../receipt` (AC-55-3): 200 com split, 404 inexistente, 400 quando não pago, reconciliação soma==taxa
- `FeeSplitRuleTests` — imutabilidade/versionamento (AC-55-4): soma 100%, supersede uma via só, não ativa antes da vigência
- `TransactionQuoteServiceTests` — banker's rounding + reconciliação (sem centavo perdido)
- Spec `FASE55-monetization` anotada com `covered_by` por AC; harness roda filtros `TransactionsController` e `FeeSplitRule`
- 15/15 testes FASE55 verdes (`dotnet test --filter TransactionsController|FeeSplitRule|TransactionQuote`)

### Adicionado — Agentes de domínio autônomos + Definition of Done (2026-07-02)

- **Autonomia local**: hook `stop` (`.cursor/hooks.json` → `.cursor/hooks/domain-review.ps1`) aciona os agentes de domínio a cada interação, sem gatilho manual, gerando pareceres em `.cursor/domain-review.md`
- **Autonomia no CI**: `agents.yml` publica os pareceres de domínio como comentários (`post-domain-consult.ps1 -PostComment`) em todo PR `opened`/`synchronize`
- `scripts/agents/domain-autoreview.ps1` — resolve coreografia sobre o `git diff`, gera pareceres e evidência (idempotente por conjunto de mudanças)
- `.cursor/rules/domain-agents-autonomy.mdc` — rule `alwaysApply` com DoD + consulta de domínio obrigatória
- `docs/governance/DEFINITION_OF_DONE.md` — DoD formal (rigor total): AC↔teste, evidência, parecer de domínio endereçado
- Pareceres de domínio enriquecidos (checklists acionáveis) em `monetizacao-split`, `carteira-arata`, `mercado-economia`
- Cobertura de paths corrigida em `choreography.yaml` (Financial, Application/Services/Marketplace, Items)
- **Fix**: matcher de glob (`Test-PathMatchesGlob`) agora trata `**` em qualquer posição; parser de block scalar (`enrich:`/`validate:`) corrigido (`(?ms)` → `(?m)`)

### Documentação — FASE54/FASE55 sync (2026-06-30)

- `docs/ops/PILOT_STAGING_CONFIG_TODO.md` — checklist config manual (secrets, Stripe, board)
- `docs/ops/CI_CD_PIPELINE.md` — passo FASE54 no deploy staging
- `docs/API.md` — Core, transações, planos comerciais
- `docs/backlog-api/README.md`, `STATUS_FASES.md` — status 52–55 atualizado
- `docs/ops/PLATFORM_STATE.md` — prioridade S0 atual

### Adicionado — FASE55 monetização v0 (2026-06-30)

- `FeeSplitRule` versionada + seed por território
- `POST /api/v1/transactions/{id}/quote` e `GET .../receipt`
- `GET /api/v1/territories/{id}/plans` (planos comerciais)
- Gate comercial: `CommercialStoreGateService` bloqueia pagamentos sem `MarketplaceAdvanced`

### Adicionado — FASE54 provisionamento piloto (2026-06-30)

- `PilotAdminBootstrapHostedService` — SystemAdmin em Postgres staging (`Pilot__BootstrapAdminEnabled`)
- Scripts: `verify-pilot-instance.ps1`, `get-pilot-admin-token.ps1`, `backup-pilot-db.ps1`, `verify-stripe-sandbox.ps1`
- Deploy staging CI — passo FASE54 pós-health (registro Core + heartbeat + JWT)
- HTTPS opcional — Caddy profile em `infrastructure/pilot/`

### Adicionado — Fechamento de fases S0 e FASE54 piloto (2026-07-01)

- FASE53 ✅ — token de instância, `POST /core/releases`, `ICoreAvailabilityCache`
- `close-completed-phases.ps1` / `github-project close-phases` — fecha issues completed em ordem
- FASE54 iniciada — `infrastructure/pilot/`, spec `FASE54-iac`, script `register-core-instance.ps1`
- `Test-PhaseCompleteFromMeta` — meta YAML como fonte de conclusão

### Adicionado — FASE53 Arah Core MVP (2026-07-01)

- Registro de instância com par RSA (`RegisterCoreInstanceResponse` + `PublicKeyPem`)
- Diretório global: `POST/GET /api/v1/core/directory/territories`
- Federação base: `POST/GET /api/v1/federation/identity`
- Testes Core: 13 passando (unit + integração)
- FASE52 marcada completa em `PHASE_ROADMAP_META.yaml`

### Adicionado — Project board operacional completo (2026-07-01)

- `sync-status` — mapeia colunas Status (Backlog/Ready/In Progress/Done) no Project #3
- `dedupe-backlog` — fecha issues `epic/phase` duplicadas (REST)
- `Remove-OrphanProjectDrafts` — remove drafts quando issue canônica existe no board
- `export-phase-status` — 49 fases, REST paginado, summary + issue links
- `.github/phase-status.generated.json` — snapshot atualizado (16 done, 33 com issue aberta)
- Workflow `project-board-sync` — concurrency, export + commit do snapshot no dispatch

### Adicionado — GitHub Project PAT + bootstrap real (2026-07-01)

- **Causa raiz**: `GITHUB_TOKEN` não acessa Projects v2; bootstrap falhava silenciosamente
- GraphQL `Ensure-ProjectV2Bootstrap` + secret `GH_PROJECT_TOKEN` (PAT classic `project`+`repo`)
- Workflow dedicado `project-bootstrap.yml` (workflow_dispatch)
- `phase-status` como artifact CI; repo guarda só `project_number`/`project_url`
- `actions/add-to-project` em novas issues com `epic/phase`

### Adicionado — GitHub Project bootstrap (2026-07-01)

- `github-project bootstrap` — pipeline completo (Project v2, views, board, phase-status)
- `GitHub-ProjectApi.ps1`, `sync-project`, `reconcile`; workflow `project-board-sync.yml`
- `.github/phase-status.generated.json` — snapshot operacional das fases 52–61
- `PHASE_QUEUE.yaml` — `frente`, `spec_id`, deps federação (FASE58→infra, FASE59→Core+58)
- Coreografia: regras `infra-deploy` e `federation-handoff`

### Adicionado — Gestão via GitHub (Issues, Project, labels) (2026-07-01)

- `.github/labels.yml`, `.github/milestones.yml`, `.github/project/arah-sustentacao.yml`
- Templates `phase-epic.yml`, `config.yml` (issue chooser)
- Scripts: `sync-github-labels`, `sync-github-milestones`, `backlog-to-issue`, `github-project`, `export-phase-status`
- Workflow `github-sync.yml`; docs/ops/GITHUB_PROJECT_MANAGEMENT.md
- `next-phase` cria Issues `[Epic]` com labels wave/priority + milestone

### Adicionado — Coreografia + LikeC4 + domínio consultivo (2026-07-01)

- **`.agents/choreography.yaml`** — regras de co-ativação (Core, specs, marketplace, governance, monetization, PR sempre qa+pr-steward)
- **`choreograph-agents.ps1`**, **`post-domain-consult.ps1`**; `run-agent-activation` integrado
- Skills **`likec4-export`**, **`domain-consult`**; export LikeC4 + tokens design system
- **`docs/architecture/likec4/`** — modelo C4; CLI `arah-agents choreograph`
- Domain agents com `scope.paths` e parecer consultivo automático em PR

### Adicionado — Solutions Architect + ADRs (2026-07-01)

- Agente **`solutions-architect`** (Uncle Bob, LikeC4, consultivo)
- Skill **`register-adr`** + `scripts/agents/register-adr.ps1`
- ADRs individuais: `docs/architecture/adrs/` + **`ADR-REGISTRY.yaml`**
- **ADR-020**: Arah Core como control plane separado (FASE53)
- Gate `architecture-review-check.ps1` em `run-gates`
- Label `area/architecture`; co-roteamento automático em PRs estruturais

### Adicionado — Agentes SDD + Arah Core (2026-07-01)

- Co-roteamento orquestrador (`area/spec`, `co_route` para specs/Core)
- Domain `control-plane`, specialist `core-control-plane`
- Skills `spec-author`, `harness-run`; gate `spec-gate-check.ps1`
- Checklist dedicado `spec-steward`; agentes backend/qa/pr-steward/release/planner evoluídos

### Adicionado — Spec-Driven Design + Harness (2026-07-01)

- `docs/specs/` — specs YAML com critérios de aceite (FASE52, FASE53, operabilidade, agentes)
- `docs/_meta/SDD_AND_HARNESS.md`, `docs/ops/PLATFORM_STATE.md`
- Scripts `scripts/harness/validate-specs.ps1`, `run-harness.ps1`
- Workflow `.github/workflows/spec-harness.yml`
- Agente `spec-steward` + skill `spec-validate`
- CLI: `arah-agents harness`, `spec-validate`

### Adicionado — FASE53 Arah Core (início) (2026-07-01)

- Projeto `backend/Arah.Core` — entidades control plane (Instance, Release, HealthCheckReport)
- Endpoints `api/v1/core/instances`, heartbeat, releases
- Testes unitários em `Arah.Tests.Core`

### Adicionado — Operação por agentes (2026-06-30)

- `AGENTS.md`, `.agents/`, `.skills/`, `CODEOWNERS`, templates e scripts em `scripts/agents/`
- [docs/ops/AGENT_OPERATION.md](./ops/AGENT_OPERATION.md), [docs/_meta/DOC_TAXONOMY.md](./_meta/DOC_TAXONOMY.md)
- Workflow `.github/workflows/agents-validate.yml`
- Handoff HTML: [Operacao por Agentes](./handoff/Operacao%20por%20Agentes%20-%20Arah.dc.html)

### Adicionado — Sustentação Operacional C4 (2026-06-30)

- Pacote **Arquitetura C4 Arah** em `docs/handoff/arquitetura-c4/` (7 documentos HTML interativos)
- [Realinhamento Sustentação Operacional](./backlog-api/REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)
- Fases **52–61**: CI/CD, Core, IaC, monetização open-core, transparência, cockpit, operação, federação, capital
- Onda **S (Sustentação)** no roadmap — precede economia local (F17–19) até go-live piloto

### Alterado — Prioridades (2026-06-30)

- FASE15 reposicionada como base para billing comercial (FASE55)
- Economia local (F17–19) rebaixada de P0 para P1 até território-piloto no ar
- Roadmap v3.2 e backlog README atualizados

- Reorganização federal da documentação (estrutura por domínios, unificação de duplicados, archive de PRs).
- Changelog unificado (conteúdo de 40_CHANGELOG incorporado).

### Adicionado - App (Flutter): entregas recentes (2026-06)

- ✅ **Documentação de Arquitetura C4** publicada no DevPortal (`/architecture/`) e referenciada no Wiki (`docs/14_C4_ARCHITECTURE.md`).
- ✅ **Governança/Votações no app**: nova jornada `governance` no BFF (proxy para `api/v1/territories/{id}/votings`) e tela de votações (listar com filtro de status, votar inline, ver resultados, criar votação).
- ✅ **Criação de eventos no app**: formulário com início/término (date/time pickers) e local, via jornada `events/create-event`.
- ✅ **Deep-links no mapa**: toque no pin abre detalhes e navega para evento/asset/alerta/feed (tratamento de toque no nível do mapa).
- ✅ **Login com Google na UI**: botão "Entrar com Google" no fluxo de login (requer `GOOGLE_SIGN_IN_CLIENT_ID` + config Firebase para E2E).
- ✅ **Perfil — ações funcionais**: "Meu território" abre o seletor de território, "Notificações" leva à aba de notificações, e atalho "Governança" no menu de configurações (antes eram no-op).
- ✅ **Busca de territórios**: campo de busca no seletor (Explorar e folha "Meu território") usando a jornada `territories/search` — encontrar território por nome/cidade sem depender de geolocalização.
- ✅ **Busca de território no onboarding**: novo usuário pode buscar, selecionar e concluir o onboarding sem conceder localização (antes a conclusão dependia de geolocalização).
- ✅ **Detalhe de post**: tocar num card do feed abre a tela "Publicação" com conteúdo completo, todas as mídias, autor/data, contadores e ações (curtir/comentar/compartilhar/excluir).
- ✅ **GET de post único** (`feed/post-detail`): novo endpoint na API + jornada BFF que retorna um post no formato de item do feed; o deep-link de pin `post` no mapa agora abre o detalhe direto (rota `/post`), com modo *fetch* na tela quando o post não está no feed carregado.
- ✅ Documentação sincronizada: `README.md`, `docs/STABLE_RELEASE_APP_ONBOARDING.md`, `docs/FEATURE_MATRIX_API_BFF_APP.md`.

### Alterado - Fase 12 Encerrada (2026-01-25)

- ✅ **Fase 12 declarada 100% encerrada.** Todas as funcionalidades críticas entregues; melhorias contínuas (cobertura >90%, P95 &lt; 200ms) fora do escopo de fechamento.
- ✅ **FASE12.md**, **FASE12_RESULTADOS.md**: status 100%, tabelas atualizadas, conclusão e próximos passos do projeto (Fase 13+).
- ✅ **README.md**, **MAPA_FASES.md**, **02_ROADMAP.md**, **backlog-api/README.md**: Fase 12 **100% (encerrada)**; MVP Essencial **100% completo**.

---

### Adicionado - Fase 12 Próximos Passos (2026-01-25, branch `feature/fase12-proximos-passos`)

- ✅ **UserProfileStatsServiceTests** (7 testes): `GetStatsAsync` com repositórios null/parciais, agregação de posts/eventos/participações/memberships, exclusão de Visitor sem verificação.
- ✅ **TerritoryMediaConfigServiceEdgeCasesTests** (6 testes): `GetConfigAsync` (cria default, retorna existente), `UpdateConfigAsync` (mismatch território → exceção, válido → salva), `GetEffectiveContentLimitsAsync` / `GetEffectiveChatLimitsAsync`.
- ✅ **FASE12_RESULTADOS.md**: atualizado com ações realizadas e referência à branch e aos novos testes.

---

### Alterado - Fase 12 Documentação (2026-01-25)

- ✅ **FASE12.md**: status atualizado para ~98%; "Resumo da Fase 10" e "Critérios da Fase 10" corrigidos para Fase 12; tabelas de implementação alinhadas.
- ✅ **FASE12_RESULTADOS.md**: documento completo com métricas finais, referências, links para FASE12/README/MAPA_FASES/02_ROADMAP e implementação (compression, testes Analytics, FeatureFlag, MediaStorageConfig, UserPreferences).
- ✅ **README.md**, **MAPA_FASES.md**, **02_ROADMAP.md**, **backlog-api/README.md**: Fase 12 atualizada para ~98%; inclusão de Response Compression (gzip/brotli) na descrição.

---

### Adicionado - Fase 14: Governança Comunitária e Sistema de Votação (2025-01-21)

#### Funcionalidades
- ✅ **Sistema de Interesses do Usuário**
  - Usuários podem definir interesses (tags/categorias)
  - Máximo 10 interesses por usuário
  - Interesses aparecem no perfil
  - Endpoints: `GET/POST/DELETE /api/v1/users/me/interests`

- ✅ **Sistema de Votação Comunitária**
  - Votações para decisões coletivas (5 tipos: ThemePrioritization, ModerationRule, TerritoryCharacterization, FeatureFlag, CommunityPolicy)
  - Controle de visibilidade (AllMembers, ResidentsOnly, CuratorsOnly)
  - Aplicação automática de resultados
  - Endpoints completos: criar, listar, votar, fechar, obter resultados

- ✅ **Moderação Dinâmica Comunitária**
  - Regras de moderação definidas pela comunidade via votações
  - Aplicação automática na criação de conteúdo (posts, items)
  - Tipos de regras: ContentType, ProhibitedWords, Behavior, MarketplacePolicy, EventPolicy
  - Regras podem ser criadas via votações ou diretamente por curadores

- ✅ **Feed Filtrado por Interesses**
  - Filtro opcional de feed por interesses do usuário
  - Feed cronológico completo permanece como padrão
  - Query parameter `filterByInterests` no endpoint de feed

- ✅ **Caracterização do Território**
  - Tags que descrevem o território
  - Podem ser definidas via votações
  - Aparecem nas respostas de território

- ✅ **Histórico de Participação no Perfil**
  - Histórico de votações participadas
  - Contribuições para moderação comunitária
  - Endpoint: `GET /api/v1/users/me/profile/governance`

#### Validações e Segurança
- ✅ Validators FluentValidation para todos os requests
- ✅ Validação de permissões (resident/curador conforme tipo de votação)
- ✅ Validação de visibilidade e elegibilidade para votar

#### Testes
- ✅ Testes unitários: `UserInterestServiceTests`, `VotingServiceTests`
- ✅ Testes de integração: `GovernanceIntegrationTests`
- ✅ Cobertura >85% para funcionalidades de governança

#### Documentação
- ✅ `docs/GOVERNANCE_SYSTEM.md` - Visão geral do sistema
- ✅ `docs/VOTING_SYSTEM.md` - Documentação detalhada do sistema de votação
- ✅ `docs/COMMUNITY_MODERATION.md` - Moderação comunitária

---

### Adicionado - Fase 12 (2026-01-21)

#### Funcionalidades
- ✅ **Sistema de Políticas de Termos e Critérios de Aceite**
  - Versionamento de termos de uso e políticas de privacidade
  - Aceite obrigatório por papel, capability ou system permission
  - Auditoria completa (IP, User Agent, versão aceita)
  - Integração com funcionalidades críticas (Posts, Events, Stores)
  - API completa com endpoints de consulta e aceite

- ✅ **Exportação de Dados (LGPD)**
  - Endpoint `GET /api/v1/users/me/export` para exportação de dados pessoais
  - Exportação em formato JSON com todos os dados do usuário
  - Endpoint `DELETE /api/v1/users/me` para exclusão de conta
  - Anonimização completa de dados pessoais identificáveis
  - Documentação de conformidade LGPD (`docs/LGPD_COMPLIANCE.md`)

- ✅ **Analytics e Métricas de Negócio**
  - Estatísticas por território (`GET /api/v1/analytics/territories/{id}/stats`)
  - Estatísticas da plataforma (`GET /api/v1/analytics/platform/stats`)
  - Estatísticas do marketplace (`GET /api/v1/analytics/marketplace/stats`)
  - Métricas: posts, eventos, membros, vendas, payouts

#### Performance e Otimizações
- ✅ **Compression HTTP**
  - Gzip e Brotli compression implementados
  - Configuração otimizada (CompressionLevel.Optimal)
  - MIME types configurados (JSON, XML, CSS, JS)

- ✅ **Otimização de Serialização JSON**
  - `WriteIndented = false` em produção (reduz tamanho de payload)
  - `DefaultIgnoreCondition = WhenWritingNull` (reduz payload)
  - CamelCase naming policy

- ✅ **Índices de Performance**
  - Migration `AddPerformanceIndexes` com índices compostos para:
    - Feed (territory, status, created_at)
    - Eventos (territory, starts_at, status)
    - Participações (user, event)
    - Notificações (user, read_at, created_at)
    - Marketplace (stores, items, checkouts)
    - Analytics (aceites de termos e políticas)

#### Testes
- ✅ **Testes de Performance**
  - SLAs definidos e validados para endpoints críticos
  - Testes de carga (LoadTests) - 10 clientes, 30 requisições
  - Testes de stress (StressTests) - carga pico, extrema, concorrente
  - Documentação completa (`docs/PERFORMANCE_TEST_RESULTS.md`)

- ✅ **Cobertura de Testes**
  - **716 testes passando, 0 falhando, 2 pulados** (100% dos executáveis)
  - Suite completa: unitários, integração, segurança, performance

#### CI/CD e Operação
- ✅ **CI/CD Pipeline**
  - Build e testes automatizados
  - Code coverage com Codecov
  - Security scan com Trivy
  - Build e push de Docker para GHCR
  - Documentação (`docs/CI_CD_PIPELINE.md`)

- ✅ **Documentação de Operação**
  - `docs/OPERATIONS_MANUAL.md` - Procedimentos completos de deploy, rollback, backup, monitoramento
  - `docs/INCIDENT_RESPONSE.md` - Plano de resposta a incidentes (P0-P3)
  - `docs/CI_CD_PIPELINE.md` - Documentação do pipeline
  - `docs/TROUBLESHOOTING.md` - Mantido e atualizado

#### Documentação
- ✅ **Avaliação e Resultados**
  - `docs/FASE12_AVALIACAO_IMPLEMENTACAO.md` - Avaliação completa da implementação
  - `docs/PLANO_ACAO_10_10_RESULTADOS.md` - Resultados do plano de ação
  - `docs/backlog-api/FASE12_STATUS.md` - Status atualizado (85% completo)

---

## [1.0.0] - 2026-01-21

### Resumo das Fases Implementadas

#### Fase 1: Segurança e Fundação Crítica ✅
- Autenticação JWT
- Rate limiting
- Health checks
- Logging estruturado (Serilog)
- OpenTelemetry (tracing e metrics)
- Prometheus metrics

#### Fase 2: Qualidade de Código ✅
- FluentValidation
- Result pattern
- Error handling centralizado
- Middleware de segurança

#### Fase 3: Performance e Escalabilidade ✅
- Concorrência otimista (RowVersion)
- Cache distribuído (Redis com fallback para IMemoryCache)
- Paginação
- Batch operations
- Connection pooling

#### Fase 4: Observabilidade ✅
- Serilog com Seq
- OpenTelemetry
- Prometheus
- Health checks detalhados
- Correlation ID

#### Fase 5: Segurança Avançada ✅
- 2FA (TOTP)
- Sanctions automáticas
- User blocking
- Audit logging

#### Fase 6: Sistema de Pagamentos ✅
- Integração com gateway de pagamento
- Checkout
- Transações financeiras
- Reconciliation records

#### Fase 7: Sistema de Payout ✅
- Seller transactions
- Territory payout config
- Platform fee config
- Seller balance

#### Fase 8: Infraestrutura de Mídia ✅
- Upload de mídias (imagens, vídeos, áudios)
- Validação de MIME types e tamanhos
- Media attachments
- Media service completo

#### Fase 9-11: Pendentes
- Perfil de Usuário Completo
- Mídias em Conteúdo
- Edição e Gestão

#### Fase 12: Otimizações Finais ✅ (85%)
- Sistema de Políticas ✅
- Exportação de Dados (LGPD) ✅
- Analytics e Métricas ✅
- Testes de Performance ✅
- Otimizações de Performance ⚠️ (60%)
- CI/CD Pipeline ✅
- Documentação de Operação ✅
- Documentação Final ⚠️ (50%)

---

## [0.9.0] - 2025-01

### Adicionado
- Sistema base de territórios
- Feed comunitário
- Sistema de mapas
- Sistema de eventos
- Marketplace básico
- Sistema de chat
- Notificações

---

**Última Atualização**: 2026-01-21
