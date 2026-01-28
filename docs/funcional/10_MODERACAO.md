# Moderação - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 🎯 Visão Geral

A **Moderação** mantém qualidade e segurança do conteúdo na plataforma. Permite que usuários reportem conteúdo inadequado e que moderadores apliquem sanções quando necessário.

### Objetivo

Permitir que usuários:
- **Reportem** conteúdo ou usuários inadequados
- **Bloqueiem** usuários indesejados
- **Moderem** conteúdo (curadores/moderadores)
- **Apliquem sanções** quando necessário

---

## 💼 Função de Negócio

### Para o Usuário

- Reportar posts ou usuários por violação
- Bloquear usuários (não vê mais conteúdo deles)
- Desbloquear usuários

### Para Moderadores

- Revisar reports
- Aplicar sanções (bloqueio, ocultação, etc.)
- Gerenciar regras de moderação comunitária

### Para a Comunidade

- **Proteção**: Proteger comunidade de abusos
- **Qualidade**: Manter qualidade do conteúdo
- **Regras Comunitárias**: Definir regras de moderação via votações

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### Report
- **Propósito**: Denúncia de conteúdo ou usuário
- **Tipos**: POST, USER
- **Status**: OPEN, RESOLVED

#### Sanction
- **Propósito**: Sanção aplicada
- **Tipos**: Bloqueio, Ocultação, etc.
- **Escopo**: Territorial ou Global

#### TerritoryModerationRule
- **Propósito**: Regras de moderação comunitária
- **Tipos**: ContentType, ProhibitedWords, Behavior, etc.

---

## 🔄 Fluxos Funcionais

### Fluxo: Reportar e Moderar

```
Usuário → Reporta Conteúdo/Usuário → 
Report criado (Status: OPEN) → 
Moderador Revisa → Aplica Sanção → 
Report resolvido → Ação auditada
```

---

## ⚙️ Regras de Negócio

1. **Permissão**: Todos podem reportar
2. **Deduplicação**: Múltiplos reports em janela de tempo são deduplicados
3. **Automação**: Threshold de reports pode gerar sanção automática
4. **Bloqueio**: Reversível, idempotente
5. **Regras Comunitárias**: Podem ser criadas por curadores ou via votações

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Governança e Votação](./13_GOVERNANCA_VOTACAO.md)** - Regras via votações
- **[API - Moderação](../api/60_12_API_MODERACAO.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
