# Feed Comunitário - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Função de Negócio](#função-de-negócio)
3. [Elementos da Arquitetura](#elementos-da-arquitetura)
4. [Fluxos Funcionais](#fluxos-funcionais)
5. [Casos de Uso](#casos-de-uso)
6. [Regras de Negócio](#regras-de-negócio)

---

## 🎯 Visão Geral

O **Feed Comunitário** é a timeline territorial onde usuários compartilham informações relevantes ao território. É o coração da comunicação comunitária na plataforma Araponga.

### Objetivo

Permitir que usuários:
- **Compartilhem informações** relevantes ao território
- **Visualizem timeline** cronológica do território
- **Interajam** com conteúdo (curtir, comentar, compartilhar)
- **Georreferenciem** posts para aparecerem no mapa

---

## 💼 Função de Negócio

### Para o Usuário

**Como Visitante**:
- Visualizar feed público do território
- Ler posts públicos
- Ver eventos e alertas públicos

**Como Morador**:
- Todas as funções de Visitante +
- Criar posts (públicos e privados para moradores)
- Comentar em posts
- Compartilhar posts
- Curtir posts
- Filtrar feed por interesses (opcional)

### Para a Comunidade

- **Comunicação**: Compartilhar informações importantes
- **Engajamento**: Facilitar interação entre membros
- **Contexto Territorial**: Conteúdo sempre relacionado ao território
- **Visibilidade Controlada**: Conteúdo público vs exclusivo para moradores

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### Post
- **Propósito**: Publicação no feed do território
- **Atributos**:
  - `Id`: Identificador único
  - `TerritoryId`: Território
  - `AuthorId`: Autor
  - `Title`: Título (máx 200 caracteres)
  - `Content`: Conteúdo (máx 4000 caracteres)
  - `Type`: GENERAL, ALERT
  - `Visibility`: PUBLIC, RESIDENTS_ONLY
  - `Status`: PUBLISHED, ARCHIVED
  - `CreatedAt`: Data de criação

#### PostGeoAnchor
- **Propósito**: Georreferenciamento de posts
- **Atributos**: Latitude, Longitude, Type
- **Características**: Deriva automaticamente de mídias

#### Media
- **Propósito**: Mídias anexadas aos posts
- **Tipos**: Imagem, Vídeo, Áudio
- **Limites**: 
  - Imagens: Múltiplas (até 10 por post)
  - Vídeos: 1 vídeo por post (máx. 50MB, até 5 minutos)
  - Áudios: 1 áudio por post (máx. 10MB, até 5 minutos)
  - Total: Máximo 10 mídias por post (imagens + 1 vídeo ou 1 áudio)
- **Feature Flags**: Controladas por território (MediaImagesEnabled, MediaVideosEnabled, MediaAudioEnabled)

---

## 🔄 Fluxos Funcionais

### Fluxo 1: Criar Post

```
Usuário → Seleciona Território → Cria Post → 
Adiciona Título/Conteúdo → Adiciona Mídias (opcional) → 
Define Visibilidade → Publica → 
Aparece no Feed e Mapa (se georreferenciado)
```

### Fluxo 2: Visualizar Feed

```
Usuário → Seleciona Território → Acessa Feed → 
Sistema filtra por visibilidade (baseado em Membership) → 
Retorna timeline cronológica → 
Usuário interage (curtir, comentar, compartilhar)
```

### Fluxo 3: Interagir com Post

```
Usuário → Visualiza Post → Escolhe ação:
- Curtir: Toggle like/deslike
- Comentar: Adiciona comentário (apenas moradores verificados)
- Compartilhar: Cria novo post referenciando original
```

---

## 📖 Casos de Uso

### Caso de Uso 1: Morador Cria Post Público

**Ator**: Morador  
**Fluxo**:
1. Acessa feed do território
2. Clica "Criar Post"
3. Informa título e conteúdo
4. (Opcional) Adiciona mídias
5. Define visibilidade: PUBLIC
6. Publica
7. Post aparece no feed para todos

### Caso de Uso 2: Morador Cria Post Exclusivo

**Ator**: Morador  
**Fluxo**:
1. Cria post
2. Define visibilidade: RESIDENTS_ONLY
3. Publica
4. Post aparece apenas para moradores verificados

### Caso de Uso 3: Visitante Visualiza Feed

**Ator**: Visitante  
**Fluxo**:
1. Acessa feed do território
2. Sistema filtra: apenas posts PUBLIC
3. Visualiza timeline cronológica
4. Pode ler posts, mas não pode comentar/compartilhar

---

## ⚙️ Regras de Negócio

### Criação de Posts

1. **Permissão**: Visitantes e moradores podem criar
2. **Limites**: Título 200 chars, conteúdo 4000 chars
3. **Visibilidade**:
   - PUBLIC: Todos veem
   - RESIDENTS_ONLY: Apenas moradores verificados
4. **Sanções**: Usuários com sanção de posting não podem criar
5. **Feature Flags**: Posts ALERT requerem flag ALERTPOSTS

### Visualização de Feed

1. **Filtragem por Visibilidade**:
   - Visitor: Apenas PUBLIC
   - Resident não verificado: Apenas PUBLIC
   - Resident verificado: PUBLIC + RESIDENTS_ONLY
2. **Bloqueios**: Posts de usuários bloqueados não aparecem
3. **Paginação**: Padrão 20 itens
4. **Ordenação**: Mais recentes primeiro
5. **Filtro por Interesses**: Opcional (filterByInterests=true)

### Interações

1. **Curtir**: Todos autenticados podem curtir
2. **Comentar**: Apenas moradores verificados
3. **Compartilhar**: Apenas moradores verificados

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Territórios e Memberships](./02_TERRITORIOS_MEMBERSHIPS.md)** - Visibilidade baseada em Membership
- **[Mapa Territorial](./05_MAPA_TERRITORIAL.md)** - Posts georreferenciados aparecem no mapa
- **[API - Feed](../api/60_04_API_FEED.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
