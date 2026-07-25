# ADR-024: Território comunitário vs referência administrativa; dados externos não autoritativos

**Status**: proposed  
**Data**: 2026-07-24  
**Autor**: Planejamento backlog (Dados Públicos Territoriais)  
**Spec-Id**: —  
**LikeC4 view**: —

---

## Contexto

[ADR-003](../10_ARCHITECTURE_DECISIONS.md) estabelece Territory como geográfico e neutro (sem lógica social). Com Public Data (CEP, município, IBGE, CNPJ), há risco de:

- reduzir o território comunitário ao município/UF/CEP;
- usar dado da Receita/IBGE como prova de residência, confiança ou representação legal;
- bloquear coletivos sem CNPJ.

## Decisão

1. **Referência administrativa é opcional e contextual** (`StateCode`, `MunicipalityIbgeCode`, etc. quando introduzidos). Não define identidade cultural, ecológica ou histórica do Territory.
2. **Dados externos são não autoritativos**: sugestão, referência ou snapshot sujeitos a confirmação e correção manual.
3. **CEP não prova residência**; **CNPJ não prova vínculo legal nem reputação comunitária**; verificação continua em Membership / WorkItem / processos humanos.
4. **Pessoa física e coletivo informal** permanecem tipos de primeira classe (PD-3).
5. Inteligência de sinais (trilha TI) e Public Data (trilha PD) **não** injetam política social na entidade Territory.

## Consequências

**Positivas**
- Coerência com visão território-first e ADR-003.
- Menor risco LGPD/falso positivo de “verificado”.
- Interoperabilidade com bases públicas sem colonizar o domínio.

**Negativas / trade-offs**
- Formulários mais complexos (sugerir + confirmar).
- Relatórios “por município IBGE” exigem joins/opcionalidade explícita.

## Alternativas consideradas

- **Territory = município IBGE** (rejeitada): apaga território comunitário.
- **CNPJ obrigatório para Store** (rejeitada): exclui economia informal local.
- **Confiar cegamente no snapshot** (rejeitada): dado público desatualiza e erra.

## Referências

- ADR-003 · [public-data-integration.md](../public-data-integration.md)
- [PD1](../../backlog-api/PD1.md) · [PD3](../../backlog-api/PD3.md)
- [01_PRODUCT_VISION](../../product/01_PRODUCT_VISION.md)
