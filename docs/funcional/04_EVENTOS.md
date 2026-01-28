# Eventos - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](funcional/00_PLATAFORMA_ARAPONGA.md)

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

**Eventos** são atividades comunitárias organizadas por território. Permitem que comunidades organizem encontros, atividades e iniciativas locais com data, hora e localização.

### Objetivo

Permitir que usuários:
- **Organizem eventos** comunitários
- **Participem de eventos** (interesse ou confirmação)
- **Visualizem eventos** no feed e mapa
- **Busquem eventos** próximos à localização

---

## 💼 Função de Negócio

### Para o Usuário

**Como Visitante ou Morador**:
- Criar eventos comunitários
- Visualizar eventos do território
- Participar de eventos (marcar interesse ou confirmação)
- Buscar eventos próximos
- Cancelar eventos criados

### Para a Comunidade

- **Organização**: Facilitar encontros e atividades
- **Engajamento**: Mobilizar comunidade para eventos
- **Visibilidade**: Eventos aparecem no feed e mapa
- **Contexto Territorial**: Eventos sempre vinculados ao território

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### Event
- **Propósito**: Evento comunitário
- **Atributos**:
  - `Id`: Identificador único
  - `TerritoryId`: Território
  - `CreatedByUserId`: Criador
  - `Title`: Título (máx 200 caracteres)
  - `Description`: Descrição (máx 2000 caracteres)
  - `StartsAtUtc`: Data/hora início
  - `EndsAtUtc`: Data/hora fim (opcional)
  - `Latitude`, `Longitude`: Localização
  - `LocationLabel`: Rótulo do local (opcional)
  - `Status`: SCHEDULED, CANCELLED
  - `CreatedAt`: Data de criação
  - `Media`: Mídias (imagem de capa + até 5 adicionais, vídeos, áudios)

#### EventParticipation
- **Propósito**: Participação em evento
- **Tipos**: INTEREST (interessado), CONFIRMED (confirmado)

---

## 🔄 Fluxos Funcionais

### Fluxo 1: Criar Evento

```
Usuário → Seleciona Território → Cria Evento → 
Informa Título/Descrição → Define Data/Hora → 
Define Localização (obrigatório) → Publica → 
Sistema cria Post automático no Feed → 
Evento aparece no Feed e Mapa
```

### Fluxo 2: Participar de Evento

```
Usuário → Visualiza Evento → Escolhe participação:
- Marcar Interesse: INTEREST
- Confirmar Presença: CONFIRMED
→ Sistema registra participação → 
Contagem atualizada
```

### Fluxo 3: Buscar Eventos Próximos

```
Usuário → Informa Localização → 
Busca Eventos Próximos → 
Sistema retorna eventos ordenados por proximidade → 
Usuário visualiza no mapa
```

---

## 📖 Casos de Uso

### Caso de Uso 1: Morador Cria Evento

**Ator**: Morador  
**Fluxo**:
1. Acessa seção de eventos
2. Clica "Criar Evento"
3. Informa título, descrição, data/hora, localização
4. Publica
5. Sistema cria post automático no feed
6. Evento aparece no feed e mapa

### Caso de Uso 2: Usuário Participa de Evento

**Ator**: Visitante ou Morador  
**Fluxo**:
1. Visualiza evento no feed ou mapa
2. Clica "Tenho Interesse" ou "Confirmar Presença"
3. Sistema registra participação
4. Contagem de interessados/confirmados atualizada

---

## ⚙️ Regras de Negócio

### Criação de Eventos

1. **Permissão**: Visitantes e moradores podem criar
2. **Geolocalização**: Obrigatória (latitude e longitude)
3. **Data**: `startsAtUtc` deve ser no futuro (ou até 1 ano no passado)
4. **Limites**: Título 200 chars, descrição 2000 chars
5. **Post Automático**: Cria post no feed referenciando evento
6. **Status**: Criado como SCHEDULED

### Visualização de Eventos

1. **Visibilidade**: Todos os eventos são públicos
2. **Paginação**: Padrão 20 itens
3. **Filtros**: Por período (startDate, endDate)
4. **Busca Próxima**: Ordenada por proximidade geográfica

### Participação

1. **Permissão**: Todos autenticados podem participar
2. **Tipos**: INTEREST ou CONFIRMED
3. **Idempotente**: Múltiplas chamadas atualizam participação

### Cancelamento

1. **Permissão**: Apenas criador pode cancelar
2. **Status**: Marcado como CANCELLED

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](funcional/00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Feed Comunitário](funcional/03_FEED_COMUNITARIO.md)** - Posts automáticos de eventos
- **[Mapa Territorial](funcional/05_MAPA_TERRITORIAL.md)** - Eventos aparecem como pins
- **[API - Eventos](api/60_05_API_EVENTOS.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
