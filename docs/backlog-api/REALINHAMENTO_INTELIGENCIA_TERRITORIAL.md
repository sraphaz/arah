# Realinhamento Estratégico — Inteligência Territorial (World Monitor)

**Versão**: 1.0  
**Data**: 2026-07-23  
**Origem**: Pacote *Arquitetura C4 Arah* (evolução) — trilha *Inteligência Territorial* + integração World Monitor  
**Status**: ✅ Aprovado para planejamento (execução após P0 de economia local 17–19, em paralelo a S1)

---

## Resumo executivo

A evolução C4 introduz a camada **Inteligência Territorial Regenerativa**: sinais globais → relevância territorial → validação humana → comunicação → ação → memória. O provedor inicial é o **World Monitor** (`api.worldmonitor.app`), consumido por API REST (MCP só para exploração por agentes).

O backlog comunitário (1–51) e a sustentação (52–61) **permanecem válidos**. A TI entra como **trilha transversal TI-0…TI-7**, dando corpo concreto a:

| Fase existente | Papel na TI |
|----------------|-------------|
| [FASE23](./FASE23.md) (IA) | Rascunho assistido de brief; nunca decide publicar |
| [FASE24](./FASE24.md) (Saúde Territorial) | Ciclo alerta → confirmação → ação → memória |
| [FASE44](./FASE44.md) (Integrações Externas) | Adapter World Monitor = Incremento TI-1 |
| [FASE53](./FASE53.md) (Arah Core) | Pré-requisito de TI-7 (federação de sinais) |

**Princípio**: revisão humana por padrão; fonte sempre visível; direito à contestação; sem ansiedade (só chega o que é relevante + útil + acionável); sem decisão opaca de IA; sem vigilância individual.

---

## Ordem de execução (recomendada)

```
P0 economia local (17–19)     ← não competir por squad
         │
TI-0 jurídico + fixtures      ← pode começar cedo (paralelo)
TI-1…TI-3 MVP (~13 semanas)   ← squad distinto
TI-4 participação
TI-5·6 governança + memória
TI-7 federação                ← só após FASE53 maduro
```

Ver [STATUS_FASES](../STATUS_FASES.md) · [Handoff TI](../handoff/inteligencia-territorial/).

---

## Incrementos (TI-0…TI-7)

| ID | Título | Esforço | Prioridade | Doc |
|----|--------|---------|------------|-----|
| TI-0 | Pesquisa e contratos | ~2 sem | P0 trilha | [TI0.md](./TI0.md) |
| TI-1 | Fundação (domínio + adapter WM) | ~4 sem | P0 | [TI1.md](./TI1.md) |
| TI-2 | Intelligence Inbox | ~4 sem | P0 | [TI2.md](./TI2.md) |
| TI-3 | Publicação territorial | ~5 sem | P0 | [TI3.md](./TI3.md) |
| TI-4 | Participação comunitária | ~4 sem | P1 | [TI4.md](./TI4.md) |
| TI-5 | Governança e ação | ~3 sem | P1 | [TI5.md](./TI5.md) |
| TI-6 | Aprendizado territorial | ~3 sem | P1 | [TI6.md](./TI6.md) |
| TI-7 | Federação de sinais | a dimensionar | P2 | [TI7.md](./TI7.md) |

**MVP**: TI-1 → TI-3 (≈ 13 semanas). Demo de aceite: cenário **Socorro-SP** (13 passos).

---

## Épicas e amostra de backlog

| ID | Item | Prio | Camada |
|----|------|------|--------|
| TI-E1 | Fundação de sinais externos | P0 | Domain·Infra |
| TI-101 | Persistir `ExternalSignal` normalizado | P0 | Domain·Infra |
| TI-102 | Configurar/desligar provedor (`emergencyOff`) | P0 | API·Admin |
| TI-103 | Ledger de quota + alarme 80% | P0 | Infra·Obs |
| TI-104 | Spike geometrias GDACS/USGS | P0 | Infra |
| TI-E2 | Inbox e revisão | P0 | App·Web admin |
| TI-201 | WorkItem `SignalReview` + Inbox | P0 | Application·Web |
| TI-202 | Score de relevância explicável | P0 | Application |
| TI-E3 | Publicação territorial | P0 | Full stack |
| TI-301 | Editor de brief + publicação | P0 | Web·API |
| TI-302 | Alerta no mapa/feed (Flutter) | P0 | Flutter |
| TI-303 | Preferências de notificação | P0 | Flutter·BFF |
| TI-E4 | Participação comunitária | P1 | Full stack |
| TI-401 | Confirmar/contestar com evidência | P1 | Flutter·API |
| TI-701 | Testes de contrato + drift OpenAPI WM | P0 | CI/CD |
| TI-702 | Threat model vivo (mutação) | P0 | Tests |

Backlog completo e critérios: [Roadmap e Backlog (HTML)](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Roadmap%20e%20Backlog.dc.html).

---

## Specs SDD (Spec Harness)

Doze specs `TI-*` em `docs/specs/` — spec antes do código:

| Spec | Título |
|------|--------|
| TI-101 | Ingestão e normalização |
| TI-102 | Gestão de provedor + emergência |
| TI-103 | Quota e custo |
| [TI-201](../specs/features/TI-201-signal-review.spec.yaml) | Revisão humana (exemplo completo) |
| TI-202 | Relevância explicável |
| TI-301 | Brief e publicação |
| TI-302 | Alerta no app |
| TI-303 | Preferências e notificação |
| TI-401 | Verificação comunitária |
| TI-402 | Evidência e privacidade |
| TI-501 | Ação e validação institucional |
| TI-601 | Memória territorial |

---

## Integração World Monitor (resumo)

| Aspecto | Decisão |
|---------|---------|
| Canal MVP | **REST** (`OpenAPI`); MCP só para `signal-scout` |
| Auth | Chave `wm_` por instância (`X-WorldMonitor-Key`) |
| Plano MVP | API **US$ 99,99/mês** (1.000 req/dia) |
| Licença | Consumo via API ≠ AGPL no backend; atribuição obrigatória |
| Resiliência | Circuit breaker, cache por freshness, ledger de quota |
| CI | Fixtures gravadas; **zero** chamada real |

Detalhe: [Integração World Monitor](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Integracao%20World%20Monitor.dc.html) · [Notas de pesquisa](../handoff/inteligencia-territorial/ti-research-notes.md).

---

## Domínio (âncoras)

Agregados: `ExternalSignal`, `TerritorialAlert`.  
Entidades: `SignalProvider`, `TerritorialImpactAssessment`, `TerritorialBrief`, `CommunityVerification`, `PublicationDecision`, `IntelligencePolicy`.  
Reuso: fila **WorkItem** existente para `SignalReview`.  
Flag: `INTELLIGENCE` (off por padrão por território).

### Multiagente (Harness · consultivo)

| Agente | Papel | Publica? |
|--------|-------|----------|
| `signal-scout` | Registra sinais (Received) | Não |
| `territorial-analyst` | Relevância/impacto explicável | Não |
| `source-steward` | Confiança/freshness de fonte | Não |
| `community-brief-writer` | Rascunho de brief (marcado IA) | Não |
| `response-orchestrator` | Sugere ações (não executa) | Não |
| `intelligence-governance-steward` | Audita invariantes | Não |

Coreografia: rule `inteligencia-territorial` em [`.agents/choreography.yaml`](../../.agents/choreography.yaml).

Territory permanece **geográfico e neutro** — relevância e política ficam em Intelligence/Membership, nunca na entidade Territory.

Handoff técnico: [Handoff de Desenvolvimento](../handoff/inteligencia-territorial/Inteligencia%20Territorial%20-%20Handoff%20de%20Desenvolvimento.dc.html).

---

## Relação com ondas existentes

```
Onda S0–S1 (sustentação)     ──►  go-live piloto
Onda comunitária 17–19       ──►  economia local (P0 squad A)
Trilha TI-0…TI-3 (paralela)  ──►  MVP inteligência (squad B)
FASE23 / FASE24              ──►  ganham corpo via TI-2…TI-4
FASE44                       ──►  TI-1 (adapter WM)
FASE53 maduro                ──►  desbloqueia TI-7
```

---

## Riscos e dependências

| Risco | Mitigação |
|-------|-----------|
| Parecer jurídico (ToS/marca WM) atrasar | TI-1/2 seguem com mock; TI-0 não bloqueia código |
| Falsos positivos de relevância | Pesos por config, não por deploy |
| Push FCM pendente no app | In-app + feed valem como aceite de TI-3 |
| Brigading em contestações | 1/morador + threshold reabre revisão |
| Prematuridade de federação | TI-7 só pós-FASE53 |
| Quota / custo API | Ledger 80%; upgrade só com demanda medida |

---

## Definição de pronto (MVP TI-1…TI-3)

- [ ] Provider configurável e desligável; zero chamada real em CI
- [ ] Sinal mock → normalizado → WorkItem → decisão auditada
- [ ] Publicação gera alerta + post + notificação por preferência
- [ ] Bloco de fonte em 100% dos alertas derivados de provedor
- [ ] Stale bloqueia publicação; timeline append-only
- [ ] Cenário Socorro passos 1–7 end-to-end com seed

---

## Referências

- [Handoff TI — índice](../handoff/inteligencia-territorial/README.md)
- [Handoff C4](../handoff/README.md)
- [REALINHAMENTO_SUSTENTACAO_OPERACIONAL](./REALINHAMENTO_SUSTENTACAO_OPERACIONAL.md)
- [Arquitetura C4 (HTML atualizado)](../handoff/arquitetura-c4/Arquitetura%20C4%20Arah.dc.html)
- [Backlog Atualizado (HTML)](../handoff/arquitetura-c4/Backlog%20Atualizado%20-%20Arah.dc.html)
