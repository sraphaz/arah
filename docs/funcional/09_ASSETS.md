# Assets - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](funcional/00_PLATAFORMA_Arah.md)

---

## 🎯 Visão Geral

**TerritoryAssets** representam recursos valiosos do território que pertencem ao próprio território (naturais, culturais, comunitários, infraestruturais, simbólicos). **NÃO são vendáveis** e não devem ser tratados como produtos ou serviços.

### Corpos d'água (diferencial de cuidado)

Rios, córregos, nascentes e fontes são **entidades curáveis** do território (patrimônio hídrico). A comunidade nomeia, marca no mapa, fala sobre e organiza cuidado — sem embutir o rio na entidade `Territory` (que permanece geográfica e neutra).

- **Ponte atual**: TerritoryAsset com tipagem natural/hídrica + MapEntity (`espaço natural`)
- **Alvo (FASE24.0)**: `NaturalAsset` + `WATERCOURSE_DETAILS` / `WATER_POINT_DETAILS` (MER)
- **Backlog**: [CORPOS_DAGUA_TERRITORIO](../backlog-api/CORPOS_DAGUA_TERRITORIO.md) · Spec: [`water-bodies-curation`](../specs/features/water-bodies-curation.spec.yaml)

### Objetivo

Permitir que usuários:
- **Cadastrem recursos** territoriais valiosos (incluindo **rios e nascentes**)
- **Visualizem assets** no mapa
- **Validem assets** (curadores)
- **Referenciem assets** em posts/eventos/observações de saúde
- **Cuidem** do patrimônio hídrico ao longo do tempo (mutirões, observações)

---

## 💼 Função de Negócio

### Para o Usuário

- Cadastrar recursos territoriais (trilhas, **rios**, nascentes, pontos culturais)
- Visualizar assets validados no mapa
- Referenciar assets em posts/eventos
- Acompanhar e contribuir no cuidado da água do território

### Para a Comunidade

- **Registro**: Catalogar recursos valiosos do território
- **Preservação**: Documentar patrimônio territorial e hídrico
- **Cuidado**: Organizar atenção coletiva a rios e fontes
- **Descoberta**: Facilitar descoberta de recursos

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### TerritoryAsset
- **Propósito**: Recurso territorial valioso
- **Atributos**: Nome, descrição, tipo, geolocalização obrigatória
- **Status**: PENDING, VALIDATED
- **Características**: Não vendável, não transferível
- **Hídrico (planejado)**: subtypes `river` | `stream` | `spring` | `waterfall` | `well` | `potable_water`

#### NaturalAsset / WaterBody *(alvo FASE24)*
- Tipagem rica no MER; curso d'água com polilinha; ponto d'água com potabilidade/sensibilidade

---

## ⚙️ Regras de Negócio

1. **Permissão**: Apenas moradores verificados ou curadores podem criar
2. **Geolocalização**: Obrigatória (pelo menos um GeoAnchor; cursos d'água: polilinha de trecho)
3. **Validação**: Apenas curadores podem validar
4. **Visibilidade**: Apenas assets validados são retornados (respeitar sensitivity em nascentes)
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

---

**Última Atualização**: 2026-08-08  
**Versão**: 1.1  
**Status**: Funcionalidade Implementada (ponte TerritoryAsset) · NaturalAsset hídrico planejado
