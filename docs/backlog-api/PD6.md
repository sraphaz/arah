# PD-6: Specialized Marketplaces (FIPE e categorias)

**Duração**: a dimensionar  
**Prioridade**: 🟢 P3  
**Trilha**: Dados Públicos Territoriais  
**Depende de**: [PD-0](./PD0.md) · categorias reais de marketplace (veículos/máquinas/etc.)  
**Status**: ⏳ Pendente — **só ativar com demanda de produto**

---

## Contexto

FIPE só faz sentido com categorias de veículos, motos, máquinas agrícolas, transporte comunitário ou equipamentos motorizados.

## Problema

Adotar FIPE “porque o endpoint existe” polui o núcleo territorial.

## Resultado esperado

Referência de preço **informativa** (não avaliação definitiva) em categorias especializadas.

## Escopo (se ativado)

- Port de referência de preço veicular
- Comparação opcional com valor anunciado
- Histórico de tabela quando disponível via snapshot

## Fora do escopo

- Avaliação oficial / perícia
- Obrigar FIPE em todo item
- Implementar sem categoria real no catálogo

## Critérios de aceite

- [ ] Feature flag por categoria
- [ ] Copy: “referência de mercado, não laudo”
- [ ] Fallback se tabela indisponível

## Referências

- [REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS](./REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)
