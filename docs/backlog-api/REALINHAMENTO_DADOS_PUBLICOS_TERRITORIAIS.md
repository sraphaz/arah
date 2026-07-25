# Realinhamento Estratégico — Dados Públicos Territoriais (Arah.PublicData)

**Versão**: 1.0  
**Data**: 2026-07-24  
**Origem**: Enriquecimento do backlog com integração gradual à [BrasilAPI](https://brasilapi.com.br/) e capacidade interna de dados públicos  
**Status**: ✅ Aprovado para planejamento (sem implementação de código nesta entrega)  
**Provedor inicial**: BrasilAPI (apenas Infrastructure)  
**Capacidade**: **Arah.PublicData** (não confundir com a trilha **TI** / World Monitor)

---

## Resumo executivo

O Araponga/Arah evolui de rede comunitária territorial → plataforma de serviços → infraestrutura de inteligência territorial → sistema operacional dos territórios. Para isso, precisa **contextualizar** o território com dados públicos (CEP, UF, município/IBGE, feriados, CNPJ, bancos, clima…) **sem** reduzir o território comunitário ao território administrativo.

Esta trilha **PD-0…PD-8** é transversal (como TI-0…TI-7): **não renumerá** FASE1–61. O backlog comunitário e a sustentação **permanecem válidos**.

| Distinção | Trilha TI | Trilha PD |
|-----------|-----------|-----------|
| Foco | Sinais externos → relevância → revisão humana → ação | Referências públicas estruturadas → cache/snapshots → formulários e contexto |
| Provedor 1º | World Monitor | BrasilAPI |
| Âncoras | FASE23/24/44 | Onboarding IBGE existente, Marketplace, Events, FASE7, FASE24, FASE44 |

**Princípios**:
1. Domínio **não** depende de DTOs/endpoints BrasilAPI.
2. Frontend **nunca** chama o provider externo.
3. Cache + proveniência (`PublicDataSnapshot`) obrigatórios.
4. Dado externo = sugestão; correção manual sempre possível.
5. Indisponibilidade externa **não** bloqueia fluxos essenciais.
6. Sem CNPJ / coletivo informal / pessoa física permanecem cidadãos de primeira classe.
7. Sem crawling / importação massiva / “cobertura total” da BrasilAPI.

---

## Diagnóstico do estado atual (baseline)

### Já existe (implementado)

| Área | Evidência | Classificação |
|------|-----------|---------------|
| Territory (city/state/lat/lng/boundary) | `Arah.Domain.Territories.Territory` | Parcial p/ referência administrativa |
| IBGE localidades + malhas | `IIbgeBoundaryResolver`, provisioning municipal, onboarding | Parcial — sem persistência de código IBGE no Territory; sem camada PublicData genérica |
| Reverse geocode | Nominatim | Existe (outro provider) |
| GeoAnchors / mapa | PostGeoAnchor, AssetGeoAnchor, MapEntity | Existe |
| Marketplace / Store | Store, items, checkout, `AddressDto.ZipCode` | Existe — CEP livre, sem lookup |
| Events | TerritoryEvent + locationLabel | Existe — sem feriados |
| Pagamentos / payouts | FASE6/7 ✅, gateways mock/Stripe/MP | Existe — sem diretório de bancos BR |
| Alertas comunitários | HealthAlert | Existe — não é clima oficial |
| Cache distribuído / obs | FASE3/4 ✅ | Reutilizável pela PD |

### Parcialmente pronto

- Associação território ↔ município (texto `City`/`State`; IBGE só no fluxo de suggest/provision).
- Endereço estruturado no checkout (`AddressDto`) vs `User.Address` string.
- Integrações externas (FASE44) — escopo social/pay; TI-1 já ancorou WM; PD ancora BrasilAPI.

### Gaps (nova capacidade)

- Ports `IAddressDataProvider`, calendário, registry de organizações, diretório bancário.
- Snapshots / proveniência / stale-while-revalidate para dados públicos BR.
- CEP assistido, feriados, CNPJ confirmável, lista de bancos normalizada.
- Entidade de organização territorial formal (não existe `Organization` / CNPJ em Store).

### Backlog existente a **não** duplicar

| Item existente | Relação com PD |
|----------------|----------------|
| TI-0…TI-7 / REALINHAMENTO TI | Complementar — não substituir; PD não recria SignalProvider |
| FASE24 (sensores WEATHER, indicadores) | PD-5 fornece previsão/contexto oficial; FASE24 = observação local |
| FASE44 (integrações) | PD adapters = incremento de dados públicos; FASE44 mantém social/pay |
| FASE7 payouts | PD-4 só referência de instituições; PSP permanece gateway |
| Onboarding IBGE (código) | Evoluir sob PD-1; documentar gap de persistência IbgeCode |

### Fora do escopo ativo (salvo demanda futura)

ISBN, corretoras, taxas financeiras genéricas, Pix participants (salvo roadmap de carteira/PSP próprio), cobertura integral BrasilAPI.

---

## Matriz de decisão dos endpoints / capacidades

| Capacidade | Caso de uso | Prioridade | Fase PD | Decisão | Dependências |
| ---------- | ----------- | ---------: | ------- | ------- | ------------ |
| CEP | Preenchimento assistido (loja, evento, recurso, checkout, usuário) | P0 | PD-1 | **Implementar** (MVP) | PD-0 |
| Estados (UF) | Listas normalizadas, filtros, cadastros | P0 | PD-1 | **Implementar** | PD-0 |
| Municípios + IBGE | Interop, anti-duplicata de grafia, Territory ref | P0 | PD-1 | **Implementar** + evoluir IBGE atual | PD-0; `IIbgeBoundaryResolver` |
| DDD | Sugestão regional de telefone | P1 | PD-1 | **Implementar** (auxiliar) | PD-0 |
| Feriados nacionais | Agenda, eventos, horários especiais | P1 | PD-2 | **Implementar** | PD-0 |
| CNPJ | Organização formal / loja — sugestão confirmável | P1 | PD-3 | **Preparar** pós marketplace org | PD-0; Store; verificação |
| Bancos | Padronizar instituição em payouts | P1 | PD-4 | **Preparar** / implementar c/ formulários payout | FASE7 ✅; PD-0 |
| Pix participants | Roteamento/auditoria Pix própria | P3 | — | **Fora** (salvo carteira/PSP próprio) | FASE55/Aratá |
| Clima (CPTEC) | Previsão / contexto de alertas | P2 | PD-5 | **Fase futura** (multi-provider) | FASE24; TI |
| FIPE | Referência preço veículos/máquinas | P3 | PD-6 | **Só se** categoria real existir | Marketplace categorias |
| NCM | Cadeias produtivas / catálogo | P3 | PD-7 | **Só se** iniciativa produtiva | Cooperativas |
| Câmbio | Comércio internacional / turismo | P3 | PD-7 | **Plugin opcional** | PD-7 |
| Taxas / indicadores fin. | Crédito/fundo territorial | P3 | — | **Fora** sem produto financeiro | FASE61? |
| Registro.br | Domínio / página pública | P3 | PD-8 | **Futuro / premium** | Hub serviços FASE26 |
| ISBN | Acervo / biblioteca | — | — | **Fora** | — |
| Corretoras | — | — | — | **Fora** | — |

---

## Incrementos (PD-0…PD-8)

| ID | Título (fase sugerida) | Esforço rel. | Prioridade | Doc |
|----|------------------------|-------------:|------------|-----|
| PD-0 | Public Data Foundation | M | 🔴 P0 | [PD0.md](./PD0.md) |
| PD-1 | Territorial Addressing | M | 🔴 P0 | [PD1.md](./PD1.md) |
| PD-2 | Territorial Calendar | S | 🟡 P1 | [PD2.md](./PD2.md) |
| PD-3 | Territorial Organizations | L | 🟡 P1 | [PD3.md](./PD3.md) |
| PD-4 | Financial Reference Data | S | 🟡 P1 | [PD4.md](./PD4.md) |
| PD-5 | Territorial Climate Intelligence | L | 🟢 P2 | [PD5.md](./PD5.md) |
| PD-6 | Specialized Marketplaces (FIPE…) | M | 🟢 P3 | [PD6.md](./PD6.md) |
| PD-7 | Productive Chains (NCM/câmbio) | M | 🟢 P3 | [PD7.md](./PD7.md) |
| PD-8 | Digital Presence (Registro.br) | M | 🟢 P3 | [PD8.md](./PD8.md) |

**Primeira entrega recomendada (MVP PD)**: **PD-0 + PD-1 + PD-2**  
CEP + UF + municípios/IBGE + DDD + feriados + providers + cache + snapshots + resiliência.

---

## Ordem de execução (respeitando roadmap atual)

```
Sustentação S0–S1 (54–55)     ← prioridade operacional atual
Economia local 17–19          ← squad A (não competir)
Trilha TI-0…TI-3              ← squad B (sinais WM)
Trilha PD-0…PD-2 (MVP)        ← squad C ou janela pós-onboarding/geo
PD-3…PD-4                     ← após Store/org + payouts forms
PD-5                          ← após FASE24 ou em paralelo a TI clima
PD-6…PD-8                     ← sob demanda de produto
```

---

## Análise de impacto (obrigatória)

### Domínio

- Territory: campos **opcionais** de referência administrativa (`MunicipalityIbgeCode`, normalização de UF) — **sem** lógica social (ADR-003 / ADR-024).
- Novo agregado/entidade de suporte: `PublicDataSnapshot` (ou equivalente).
- Futuro PD-3: `TerritorialOrganization` (ou extensão de Store) com `OrganizationType`, `TaxId`, snapshot, `VerificationStatus` — distinto de confiança comunitária.
- Invariante: dado público nunca auto-aprova membership/KYC/residência.

### Aplicação

- Use cases de lookup (CEP, municípios, feriados) + comandos de “confirmar/corrigir sugestão”.
- Jobs de refresh de cache / expiração de snapshots.
- Feature flags por dataset.

### Infraestrutura

- `HttpClient` tipado por adapter; Polly (timeout/retry/circuit); cache; secrets só se BrasilAPI exigir no futuro (hoje pública).
- Migrações para snapshots e, se aprovado, colunas administrativas em Territory.

### API

- Endpoints Arah versionados (`/v1/public-data/...` ou sob journeys); erros tipados; rate limit; auth conforme sensibilidade (CNPJ autenticado).

### Frontend

- Autocomplete CEP; loading; fallback manual; mensagem “fonte pública / confirme”; a11y.

### Dados

- Migração cuidadosa de City/State existentes; dedupe municípios; TTL e retenção LGPD.

### Segurança / operação

- Sem log de payload sensível; health; dashboards de circuit/cache; custo baixo (API pública) mas risco de rate limit externo.

---

## ADRs

| ADR | Tema |
|-----|------|
| [ADR-023](../architecture/adrs/ADR-023-arah-publicdata-providers.md) | Ports, BrasilAPI só como adapter, cache, resiliência, proveniência |
| [ADR-024](../architecture/adrs/ADR-024-territorio-comunitario-vs-referencia-administrativa.md) | Território comunitário ≠ administrativo; dados externos não autoritativos |

Arquitetura: [public-data-integration.md](../architecture/public-data-integration.md).

---

## Definição de pronto (MVP PD-0…PD-2)

Alinhada a [DEFINITION_OF_DONE](../governance/DEFINITION_OF_DONE.md):

- [ ] Ports + adapter BrasilAPI + mocks; zero chamada real em CI
- [ ] Cache hit path e fallback manual testados
- [ ] Snapshot com proveniência persistido nos lookups MVP
- [ ] Formulários críticos (endereço) não bloqueiam se provider down
- [ ] Territory permanece sem lógica social; IBGE opcional
- [ ] Docs/sync: STATUS_FASES, CHANGELOG, specs quando SDD aplicável
- [ ] Pareceres de domínio (territorio-membership, mapa-lugares, identidade-privacidade)

---

## Resumo executivo (decisão)

| Tema | Recomendação |
|------|----------------|
| Capacidade | **Arah.PublicData** (trilha PD); TI permanece World Monitor |
| 1ª entrega | PD-0 + PD-1 + PD-2 |
| Adiar | CNPJ (PD-3), bancos (PD-4) até modelagem org/payout forms |
| Extensões | Clima, FIPE, NCM, câmbio, Registro.br sob demanda |
| Fora | ISBN, corretoras, taxas genéricas, Pix directory sem PSP próprio |
| Esforço MVP | Médio (~1 squad · 4–8 semanas, a dimensionar em PD-0) |
| Risco principal | Acoplamento a BrasilAPI / tratar dado público como verdade |

---

## Referências

- [public-data-integration.md](../architecture/public-data-integration.md)
- [REALINHAMENTO_INTELIGENCIA_TERRITORIAL](./REALINHAMENTO_INTELIGENCIA_TERRITORIAL.md)
- [FASE44](./FASE44.md) · [FASE24](./FASE24.md) · [FASE7](./FASE7.md)
- BrasilAPI: https://brasilapi.com.br/
