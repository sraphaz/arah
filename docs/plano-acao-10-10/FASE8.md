# Fase 8: Infraestrutura de Mídia e Armazenamento

**Duração**: 3 semanas (15 dias úteis)  
**Prioridade**: 🔴 CRÍTICA (Bloqueante para outras fases)  
**Bloqueia**: Fases 9, 10, 11 (todas dependem de mídia)  
**Estimativa Total**: 120 horas  
**Status**: ⏳ Pendente

---

## 🎯 Objetivo

Criar infraestrutura completa de armazenamento e gerenciamento de mídias (imagens, vídeos) que será base para:
- Avatar/Foto de perfil
- Imagens em posts
- Imagens em eventos
- Imagens em anúncios (marketplace)
- Imagens em mensagens

**Valores Mantidos**: Mídias servem para **documentar território** e **fortalecer comunidade**, não para capturar atenção.

---

## 📋 Contexto e Requisitos

### Estado Atual
- ✅ `TerritoryAsset` existe (recursos territoriais não-vendáveis)
- ❌ Sistema de mídia (`MediaAsset`, `MediaAttachment`) não implementado
- ❌ Armazenamento de arquivos não implementado
- ❌ Upload/download de imagens não implementado
- ❌ Validação e processamento de imagens não implementado

### Requisitos Funcionais
- ✅ Upload de imagens (JPEG, PNG, WebP)
- ✅ Upload de vídeos (MP4, opcional para Fase 11)
- ✅ Armazenamento seguro (local ou cloud)
- ✅ Validação de tamanho e formato
- ✅ Redimensionamento/otimização de imagens
- ✅ Download de mídias
- ✅ Associação de mídias a entidades (User, Post, Event, StoreItem, ChatMessage)
- ✅ Soft delete de mídias

### Requisitos Não-Funcionais
- ✅ Segurança: Validação de tipo MIME, tamanho máximo
- ✅ Performance: Otimização automática de imagens
- ✅ Escalabilidade: Preparado para cloud storage (S3, Azure Blob)
- ✅ Privacidade: Controle de acesso por território/usuário

---

## 📋 Tarefas Detalhadas

### Semana 29: Modelo de Domínio e Armazenamento Base

#### 29.1 Modelo de Domínio de Mídia
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `MediaAsset` (entidade de domínio)
  - [ ] `Id`, `UploadedByUserId`, `MediaType` (Image, Video, Audio, Document)
  - [ ] `MimeType`, `StorageKey`, `SizeBytes`
  - [ ] `WidthPx`, `HeightPx` (para imagens)
  - [ ] `Checksum` (integridade)
  - [ ] `CreatedAtUtc`
- [ ] Criar `MediaAttachment` (associação de mídia a entidade)
  - [ ] `MediaAssetId`, `OwnerType` (User, Post, Event, StoreItem, ChatMessage)
  - [ ] `OwnerId`, `DisplayOrder` (ordem em múltiplas mídias)
  - [ ] `CreatedAtUtc`
- [ ] Criar enums: `MediaType`, `MediaOwnerType`
- [ ] Validações de domínio (tamanho máximo, tipos permitidos)
- [ ] Testes unitários do modelo

**Arquivos a Criar**:
- `backend/Araponga.Domain/Media/MediaAsset.cs`
- `backend/Araponga.Domain/Media/MediaAttachment.cs`
- `backend/Araponga.Domain/Media/MediaType.cs`
- `backend/Araponga.Domain/Media/MediaOwnerType.cs`
- `backend/Araponga.Tests/Domain/Media/MediaAssetTests.cs`

**Critérios de Sucesso**:
- ✅ Modelo de domínio criado
- ✅ Validações implementadas
- ✅ Testes unitários passando (>90% cobertura)

---

#### 29.2 Interface de Armazenamento
**Estimativa**: 8 horas (1 dia)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `IMediaStorageService` (interface de armazenamento)
  - [ ] `UploadAsync(Stream, string mimeType, string fileName)`
  - [ ] `DownloadAsync(string storageKey)`
  - [ ] `DeleteAsync(string storageKey)`
  - [ ] `GetUrlAsync(string storageKey)` (URL pública ou signed URL)
- [ ] Criar `IMediaProcessingService` (processamento de imagens)
  - [ ] `ResizeImageAsync(Stream, int maxWidth, int maxHeight)`
  - [ ] `OptimizeImageAsync(Stream)` (compressão)
  - [ ] `ValidateImageAsync(Stream)` (validação de formato)
- [ ] Criar `IMediaValidator` (validação de mídias)
  - [ ] `ValidateAsync(Stream, string mimeType, long sizeBytes)`
  - [ ] Tipos permitidos, tamanhos máximos

**Arquivos a Criar**:
- `backend/Araponga.Application/Interfaces/Media/IMediaStorageService.cs`
- `backend/Araponga.Application/Interfaces/Media/IMediaProcessingService.cs`
- `backend/Araponga.Application/Interfaces/Media/IMediaValidator.cs`

**Critérios de Sucesso**:
- ✅ Interfaces criadas
- ✅ Documentação XML completa
- ✅ Contratos bem definidos

---

#### 29.3 Implementação de Armazenamento Local
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `LocalMediaStorageService` (armazenamento em disco)
  - [ ] Configuração de diretório base (`wwwroot/media` ou configurável)
  - [ ] Estrutura de pastas por tipo/ano/mês
  - [ ] Geração de nomes únicos (GUID + extensão)
  - [ ] Upload de arquivos
  - [ ] Download de arquivos
  - [ ] Exclusão de arquivos
- [ ] Criar `LocalMediaProcessingService` (processamento local)
  - [ ] Usar `SixLabors.ImageSharp` para redimensionamento
  - [ ] Otimização de imagens (compressão)
  - [ ] Validação de formato
- [ ] Criar `MediaValidator` (validação)
  - [ ] Validação de tipo MIME
  - [ ] Validação de tamanho (máx. 10MB para imagens, 50MB para vídeos)
  - [ ] Validação de dimensões (máx. 4000x4000px para imagens)
- [ ] Configuração em `appsettings.json`
  - [ ] `MediaStorage:Provider` (Local, S3, AzureBlob)
  - [ ] `MediaStorage:LocalPath`
  - [ ] `MediaStorage:MaxImageSizeBytes`
  - [ ] `MediaStorage:MaxVideoSizeBytes`

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Media/LocalMediaStorageService.cs`
- `backend/Araponga.Infrastructure/Media/LocalMediaProcessingService.cs`
- `backend/Araponga.Infrastructure/Media/MediaValidator.cs`
- `backend/Araponga.Infrastructure/Media/MediaStorageOptions.cs`

**Arquivos a Modificar**:
- `backend/Araponga.Api/appsettings.json`
- `backend/Araponga.Api/Extensions/ServiceCollectionExtensions.cs` (registro de serviços)

**Dependências NuGet**:
- `SixLabors.ImageSharp` (processamento de imagens)

**Critérios de Sucesso**:
- ✅ Armazenamento local funcionando
- ✅ Upload/download de imagens funcionando
- ✅ Redimensionamento automático funcionando
- ✅ Validações funcionando
- ✅ Testes de integração passando

---

### Semana 30: Repositórios e Serviços de Aplicação

#### 30.1 Repositórios de Mídia
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `IMediaAssetRepository`
  - [ ] `CreateAsync(MediaAsset)`
  - [ ] `GetByIdAsync(Guid id)`
  - [ ] `ListByUserIdAsync(Guid userId)`
  - [ ] `DeleteAsync(Guid id)` (soft delete)
- [ ] Criar `IMediaAttachmentRepository`
  - [ ] `CreateAsync(MediaAttachment)`
  - [ ] `ListByOwnerAsync(MediaOwnerType, Guid ownerId)`
  - [ ] `DeleteAsync(Guid id)`
  - [ ] `DeleteByOwnerAsync(MediaOwnerType, Guid ownerId)`
- [ ] Implementar `PostgresMediaAssetRepository`
- [ ] Implementar `PostgresMediaAttachmentRepository`
- [ ] Implementar `InMemoryMediaAssetRepository`
- [ ] Implementar `InMemoryMediaAttachmentRepository`
- [ ] Criar migrations do banco de dados
  - [ ] Tabela `media_assets`
  - [ ] Tabela `media_attachments`
  - [ ] Índices apropriados

**Arquivos a Criar**:
- `backend/Araponga.Application/Interfaces/Media/IMediaAssetRepository.cs`
- `backend/Araponga.Application/Interfaces/Media/IMediaAttachmentRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresMediaAssetRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/PostgresMediaAttachmentRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Entities/MediaAssetRecord.cs`
- `backend/Araponga.Infrastructure/Postgres/Entities/MediaAttachmentRecord.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryMediaAssetRepository.cs`
- `backend/Araponga.Infrastructure/InMemory/InMemoryMediaAttachmentRepository.cs`
- `backend/Araponga.Infrastructure/Postgres/Migrations/YYYYMMDDHHMMSS_AddMediaAssets.cs`

**Critérios de Sucesso**:
- ✅ Repositórios implementados
- ✅ Migrations criadas e testadas
- ✅ Testes de repositório passando

---

#### 30.2 Serviço de Aplicação de Mídia
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `MediaService`
  - [ ] `UploadMediaAsync(Stream, string mimeType, Guid userId, CancellationToken)`
    - [ ] Validar mídia
    - [ ] Processar (redimensionar/otimizar se imagem)
    - [ ] Upload para storage
    - [ ] Criar `MediaAsset` no banco
    - [ ] Retornar `MediaAsset`
  - [ ] `AttachMediaToOwnerAsync(Guid mediaAssetId, MediaOwnerType ownerType, Guid ownerId, int? displayOrder)`
    - [ ] Criar `MediaAttachment`
  - [ ] `GetMediaUrlAsync(Guid mediaAssetId)` (URL pública ou signed)
  - [ ] `DeleteMediaAsync(Guid mediaAssetId, Guid userId)`
    - [ ] Verificar permissão (apenas criador)
    - [ ] Soft delete `MediaAsset`
    - [ ] Deletar `MediaAttachment`
    - [ ] Deletar arquivo do storage
  - [ ] `ListMediaByOwnerAsync(MediaOwnerType ownerType, Guid ownerId)`
- [ ] Tratamento de erros (exceções tipadas)
- [ ] Logging adequado
- [ ] Testes unitários

**Arquivos a Criar**:
- `backend/Araponga.Application/Services/MediaService.cs`
- `backend/Araponga.Tests/Application/Services/MediaServiceTests.cs`

**Critérios de Sucesso**:
- ✅ Serviço implementado
- ✅ Upload funcionando
- ✅ Associação funcionando
- ✅ Exclusão funcionando
- ✅ Testes unitários passando (>90% cobertura)

---

#### 30.3 Controller de Mídia
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `MediaController`
  - [ ] `POST /api/v1/media/upload` (upload de mídia)
    - [ ] Aceitar `multipart/form-data` com arquivo
    - [ ] Validar autenticação
    - [ ] Chamar `MediaService.UploadMediaAsync`
    - [ ] Retornar `MediaAssetResponse`
  - [ ] `GET /api/v1/media/{id}` (download de mídia)
    - [ ] Buscar `MediaAsset`
    - [ ] Verificar permissão de acesso
    - [ ] Retornar arquivo via `FileResult`
  - [ ] `GET /api/v1/media/{id}/url` (obter URL pública)
    - [ ] Retornar URL assinada ou pública
  - [ ] `DELETE /api/v1/media/{id}` (excluir mídia)
    - [ ] Verificar autenticação e permissão
    - [ ] Chamar `MediaService.DeleteMediaAsync`
- [ ] Validação de request (FluentValidation)
- [ ] Rate limiting (endpoint de upload)
- [ ] Documentação Swagger

**Arquivos a Criar**:
- `backend/Araponga.Api/Controllers/MediaController.cs`
- `backend/Araponga.Api/Contracts/Media/UploadMediaRequest.cs`
- `backend/Araponga.Api/Contracts/Media/MediaAssetResponse.cs`
- `backend/Araponga.Api/Validators/UploadMediaRequestValidator.cs`

**Critérios de Sucesso**:
- ✅ Controller implementado
- ✅ Upload funcionando via API
- ✅ Download funcionando via API
- ✅ Exclusão funcionando via API
- ✅ Testes de integração passando
- ✅ Documentação Swagger completa

---

### Semana 31: Testes, Otimizações e Preparação para Cloud

#### 31.1 Testes de Integração
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Testes de integração de `MediaService`
  - [ ] Upload de imagem válida
  - [ ] Upload de imagem inválida (tipo, tamanho)
  - [ ] Associação de mídia a entidade
  - [ ] Exclusão de mídia
  - [ ] Download de mídia
- [ ] Testes de integração de `MediaController`
  - [ ] Upload via API
  - [ ] Download via API
  - [ ] Exclusão via API
  - [ ] Validação de autenticação
  - [ ] Validação de permissões
- [ ] Testes de performance
  - [ ] Upload de múltiplas imagens
  - [ ] Redimensionamento de imagens grandes
- [ ] Testes de segurança
  - [ ] Upload de arquivo malicioso (tentativa)
  - [ ] Validação de tipo MIME

**Arquivos a Criar**:
- `backend/Araponga.Tests/Integration/MediaServiceIntegrationTests.cs`
- `backend/Araponga.Tests/Integration/MediaControllerIntegrationTests.cs`

**Critérios de Sucesso**:
- ✅ Testes de integração passando
- ✅ Cobertura >90%
- ✅ Testes de segurança passando

---

#### 31.2 Preparação para Cloud Storage (Opcional)
**Estimativa**: 16 horas (2 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Criar `S3MediaStorageService` (opcional, para futuro)
  - [ ] Interface `IMediaStorageService`
  - [ ] Configuração de bucket S3
  - [ ] Upload para S3
  - [ ] Download de S3
  - [ ] Signed URLs para acesso privado
- [ ] Criar `AzureBlobMediaStorageService` (opcional, para futuro)
  - [ ] Interface `IMediaStorageService`
  - [ ] Configuração de container Azure Blob
  - [ ] Upload para Azure Blob
  - [ ] Download de Azure Blob
  - [ ] Signed URLs
- [ ] Configuração via `appsettings.json`
  - [ ] `MediaStorage:Provider` (Local, S3, AzureBlob)
  - [ ] Configurações específicas por provider
- [ ] Factory pattern para seleção de provider
- [ ] Documentação de configuração

**Arquivos a Criar**:
- `backend/Araponga.Infrastructure/Media/S3MediaStorageService.cs` (opcional)
- `backend/Araponga.Infrastructure/Media/AzureBlobMediaStorageService.cs` (opcional)
- `backend/Araponga.Infrastructure/Media/MediaStorageFactory.cs`
- `docs/MEDIA_STORAGE_CONFIGURATION.md`

**Dependências NuGet** (opcional):
- `AWSSDK.S3` (para S3)
- `Azure.Storage.Blobs` (para Azure Blob)

**Critérios de Sucesso**:
- ✅ Estrutura preparada para cloud storage
- ✅ Factory pattern implementado
- ✅ Documentação completa
- ⚠️ **Nota**: Implementação completa de cloud storage pode ser feita depois, quando necessário

---

#### 31.3 Otimizações e Documentação
**Estimativa**: 12 horas (1.5 dias)  
**Status**: ❌ Não implementado

**Tarefas**:
- [ ] Otimizações de performance
  - [ ] Cache de URLs de mídia
  - [ ] Processamento assíncrono de imagens grandes
  - [ ] Lazy loading de mídias
- [ ] Documentação técnica
  - [ ] `docs/MEDIA_SYSTEM.md` (arquitetura do sistema de mídia)
  - [ ] `docs/MEDIA_STORAGE_CONFIGURATION.md` (configuração)
  - [ ] Exemplos de uso
- [ ] Atualizar `docs/CHANGELOG.md`
- [ ] Revisão de código
- [ ] Validação final

**Arquivos a Criar**:
- `docs/MEDIA_SYSTEM.md`
- `docs/MEDIA_STORAGE_CONFIGURATION.md`

**Arquivos a Modificar**:
- `docs/CHANGELOG.md`

**Critérios de Sucesso**:
- ✅ Otimizações implementadas
- ✅ Documentação completa
- ✅ Changelog atualizado
- ✅ Código revisado

---

## 📊 Resumo da Fase 11

| Tarefa | Estimativa | Status | Prioridade |
|--------|------------|--------|------------|
| Modelo de Domínio de Mídia | 8h | ❌ Pendente | 🔴 Crítica |
| Interface de Armazenamento | 8h | ❌ Pendente | 🔴 Crítica |
| Implementação de Armazenamento Local | 16h | ❌ Pendente | 🔴 Crítica |
| Repositórios de Mídia | 12h | ❌ Pendente | 🔴 Crítica |
| Serviço de Aplicação de Mídia | 16h | ❌ Pendente | 🔴 Crítica |
| Controller de Mídia | 12h | ❌ Pendente | 🔴 Crítica |
| Testes de Integração | 12h | ❌ Pendente | 🟡 Importante |
| Preparação para Cloud Storage | 16h | ❌ Pendente | 🟢 Opcional |
| Otimizações e Documentação | 12h | ❌ Pendente | 🟡 Importante |
| **Total** | **120h (15 dias)** | | |

---

## ✅ Critérios de Sucesso da Fase 11

### Funcionalidades
- ✅ Upload de imagens funcionando
- ✅ Download de imagens funcionando
- ✅ Redimensionamento automático funcionando
- ✅ Validação de mídias funcionando
- ✅ Associação de mídias a entidades funcionando
- ✅ Exclusão de mídias funcionando

### Qualidade
- ✅ Cobertura de testes >90%
- ✅ Testes de integração passando
- ✅ Testes de segurança passando
- ✅ Performance adequada (upload < 2s para imagens < 5MB)

### Documentação
- ✅ Documentação técnica completa
- ✅ Documentação de configuração
- ✅ Exemplos de uso
- ✅ Changelog atualizado

### Infraestrutura
- ✅ Armazenamento local funcionando
- ✅ Estrutura preparada para cloud storage (opcional)
- ✅ Configuração flexível

---

## 🔗 Dependências

- **Nenhuma**: Esta é a fase base para todas as outras fases de mídia
- **Bloqueia**: Fases 12, 13, 14 (todas dependem de sistema de mídia)

---

## 📝 Notas de Implementação

### Estrutura de Armazenamento Local

```
wwwroot/
  media/
    images/
      2025/
        01/
          {guid}.jpg
          {guid}.png
    videos/
      2025/
        01/
          {guid}.mp4
```

### Validações de Mídia

**Imagens**:
- Tipos permitidos: JPEG, PNG, WebP
- Tamanho máximo: 10MB
- Dimensões máximas: 4000x4000px
- Redimensionamento automático: máx. 1920x1920px (mantém aspect ratio)

**Vídeos** (futuro):
- Tipos permitidos: MP4
- Tamanho máximo: 50MB
- Duração máxima: 5 minutos

### Segurança

- Validação de tipo MIME (não apenas extensão)
- Validação de conteúdo (magic bytes)
- Limite de tamanho por tipo
- Rate limiting no endpoint de upload
- Verificação de permissões no download

---

## 🔄 Impacto em Funcionalidades Existentes

### Análise de Impacto

**Fase 11 (Infraestrutura de Mídia)** não impacta funcionalidades existentes diretamente, pois apenas cria a base. No entanto, prepara o terreno para mudanças nas fases seguintes.

### Ajustes Preventivos

**Nenhum ajuste necessário nesta fase**, mas é importante:

1. **Documentar** que o sistema de mídia será usado nas fases seguintes
2. **Garantir** que a interface `IMediaStorageService` seja flexível o suficiente
3. **Preparar** estrutura de pastas e organização de arquivos

### Validação

- [ ] Sistema de mídia funcionando isoladamente
- [ ] Testes de mídia passando
- [ ] Documentação completa
- [ ] Pronto para integração nas fases seguintes

---

**Status**: ⏳ **FASE 8 PENDENTE**  
**Base para**: Fases 9, 10, 11 (Perfil, Mídias em Conteúdo, Edição)  
**Impacto**: ⚪ Nenhum (apenas preparação)
