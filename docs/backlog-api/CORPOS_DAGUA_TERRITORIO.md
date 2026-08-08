# Corpos d'água do território — rios, nascentes e fontes curáveis

**Versão**: 1.1  
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

```text
Territory (geografia neutra)
    └── contains → NaturalAsset / TerritoryAsset (corpo d'água)
                        ├── curated by → MembershipCapability.Curator + WorkItem
                        ├── referenced by → Post / Map pin / HealthObservation
                        └── maintained via → TerritoryAction (FASE24)
```

---

## Estado atual vs alvo

| Camada | Modelo documental (hoje neste PR) | Implementação (código) | Alvo de runtime |
|--------|-----------------------------------|------------------------|-----------------|
| TerritoryAsset | Subtipos hídricos documentados | Tipos livres; exemplos nascente/rio | Subtypes canônicos + alias → `NATURAL_ASSET.type` |
| MapEntity | Categoria `espaço natural` | Implementado | Espelhar/apontar para asset hídrico |
| MER `NATURAL_ASSET` | Inclui `RIVER`, `STREAM` + `WATERCOURSE_DETAILS` | **Ainda não** há entidade `NaturalAsset` no código | Persistência + API + curadoria (FASE24.0) |
| FASE24 | Observações `WATER` + `RelatedNaturalAssetId` planejados | Só alertas básicos | Cadastro hídrico **antes** das observações |
| TI | Sinais de enchente podem citar o rio | Demo / trilhas | Referência a corpo d'água local quando existir |

---

## Vocabulário canônico

| Conceito | Decisão |
|----------|---------|
| Persistência | `NaturalAsset` (MER). **WaterBody** = alias de produto/API para tipos hídricos — **não** tabela separada |
| `NATURAL_ASSET.type` | `RIVER` \| `STREAM` \| `SPRING` \| `WATERFALL` \| `POTABLE_WATER` \| `NATIVE_TREE` \| `SANCTUARY` \| `VIEWPOINT` \| `TRAIL` (UPPERCASE) |
| Poço (`WELL`) | **Não** é tipo top-level; `POTABLE_WATER` + `WATER_POINT_DETAILS.water_type=WELL` |
| Ponte TerritoryAsset | subtypes minúsculos mapeados: `river→RIVER`, `stream→STREAM`, `spring→SPRING`, `waterfall→WATERFALL`, `well→POTABLE_WATER+WELL`, `potable_water→POTABLE_WATER` |
| Status NaturalAsset | `PENDING` → `PUBLISHED` \| `HIDDEN` \| `REVIEW` |
| Status ponte TerritoryAsset | `PENDING` → `VALIDATED` |

---

## Backlog — onde entra

### A) Ponte imediata (enhancement, sem nova FASE*)

Refinar o que já existe em Assets/Mapa:

| ID | Item | Prio | Notas |
|----|------|------|-------|
| WA-E1 | Tipagem hídrica em TerritoryAsset (`natural` + subtype) | P1 | Sem mudar Territory |
| WA-E2 | Pins/filtros de mapa para corpos d'água | P1 | Flutter + BFF; filtrar HIGH/RESTRICTED server-side |
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
| Feed | Post com `naturalAssetId` (mesmo `territoryId`) |
| Governança | WorkItem de curadoria e contestação de cadastro sensível |

---

## Modelo conceitual (hídrico)

### Tipos de NaturalAsset (extensão)

`SPRING` · `WATERFALL` · `POTABLE_WATER` · **`RIVER`** · **`STREAM`** · demais tipos naturais existentes. Poço via `WATER_POINT_DETAILS.water_type=WELL`.

### Detalhes

- **WATER_POINT_DETAILS** — ponto (nascente, torneira comunitária, poço, fonte filtrada): potabilidade, último teste, notas
- **WATERCOURSE_DETAILS** — curso d'água (rio/córrego): `path_geojson` LineString (≤500 vértices, WGS84, intersecta território), regime, uso comunitário, notas de cuidado

### Ciclo de cuidado

1. Morador **sugere** o corpo d'água  
2. Curador **valida** (ou fila WorkItem) → `PUBLISHED` (alvo) / `VALIDATED` (ponte)  
3. Comunidade **confirma** / contesta  
4. Qualquer residente **observa** saúde da água (FASE24)  
5. Comunidade **age** (mutirão de limpeza, restauração de mata ciliar)  
6. Memória territorial acumula histórico do rio

---

## Spec SDD

- Draft: [`docs/specs/features/water-bodies-curation.spec.yaml`](../specs/features/water-bodies-curation.spec.yaml) (`Spec-Id: water-bodies-curation`)  
- PRs de implementação devem trazer `Spec-Id: water-bodies-curation` (ou spec de fase FASE24 quando promovida)

---

## Fora de escopo (explícito)

- Transformar `Territory` em agregador social de rios  
- Dados de vigilância individual junto ao rio  
- Cadastro nacional hidrológico como fonte única da verdade (pode enriquecer via dados públicos/TI, nunca substituir curadoria local)  
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

- **1.1** (2026-08-08): Alinhamento CodeRabbit — vocabulário canônico, MER vs implementação, typo hídrico, WaterBody alias.
- **1.0** (2026-08-08): Introdução da capacidade no backlog — rios e fontes como entidades curáveis do território.
