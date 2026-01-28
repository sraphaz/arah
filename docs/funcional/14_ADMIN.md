# Admin - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 🎯 Visão Geral

O módulo **Admin** fornece ferramentas de administração do sistema e configurações globais. Permite gerenciar territórios, usuários, configurações e work queue.

### Objetivo

Permitir que administradores:
- **Gerenciem territórios** e usuários
- **Configurem sistema** globalmente
- **Revisem work queue** (verificações, curadoria, moderação)
- **Monitorem** plataforma

---

## 💼 Função de Negócio

### Para Administradores

- Gerenciar territórios
- Gerenciar usuários e permissões
- Configurar sistema (SystemConfig)
- Revisar work queue
- Monitorar métricas

### Para a Plataforma

- **Configuração Centralizada**: SystemConfig para ajustes globais
- **Work Queue**: Fila de tarefas para revisão humana
- **Auditoria**: Rastreabilidade completa

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### SystemConfig
- **Propósito**: Configurações globais do sistema
- **Características**: Auditável, configurável

#### SystemPermission
- **Propósito**: Permissões globais
- **Tipos**: Admin, SystemOperator

#### WorkItem
- **Propósito**: Item na fila de trabalho
- **Tipos**: IdentityVerification, ResidencyVerification, ModerationCase, AssetCuration

---

## ⚙️ Regras de Negócio

1. **Permissões Globais**: SystemAdmin tem acesso a tudo
2. **Work Queue**: Fila de tarefas para revisão humana
3. **SystemConfig**: Configurações auditáveis
4. **Monitoramento**: Métricas e observabilidade

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Autenticação e Identidade](./01_AUTENTICACAO_IDENTIDADE.md)** - Permissões
- **[API - Admin](../api/60_14_API_ADMIN.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
