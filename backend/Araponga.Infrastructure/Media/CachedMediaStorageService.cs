using Araponga.Application.Interfaces.Media;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Araponga.Infrastructure.Media;

/// <summary>
/// Decorador para IMediaStorageService que adiciona cache de URLs de mídia.
/// </summary>
public sealed class CachedMediaStorageService : IMediaStorageService
{
    private readonly IMediaStorageService _innerService;
    private readonly IDistributedCache? _distributedCache;
    private readonly ILogger<CachedMediaStorageService>? _logger;
    private readonly bool _enableCache;
    private readonly TimeSpan _cacheExpiration;

    public CachedMediaStorageService(
        IMediaStorageService innerService,
        IDistributedCache? distributedCache = null,
        ILogger<CachedMediaStorageService>? logger = null,
        bool enableCache = true,
        TimeSpan? cacheExpiration = null)
    {
        _innerService = innerService;
        _distributedCache = distributedCache;
        _logger = logger;
        _enableCache = enableCache && distributedCache != null;
        _cacheExpiration = cacheExpiration ?? TimeSpan.FromHours(24);
    }

    public Task<string> UploadAsync(Stream stream, string mimeType, string fileName, CancellationToken cancellationToken = default)
    {
        // Upload não é cacheado
        return _innerService.UploadAsync(stream, mimeType, fileName, cancellationToken);
    }

    public Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        // Download não é cacheado (arquivo completo)
        return _innerService.DownloadAsync(storageKey, cancellationToken);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        // Invalidar cache ao deletar
        if (_enableCache)
        {
            _ = InvalidateCacheAsync(storageKey);
        }

        return _innerService.DeleteAsync(storageKey, cancellationToken);
    }

    public async Task<string> GetUrlAsync(string storageKey, TimeSpan? expiresIn = null, CancellationToken cancellationToken = default)
    {
        if (!_enableCache || expiresIn.HasValue)
        {
            // Se cache está desabilitado ou tem expiração customizada, não usar cache
            return await _innerService.GetUrlAsync(storageKey, expiresIn, cancellationToken);
        }

        var cacheKey = GetCacheKey(storageKey);

        try
        {
            // Tentar obter do cache
            var cachedUrl = await _distributedCache!.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(cachedUrl))
            {
                _logger?.LogDebug("URL cacheada encontrada para storage key: {StorageKey}", storageKey);
                return cachedUrl;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Erro ao buscar URL do cache para storage key: {StorageKey}", storageKey);
        }

        // Obter URL do serviço interno
        var url = await _innerService.GetUrlAsync(storageKey, expiresIn, cancellationToken);

        try
        {
            // Armazenar no cache
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };

            await _distributedCache!.SetStringAsync(cacheKey, url, options, cancellationToken);
            _logger?.LogDebug("URL armazenada no cache para storage key: {StorageKey}", storageKey);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Erro ao armazenar URL no cache para storage key: {StorageKey}", storageKey);
        }

        return url;
    }

    public Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        return _innerService.ExistsAsync(storageKey, cancellationToken);
    }

    private async Task InvalidateCacheAsync(string storageKey)
    {
        try
        {
            var cacheKey = GetCacheKey(storageKey);
            await _distributedCache!.RemoveAsync(cacheKey);
            _logger?.LogDebug("Cache invalidado para storage key: {StorageKey}", storageKey);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Erro ao invalidar cache para storage key: {StorageKey}", storageKey);
        }
    }

    private static string GetCacheKey(string storageKey)
    {
        return $"media:url:{storageKey}";
    }
}