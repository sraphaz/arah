# Changelog

## Unreleased

### [Fase 8] Sistema de Mídia - 2025-01-16

#### ✅ Implementado
- **Infraestrutura de Mídia**: Sistema completo de armazenamento e gerenciamento de mídias (imagens, vídeos, áudios, documentos)
  - Modelo de domínio: `MediaAsset` e `MediaAttachment` com validações robustas
  - Enums: `MediaType` (Image, Video, Audio, Document) e `MediaOwnerType` (User, Post, Event, StoreItem, ChatMessage)
  - Interfaces: `IMediaStorageService`, `IMediaProcessingService`, `IMediaValidator`
  - Implementações: `LocalMediaStorageService`, `LocalMediaProcessingService`, `MediaValidator`
  - Processamento de imagens com SixLabors.ImageSharp (redimensionamento automático, otimização)
  - Validações de segurança: tipo MIME, tamanho máximo, dimensões, path traversal
  - Checksum SHA-256 para verificação de integridade

- **Serviços de Aplicação**: `MediaService` com operações completas
  - Upload de mídia com validação e processamento automático
  - Associação de mídias a entidades (User, Post, Event, StoreItem, ChatMessage)
  - Soft delete de mídias com verificação de permissões
  - Download e obtenção de URLs de mídias
  - Listagem de mídias por proprietário

- **API REST**: `MediaController` com endpoints completos
  - `POST /api/v1/media/upload` - Upload de mídia (multipart/form-data)
  - `GET /api/v1/media/{id}` - Download de mídia
  - `GET /api/v1/media/{id}/info` - Informações da mídia
  - `DELETE /api/v1/media/{id}` - Exclusão de mídia (apenas criador)
  - Rate limiting configurado no endpoint de upload
  - Validação de autenticação em todos os endpoints

- **Repositórios PostgreSQL**: Implementações completas
  - `PostgresMediaAssetRepository` com operações CRUD e soft delete
  - `PostgresMediaAttachmentRepository` com associações
  - Mappers para conversão Domain ↔ Record
  - Configuração Entity Framework Core para tabelas `media_assets` e `media_attachments`

- **Testes**: Cobertura abrangente de testes
  - Testes unitários do modelo de domínio (MediaAsset, MediaAttachment)
  - Testes de serviço (MediaService com Moq)
  - Testes de segurança avançada (validação MIME, path traversal, tamanho, rate limiting)

- **Configuração**: Opções configuráveis em `appsettings.json`
  - Provider de armazenamento (Local, S3, AzureBlob - preparado para futuro)
  - Tamanhos máximos configuráveis (imagens: 10MB, vídeos: 50MB)
  - Dimensões máximas configuráveis (4000x4000px)
  - Redimensionamento automático (1920x1920px)

#### ⚠️ Pendências
- Migrations do banco de dados para criar tabelas `media_assets` e `media_attachments`
- Implementações InMemory dos repositórios para testes completos
- Testes de integração completos do MediaController
- Documentação de uso com exemplos práticos

#### 📚 Documentação
- Documentação técnica completa em `docs/MEDIA_SYSTEM.md`
- FASE8.md atualizado com status de implementação
- Todos os arquivos com documentação XML completa

---

### Anterior

- Refactored territory to be purely geographic and moved social logic into membership entities and services.
- Added revised user stories documentation under `docs/user-stories.md`.
- Updated API endpoints for territory search/nearby/suggestions and membership handling.
- Adjusted feed/map/alerts filtering to use social membership roles.
- Added optional Postgres persistence with EF Core mappings alongside the InMemory provider.
- Added a minimal static API home page plus configuration helper UI.
- Added structured error handling with `ProblemDetails` and testing hooks for exception scenarios.
- Published the self-service portal as a static site in `docs/` for GitHub Pages, linking to documentation and changelog.
- Added notification outbox/inbox flow with in-app notifications and API endpoints to list/mark as read.
