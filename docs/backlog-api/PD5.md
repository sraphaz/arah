# PD-5: Territorial Climate Intelligence

**Duração**: ~4–6 semanas (a dimensionar)  
**Prioridade**: 🟢 P2  
**Trilha**: Dados Públicos Territoriais  
**Depende de**: [PD-0](./PD0.md) · [FASE24](./FASE24.md) (saúde/sensores) · alinhamento com trilha [TI](./REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)  
**Status**: ⏳ Pendente (não antecipar sem demanda FASE24/TI)

---

## Contexto

FASE24 prevê sensores `WEATHER` e indicadores; TI trata sinais/alertas com revisão humana. BrasilAPI/CPTEC é **uma** fonte possível de previsão — não a única.

## Problema

Misturar previsão, alerta oficial, observação comunitária e ocorrência confirmada gera falsa urgência e acoplamento.

## Resultado esperado

Abstração multi-provider:

```text
TerritorialWeatherService (Application)
├── BrasilAPI/CPTEC (adapter)
├── outros provedores
├── Defesa Civil (futuro)
└── observações comunitárias (FASE24)
```

## Escopo (potencial)

- Previsão na página do território (opt-in)
- Contextualizar eventos ao ar livre
- Apoio a alertas com validade temporal
- Confirmação por moradores (reuso TI-4 / FASE24)

## Fora do escopo

- BrasilAPI como única fonte
- Alertas sem revisão/fonte visível
- Decisão automática de evacuação/ação institucional

## Diferenciação obrigatória

| Tipo | Autoridade |
|------|------------|
| Previsão | Informativa |
| Alerta oficial | Fonte + atribuição |
| Observação comunitária | Confirmável |
| Ocorrência confirmada | Após processo territorial |

## Critérios de aceite

- [ ] Port `ITerritorialWeatherProvider` (nome final na spec)
- [ ] Pelo menos 1 adapter + mock; segundo provider stub
- [ ] UI distingue tipos de informação climática
- [ ] Integração com FASE24/TI documentada (sem duplicar SignalProvider)

## Riscos

- Ansiedade / spam de alertas · dados marítimos/aeroportos sem caso de uso

## Referências

- [FASE24](./FASE24.md) · [TI3](./TI3.md)–[TI4](./TI4.md)
- [REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS](./REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)
