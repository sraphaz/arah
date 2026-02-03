using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Microsoft.Extensions.Logging;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar notificações push.
/// </summary>
public sealed class PushNotificationService
{
    private readonly IUserDeviceRepository _deviceRepository;
    private readonly IPushNotificationProvider? _pushProvider;
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(
        IUserDeviceRepository deviceRepository,
        IPushNotificationProvider? pushProvider,
        ILogger<PushNotificationService> logger)
    {
        _deviceRepository = deviceRepository;
        _pushProvider = pushProvider;
        _logger = logger;
    }

    /// <summary>
    /// Registra um dispositivo para receber notificações push.
    /// </summary>
    public async Task<Result<UserDevice>> RegisterDeviceAsync(
        Guid userId,
        string deviceToken,
        string platform,
        string? deviceName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(deviceToken))
        {
            return Result<UserDevice>.Failure("Device token is required.");
        }

        if (string.IsNullOrWhiteSpace(platform))
        {
            return Result<UserDevice>.Failure("Platform is required.");
        }

        // Verificar se dispositivo já existe
        var existing = await _deviceRepository.GetByTokenAsync(deviceToken, cancellationToken);
        if (existing is not null)
        {
            // Atualizar dispositivo existente
            existing.UpdateToken(deviceToken);
            if (!string.IsNullOrWhiteSpace(deviceName))
            {
                existing.UpdateDeviceName(deviceName);
            }
            existing.MarkAsActive();

            await _deviceRepository.UpdateAsync(existing, cancellationToken);
            return Result<UserDevice>.Success(existing);
        }

        // Criar novo dispositivo
        var device = new UserDevice(
            Guid.NewGuid(),
            userId,
            deviceToken,
            platform,
            deviceName,
            DateTime.UtcNow);

        await _deviceRepository.AddAsync(device, cancellationToken);
        return Result<UserDevice>.Success(device);
    }

    /// <summary>
    /// Lista todos os dispositivos ativos de um usuário.
    /// </summary>
    public async Task<Result<IReadOnlyList<UserDevice>>> ListDevicesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var devices = await _deviceRepository.ListActiveByUserIdAsync(userId, cancellationToken);
        return Result<IReadOnlyList<UserDevice>>.Success(devices);
    }

    /// <summary>
    /// Obtém um dispositivo específico.
    /// </summary>
    public async Task<Result<UserDevice>> GetDeviceAsync(
        Guid userId,
        Guid deviceId,
        CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId, cancellationToken);
        if (device is null)
        {
            return Result<UserDevice>.Failure("Device not found.");
        }

        if (device.UserId != userId)
        {
            return Result<UserDevice>.Failure("Device does not belong to user.");
        }

        return Result<UserDevice>.Success(device);
    }

    /// <summary>
    /// Remove um dispositivo.
    /// </summary>
    public async Task<Result<bool>> UnregisterDeviceAsync(
        Guid userId,
        Guid deviceId,
        CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId, cancellationToken);
        if (device is null)
        {
            return Result<bool>.Failure("Device not found.");
        }

        if (device.UserId != userId)
        {
            return Result<bool>.Failure("Device does not belong to user.");
        }

        await _deviceRepository.DeleteAsync(deviceId, cancellationToken);
        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Envia notificação push para um usuário.
    /// </summary>
    public async Task<Result<int>> SendToUserAsync(
        Guid userId,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        if (_pushProvider is null)
        {
            _logger.LogWarning("Push notification provider not configured. Skipping push notification.");
            return Result<int>.Success(0);
        }

        var devices = await _deviceRepository.ListActiveByUserIdAsync(userId, cancellationToken);
        if (devices.Count == 0)
        {
            return Result<int>.Success(0);
        }

        var deviceList = devices.Select(d => (d.DeviceToken, d.Platform)).ToList();
        var sentCount = await _pushProvider.SendBatchAsync(deviceList, title, body, data, cancellationToken);

        // Atualizar LastUsedAtUtc para dispositivos que receberam notificação
        foreach (var device in devices)
        {
            device.MarkAsActive();
            await _deviceRepository.UpdateAsync(device, cancellationToken);
        }

        return Result<int>.Success(sentCount);
    }

    /// <summary>
    /// Envia notificação push para múltiplos usuários.
    /// </summary>
    public async Task<Result<int>> SendToUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        string title,
        string body,
        IReadOnlyDictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        if (_pushProvider is null)
        {
            _logger.LogWarning("Push notification provider not configured. Skipping push notification.");
            return Result<int>.Success(0);
        }

        var totalSent = 0;
        foreach (var userId in userIds)
        {
            var result = await SendToUserAsync(userId, title, body, data, cancellationToken);
            if (result.IsSuccess)
            {
                totalSent += result.Value;
            }
        }

        return Result<int>.Success(totalSent);
    }
}
