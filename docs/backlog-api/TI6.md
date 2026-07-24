# TI-6: Aprendizado territorial (memória)

**Duração**: ~3 semanas  
**Prioridade**: 🟡 P1  
**Trilha**: Inteligência Territorial  
**Depende de**: [TI4](./TI4.md) (TI-5 pode paralelizar)  
**Ancora fases**: [FASE24](./FASE24.md), métricas de serviço (não engajamento)  
**Status**: ⏳ Pendente

---

## Objetivo

Encerramento vira memória: histórico consultável, padrões por categoria/estação, confiança de fonte ajustada por desempenho — **sem métricas de vício**.

---

## Entregas

- Read model de memória territorial
- Tela de histórico no app/admin
- Relatório por categoria/estação
- Ajuste de confiança de fonte por correções/contestações
- Spec TI-601
- Métricas `ti_*` (serviço): revisão p95, precisão geo, stale ratio, custo/quota
- Cenário Socorro passo 13

---

## Critérios de aceite

- [ ] Memória consultável por categoria e período
- [ ] Correção nunca apaga histórico (marca)
- [ ] Nenhuma métrica de engajamento-por-engajamento no painel
- [ ] Metas piloto documentadas: p95 revisão < 60 min · precisão > 90% · zero pub sem decisão

---

## Referências

- [REALINHAMENTO_INTELIGENCIA_TERRITORIAL](./REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)
- [TI7](./TI7.md)
