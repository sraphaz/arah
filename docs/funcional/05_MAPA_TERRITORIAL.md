# Mapa Territorial - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 🎯 Visão Geral

O **Mapa Territorial** permite visualização geográfica de conteúdos do território. Posts, eventos, entidades e assets aparecem como pins no mapa, criando uma experiência espacial da comunidade.

### Objetivo

Permitir que usuários:
- **Visualizem conteúdos** geograficamente
- **Explorem território** espacialmente
- **Descubram entidades** territoriais
- **Naveguem** entre feed e mapa

---

## 💼 Função de Negócio

### Para o Usuário

- Visualizar posts georreferenciados no mapa
- Ver eventos com localização
- Explorar entidades territoriais (estabelecimentos, espaços públicos)
- Navegar entre feed e mapa
- Buscar por proximidade geográfica

### Para a Comunidade

- **Contexto Espacial**: Conteúdo sempre relacionado ao lugar
- **Descoberta**: Facilitar descoberta de recursos territoriais
- **Visualização**: Mapa como interface de exploração

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### MapEntity
- **Propósito**: Entidade territorial (estabelecimento, espaço público, etc.)
- **Atributos**: Nome, categoria, localização, status, visibilidade

#### MapEntityRelation
- **Propósito**: Vínculo de usuário com entidade
- **Características**: Moradores podem se vincular a entidades

#### PostGeoAnchor
- **Propósito**: Georreferenciamento de posts
- **Características**: Deriva automaticamente de mídias

---

## 🔄 Fluxos Funcionais

### Fluxo: Visualizar Mapa

```
Usuário → Seleciona Território → Acessa Mapa → 
Sistema carrega pins (posts, eventos, entidades, assets) → 
Usuário explora mapa → Clica em pin → 
Visualiza detalhes do conteúdo
```

---

## ⚙️ Regras de Negócio

1. **Georreferenciamento**: Posts com mídias geram GeoAnchors automaticamente
2. **Visibilidade**: Filtrada por Membership (público vs moradores)
3. **Entidades**: Podem ser estabelecimentos, espaços públicos, naturais
4. **Vínculos**: Moradores podem se vincular a entidades

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Feed Comunitário](./03_FEED_COMUNITARIO.md)** - Posts no mapa
- **[Eventos](./04_EVENTOS.md)** - Eventos no mapa
- **[API - Mapa](../api/60_06_API_MAPA.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
