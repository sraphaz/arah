# Inteligência Territorial — Handoff interativo

**Versão**: 1.0  
**Data**: 2026-07-23  
**Origem**: Evolução do pacote *Arquitetura C4 Arah* — integração World Monitor

Documentação interativa da trilha **TI-0…TI-7**. Design system compartilhado com [arquitetura-c4/_ds](../arquitetura-c4/_ds/).

Abra no navegador (servidor local). Comece pelo **Hub**.

---

## Documentos

| Material | Arquivo | Público |
|----------|---------|---------|
| **Hub** | [Inteligencia Territorial - Hub.dc.html](./Inteligencia%20Territorial%20-%20Hub.dc.html) | Todos |
| Visão estratégica | [Visao Estrategica](./Inteligencia%20Territorial%20-%20Visao%20Estrategica.dc.html) | Produto, exec |
| Estado atual e impacto | [Estado Atual e Impacto](./Inteligencia%20Territorial%20-%20Estado%20Atual%20e%20Impacto.dc.html) | Eng, produto |
| Capability map | [Capability Map](./Inteligencia%20Territorial%20-%20Capability%20Map.dc.html) | Produto, eng |
| Arquitetura e domínio | [Arquitetura e Dominio](./Inteligencia%20Territorial%20-%20Arquitetura%20e%20Dominio.dc.html) | Backend, arquitetura |
| Jornadas e UX | [Jornadas e UX](./Inteligencia%20Territorial%20-%20Jornadas%20e%20UX.dc.html) | Flutter, design |
| Protótipos | [Prototipos](./Inteligencia%20Territorial%20-%20Prototipos.dc.html) | Design, Flutter |
| Deck executivo | [Deck Executivo](./Inteligencia%20Territorial%20-%20Deck%20Executivo.dc.html) | Exec, BNDES |
| Integração World Monitor | [Integracao World Monitor](./Inteligencia%20Territorial%20-%20Integracao%20World%20Monitor.dc.html) | Backend, segurança |
| Agentes e salvaguardas | [Agentes e Salvaguardas](./Inteligencia%20Territorial%20-%20Agentes%20e%20Salvaguardas.dc.html) | Agentes, jurídico |
| Handoff de desenvolvimento | [Handoff de Desenvolvimento](./Inteligencia%20Territorial%20-%20Handoff%20de%20Desenvolvimento.dc.html) | Backend, Flutter, web |
| Roadmap e backlog | [Roadmap e Backlog](./Inteligencia%20Territorial%20-%20Roadmap%20e%20Backlog.dc.html) | Planejamento, PM |
| Notas de pesquisa (fonte) | [ti-research-notes.md](./ti-research-notes.md) | Eng, jurídico |

---

## Servir localmente

```powershell
cd docs/handoff
python -m http.server 8080
# Hub: http://localhost:8080/inteligencia-territorial/Inteligencia%20Territorial%20-%20Hub.dc.html
```

---

## Integração com o backlog

- **[REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md](../../backlog-api/REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)** — ondas, âncoras FASE23/24/44/53
- **TI-0…TI-7**: [TI0](../../backlog-api/TI0.md) … [TI7](../../backlog-api/TI7.md)
- Spec exemplo: [TI-201](../../specs/features/TI-201-signal-review.spec.yaml)
- C4 atualizado (container Intelligence): [Arquitetura C4](../arquitetura-c4/Arquitetura%20C4%20Arah.dc.html)
- Backlog HTML atualizado (bloco TI): [Backlog Atualizado](../arquitetura-c4/Backlog%20Atualizado%20-%20Arah.dc.html)

---

## Princípios (resumo)

1. Revisão humana por padrão (WorkItem `SignalReview`)
2. Fonte sempre visível (origem, data, validade, confiança, freshness)
3. Direito à contestação com evidência
4. Sem ansiedade — só sinal relevante + útil + acionável
5. Sem decisão opaca de IA; LLM só rascunha com rastro
6. Territory geográfico e neutro — inteligência fora da entidade Territory
7. Consumo WM via API (sem incorporar AGPL); atribuição obrigatória
