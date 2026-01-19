# Mídias em Conteúdo (Imagens, Vídeos e Áudios) - API Araponga

**Parte de**: [API Araponga - Lógica de Negócio e Usabilidade](./60_API_LÓGICA_NEGÓCIO_INDEX.md)  
**Versão**: 2.0  
**Data**: 2025-01-20

---

## 📸🎥🎧 Mídias em Conteúdo (Imagens, Vídeos e Áudios)

### Visão Geral

A plataforma Araponga suporta **mídias ricas** (imagens, vídeos e áudios) em diferentes tipos de conteúdo, permitindo documentação territorial e fortalecimento comunitário através de conteúdo visual e multimídia.

### Upload de Mídia (`POST /api/v1/media/upload`)

**Descrição**: Faz upload de um arquivo de mídia (imagem, vídeo ou áudio).

**Como usar**:
- Exige autenticação
- Multipart/form-data com arquivo (`file`)
- Parâmetros opcionais: `title`, `description`

**Tipos suportados**:
- **Imagens**: JPEG, PNG, WebP (até 10MB)
- **Vídeos**: MP4, WebM, MOV (até 50MB no upload, limites variam por conteúdo)
- **Áudios**: MP3, WAV, OGG (até 20MB no upload, limites variam por conteúdo)

**Regras de negócio**:
- Arquivo é validado quanto a tipo MIME e tamanho
- Mídia é associada ao usuário que fez upload (`UploadedByUserId`)
- Retorna `MediaAsset` com `id`, `url`, `mimeType`, `sizeBytes`

---

### Mídias em Posts

**Endpoints**: `POST /api/v1/feed` (parâmetro `mediaIds` no body)

**Limites**:
- **Imagens**: Múltiplas (até 10 imagens por post)
- **Vídeos**: 1 vídeo por post (máximo 50MB, até 5 minutos)
- **Áudios**: 1 áudio por post (máximo 10MB, até 5 minutos)
- **Total**: Máximo 10 mídias por post (imagens + 1 vídeo ou 1 áudio)

**Regras de negócio**:
- Mídias devem pertencer ao usuário autenticado
- Vídeos e áudios não podem ser duplicados (apenas 1 de cada tipo)
- Ordem de exibição respeitada via `DisplayOrder` no `MediaAttachment`
- Exclusão de post deleta automaticamente todas as mídias associadas

**Feature Flags**:
- `MediaImagesEnabled` - Controla se imagens são permitidas em posts
- `MediaVideosEnabled` - Controla se vídeos são permitidos em posts
- `MediaAudioEnabled` - Controla se áudios são permitidos em posts

---

### Mídias em Eventos

**Endpoints**: `POST /api/v1/events` (parâmetros `coverMediaId` e `additionalMediaIds`)

**Limites**:
- **Imagem/Vídeo/Áudio de capa**: 1 (capa do evento)
- **Imagens adicionais**: Até 5 imagens
- **Vídeos adicionais**: 1 vídeo adicional (máximo 100MB, até 10 minutos)
- **Áudios adicionais**: 1 áudio adicional (máximo 20MB, até 10 minutos)
- **Total**: Máximo 6 mídias (1 capa + 5 adicionais, incluindo no máximo 1 vídeo e 1 áudio)

**Regras de negócio**:
- Mídia de capa pode ser imagem, vídeo ou áudio
- Capa e adicionais não podem ter tipos duplicados (apenas 1 vídeo total, 1 áudio total)
- Exclusão de evento deleta automaticamente todas as mídias associadas

---

### Mídias em Marketplace (Items)

**Endpoints**: `POST /api/v1/items` (parâmetro `mediaIds` no body)

**Limites**:
- **Imagens**: Múltiplas (até 10 imagens por item)
- **Vídeos**: 1 vídeo por item (máximo 30MB, até 2 minutos)
- **Áudios**: 1 áudio por item (máximo 5MB, até 2 minutos)
- **Total**: Máximo 10 mídias por item (imagens + 1 vídeo ou 1 áudio)

**Regras de negócio**:
- Primeira mídia é considerada imagem principal (`PrimaryImageUrl`)
- Vídeos e áudios não podem ser duplicados (apenas 1 de cada tipo)
- Exclusão de item deleta automaticamente todas as mídias associadas

---

### Mídias em Chat

**Endpoints**: `POST /api/v1/chat/conversations/{conversationId}/messages` (parâmetro `mediaId` no body)

**Limites**:
- **Imagens**: 1 imagem por mensagem (máximo 5MB)
- **Áudios**: 1 áudio por mensagem (máximo 2MB, até 60 segundos)
- **Vídeos**: Não permitidos (performance e privacidade)

**Regras de negócio**:
- Apenas imagens e áudios curtos são permitidos
- Vídeos retornam erro `400 BadRequest` com mensagem "Only images and audios are allowed in chat messages"
- Áudios são limitados a 60 segundos (mensagens de voz)

**Feature Flags**:
- `ChatMediaImagesEnabled` - Controla se imagens são permitidas em chat
- `ChatMediaAudioEnabled` - Controla se áudios são permitidos em chat

---

### Configuração Avançada de Mídias por Território

**Endpoints**:
- `GET /api/v1/territories/{territoryId}/media-config` - Obter configuração
- `PUT /api/v1/territories/{territoryId}/media-config` - Atualizar configuração (requer Curator)

**Recursos**:
- **Configuração por tipo de conteúdo**: Posts, Eventos, Marketplace, Chat
- **Limites personalizáveis**: Tamanho máximo, quantidade, duração (vídeos/áudios)
- **Habilitação por tipo**: Imagens, Vídeos, Áudios podem ser habilitados/desabilitados por território
- **Tipos MIME permitidos**: Override opcional dos tipos MIME permitidos por tipo de mídia (imagem, vídeo, áudio)
- **Validação contra limites globais**: Limites territoriais não podem exceder valores globais (`MediaStorageOptions`)
- **Fallback automático**: Quando limites territoriais não estão configurados, usa valores globais automaticamente

**Exemplo de configuração (Posts)**:
- `ImagesEnabled`: true/false
- `VideosEnabled`: true/false
- `AudioEnabled`: true/false
- `MaxMediaCount`: Quantidade máxima de mídias no total
- `MaxVideoCount`: Quantidade máxima de vídeos (padrão: 1)
- `MaxAudioCount`: Quantidade máxima de áudios (padrão: 1)
- `MaxImageSizeBytes`: Tamanho máximo de imagens (não pode exceder limite global)
- `MaxVideoSizeBytes`: Tamanho máximo de vídeos (não pode exceder limite global)
- `MaxAudioSizeBytes`: Tamanho máximo de áudios (não pode exceder limite global)
- `AllowedImageMimeTypes`: Lista opcional de tipos MIME permitidos para imagens (ex: `["image/jpeg", "image/png"]`)
- `AllowedVideoMimeTypes`: Lista opcional de tipos MIME permitidos para vídeos (ex: `["video/mp4"]`)
- `AllowedAudioMimeTypes`: Lista opcional de tipos MIME permitidos para áudios (ex: `["audio/mpeg", "audio/wav"]`)

**Validações**:
- Limites territoriais são validados contra valores globais (não podem exceder)
- Tipos MIME configurados são validados durante criação de conteúdo
- Se tipos MIME não estiverem configurados, usa tipos MIME globais como fallback

**Preferências do Usuário**:
- `GET /api/v1/user/media-preferences` - Obter preferências
- `PUT /api/v1/user/media-preferences` - Atualizar preferências

**Recursos**:
- Controlar auto-play de vídeos e áudios
- Escolher quais tipos de mídia visualizar (imagens, vídeos, áudios)

---

## 📚 Documentação Relacionada

- **[Feed Comunitário](./60_04_API_FEED.md)** - Mídias em posts
- **[Eventos](./60_05_API_EVENTOS.md)** - Mídias em eventos
- **[Marketplace](./60_09_API_MARKETPLACE.md)** - Mídias em items
- **[Chat](./60_10_API_CHAT.md)** - Mídias em mensagens
- **[Feature Flags](./60_16_API_FEATURE_FLAGS.md)** - Controle de habilitação de mídias
- **[MEDIA_IN_CONTENT.md](../MEDIA_IN_CONTENT.md)** - Documentação técnica completa
- **[MEDIA_SYSTEM.md](../MEDIA_SYSTEM.md)** - Sistema de armazenamento e processamento

---

**Voltar para**: [Índice da Documentação da API](./60_API_LÓGICA_NEGÓCIO_INDEX.md)
