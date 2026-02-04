namespace Araponga.Application.Interfaces;

/// <summary>
/// Provider para envio de notificações push.
/// </summary>
public interface IPushNotificationProvider
{
    /// <summary>
    /// Envia notificação push para um dispositivo.
    /// </summary>
    Task<bool> SendAsync(
        string deviceToken,
        string platform,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação push para múltiplos dispositivos.
    /// </summary>
    Task<int> SendBatchAsync(
        IReadOnlyCollection<(string DeviceToken, string Platform)> devices,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default);
}
