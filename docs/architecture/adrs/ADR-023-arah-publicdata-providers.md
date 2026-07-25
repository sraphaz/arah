# ADR-023: Capacidade Arah.PublicData — providers, cache e proveniência

**Status**: proposed  
**Data**: 2026-07-24  
**Autor**: Planejamento backlog (Dados Públicos Territoriais)  
**Spec-Id**: — (specs PD-* a criar antes da implementação SDD)  
**LikeC4 view**: —

---

## Contexto

O backlog incorpora dados públicos brasileiros (inicialmente via [BrasilAPI](https://brasilapi.com.br/)) para enriquecer endereços, referência administrativa, calendário, organizações e, no futuro, clima e extensões. Já existem clientes HTTP pontuais (`IIbgeBoundaryResolver`, Nominatim) sem uma capacidade transversal de cache, proveniência e resiliência.

Há risco concreto de emergir um `BrasilApiService` monolítico e de o frontend chamar a API externa diretamente.

## Decisão

1. Introduzir a capacidade interna **Arah.PublicData** (trilha de backlog **PD-0…PD-8**), separada da trilha **TI** (World Monitor / sinais).
2. Expor **ports orientados a capacidade** no Application (`IAddressDataProvider`, `IAdministrativeRegionProvider`, `ICalendarReferenceProvider`, `IOrganizationRegistryProvider`, `IFinancialInstitutionDirectory`, …). BrasilAPI aparece **somente** como adapters em Infrastructure.
3. Obrigar fluxo `Cliente → API Arah → cache/snapshot → adapter → provider`.
4. Persistir proveniência via **`PublicDataSnapshot`** (ou equivalente): provider, dataset, identificador externo, validade, hash do payload, payload normalizado, confiança, status.
5. Aplicar resiliência padrão: timeout, retry limitado + jitter, circuit breaker, stale-while-revalidate, fallback manual, feature flags, health/métricas.
6. CI com fixtures/mocks — **zero** chamada real ao provider.

## Consequências

**Positivas**
- Domínio estável se a BrasilAPI mudar ou for trocada.
- Fluxos essenciais sobrevivem a outage externo.
- Auditoria/LGPD com hash em vez de log indiscriminado.

**Negativas / trade-offs**
- Mais abstração e migrações iniciais (PD-0).
- Necessidade de convergir `IIbgeBoundaryResolver` existente com os novos ports (plano em PD-1).

## Alternativas consideradas

- **`BrasilApiService` único no Application** (rejeitada): acoplamento e God-object.
- **Chamada direta do Flutter/Web à BrasilAPI** (rejeitada): sem cache central, CORS, chave/abuse, impossível garantir proveniência.
- **Só IBGE oficial, sem BrasilAPI** (adiada como exclusividade): BrasilAPI acelera CEP/feriados/CNPJ; IBGE permanece fonte de malhas/localidades.

## Referências

- [public-data-integration.md](../public-data-integration.md)
- [REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS](../../backlog-api/REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)
- [ADR-006](../10_ARCHITECTURE_DECISIONS.md) (persistência) · padrão Geo HTTP clients
