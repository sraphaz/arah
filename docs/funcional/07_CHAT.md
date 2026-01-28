# Chat - Documentação Funcional

**Versão**: 1.0  
**Data**: 2026-01-28  
**Status**: Funcionalidade Implementada  
**Parte de**: [Documentação Funcional da Plataforma](./00_PLATAFORMA_ARAPONGA.md)

---

## 🎯 Visão Geral

O **Chat** fornece comunicação em tempo real/assíncrona entre usuários com governança territorial. Suporta canais públicos, canais de moradores, grupos privados e mensagens diretas (DM).

### Objetivo

Permitir que usuários:
- **Comuniquem-se** em canais territoriais
- **Criem grupos** privados
- **Enviem mensagens diretas** (se habilitado)
- **Moderem** conversas (curadores/moderadores)

---

## 💼 Função de Negócio

### Para o Usuário

**Canais Territoriais**:
- Canal Público: Leitura para membros, escrita para moradores verificados
- Canal de Moradores: Apenas moradores verificados

**Grupos**:
- Criar grupos privados (apenas moradores verificados)
- Aprovação por curadoria
- Invite-only

**DM**:
- Mensagens diretas (se habilitado por território)
- Respeita bloqueios e preferências

---

## 🏗️ Elementos da Arquitetura

### Entidades Principais

#### ChatConversation
- **Tipos**: TERRITORY_PUBLIC, TERRITORY_RESIDENTS, GROUP, DIRECT
- **Status**: PENDING_APPROVAL, ACTIVE, ARCHIVED

#### ChatMessage
- **Propósito**: Mensagem em conversa
- **Tipos**: Texto, Mídia (imagens, áudios curtos)

#### ConversationParticipant
- **Propósito**: Participante em conversa
- **Roles**: Owner, Member

---

## ⚙️ Regras de Negócio

1. **Feature Flags**: CHATENABLED, CHATGROUPS, CHATDMENABLED controlam funcionalidades
2. **Permissões**: Baseadas em Membership e verificação
3. **Grupos**: Requerem aprovação de curadoria
4. **Privacidade**: Respeita bloqueios e preferências
5. **Moderação**: Moderadores podem trancar/desabilitar

---

## 📚 Documentação Relacionada

- **[Plataforma Araponga](./00_PLATAFORMA_ARAPONGA.md)** - Visão geral
- **[Territórios e Memberships](./02_TERRITORIOS_MEMBERSHIPS.md)** - Permissões baseadas em Membership
- **[API - Chat](../api/60_10_API_CHAT.md)** - Documentação técnica

---

**Última Atualização**: 2026-01-28  
**Versão**: 1.0  
**Status**: Funcionalidade Implementada
