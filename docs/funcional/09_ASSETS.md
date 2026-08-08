# Assets - Documentação Funcional

**Versão**: 1.1  
**Data**: 2026-08-08  
**Status**: Ponte TerritoryAsset implementada · NaturalAsset hídrico planejado (FASE24.0)  
**Parte de**: [Documentação Funcional da Plataforma](funcional/00_PLATAFORMA_Arah.md)

---

## 🎯 Visão Geral

**TerritoryAssets** representam recursos valiosos do território que pertencem ao próprio território (naturais, culturais, comunitários, infraestruturais, simbólicos). **NÃO são vendáveis** e não devem ser tratados como produtos ou serviços.

### Corpos d'água (diferencial de cuidado)

Rios, córregos, nascentes e fontes são **entidades curáveis** do território (patrimônio hídrico). A comunidade nomeia, marca no mapa, fala sobre e organiza cuidado — sem embutir o rio na entidade `Territory` (que permanece geográfica e neutra).

- **Ponte atual (WA-E1)**: TerritoryAsset `type=natural` + `subtype` opcional (`river`|`stream`|`spring`|`waterfall`|`well`|`potable_water`) + MapEntity (`espaço natural`); status canônico `Suggested`→`Active`
- **Alvo (FASE24.0)**: `NaturalAsset` + `WATERCOURSE_DETAILS` / `WATER_POINT_DETAILS`; status `PENDING`→`PUBLISHED`
- **WaterBody**: alias de produto/API para NaturalAsset tipado hídrico (`naturalAssetId`) — não é tabela separada
- **Backlog**: [CORPOS_DAGUA_TERRITORIO](../backlog-api/CORPOS_DAGUA_TERRITORIO.md) · Spec-Id: [`water-bodies-curation`](../specs/features/water-bodies-curation.spec.yaml)

### Objetivo

Permitir que usuários:
- **Cadastrem recursos** territoriais valiosos (incluindo **rios, córregos, nascentes e fontes**)
- **Visualizem assets** no mapa (respeitando HIGH/RESTRICTED)
- **Validem assets** (curadores)
- **Referenciem assets** em posts/eventos/observações de saúde via `naturalAssetId`
- **Cuidem** do patrimônio hídrico ao longo do tempo (mutirões, observações)

---

## 💼 Função de Negócio

### Para o Usuário

- Cadastrar recursos territoriais (trilhas, **rios**, **córregos**, nascentes, fontes, pontos culturais)
- Visualizar assets validados no mapa
- Referenciar assets em posts/eventos
- Acompanhar e contribuir no cuidado da água do território

### Para a Comunidade

- **Registro**: Catalogar recursos valiosos do território
- **Preservação**: Documentar patrimônio territorial e hídrico
- **Cuidado**: Organizar atenção coletiva a rios, córregos, nascentes e fontes
- **Descoberta**: Facilitar descoberta de recursos

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### TerritoryAsset (ponte)
- **Propósito**: Recurso territorial valioso
- **Atributos**: Nome, descrição, tipo, **subtype** (opcional), geolocalização obrigatória
- **Status canônico**: `Suggested` → `Active` (também `Archived`|`Rejected`). Termos legados PENDING/VALIDATED = aliases documentais de Suggested/Active
- **Características**: Não vendável, não transferível
- **Hídrico (WA-E1)**: `type=natural` + `subtype` ∈ `river`|`stream`|`spring`|`waterfall`|`well`|`potable_water` (aliases → tipos UPPERCASE do MER); subtype sem `natural` → rejeitado; PATCH omite subtype → preserva

#### NaturalAsset / WaterBody *(alvo FASE24)*
- Persistência: `NaturalAsset` tipado; **WaterBody** = alias de API
- Tipos hídricos: `RIVER`|`STREAM`|`SPRING`|`WATERFALL`|`POTABLE_WATER` (`WELL` só em `WATER_POINT_DETAILS.water_type`)
- Status: `PENDING` → `PUBLISHED`|`HIDDEN`|`REVIEW`
- Curso: `WATERCOURSE_DETAILS` (só RIVER/STREAM); ponto: `WATER_POINT_DETAILS`

---

## ⚙️ Regras de Negócio

1. **Permissão**: Apenas moradores verificados ou curadores podem criar
2. **Geolocalização**: Obrigatória (ponto; cursos: LineString ≤500 vértices, intersecta território)
3. **Validação**: Apenas curadores podem publicar/validar
4. **Visibilidade**:
   - Ponte TerritoryAsset: status canônico `Suggested`/`Active`/`Archived`/`Rejected` (listagens tipicamente após curadoria em `Active`)
   - NaturalAsset (alvo): apenas `PUBLISHED` (salvo regras de membership)
   - `sensitivity HIGH` / `access RESTRICTED`: filtrar ou omitir coordenadas em list/get/mapa/pins sem autorização de leitura
5. **Não vendável**: Assets não podem ser vendidos via marketplace
6. **Territory neutro**: nenhum campo de rio/membership dentro de Territory

---

## 📚 Documentação Relacionada

- **[Plataforma Arah](funcional/00_PLATAFORMA_Arah.md)** - Visão geral
- **[Corpos d'água — backlog](../backlog-api/CORPOS_DAGUA_TERRITORIO.md)** - Rios e fontes como entidade de domínio
- **[FASE24](../backlog-api/FASE24.md)** - Saúde territorial (tarefa 24.0)
- **[Marketplace](funcional/06_MARKETPLACE.md)** - Diferenciação: Assets não são vendáveis
- **[Mapa Territorial](funcional/05_MAPA_TERRITORIAL.md)** - Assets aparecem no mapa
- **[API - Assets](api/60_08_API_ASSETS.md)** - Documentação técnica
