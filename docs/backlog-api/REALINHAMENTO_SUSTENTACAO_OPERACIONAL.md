# Realinhamento Estratégico — Camada de Sustentação Operacional

**Versão**: 1.0  
**Data**: 2026-06-30  
**Origem**: Pacote *Arquitetura C4 Arah* + *Backlog Atualizado*  
**Status**: ✅ Aprovado para execução

---

## Ordem de execução (atualizada 2026-06-30)

```
0. Operação por agentes (PR 1–4)  ← AGENTS.md, .agents/, .skills/
1. Onda S0 — FASE52–54 (CI/CD, Core, IaC)
2. Onda S1 — FASE55–57 (receita, cockpit)
3. Ondas comunitárias 17+ (após piloto)
```

Ver [docs/ops/AGENT_OPERATION.md](../ops/AGENT_OPERATION.md).

---

## Resumo executivo

A documentação C4 e os handoffs de monetização/operação introduzem uma **camada de sustentação** que a plataforma precisa antes de operar territórios reais em produção. O backlog comunitário (fases 1–51) permanece válido; ganha **ganchos de receita** e **dependências de infraestrutura** explícitas.

| Métrica | Valor |
|---------|-------|
| Novas frentes (épicos) | 7 (+ fundação técnica) |
| Novos itens de backlog | ~40 |
| Fases existentes tocadas | 9 (ganchos, sem reescrita) |
| Novas fases documentadas | 52–61 |
| Ondas de entrega | 5 (Onda S0–S4) |

**Princípio**: a camada de sustentação **não atrasa** funcionalidades comunitárias — ela **viabiliza** financiá-las e colocá-las no ar.

---

## O que mudou

### 1. Fundação técnica (novo bloqueador)

A plataforma **ainda não roda em produção**. Antes de escalar funcionalidades comunitárias, é necessário:

- CI/CD formal (GitHub Actions, GHCR)
- Arah Core mínimo (registro, identidade, releases, ledger)
- Infraestrutura como código + 1ª instância gerenciada
- Observabilidade, backups e gateway PIX em produção

**Ref.**: [FASE52](./FASE52.md) · [FASE53](./FASE53.md) · [FASE54](./FASE54.md) · [Plano Operacional](../handoff/arquitetura-c4/Plano%20Operacional%20e%20Pipelines%20-%20Arah.dc.html)

### 2. Monetização open-core

Modelo de receita sem custo ao morador:

- Assinaturas **comerciais** (habilitar loja/vender)
- Taxa transacional com **split** (implementador / fundo / plataforma)
- **Carteira Aratá** com ledger, payouts automáticos
- Medição de consumo (IA, mídia, notificações)
- Patrocínios e doações

**Ref.**: [FASE55](./FASE55.md) · [Adendo Monetização](../handoff/arquitetura-c4/Adendo%20de%20Monetizacao%20-%20Handoff%20Arah.dc.html)

### 3. Cockpit do implementador

Painel web de gestão: territórios, comércios, receita/repasses, onboarding, formação, operações.

**Ref.**: [FASE57](./FASE57.md) · [Cockpit](../handoff/arquitetura-c4/Cockpit%20do%20Implementador%20Arah.dc.html)

### 4. Multi-instância e federação

Modelo **Core + instâncias** (gerenciada/soberana): provisionamento, deploy, saúde, federação opt-in entre territórios.

**Ref.**: [FASE58](./FASE58.md) · [FASE59](./FASE59.md) · [Anexo Operação](../handoff/arquitetura-c4/Anexo%20Handoff%20-%20Operacao%20Instancias%20e%20Federacao.dc.html)

---

## Frentes de trabalho (épicos)

| Épico | Itens | Fases | Handoff |
|-------|-------|-------|---------|
| Plataforma & fundação técnica | 6 | 52–54 | Plano Operacional |
| Monetização & billing | 8 | 55 (+ F15, F6, F7) | Adendo A3–A7 |
| Transparência & governança de taxas | 5 | 56 (+ F14) | Adendo A6, A8 |
| Cockpit do implementador (web) | 7 | 57 | Adendo A12 + Cockpit |
| App — papel do implementador | 4 | 60 | Adendo A5, A12 |
| Operação, instâncias & federação | 8 | 58–59 | Anexo O1–O10 |
| Capital territorial | 2 | 61 | Adendo A9 |

---

## Impacto nas fases existentes

| Fase / domínio | O que muda | Tipo |
|----------------|------------|------|
| Marketplace / checkout (F6) | Taxa, split visível e repasse no checkout | Alterada |
| Minha loja | Exige assinatura comercial (plano Loja/Pro) para vender | Alterada |
| Carteira / Moeda (F22) | Ledger de taxas, split por regra, payouts automáticos | Ampliada → F55 |
| Assinaturas (F15) | Reposicionada: billing comercial + apoio recorrente | Alterada |
| Governança (F14) | Novo objeto: proposta/votação de taxas e splits | Ampliada → F56 |
| Métricas (F24/25) | Alimenta painel de transparência público | Ampliada → F56 |
| Assistente IA / Mídia (F23, F10) | Consumo medido e cobrável por uso | Alterada |
| Arquitetura / Deploy | De instância única para multi-instância federada | Estrutural → F52–59 |
| Identidade & Auth (F1, F5) | Identidade federada (login entre territórios) | Ampliada → F53, F59 |
| Economia local (F17–19) | **Rebaixada para P1** até go-live piloto (S0–S1) | Prioridade |
| Serviços territoriais (F50–51) | Permanecem **P2** — após sustentação | Prioridade |

---

## Backlog priorizado (consolidado)

### P0 — Destrava primeira receita e go-live

| Item | Frente | Fase |
|------|--------|------|
| CI/CD + GHCR + ambientes dev/staging | Fundação técnica | 52 |
| Arah Core mínimo (registro, identidade, releases) | Fundação técnica | 53 |
| IaC + 1ª instância gerenciada | Fundação técnica | 54 |
| Planos & billing comercial (assinar loja) | Monetização | 55 (+ F15) |
| Carteira Aratá: ledger, taxa & split | Monetização | 55 |
| Payouts automáticos por período | Monetização | 55 (+ F7) |
| Painel de transparência público | Transparência | 56 |
| Cockpit: visão geral, territórios, receita | Cockpit | 57 |
| Registro de instância & health no Core | Operação | 58 |

### P1 — Confiança e operação do dia a dia

| Item | Frente | Fase |
|------|--------|------|
| Medição de consumo (IA/mídia/notif.) | Monetização | 55 |
| Governança de taxas (proposta/votação) | Transparência | 56 (+ F14) |
| Cockpit: onboarding, operações, configurações | Cockpit | 57 |
| App: visão leve do implementador | App | 60 |
| Deploy com migração & rollback automático | Operação | 58 |
| Patrocínio & doação | Monetização | 61 |
| Emails transacionais (F13) | Comunicação | 13 ✅ |

### P2 — Escala rede e capital

| Item | Frente | Fase |
|------|--------|------|
| Federação entre instâncias (opt-in) | Operação | 59 |
| Modo soberano (self-hosted) completo | Operação | 59 |
| Fundo territorial & aportes | Capital | 61 |
| Cockpit: formação & mentoria | Cockpit | 57 |
| Economia local (F17–19) | Comunitário | 17–19 |
| Serviços territoriais (F50–51) | Comunitário | 50–51 |

---

## Roadmap em ondas (Sustentação)

### Onda S0 — Fundação técnica (semanas 1–8) 🔴 CRÍTICO

**Objetivo**: a plataforma sai do papel e sobe.

- CI/CD + GHCR
- Arah Core mínimo
- IaC + 1ª instância
- Observabilidade

**Fases**: 52, 53, 54

---

### Onda S1 — Fundação de receita (meses 2–4) 🔴 CRÍTICO

**Objetivo**: o território fatura pela primeira vez.

- Billing comercial
- Carteira Aratá + split
- Payouts
- Cockpit núcleo
- Registro de instância

**Fases**: 55, 57 (MVP), 58 (health)

**Marco**: território-piloto (ex.: Camburi) no ar com comércios assinando.

---

### Onda S2 — Transparência & gestão (meses 3–6) 🟡 ALTA

**Objetivo**: confiança e operação do dia a dia.

- Painel de transparência
- Governança de taxas
- Consumo medido
- Cockpit completo
- Deploy + rollback

**Fases**: 56, 57 (completo), 58 (deploy)

---

### Onda S3 — Rede & federação (meses 6–12) 🟡 ALTA

**Objetivo**: crescer para múltiplos territórios.

- Federação opt-in
- Identidade federada
- App do implementador
- Modo soberano
- Patrocínios

**Fases**: 59, 60, 61 (parcial)

---

### Onda S4 — Escala & capital (12 meses+) 🟢 MÉDIA

**Objetivo**: sustentar expansão.

- Fundo territorial
- Coordenação regional
- Marketplace inter-territorial
- Observabilidade avançada

**Fases**: 61, 59 (marketplace federado)

---

## Relação com ondas comunitárias existentes

```
Onda S0–S1 (sustentação)  ──►  Go-live piloto
         │
         ├── Paralelo: F13 ✅, F14 ✅, F15 ✅, F16 ✅ (base comunitária pronta)
         │
Onda S2 (transparência)   ──►  Confiança da comunidade
         │
Onda comunitária 17–19    ──►  Economia local (após piloto faturando)
         │
Onda S3–S4                ──►  Rede multi-território
         │
Fases 20+                 ──►  Expansão funcional conforme roadmap original
```

---

## Riscos e dependências

| Risco | Mitigação |
|-------|-----------|
| **Conformidade financeira** (PSP, KYC, NF) | Tratar na Onda S1; consultoria regulatória cedo |
| **Complexidade multi-instância** | Começar managed; soberano só na Onda S3 |
| **Dependência do Core** | Modo autônomo da instância; cache de identidade |
| **Governança das taxas** | Processo de conselho maduro; nunca retroagir regras |

---

## Definição de pronto (go-live piloto)

- [ ] Morador usa o app por completo sem cobrança ou bloqueio
- [ ] Toda taxa exibida antes do pagamento e no comprovante, com split visível
- [ ] Painel de transparência em tempo real, exportável
- [ ] Splits e payouts server-side, idempotentes, auditáveis no ledger
- [ ] Alterações de taxa exigem proposta + votação; nunca retroagem
- [ ] Instância registra-se, health verde, backup verificado antes de deploy
- [ ] Território opera com Core indisponível (modo autônomo)

---

## Referências

- [Handoff — índice](../handoff/README.md)
- [Arquitetura C4 (HTML)](../handoff/arquitetura-c4/Arquitetura%20C4%20Arah.dc.html)
- [Backlog Atualizado (HTML)](../handoff/arquitetura-c4/Backlog%20Atualizado%20-%20Arah.dc.html)
- [Roadmap estratégico](../02_ROADMAP.md)
- [Backlog API](./README.md)
