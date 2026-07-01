# Changelog - Arah

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/lang/pt-BR/).

---

## [Unreleased]

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
