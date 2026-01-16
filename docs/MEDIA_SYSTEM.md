# Sistema de Mídia - Documentação Técnica

**Data**: 2025-01-16  
**Versão**: 1.0.0  
**Status**: ✅ Implementado

---

## 🎯 Visão Geral

O Sistema de Mídia do Araponga fornece infraestrutura completa para armazenamento, processamento e gerenciamento de mídias (imagens, vídeos, áudios, documentos). O sistema foi projetado para ser seguro, escalável e preparado para migração para cloud storage quando necessário.

**Valores Mantidos**: Mídias servem para **documentar território** e **fortalecer comunidade**, não para capturar atenção.

---

## 📐 Arquitetura

### Camadas

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer                            │
│  MediaController (Endpoints REST)                       │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│                Application Layer                         │
│  MediaService (Lógica de negócio)                       │
│  - Upload/Download                                       │
│  - Associação a entidades                                │
│  - Soft delete                                           │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│              Infrastructure Layer                        │
│  - IMediaStorageService (LocalMediaStorageService)      │
│  - IMediaProcessingService (LocalMediaProcessingService)│
│  - IMediaValidator (MediaValidator)                     │
│  - IMediaAssetRepository (PostgresMediaAssetRepository) │
│  - IMediaAttachmentRepository (PostgresMediaAttachment) │
└─────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────┐
│                  Domain Layer                            │
│  - MediaAsset (entidade)                                 │
│  - MediaAttachment (entidade)                            │
│  - MediaType (enum)                                      │
│  - MediaOwnerType (enum)                                 │
└─────────────────────────────────────────────────────────┘
```

---

## 🗂️ Modelo de Domínio

### MediaAsset

Representa um arquivo de mídia armazenado no sistema.

**Propriedades**:
- `Id` (Guid): Identificador único
- `UploadedByUserId` (Guid): ID do usuário que fez upload
- `MediaType` (MediaType): Tipo de mídia (Image, Video, Audio, Document)
- `MimeType` (string): Tipo MIME do arquivo (ex: "image/jpeg")
- `StorageKey` (string): Chave única no sistema de armazenamento
- `SizeBytes` (long): Tamanho do arquivo em bytes
- `WidthPx` (int?): Largura em pixels (apenas para imagens/vídeos)
- `HeightPx` (int?): Altura em pixels (apenas para imagens/vídeos)
- `Checksum` (string): Checksum SHA-256 para verificação de integridade
- `CreatedAtUtc` (DateTime): Data/hora de criação
- `DeletedByUserId` (Guid?): ID do usuário que deletou (soft delete)
- `DeletedAtUtc` (DateTime?): Data/hora de exclusão (soft delete)

**Métodos**:
- `Delete(Guid deletedByUserId, DateTime deletedAtUtc)`: Soft delete
- `Restore()`: Restaura mídia deletada

### MediaAttachment

Representa a associação de uma mídia a uma entidade do sistema.

**Propriedades**:
- `Id` (Guid): Identificador único
- `MediaAssetId` (Guid): ID da mídia associada
- `OwnerType` (MediaOwnerType): Tipo da entidade proprietária
- `OwnerId` (Guid): ID da entidade proprietária
- `DisplayOrder` (int): Ordem de exibição (quando múltiplas mídias)
- `CreatedAtUtc` (DateTime): Data/hora de criação

**Tipos de Owner**:
- `User`: Avatar/foto de perfil
- `Post`: Imagens em posts
- `Event`: Imagens em eventos
- `StoreItem`: Imagens em anúncios (marketplace)
- `ChatMessage`: Imagens em mensagens

---

## 🔐 Segurança

### Validações Implementadas

1. **Tipo MIME**:
   - Validação do tipo MIME real (não apenas extensão)
   - Tipos permitidos configuráveis em `appsettings.json`

2. **Tamanho de Arquivo**:
   - Imagens: máximo 10MB
   - Vídeos: máximo 50MB
   - Configurável em `MediaStorageOptions`

3. **Dimensões de Imagem**:
   - Máximo 4000x4000px
   - Redimensionamento automático para 1920x1920px (mantém aspect ratio)

4. **Path Traversal**:
   - Validação de caminhos para evitar acesso a arquivos fora do diretório permitido
   - Normalização de caminhos antes de operações de I/O

5. **Checksum**:
   - Verificação SHA-256 de integridade de arquivos
   - Armazenado junto com o MediaAsset

6. **Permissões**:
   - Apenas o criador pode deletar sua própria mídia
   - Autenticação obrigatória para upload/download

7. **Rate Limiting**:
   - Rate limiting configurado no endpoint de upload
   - Proteção contra abuso de uploads

---

## 🚀 Uso da API

### Upload de Mídia

```http
POST /api/v1/media/upload
Content-Type: multipart/form-data
Authorization: Bearer {token}

file: {arquivo}
```

**Resposta**:
```json
{
  "id": "guid",
  "uploadedByUserId": "guid",
  "mediaType": "IMAGE",
  "mimeType": "image/jpeg",
  "storageKey": "images/2025/01/guid.jpg",
  "sizeBytes": 1024,
  "widthPx": 1920,
  "heightPx": 1080,
  "checksum": "abc123...",
  "createdAtUtc": "2025-01-16T10:00:00Z",
  "isDeleted": false
}
```

### Download de Mídia

```http
GET /api/v1/media/{id}
```

**Resposta**: Arquivo binário com Content-Type apropriado

### Obter Informações da Mídia

```http
GET /api/v1/media/{id}/info
```

**Resposta**: `MediaAssetResponse` (JSON)

### Deletar Mídia

```http
DELETE /api/v1/media/{id}
Authorization: Bearer {token}
```

**Resposta**: `204 No Content` (apenas criador pode deletar)

---

## ☁️ Cloud Storage

### Amazon S3

Para usar S3, configure:

```json
{
  "MediaStorage": {
    "Provider": "S3",
    "S3BucketName": "meu-bucket",
    "S3Region": "us-east-1",
    "S3AccessKeyId": "AKIA...",
    "S3SecretAccessKey": "...",
    "S3Prefix": "media"
  }
}
```

**Características**:
- URLs pré-assinadas para acesso seguro
- Suporte a múltiplas regiões AWS
- Prefixo opcional para organização
- Integração com IAM roles (se credenciais não fornecidas)

### Azure Blob Storage

Para usar Azure Blob Storage, configure:

```json
{
  "MediaStorage": {
    "Provider": "AzureBlob",
    "AzureBlobConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
    "AzureBlobContainerName": "media",
    "AzureBlobPrefix": "media"
  }
}
```

**Características**:
- URLs com SAS (Shared Access Signature) para acesso seguro
- Suporte a containers privados
- Prefixo opcional para organização
- Integração com Azure Key Vault (via connection string)

## 💾 Cache de URLs

O sistema implementa cache de URLs de mídia usando `IDistributedCache`:

- **Cache em Memória**: Se Redis não estiver configurado
- **Redis**: Se `ConnectionStrings:Redis` estiver configurado
- **Expiração Padrão**: 24 horas (configurável via `UrlCacheExpiration`)

**Configuração**:
```json
{
  "MediaStorage": {
    "EnableUrlCache": true,
    "UrlCacheExpiration": "24:00:00"
  },
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

## ⚙️ Configuração

### appsettings.json

```json
{
  "MediaStorage": {
    "Provider": "Local",
    "LocalPath": "wwwroot/media",
    "MaxImageSizeBytes": 10485760,
    "MaxVideoSizeBytes": 52428800,
    "MaxImageWidthPx": 4000,
    "MaxImageHeightPx": 4000,
    "AutoResizeMaxWidthPx": 1920,
    "AutoResizeMaxHeightPx": 1920,
    "EnableUrlCache": true,
    "UrlCacheExpiration": "24:00:00",
    "EnableAsyncProcessing": true,
    "AsyncProcessingThresholdBytes": 5242880,
    "S3BucketName": null,
    "S3Region": "us-east-1",
    "S3AccessKeyId": null,
    "S3SecretAccessKey": null,
    "S3Prefix": null,
    "AzureBlobConnectionString": null,
    "AzureBlobContainerName": null,
    "AzureBlobPrefix": null
  }
}
```

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

---

## 📊 Testes

### Cobertura de Testes

- ✅ **Testes de Domínio**: MediaAsset, MediaAttachment (100%)
- ✅ **Testes de Serviço**: MediaService (com Moq) (100%)
- ✅ **Testes de Segurança**: Validações, path traversal, rate limiting (100%)
- ✅ **Testes de Integração**: MediaController completo (100%)
- ✅ **Testes de Performance**: Upload múltiplas imagens, cache de URLs, listagem (100%)

### Testes de Segurança Implementados

1. Validação de tipo MIME inválido
2. Proteção contra path traversal
3. Validação de tamanho de arquivo
4. Validação de autenticação
5. Rate limiting
6. Validação de extensões maliciosas

---

## 🔄 Processamento de Imagens

### Otimização Automática

Quando uma imagem é uploadada:

1. **Validação**: Verifica tipo MIME e dimensões
2. **Redimensionamento**: Se exceder 1920x1920px, redimensiona mantendo aspect ratio
3. **Compressão**: 
   - JPEG: Qualidade 85%
   - PNG: Compressão máxima
   - WebP: Qualidade 85%
4. **Armazenamento**: Salva arquivo otimizado
5. **Checksum**: Calcula SHA-256 do arquivo final

### Processamento Assíncrono (Opcional)

Para imagens grandes (> 5MB), o sistema pode processar de forma assíncrona:

1. **Upload Inicial**: Upload imediato com processamento básico
2. **Enfileiramento**: Imagem é enfileirada para processamento assíncrono
3. **Processamento em Background**: `AsyncMediaProcessingBackgroundService` processa a imagem
4. **Otimização**: Redimensionamento e compressão adicionais são aplicados
5. **Substituição**: Arquivo original é substituído pela versão otimizada

**Configuração**: `EnableAsyncProcessing: true` em `MediaStorageOptions`

### Biblioteca Utilizada

- **SixLabors.ImageSharp** 3.1.6: Processamento de imagens em .NET

---

## 📝 Pendências e Melhorias Futuras

### Pendências

✅ **Todas as pendências foram resolvidas!**

1. ✅ **Migrations do banco de dados**: Migration criada (`20260120120000_AddMediaSystem.cs`)
2. ✅ **Implementações InMemory**: Repositórios InMemory criados e registrados
3. ✅ **Testes de Integração**: Testes completos do MediaController implementados

### Melhorias Futuras

1. ✅ **Cloud Storage**: ✅ Implementado S3MediaStorageService e AzureBlobMediaStorageService
2. ✅ **Cache**: ✅ Cache de URLs de mídia implementado (CachedMediaStorageService)
3. ✅ **Processamento Assíncrono**: ✅ Background jobs para processamento de imagens grandes (AsyncMediaProcessingBackgroundService)
4. ✅ **Testes de Performance**: ✅ Testes de performance implementados (upload múltiplas imagens, cache, listagem)
5. **CDN Integration**: Integração com CDN para distribuição de mídias
6. **Watermark**: Adicionar watermark opcional em imagens
7. **Thumbnails**: Geração automática de thumbnails

---

## 📚 Referências

- [Fase 8 - Plano de Ação](./backlog-api/FASE8.md)
- [SixLabors.ImageSharp Documentation](https://docs.sixlabors.com/articles/imagesharp/)
- [ASP.NET Core File Upload](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads)

---

---

## 🏭 Factory Pattern

O sistema usa `MediaStorageFactory` para criar instâncias de `IMediaStorageService` baseado na configuração:

```csharp
var factory = serviceProvider.GetRequiredService<MediaStorageFactory>();
var storageService = factory.CreateStorageService();
```

A factory:
1. Cria o serviço de storage apropriado (Local, S3, ou Azure Blob)
2. Adiciona cache se habilitado (`CachedMediaStorageService`)
3. Retorna uma instância pronta para uso

---

**Última Atualização**: 2025-01-16  
**Funcionalidades Opcionais**: ✅ 100% Implementado (Cloud Storage, Cache, Processamento Assíncrono, Testes de Performance)