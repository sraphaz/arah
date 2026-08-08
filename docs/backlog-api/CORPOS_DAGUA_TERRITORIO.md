# Corpos d'água do território — rios, nascentes e fontes curáveis

**Versão**: 1.0  
**Data**: 2026-08-08  
**Status**: ✅ Aprovado para planejamento (backlog)  
**Domínio dono**: `mapa-lugares` (primário) · co-ativação: `territorio-membership`, `governanca-transparencia`, `feed-conteudo`, `design-ux` · sinais externos: trilha TI  
**Âncora de fase**: [FASE24](./FASE24.md) (Saúde Territorial) · ponte atual: TerritoryAsset / MapEntity

---

## Resumo executivo

O Ará reconhece que **cuidar do território inclui cuidar da água**. Rios, córregos, nascentes, poços e pontos de água potável são **entidades curáveis do território** — não atributos sociais embutidos em `Territory`, e não mercadoria.

A comunidade deve poder:

- **Nomear e marcar** o rio (ou nascente, córrego, fonte) no mapa do território
- **Falar sobre** o corpo d'água (posts, alertas, observações) com vínculo explícito à entidade
- **Curar** cadastro e status (Resident sugere · Curator valida · confirmações comunitárias)
- **Cuidar** ao longo do tempo (observações de saúde hídrica, mutirões, manutenção — FASE24)

Isso diferencia o produto: o mapa deixa de ser só POIs comerciais/públicos e passa a carregar **patrimônio hídrico vivo**, alinhado a autonomia local e cuidado coletivo.

---

## Decisão de domínio (invariantes)

| Regra | Decisão |
|-------|---------|
| Onde mora a entidade? | **Fora** de `Territory`. Escopo sempre por `territoryId`. |
| Nome canônico (alvo) | **NaturalAsset** (MER) com tipos hídricos; ponte MVP = **TerritoryAsset** (`type=natural`) + tipagem hídrica |
| Nome de produto | **Corpo d'água** / **WaterBody** (especialização hídrica de NaturalAsset) |
| Curadoria | Camada social separada: capability **Curator** + WorkItem de curadoria (mesmo padrão de assets) |
| Geometria | Ponto (nascente, poço, fonte) · **linha/polilinha** (rio, córrego) · opcional polígono (trecho/bacia local) |
| Visibilidade / sensibilidade | Nascentes e fontes podem ser `RESTRICTED` / `HIGH` sensitivity (evitar exposição de captação vulnerável) |
| Marketplace | **Nunca** vendável |
| Nomenclatura proibida | Não usar "Place"; não colocar membership/moderação dentro do corpo d'água nem do Territory |

```
Territory (geografia neutra)
    └── contains → NaturalAsset / TerritoryAsset (corpo d'água)
                        ├── curated by → MembershipCapability.Curator + WorkItem
                        ├── referenced by → Post / Map pin / HealthObservation
                        └── maintained via → TerritoryAction (FASE24)
```

---

## Estado atual vs alvo

| Camada | Hoje | Alvo |
|--------|------|------|
| TerritoryAsset | Tipos livres; exemplos citam nascente/rio | Subtipos hídricos explícitos (`river`, `stream`, `spring`, `waterfall`, `well`, `potable_water`) |
| MapEntity | Categoria `espaço natural` | Pode espelhar/apontar para o asset hídrico no mapa |
| MER `NATURAL_ASSET` | SPRING, WATERFALL, POTABLE_WATER, TRAIL… **sem RIVER** | Incluir `RIVER`, `STREAM` + `WATERCOURSE_DETAILS` (geometria de curso) |
| FASE24 | Observações `WATER` + `RelatedNaturalAssetId` | Exige cadastro curável de corpos d'água **antes ou no início** da fase |
| TI | Sinais de enchente (ex. demo Rio do Peixe) | Podem referenciar corpo d'água local quando existir |

---

## Backlog — onde entra

### A) Ponte imediata (enhancement, sem nova FASE*)

Refinar o que já existe em Assets/Mapa:

| ID | Item | Prio | Notas |
|----|------|------|-------|
| WA-E1 | Tipagem hídrica em TerritoryAsset (`natural` + subtype) | P1 | Sem mudar Territory |
| WA-E2 | Pins/filtros de mapa para corpos d'água | P1 | Flutter + BFF |
| WA-E3 | Glossário + docs funcionais alinhados | P0 | Este pacote |
| WA-E4 | Curadoria: copy/UX “cuidar do rio / da nascente” | P2 | design-ux |

### B) Fundação na FASE24 (canônico)

| ID | Item | Doc |
|----|------|-----|
| **24.0** | Cadastro curável de corpos d'água (`NATURAL_ASSET` hídrico + detalhes) | [FASE24 §24.0](./FASE24.md) |
| 24.1+ | Observações/sensores/ações com vínculo ao corpo d'água | FASE24 restante |
| FASE42 | `NaturalAssetMaintenance` / contribuições de cuidado | [FASE42](./FASE42.md) |

### C) Integrações transversais

| Trilha / fase | Papel |
|---------------|-------|
| TI-3…TI-6 | Alerta de enchente/estiagem pode citar o rio curado do território |
| Dados climáticos públicos (futuro) | Contexto climático **não** substitui observação comunitária da água |
| Feed | Post com referência a `naturalAssetId` / asset hídrico |
| Governança | WorkItem de curadoria e contestação de cadastro sensível |

---

## Modelo conceitual (hírico)

### Tipos de NaturalAsset (extensão)

`SPRING` · `WATERFALL` · `POTABLE_WATER` · **`RIVER`** · **`STREAM`** · `WELL` (via water details) · demais tipos naturais existentes

### Detalhes

- **WATER_POINT_DETAILS** — ponto (nascente, torneira comunitária, poço, fonte filtrada): potabilidade, último teste, notas
- **WATERCOURSE_DETAILS** (novo) — curso d'água (rio/córrego): trecho (polilinha GeoJSON), nome popular, regime (permanente/sazonal), uso comunitário (lazer, captação, sagrado), notas de cuidado

### Ciclo de cuidado

1. Morador **sugere** o corpo d'água  
2. Curador **valida** (ou fila WorkItem)  
3. Comunidade **confirma** / contesta  
4. Qualquer residente **observa** saúde da água (FASE24)  
5. Comunidade **age** (mutirão de limpeza, restauração de mata ciliar)  
6. Memória territorial acumula histórico do rio

---

## Spec SDD

- Draft: [`docs/specs/features/water-bodies-curation.spec.yaml`](../specs/features/water-bodies-curation.spec.yaml)  
- PRs de implementação devem trazer `Spec-Id: water-bodies-curation` (ou spec de fase FASE24 quando promovida)

---

## Fora de escopo (explícito)

- Transformar `Territory` em agregador social de rios  
- Dados de vigilância individual junto ao rio  
- Cadastro nacional hidrológico como fonte única da verdade (pode enriquecer via PD/TI, nunca substituir curadoria local)  
- Monetizar acesso à água via marketplace

---

## Referências

- [FASE24 — Saúde Territorial](./FASE24.md)  
- [09_ASSETS](../funcional/09_ASSETS.md) · [60_08_API_ASSETS](../api/60_08_API_ASSETS.md)  
- [Glossário](../product/05_GLOSSARY.md) · [Domain model](../backend/12_DOMAIN_MODEL.md)  
- MER: `design/Archtecture/MER.md` (NATURAL_ASSET)  
- Agente: `.agents/domain/mapa-lugares.agent.yaml`

---

### Changelog

- **1.0** (2026-08-08): Introdução da capacidade no backlog — rios e fontes como entidades curáveis do território.
