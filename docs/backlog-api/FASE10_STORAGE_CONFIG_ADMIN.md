# Configuração de Blob Storage via Painel Administrativo

## Objetivo

Permitir configuração explícita e aberta do provedor de blob storage para mídias através do painel administrativo do sistema, substituindo a configuração fixa via `appsettings.json`.

## Situação Atual

- **Configuração atual**: Fixa via `appsettings.json` (seção `MediaStorage`)
- **Providers suportados**: Local, S3 (AWS), AzureBlob
- **Configuração**: `MediaStorageOptions` com propriedades para cada provider
- **Factory**: `MediaStorageFactory` cria instâncias baseadas no provider configurado

## Requisitos

1. **Configuração explícita**: Permitir selecionar provider de storage no painel administrativo
2. **Configuração aberta**: Documentar claramente os providers suportados e como configurá-los
3. **Painel administrativo**: Interface via API para gerenciar configuração de storage
4. **Documentação**: Instruções claras na documentação e no DevPortal

## Arquitetura Proposta

### 1. Modelo de Domínio

```csharp
// Araponga.Domain.Media/MediaStorageConfig.cs
public sealed class MediaStorageConfig
{
    public Guid Id { get; }
    public MediaStorageProvider Provider { get; }
    public MediaStorageSettings Settings { get; }
    public bool IsActive { get; }
    public DateTime CreatedAtUtc { get; }
    public Guid CreatedByUserId { get; }
    public DateTime? UpdatedAtUtc { get; }
    public Guid? UpdatedByUserId { get; }
}

public enum MediaStorageProvider
{
    Local = 1,
    S3 = 2,
    AzureBlob = 3
}

public sealed record MediaStorageSettings
{
    // Configurações comuns
    public bool EnableUrlCache { get; init; }
    public TimeSpan? UrlCacheExpiration { get; init; }
    
    // Configurações específicas por provider
    public LocalStorageSettings? Local { get; init; }
    public S3StorageSettings? S3 { get; init; }
    public AzureBlobStorageSettings? AzureBlob { get; init; }
}

public sealed record LocalStorageSettings(string BasePath);
public sealed record S3StorageSettings(string BucketName, string Region, string AccessKeyId, string? Prefix);
public sealed record AzureBlobStorageSettings(string ConnectionString, string ContainerName, string? Prefix);
```

### 2. Repositório

```csharp
// Araponga.Application/Interfaces/Media/IMediaStorageConfigRepository.cs
public interface IMediaStorageConfigRepository
{
    Task<MediaStorageConfig?> GetActiveAsync(CancellationToken cancellationToken);
    Task<MediaStorageConfig?> GetByIdAsync(Guid configId, CancellationToken cancellationToken);
    Task<IReadOnlyList<MediaStorageConfig>> ListAllAsync(CancellationToken cancellationToken);
    Task AddAsync(MediaStorageConfig config, CancellationToken cancellationToken);
    Task UpdateAsync(MediaStorageConfig config, CancellationToken cancellationToken);
    Task DeactivateAllAsync(CancellationToken cancellationToken);
}
```

### 3. Serviço

```csharp
// Araponga.Application/Services/Media/MediaStorageConfigService.cs
public sealed class MediaStorageConfigService
{
    private readonly IMediaStorageConfigRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    
    public async Task<MediaStorageConfig> GetActiveConfigAsync(CancellationToken cancellationToken);
    public async Task<MediaStorageConfig> CreateConfigAsync(
        MediaStorageProvider provider,
        MediaStorageSettings settings,
        Guid createdByUserId,
        CancellationToken cancellationToken);
    public async Task<MediaStorageConfig> UpdateConfigAsync(
        Guid configId,
        MediaStorageSettings settings,
        Guid updatedByUserId,
        CancellationToken cancellationToken);
    public async Task<MediaStorageConfig> ActivateConfigAsync(
        Guid configId,
        Guid updatedByUserId,
        CancellationToken cancellationToken);
}
```

### 4. API Controller

```csharp
// Araponga.Api/Controllers/MediaStorageConfigController.cs
[ApiController]
[Route("api/v1/admin/media-storage-config")]
[Tags("Admin - Media Storage")]
public sealed class MediaStorageConfigController : ControllerBase
{
    // GET: Obter configuração ativa
    // GET: Listar todas as configurações
    // POST: Criar nova configuração (SystemAdmin)
    // PUT: Atualizar configuração (SystemAdmin)
    // POST: Ativar configuração (SystemAdmin)
}
```

### 5. Integração com MediaStorageFactory

```csharp
// Araponga.Infrastructure/Media/MediaStorageFactory.cs
public sealed class MediaStorageFactory
{
    private readonly IMediaStorageConfigService _configService;
    
    public async Task<IMediaStorageService> CreateStorageServiceAsync(
        CancellationToken cancellationToken)
    {
        var config = await _configService.GetActiveConfigAsync(cancellationToken);
        // Criar service baseado na configuração ativa
    }
}
```

## Fluxo de Configuração

1. **SystemAdmin acessa painel administrativo**
2. **Navega para "Configurações > Media Storage"**
3. **Cria nova configuração**:
   - Seleciona provider (Local, S3, AzureBlob)
   - Preenche configurações específicas do provider
   - Salva configuração (inativa)
4. **Ativa configuração**:
   - Sistema desativa todas as configurações anteriores
   - Ativa a nova configuração
   - `MediaStorageFactory` começa a usar a nova configuração

## Segurança

- **Acesso restrito**: Apenas usuários com `SystemAdmin` permission
- **Auditoria**: Todas as mudanças registradas via `IAuditLogger`
- **Secrets**: Secrets (S3 keys, Azure connection strings) armazenados via `ISecretsService` (não em `MediaStorageConfig`)

## Migração

- **Configuração existente**: Ler `appsettings.json` e criar `MediaStorageConfig` inicial na primeira execução
- **Fallback**: Se não houver configuração ativa, usar `appsettings.json` como fallback

## Documentação

### DevPortal (`index.html`)

Adicionar seção:
```html
<section id="media-storage-config">
  <h2>📦 Configuração de Blob Storage para Mídias</h2>
  
  <h3>Providers Suportados</h3>
  <ul>
    <li><strong>Local</strong>: Armazenamento em disco local (desenvolvimento/testes)</li>
    <li><strong>S3</strong>: Amazon S3 (produção recomendado)</li>
    <li><strong>AzureBlob</strong>: Azure Blob Storage (produção)</li>
  </ul>
  
  <h3>Configuração via API</h3>
  <p>Endpoints disponíveis em <code>/api/v1/admin/media-storage-config</code></p>
  
  <h3>Exemplo: Configurar S3</h3>
  <pre><code>POST /api/v1/admin/media-storage-config
{
  "provider": "S3",
  "settings": {
    "bucketName": "my-media-bucket",
    "region": "us-east-1",
    "accessKeyId": "AKIA...",
    "prefix": "media/"
  }
}</code></pre>
</section>
```

### Documentação (`FASE10.md`)

Adicionar seção sobre configuração de storage:
- Instruções para cada provider
- Migração de `appsettings.json` para painel administrativo
- Troubleshooting

## Implementação

### Fase 1: Modelo de Domínio
- [ ] Criar `MediaStorageConfig` domain model
- [ ] Criar `MediaStorageProvider` enum
- [ ] Criar `MediaStorageSettings` records

### Fase 2: Repositório
- [ ] Criar `IMediaStorageConfigRepository`
- [ ] Implementar `InMemoryMediaStorageConfigRepository`
- [ ] Implementar `PostgresMediaStorageConfigRepository` (futuro)

### Fase 3: Serviço
- [ ] Criar `MediaStorageConfigService`
- [ ] Integrar com `IAuditLogger`

### Fase 4: API
- [ ] Criar `MediaStorageConfigController`
- [ ] Adicionar validação (FluentValidation)
- [ ] Adicionar autorização (SystemAdmin)

### Fase 5: Integração
- [ ] Atualizar `MediaStorageFactory` para usar configuração do painel
- [ ] Implementar fallback para `appsettings.json`
- [ ] Implementar migração automática na primeira execução

### Fase 6: Documentação
- [ ] Atualizar `FASE10.md` com instruções de configuração
- [ ] Atualizar DevPortal (`index.html`) com seção de storage
- [ ] Adicionar exemplos de configuração para cada provider

## Testes

- [ ] Testes de unidade para `MediaStorageConfigService`
- [ ] Testes de integração para `MediaStorageConfigController`
- [ ] Testes de migração de `appsettings.json` para configuração do painel
- [ ] Testes de fallback quando não houver configuração ativa

## Considerações Futuras

- **Configuração por território**: Permitir diferentes providers por território (futuro)
- **Múltiplos providers ativos**: Suporte para replicação entre providers (futuro)
- **Health checks**: Verificar saúde do storage provider configurado
- **Métricas**: Métricas de uso por provider (bandwidth, storage, etc.)
