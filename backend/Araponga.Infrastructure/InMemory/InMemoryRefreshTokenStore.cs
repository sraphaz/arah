using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Araponga.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private static readonly TimeSpan DefaultRefreshExpiration = TimeSpan.FromDays(7);
    private readonly ConcurrentDictionary<string, (Guid UserId, DateTime ExpiresAt)> _store = new();
    private readonly TimeSpan _refreshTokenValidity;

    public InMemoryRefreshTokenStore(IOptions<RefreshTokenOptions>? options = null)
    {
        _refreshTokenValidity = options?.Value.RefreshTokenValidity ?? DefaultRefreshExpiration;
    }

    public (string Token, DateTime ExpiresAt) Issue(Guid userId)
    {
        var tokenBytes = new byte[32];
        RandomNumberGenerator.Fill(tokenBytes);
        var token = Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var hash = Hash(token);
        var expiresAt = DateTime.UtcNow.Add(_refreshTokenValidity);
        _store[hash] = (userId, expiresAt);
        return (token, expiresAt);
    }

    public Task<Guid?> ConsumeAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.FromResult<Guid?>(null);
        }

        var hash = Hash(token);
        if (!_store.TryRemove(hash, out var entry))
        {
            return Task.FromResult<Guid?>(null);
        }

        if (entry.ExpiresAt < DateTime.UtcNow)
        {
            return Task.FromResult<Guid?>(null);
        }

        return Task.FromResult<Guid?>(entry.UserId);
    }

    private static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }
}

public sealed class RefreshTokenOptions
{
    public TimeSpan RefreshTokenValidity { get; set; } = TimeSpan.FromDays(7);
}
