namespace Araponga.Application.Interfaces;

/// <summary>
/// Interface para cache distribuído (Redis ou fallback para IMemoryCache).
/// </summary>
public interface IDistributedCacheService
{
    /// <summary>
    /// Obtém um valor do cache.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Define um valor no cache com TTL.
    /// </summary>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um valor do cache.
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove múltiplas chaves do cache.
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se uma chave existe no cache.
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
