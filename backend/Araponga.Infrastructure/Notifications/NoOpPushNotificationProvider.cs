using Araponga.Application.Interfaces;

namespace Araponga.Infrastructure.Notifications;

/// <summary>
/// Provider no-op para notificações push (testes ou quando Firebase não está configurado).
/// </summary>
public sealed class NoOpPushNotificationProvider : IPushNotificationProvider
{
    public Task<bool> SendAsync(
        string deviceToken,
        string platform,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public Task<int> SendBatchAsync(
        IReadOnlyCollection<(string DeviceToken, string Platform)> devices,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(0);
}
