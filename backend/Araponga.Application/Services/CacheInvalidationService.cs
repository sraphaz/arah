using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço centralizado para invalidação de cache.
/// Gerencia a invalidação de cache quando dados são modificados.
/// </summary>
public sealed class CacheInvalidationService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheInvalidationService> _logger;

    public CacheInvalidationService(IMemoryCache cache, ILogger<CacheInvalidationService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Invalida cache relacionado a membership de um usuário em um território.
    /// </summary>
    public void InvalidateMembershipCache(Guid userId, Guid territoryId)
    {
        var keys = new[]
        {
            $"membership:resident:{userId}:{territoryId}",
            $"membership:role:{userId}:{territoryId}",
            $"membership:capability:{userId}:{territoryId}:*"
        };

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated membership cache for user {UserId} in territory {TerritoryId}", userId, territoryId);
    }

    /// <summary>
    /// Invalida cache relacionado a capabilities de um membership.
    /// </summary>
    public void InvalidateCapabilityCache(Guid membershipId, Guid? userId = null, Guid? territoryId = null)
    {
        var keys = new List<string>
        {
            $"membership:capability:{membershipId}:*"
        };

        if (userId.HasValue && territoryId.HasValue)
        {
            keys.Add($"membership:capability:{userId.Value}:{territoryId.Value}:*");
        }

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated capability cache for membership {MembershipId}", membershipId);
    }

    /// <summary>
    /// Invalida cache relacionado a permissões do sistema de um usuário.
    /// </summary>
    public void InvalidateSystemPermissionCache(Guid userId)
    {
        var keys = new[]
        {
            $"system:permission:{userId}:*",
            $"system:admin:{userId}"
        };

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated system permission cache for user {UserId}", userId);
    }

    /// <summary>
    /// Invalida cache relacionado a território.
    /// </summary>
    public void InvalidateTerritoryCache(Guid territoryId)
    {
        var keys = new[]
        {
            $"territory:{territoryId}",
            $"territory:feature-flag:{territoryId}:*",
            $"territory:assets:{territoryId}:*",
            $"territory:alerts:{territoryId}:*",
            $"territory:events:{territoryId}:*"
        };

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated territory cache for {TerritoryId}", territoryId);
    }

    /// <summary>
    /// Invalida cache relacionado a assets de um território.
    /// </summary>
    public void InvalidateAssetCache(Guid territoryId, Guid? assetId = null)
    {
        var keys = new List<string>
        {
            $"territory:assets:{territoryId}:*"
        };

        if (assetId.HasValue)
        {
            keys.Add($"asset:{assetId.Value}");
        }

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated asset cache for territory {TerritoryId}", territoryId);
    }

    /// <summary>
    /// Invalida cache relacionado a alertas de um território.
    /// </summary>
    public void InvalidateAlertCache(Guid territoryId)
    {
        var keys = new[]
        {
            $"territory:alerts:{territoryId}:*",
            $"alert:list:{territoryId}"
        };

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated alert cache for territory {TerritoryId}", territoryId);
    }

    /// <summary>
    /// Invalida cache relacionado a eventos de um território.
    /// </summary>
    public void InvalidateEventCache(Guid territoryId, Guid? eventId = null)
    {
        var keys = new List<string>
        {
            $"territory:events:{territoryId}:*"
        };

        if (eventId.HasValue)
        {
            keys.Add($"event:{eventId.Value}");
        }

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated event cache for territory {TerritoryId}", territoryId);
    }

    /// <summary>
    /// Invalida cache relacionado a feed de um território.
    /// </summary>
    public void InvalidateFeedCache(Guid territoryId)
    {
        var keys = new[]
        {
            $"feed:territory:{territoryId}:*",
            $"feed:user:*:{territoryId}:*"
        };

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated feed cache for territory {TerritoryId}", territoryId);
    }

    /// <summary>
    /// Invalida cache relacionado a map entities de um território.
    /// </summary>
    public void InvalidateMapEntityCache(Guid territoryId)
    {
        var keys = new[]
        {
            $"map:entities:{territoryId}:*",
            $"map:pins:{territoryId}:*"
        };

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated map entity cache for territory {TerritoryId}", territoryId);
    }

    /// <summary>
    /// Invalida cache relacionado a feature flags de um território.
    /// </summary>
    public void InvalidateFeatureFlagCache(Guid territoryId)
    {
        var keys = new[]
        {
            $"territory:feature-flag:{territoryId}:*",
            $"feature-flag:{territoryId}:*"
        };

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated feature flag cache for territory {TerritoryId}", territoryId);
    }

    /// <summary>
    /// Invalida cache relacionado a stores de um território.
    /// </summary>
    public void InvalidateStoreCache(Guid territoryId, Guid? storeId = null)
    {
        var keys = new List<string>
        {
            $"territory:stores:{territoryId}:*"
        };

        if (storeId.HasValue)
        {
            keys.Add($"store:{storeId.Value}");
        }

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated store cache for territory {TerritoryId}", territoryId);
    }

    /// <summary>
    /// Invalida cache relacionado a items de uma store.
    /// </summary>
    public void InvalidateItemCache(Guid storeId, Guid? itemId = null)
    {
        var keys = new List<string>
        {
            $"store:items:{storeId}:*"
        };

        if (itemId.HasValue)
        {
            keys.Add($"item:{itemId.Value}");
        }

        InvalidateKeys(keys);
        _logger.LogDebug("Invalidated item cache for store {StoreId}", storeId);
    }

    /// <summary>
    /// Invalida todas as chaves de cache que correspondem aos padrões fornecidos.
    /// Nota: IMemoryCache não suporta busca por padrão diretamente.
    /// Esta implementação é limitada - em produção, considere usar Redis com SCAN.
    /// </summary>
    private void InvalidateKeys(IEnumerable<string> keyPatterns)
    {
        // IMemoryCache não suporta busca por padrão diretamente.
        // Para uma implementação completa, seria necessário:
        // 1. Manter um registro de todas as chaves criadas
        // 2. Usar Redis com SCAN para busca por padrão
        // 3. Ou implementar um wrapper que rastreia chaves
        
        // Por enquanto, apenas logamos que a invalidação foi solicitada
        // Em produção, implementar com Redis ou sistema de tracking de chaves
        foreach (var pattern in keyPatterns)
        {
            if (pattern.EndsWith("*"))
            {
                // Padrão com wildcard - em produção, usar Redis SCAN ou tracking
                _logger.LogWarning("Cache invalidation pattern with wildcard not fully supported: {Pattern}", pattern);
            }
            else
            {
                // Chave específica - pode invalidar diretamente
                _cache.Remove(pattern);
            }
        }
    }

    /// <summary>
    /// Invalida uma chave específica de cache.
    /// </summary>
    public void InvalidateKey(string key)
    {
        _cache.Remove(key);
        _logger.LogDebug("Invalidated cache key: {Key}", key);
    }

    /// <summary>
    /// Limpa todo o cache (use com cuidado).
    /// </summary>
    public void ClearAllCache()
    {
        // IMemoryCache não tem método Clear() direto
        // Em produção, usar Redis FLUSHDB ou implementar tracking de todas as chaves
        _logger.LogWarning("ClearAllCache called - not fully supported with IMemoryCache");
    }
}
