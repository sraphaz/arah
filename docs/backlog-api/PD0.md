# PD-0: Public Data Foundation (Arah.PublicData)

**Duração**: ~2–3 semanas  
**Prioridade**: 🔴 P0 (trilha PD)  
**Trilha**: Dados Públicos Territoriais  
**Depende de**: — (pode paralelizar com S1/TI-0; não compete com next-phase FASE*)  
**Status**: ⏳ Pendente  
**Arquitetura**: [public-data-integration.md](../architecture/public-data-integration.md)  
**ADRs**: [ADR-023](../architecture/adrs/ADR-023-arah-publicdata-providers.md) · [ADR-024](../architecture/adrs/ADR-024-territorio-comunitario-vs-referencia-administrativa.md)

---

## Contexto

Sem uma fundação comum, cada integração (CEP, CNPJ, clima…) vira um `*Service` monolítico acoplado ao fornecedor. Já existem padrões de HTTP client (`IIbgeBoundaryResolver`) e cache (FASE3) a reutilizar.

## Problema

Não há ports genéricos, snapshots de proveniência, política de resiliência nem feature flags para dados públicos brasileiros.

## Resultado esperado

Capacidade **Arah.PublicData** pronta para plugar datasets sem vazar BrasilAPI no domínio.

## Escopo

- Contratos Application: ports por capacidade (endereço, região administrativa, calendário, …)
- Pacote Infrastructure `PublicData` / `BrasilApi*` (adapters vazios ou com 1 dataset piloto mockado)
- Entidade/persistência `PublicDataSnapshot` (ou nome alinhado na implementação)
- Cache + políticas: timeout, retry+jitter, circuit breaker, SWR, fallback
- Feature flags (`PUBLIC_DATA`, `PUBLIC_DATA_*`)
- Health checks + métricas
- Mocks/fixtures; **zero** chamada real em CI
- Documentação de operação e ADRs aceitos/revisados

## Fora do escopo

- UI de formulários (PD-1+)
- CNPJ, bancos, clima, FIPE, NCM, Registro.br
- `BrasilApiService` monolítico
- Chamada direta do frontend

## Dependências

- FASE3 cache · FASE4 observabilidade · padrão `AddHttpClient` Geo existente

## Critérios de aceite

- [ ] Nenhum tipo do domínio referencia namespaces/DTOs BrasilAPI
- [ ] DI registra ports → adapters; flag off = NoOp/disabled provider
- [ ] Snapshot gravável com provider, dataset, hash, validade, status
- [ ] Circuit aberto serve stale/fallback sem exceção não tratada no use case
- [ ] Testes unitários de política de resiliência + contract test do adapter com fixture
- [ ] CI sem rede para brasilapi.com.br

## Requisitos não funcionais

- Performance: lookup cache-hit p95 alvo a definir em spec (ordem de dezenas de ms locais)
- Segurança: não logar payload bruto sensível
- Observabilidade: métricas de hit/miss, erros, circuit state
- Resiliência: indisponibilidade externa não derruba API host

## Riscos

- Over-engineering da fundação antes do 1º dataset → mitigar com 1 dataset mock no PD-0 e CEP real no PD-1
- Drift vs `IIbgeBoundaryResolver` → plano de convergência documentado

## Estratégia de testes

- Unit: normalização, TTL, circuit, hash
- Integração: DI + InMemory snapshot store
- Contract: fixture JSON BrasilAPI (quando PD-1)
- Negativos: timeout, 5xx, payload inesperado

## Definição de pronto

DoD do repositório + ADRs referenciados + sync-docs.

## Referências

- [REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS](./REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)
- [PD1](./PD1.md)
