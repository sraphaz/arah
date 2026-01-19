using Araponga.Application.Interfaces.Media;
using Araponga.Domain.Media;

namespace Araponga.Infrastructure.InMemory;

/// <summary>
/// Implementação in-memory do repositório de configurações de mídia por território.
/// </summary>
public sealed class InMemoryTerritoryMediaConfigRepository : ITerritoryMediaConfigRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTerritoryMediaConfigRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<TerritoryMediaConfig?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        var config = _dataStore.TerritoryMediaConfigs.FirstOrDefault(c => c.TerritoryId == territoryId);
        return Task.FromResult(config);
    }

    public Task<TerritoryMediaConfig> GetOrCreateDefaultAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        var existing = _dataStore.TerritoryMediaConfigs.FirstOrDefault(c => c.TerritoryId == territoryId);
        if (existing is not null)
        {
            return Task.FromResult(existing);
        }

        // Criar configuração padrão
        var defaultConfig = new TerritoryMediaConfig
        {
            TerritoryId = territoryId,
            Posts = new MediaContentConfig
            {
                ImagesEnabled = true,
                VideosEnabled = true,
                AudioEnabled = true,
                MaxMediaCount = 10,
                MaxVideoCount = 1,
                MaxAudioCount = 1,
                MaxImageSizeBytes = 10 * 1024 * 1024, // 10MB
                MaxVideoSizeBytes = 50 * 1024 * 1024, // 50MB
                MaxAudioSizeBytes = 10 * 1024 * 1024, // 10MB
                MaxVideoDurationSeconds = null, // Validação futura
                MaxAudioDurationSeconds = null // Validação futura
            },
            Events = new MediaContentConfig
            {
                ImagesEnabled = true,
                VideosEnabled = true,
                AudioEnabled = true,
                MaxMediaCount = 6, // 1 capa + 5 adicionais
                MaxVideoCount = 1,
                MaxAudioCount = 1,
                MaxImageSizeBytes = 10 * 1024 * 1024, // 10MB
                MaxVideoSizeBytes = 50 * 1024 * 1024, // 50MB (não pode exceder limite global)
                MaxAudioSizeBytes = 20 * 1024 * 1024, // 20MB
                MaxVideoDurationSeconds = null,
                MaxAudioDurationSeconds = null
            },
            Marketplace = new MediaContentConfig
            {
                ImagesEnabled = true,
                VideosEnabled = true,
                AudioEnabled = true,
                MaxMediaCount = 10,
                MaxVideoCount = 1,
                MaxAudioCount = 1,
                MaxImageSizeBytes = 10 * 1024 * 1024, // 10MB
                MaxVideoSizeBytes = 30 * 1024 * 1024, // 30MB
                MaxAudioSizeBytes = 5 * 1024 * 1024, // 5MB
                MaxVideoDurationSeconds = null,
                MaxAudioDurationSeconds = null
            },
            Chat = new MediaChatConfig
            {
                ImagesEnabled = true,
                AudioEnabled = true,
                VideosEnabled = false, // Sempre bloqueado
                MaxImageSizeBytes = 5 * 1024 * 1024, // 5MB
                MaxAudioSizeBytes = 2 * 1024 * 1024, // 2MB
                MaxAudioDurationSeconds = 60 // Mensagens de voz
            },
            UpdatedAtUtc = DateTime.UtcNow,
            UpdatedByUserId = null
        };

        _dataStore.TerritoryMediaConfigs.Add(defaultConfig);
        return Task.FromResult(defaultConfig);
    }

    public Task SaveAsync(TerritoryMediaConfig config, CancellationToken cancellationToken = default)
    {
        var existing = _dataStore.TerritoryMediaConfigs.FirstOrDefault(c => c.TerritoryId == config.TerritoryId);
        if (existing is not null)
        {
            _dataStore.TerritoryMediaConfigs.Remove(existing);
        }

        _dataStore.TerritoryMediaConfigs.Add(config);
        return Task.CompletedTask;
    }
}
