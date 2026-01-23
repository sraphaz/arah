using Araponga.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Notifications;

/// <summary>
/// Provider de notificações push usando Firebase Cloud Messaging (FCM).
/// Implementação básica - em produção, usar Firebase Admin SDK.
/// </summary>
public sealed class FirebasePushNotificationProvider : IPushNotificationProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirebasePushNotificationProvider> _logger;

    public FirebasePushNotificationProvider(
        IConfiguration configuration,
        ILogger<FirebasePushNotificationProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<bool> SendAsync(
        string deviceToken,
        string platform,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        var serverKey = _configuration["Firebase:ServerKey"];
        if (string.IsNullOrWhiteSpace(serverKey))
        {
            _logger.LogWarning("Firebase ServerKey not configured. Push notification skipped.");
            return Task.FromResult(false);
        }

        // Em produção, usar Firebase Admin SDK
        // Por enquanto, apenas logamos
        _logger.LogInformation(
            "Push notification would be sent to {Platform} device {DeviceToken}: {Title} - {Body}",
            platform, deviceToken.Length > 8 ? deviceToken[..8] + "..." : deviceToken, title, body);

        // TODO: Implementar chamada real ao FCM quando necessário
        // var fcmMessage = new { to = deviceToken, notification = new { title, body }, data };
        // await _httpClient.PostAsync("https://fcm.googleapis.com/fcm/send", ...);

        return Task.FromResult(true);
    }

    public async Task<int> SendBatchAsync(
        IReadOnlyCollection<(string DeviceToken, string Platform)> devices,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        if (devices.Count == 0)
        {
            return 0;
        }

        var tasks = devices.Select(d => SendAsync(d.DeviceToken, d.Platform, title, body, data, cancellationToken));
        var results = await Task.WhenAll(tasks);

        return results.Count(r => r);
    }
}
