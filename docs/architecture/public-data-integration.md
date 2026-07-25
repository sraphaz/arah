# Integração de Dados Públicos Territoriais (Arah.PublicData)

**Versão**: 1.0  
**Data**: 2026-07-24  
**Status**: Proposto (planejamento; sem implementação nesta entrega)  
**Trilha**: [PD-0…PD-8](../backlog-api/REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)  
**ADRs**: [ADR-023](./adrs/ADR-023-arah-publicdata-providers.md) · [ADR-024](./adrs/ADR-024-territorio-comunitario-vs-referencia-administrativa.md)

---

## Objetivo

Definir a capacidade interna **Arah.PublicData**: enriquecer o território com dados públicos estruturados **sem** acoplar o domínio aos contratos de um provedor externo.

O primeiro adapter é a [BrasilAPI](https://brasilapi.com.br/). Outros (IBGE direto, Defesa Civil, etc.) entram pela mesma porta.

```text
Ação do usuário
    ↓
API / BFF Arah
    ↓
Application (use cases + cache + snapshots)
    ↓
Ports (IAddressDataProvider, IAdministrativeRegionProvider, …)
    ↓
Infrastructure adapters (BrasilApi*, Ibge*, …)
    ↓
Provider externo
```

**Proibido**: `Frontend → BrasilAPI`.

---

## Relação com o que já existe

| Capacidade atual | Papel | Evolução PD |
|------------------|-------|-------------|
| `IIbgeBoundaryResolver` + malhas | Provisioning municipal / onboarding | Manter; alinhar a `IAdministrativeRegionProvider` + snapshots |
| `IReverseGeocodingService` (Nominatim) | Reverse geocode | Fora do escopo BrasilAPI; mesmo padrão de resiliência |
| `Territory.City` / `Territory.State` | Texto livre | Referência administrativa opcional (`MunicipalityIbgeCode`, etc.) — **não** substitui identidade comunitária |
| `AddressDto.ZipCode` (checkout) | Campo livre | Preenchimento assistido via `IAddressDataProvider` |
| Trilha TI (World Monitor) | Sinais / alertas / briefs | **Complementar**; PD contextualiza; TI sinaliza |
| `HealthAlert` / FASE24 | Observação comunitária | Clima PD-5 alimenta contexto; não substitui confirmação humana |

---

## Ports (contratos de capacidade)

Orientados ao domínio Arah — **nunca** nomes de endpoints BrasilAPI:

```csharp
public interface IAddressDataProvider
{
    Task<AddressLookupResult?> FindByPostalCodeAsync(
        string postalCode,
        CancellationToken cancellationToken);
}

public interface IAdministrativeRegionProvider
{
    Task<IReadOnlyList<StateReference>> ListStatesAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MunicipalityReference>> ListMunicipalitiesAsync(
        string stateCode,
        CancellationToken cancellationToken);
}

public interface ICalendarReferenceProvider
{
    Task<IReadOnlyList<HolidayReference>> ListHolidaysAsync(
        int year,
        CancellationToken cancellationToken);
}

public interface IOrganizationRegistryProvider
{
    Task<OrganizationRegistryResult?> FindByTaxIdAsync(
        string taxId,
        CancellationToken cancellationToken);
}

public interface IFinancialInstitutionDirectory
{
    Task<IReadOnlyList<BankReference>> ListBanksAsync(
        CancellationToken cancellationToken);
}

// Futuro (PD-5+): ITerritorialWeatherProvider, etc.
```

Adapters em Infrastructure, ex.: `BrasilApiAddressDataProvider`. **Não** criar `BrasilApiService` monolítico.

---

## Proveniência — `PublicDataSnapshot`

Nome alinhado ao glossário. Estrutura alvo (ajustar na implementação):

| Campo | Função |
|-------|--------|
| `Id` | Identidade interna |
| `Provider` | Ex.: `brasilapi`, `ibge` |
| `Dataset` | Ex.: `postal-code`, `municipality`, `cnpj` |
| `ExternalIdentifier` | Chave externa normalizada |
| `RetrievedAtUtc` / `ValidUntilUtc` | Frescor |
| `SourceVersion` | Versão/etag quando existir |
| `RawPayloadHash` | Integridade sem logar payload sensível |
| `NormalizedPayload` | JSON normalizado (domínio Arah) |
| `Confidence` | Confiança heurística (0–1) |
| `Status` | Active / Stale / Failed / Superseded |
| `LastSuccessfulRefreshAtUtc` | Último refresh ok |

Dados externos = **sugestão / referência / snapshot**, nunca verdade absoluta. Fluxos essenciais aceitam **preenchimento manual** se o provider falhar.

---

## Cache e resiliência (obrigatório)

- Cache interno (distribuído quando disponível; `IDistributedCacheService` / Redis — FASE3)
- Timeout, retry limitado + jitter, circuit breaker
- Stale-while-revalidate + fallback para snapshot
- Feature flags por dataset / território
- Health checks e métricas (latência, hit ratio, circuit open)
- Zero chamada real a providers em CI (fixtures / mocks)
- Indisponibilidade **não** bloqueia: criar território, evento, loja, endereço, recurso

---

## Segurança e privacidade (LGPD)

- Não logar payloads completos de CNPJ/endereço/PII
- Hash + metadados de proveniência nos logs
- CNPJ/CEP não provam residência, representação legal nem confiança
- Rate limiting nos endpoints Arah que expõem lookups
- Auditoria de consultas sensíveis (CNPJ) sem reter dado excessivo

---

## Âncoras de produto

| Fase / trilha | Uso de PublicData |
|---------------|-------------------|
| Onboarding / Territory | UF, município, IBGE (já parcial via IBGE) |
| Marketplace / Store | CEP; depois CNPJ (PD-3) |
| Events | CEP; feriados (PD-2) |
| Payouts (FASE7 ✅) | Diretório de bancos (PD-4) — referência, não PSP |
| FASE24 / TI | Clima e contexto (PD-5) + sinais WM |

---

## Referências

- [REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS](../backlog-api/REALINHAMENTO_DADOS_PUBLICOS_TERRITORIAIS.md)
- [REALINHAMENTO_INTELIGENCIA_TERRITORIAL](../backlog-api/REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)
- [ADR-003](./10_ARCHITECTURE_DECISIONS.md) — Territory vs camadas sociais
- BrasilAPI: https://brasilapi.com.br/
