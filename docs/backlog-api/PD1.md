# PD-1: Territorial Addressing (CEP, UF, municípios, IBGE, DDD)

**Duração**: ~3–4 semanas  
**Prioridade**: 🔴 P0  
**Trilha**: Dados Públicos Territoriais  
**Depende de**: [PD-0](./PD0.md)  
**Ancora**: onboarding IBGE existente · Territory · AddressDto · GeoAnchors  
**Status**: ⏳ Pendente

---

## Contexto

Cadastros de endereço e território usam texto livre (`City`/`State`, `ZipCode`). IBGE já resolve município/malha no onboarding, mas **não** há CEP assistido nem código IBGE persistido de forma canônica no Territory.

## Problema

Erros de grafia, CEP inconsistente, sem proveniência; risco de tratar CEP/IBGE como definição do território comunitário.

## Resultado esperado

Preenchimento assistido e referência administrativa **opcional**, preservando identidade comunitária do Territory (ADR-024).

## Escopo

### Histórias

**Endereço assistido por CEP**  
Como pessoa cadastrando endereço, quero informar o CEP e receber sugestão, para reduzir erros.  
Critérios: CEP normalizado; consulta só via API Arah; cache; correção manual; falha externa não bloqueia; origem/data registradas.

**Município normalizado**  
Como plataforma, quero relacionar territórios a códigos oficiais, para interoperar.  
Critérios: Territory pode ter `MunicipalityIbgeCode` (opcional); identidade comunitária permanece; migração segura; evitar municípios duplicados na referência.

### Técnico

- `IAddressDataProvider` → `BrasilApiAddressDataProvider`
- `IAdministrativeRegionProvider` (estados/municípios) — convergir com `IIbgeBoundaryResolver` onde fizer sentido
- DDD como capacidade auxiliar (sugestão de contato), não produto isolado
- Atualizar formulários: loja, evento, recurso, checkout, onboarding (onde aplicável)
- Migração/backfill cuidadoso de territórios existentes

## Fora do escopo

- CEP como prova de residência / membership
- Substituir BoundaryPolygon / GeoAnchor por CEP
- Página isolada “consulta CEP”
- CNPJ / feriados (PD-2/PD-3)

## Dependências

- PD-0 · Territory · Membership (não misturar) · Flutter/Web forms · BFF journeys

## Critérios de aceite

- [ ] Lookup CEP via backend com cache/snapshot
- [ ] Listagem UF e municípios por UF normalizada
- [ ] Territory aceita referência IBGE opcional sem mudar ADR-003
- [ ] DDD disponível como API interna auxiliar (se exposto, rate-limited)
- [ ] Fallback manual em todos os formulários tocados
- [ ] Testes: normalização CEP, cache hit, provider down, payload incompleto, correção manual

## Requisitos não funcionais

- Privacidade: não logar endereço completo em claro desnecessariamente
- A11y: autocomplete operável por teclado/leitor de tela
- i18n: labels PT; códigos IBGE como dados BR

## Riscos

- Divergência BrasilAPI vs IBGE oficial · dados desatualizados · rate limit externo

## Estratégia de testes

Unit + integração + contract fixtures + testes de UI/journey (loading/fallback)

## Definição de pronto

DoD + docs de API + STATUS sync.

## Referências

- [REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS](./REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)
- [PD0](./PD0.md) · [PD2](./PD2.md)
- `MunicipalityTerritoryProvisioningService` · `IbgeBoundaryResolver`
