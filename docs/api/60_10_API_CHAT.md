# Chat (Canais, Grupos e DM) - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 💬 Chat (Canais, Grupos e DM)

### Objetivo

Fornecer comunicação em tempo real/assíncrona entre usuários com governança territorial, respeitando:
- **Papéis territoriais**: `VISITOR` e `RESIDENT`
- **Capabilidades territoriais**: `CURATOR` e `MODERATOR`
- **Permissões globais**: `SYSTEM_ADMIN`
- **Privacidade** (bloqueio e preferências) e **anti-spam**
- **Feature flags por território** para rollout seguro

### Tipos de conversa (ConversationKind)

- **`TERRITORY_PUBLIC`**: canal público do território (leitura para membros do território; escrita restrita).
- **`TERRITORY_RESIDENTS`**: canal exclusivo de moradores validados e usuários verificados.
- **`GROUP`**: grupo privado (invite-only), criado por moradores validados/verificados e **habilitado por curadoria**.
- **`DIRECT`**: DM (habilitável por território via flag, e sempre respeitando preferências/bloqueios).

### Feature flags (por território)

Todas as operações de chat devem checar flags antes de qualquer acesso ao banco (cacheável).

> Observação: a API de feature flags hoje serializa `FeatureFlag.ToString().ToUpperInvariant()`.  
> Portanto, os valores trafegados tendem a ser como `CHATENABLED`, `CHATGROUPS`, etc. (sem underscores).

- **`CHATENABLED`** (`FeatureFlag.ChatEnabled`): master switch do chat no território.
- **`CHATTERITORYPUBLICCHANNEL`** (`FeatureFlag.ChatTerritoryPublicChannel`): habilita o canal público.
- **`CHATTERITORYRESIDENTSCHANNEL`** (`FeatureFlag.ChatTerritoryResidentsChannel`): habilita o canal de moradores.
- **`CHATGROUPS`** (`FeatureFlag.ChatGroups`): habilita criação/consulta de grupos.
- **`CHATDMENABLED`** (`FeatureFlag.ChatDmEnabled`): habilita DM no território.
- **`CHATMEDIAENABLED`** (`FeatureFlag.ChatMediaEnabled`): habilita envio/visualização de mídia (fase 2).

### Regras de permissão (resumo)

**Premissas**:
- "Usuário verificado" = `User.IdentityVerificationStatus == Verified`.
- "Morador validado" = `IsResidentAsync(userId, territoryId) == true`.

#### Canais do território

- **`TERRITORY_PUBLIC`**
  - **Ler**: usuário autenticado com membership no território (`VISITOR` ou `RESIDENT`).
  - **Escrever**: usuário verificado **e** morador validado.
- **`TERRITORY_RESIDENTS`**
  - **Ler/Escrever**: usuário verificado **e** morador validado.

#### Grupos

- **Criar grupo**: usuário verificado **e** morador validado (**visitante não cria**).
- **Estado inicial**: `PENDING_APPROVAL` (não aparece na descoberta do território).
- **Aprovar/habilitar**: `CURATOR` do território (ou `SYSTEM_ADMIN`).
- **Trancar/desabilitar**: `MODERATOR` do território (ou `SYSTEM_ADMIN`).
- **Participação**: invite-only (admin/owner adiciona/removem participantes).

#### DM (Direct)

- **Habilitação**: depende de flag territorial `CHATDMENABLED` (`FeatureFlag.ChatDmEnabled`).
- **Iniciar**: usuário verificado e permitido pelas preferências do destinatário (`contactVisibility`/chat settings) e por `UserBlock`.
- **Ler/Escrever**: apenas participantes (ou `SYSTEM_ADMIN`).

### Privacidade e bloqueios

- **Bloqueio (`UserBlock`)**:
  - bloqueia DM/convites e impede interação direta entre `A` e `B`.
  - (opcional fase 2) pode filtrar exibição de mensagens em grupos/canais.
- **Preferências**:
  - defaults já protegem contra spam (`contactVisibility: ResidentsOnly`).
  - (planejado) chat settings específicos: quem pode iniciar DM, convites, recibos de leitura etc.

### Conteúdo das mensagens (MVP e evolução)

- **MVP**: texto simples.
- **Fase 2** (atrás de flag):
  - **Mídia** (imagem/anexo) com storage externo + URL assinada e validações.
  - **Referências** a posts/eventos/assets do território (payload estruturado com checagem de acesso no read).

### Performance (recomendação)

- Paginação de mensagens **cursor-based**: `before=<messageId|timestamp>&limit=<N>`.
- Evitar N+1 e agregações pesadas:
  - manter `conversation_stats` (última mensagem/preview/contagem).
  - manter estado do participante (`last_read_*`, mute).

### Endpoints Principais

- `GET /api/v1/territories/{territoryId}/chat/channels` - Listar canais do território
- `GET /api/v1/territories/{territoryId}/chat/groups` - Listar grupos do território
- `POST /api/v1/territories/{territoryId}/chat/groups` - Criar grupo
- `POST /api/v1/territories/{territoryId}/chat/groups/{groupId}/approve` - Aprovar grupo (curadoria)
- `GET /api/v1/chat/conversations/{conversationId}` - Detalhes da conversa
- `GET /api/v1/chat/conversations/{conversationId}/messages` - Listar mensagens (cursor-based)
- `POST /api/v1/chat/conversations/{conversationId}/messages` - Enviar mensagem
- `POST /api/v1/chat/conversations/{conversationId}/read` - Marcar como lida

---

## 📚 Documentação Relacionada

- **[Mídias em Conteúdo](./60_15_API_MIDIAS.md)** - Enviar imagens e áudios no chat
- **[Feature Flags](./60_16_API_FEATURE_FLAGS.md)** - Controle de habilitação do chat
- **[Preferências de Usuário](./60_18_API_PREFERENCIAS.md)** - Configurações de privacidade e chat
- **[Moderação](./60_12_API_MODERACAO.md)** - Bloqueios e moderação de chat

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
