# Governança e Votação - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 🎯 Visão Geral

A **Governança e Votação** permite que comunidades tomem decisões coletivas de forma transparente e auditável. Suporta diferentes tipos de votações e caracterização do território.

### Objetivo

Permitir que usuários:
- **Criem votações** comunitárias
- **Votem** em propostas
- **Visualizem resultados** de votações
- **Definam interesses** pessoais
- **Caracterizem território** através de tags

---

## 💼 Função de Negócio

### Para o Usuário

**Interesses**:
- Definir até 10 interesses pessoais
- Filtrar feed por interesses (opcional)

**Votações**:
- Votar em propostas comunitárias
- Visualizar resultados
- Ver histórico de participação

### Para a Comunidade

- **Decisões Coletivas**: Votações sobre temas importantes
- **Caracterização**: Tags que definem o território
- **Transparência**: Todas as votações são auditáveis
- **Democracia**: Processo participativo

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### Voting
- **Propósito**: Votação comunitária
- **Tipos**: ThemePrioritization, ModerationRule, TerritoryCharacterization, FeatureFlag, CommunityPolicy
- **Visibilidade**: AllMembers, ResidentsOnly, CuratorsOnly

#### Vote
- **Propósito**: Voto individual
- **Atributos**: Opção escolhida, data/hora

#### UserInterest
- **Propósito**: Interesse do usuário
- **Características**: Máximo 10, normalizado (lowercase)

---

## 🔄 Fluxos Funcionais

### Fluxo 1: Criar e Votar

```
Curador → Cria Votação → Define Tipo/Opções/Visibilidade → 
Votação Ativa → Moradores Votam → 
Período Encerra → Sistema Calcula Resultado → 
Resultado Publicado → Ação Executada (se aplicável)
```

### Fluxo 2: Caracterização do Território

```
Votação TerritoryCharacterization → 
Moradores Votam em Tags → 
Tags Mais Votadas → 
Território Caracterizado → 
Tags Aparecem no Território
```

### Fluxo 3: Interesses do Usuário

```
Usuário → Adiciona Interesse → 
Sistema Normaliza (lowercase) → 
Interesse Salvo → 
Feed Pode Filtrar por Interesses
```

---

## ⚙️ Regras de Negócio

1. **Tipos de Votação**:
   - ThemePrioritization: Priorização de temas
   - ModerationRule: Regras de moderação
   - TerritoryCharacterization: Tags do território
   - FeatureFlag: Habilitar/desabilitar funcionalidades
   - CommunityPolicy: Políticas comunitárias

2. **Visibilidade**:
   - AllMembers: Todos os membros
   - ResidentsOnly: Apenas moradores
   - CuratorsOnly: Apenas curadores

3. **Interesses**:
   - Máximo 10 por usuário
   - Normalizados (trim, lowercase)
   - Máximo 50 caracteres cada

4. **Votos**:
   - Um voto por usuário por votação
   - Auditável (histórico completo)

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Moderação](./10_MODERACAO.md)** - Regras via votações
- **[Territórios e Memberships](./02_TERRITORIOS_MEMBERSHIPS.md)** - Caracterização do território
- **[API - Governança](../api/60_19_API_GOVERNANCA.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
