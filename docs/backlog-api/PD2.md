# PD-2: Territorial Calendar (feriados nacionais)

**Duração**: ~1–2 semanas  
**Prioridade**: 🟡 P1  
**Trilha**: Dados Públicos Territoriais  
**Depende de**: [PD-0](./PD0.md) (idealmente após ou em paralelo final de PD-1)  
**Ancora**: Events · Stores (horários) · notificações  
**Status**: ⏳ Pendente

---

## Contexto

Eventos e operação territorial se beneficiam de calendário nacional, sem virar “app de feriados”.

## Problema

Não há referência de feriados; organizadores não são avisados quando um evento cai em feriado.

## Resultado esperado

`ICalendarReferenceProvider` com cache anual; avisos **não bloqueantes** em criação/edição de evento e, opcionalmente, horários especiais de loja.

## Escopo

### História — Evento em feriado

Como organizador, quero ser avisado quando o evento ocorrer em feriado nacional, para planejar comunicação.  
Critérios: aviso não bloqueia; cache; ano futuro consultável; indisponibilidade externa não impede criação.

### Técnico

- `BrasilApiCalendarReferenceProvider`
- Snapshots por ano
- Integração leve em fluxo de Events (e hooks futuros para lojas/notificações)

## Fora do escopo

- Página isolada de feriados
- Feriados municipais/estaduais (avaliar depois com outra fonte)
- Bloqueio hard de publicação

## Dependências

- PD-0 · módulo Events · (opcional) notificações FASE12/13

## Critérios de aceite

- [ ] ListHolidays(year) via API Arah com cache
- [ ] UI/API de evento sinaliza conflito de feriado (warning)
- [ ] Provider down → criação segue sem warning ou com “calendário indisponível”
- [ ] Testes: ano bissexto/limites, cache, timeout, payload vazio

## Riscos

- Lista incompleta vs realidade local · mudança de legislação

## Definição de pronto

DoD do repositório.

## Referências

- [REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS](./REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)
- [PD1](./PD1.md) · [PD3](./PD3.md)
