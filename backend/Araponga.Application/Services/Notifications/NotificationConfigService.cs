using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Interfaces.Notifications;
using Araponga.Domain.Notifications;

namespace Araponga.Application.Services.Notifications;

/// <summary>
/// Serviço para gerenciar configurações avançadas de notificações.
/// </summary>
public sealed class NotificationConfigService
{
    private readonly INotificationConfigRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationConfigService(
        INotificationConfigRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Busca configuração por território (com fallback para global).
    /// </summary>
    public async Task<Result<NotificationConfig>> GetConfigAsync(
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        // Primeiro tenta buscar config territorial
        if (territoryId.HasValue)
        {
            var territorial = await _repository.GetByTerritoryIdAsync(territoryId, cancellationToken);
            if (territorial is not null)
            {
                return Result<NotificationConfig>.Success(territorial);
            }
        }

        // Fallback para config global
        var global = await _repository.GetGlobalAsync(cancellationToken);
        if (global is not null)
        {
            return Result<NotificationConfig>.Success(global);
        }

        // Se não existe config, retorna config padrão
        return Result<NotificationConfig>.Success(CreateDefaultConfig());
    }

    /// <summary>
    /// Cria ou atualiza configuração.
    /// </summary>
    public async Task<Result<NotificationConfig>> CreateOrUpdateConfigAsync(
        Guid? territoryId,
        IReadOnlyDictionary<string, NotificationTypeConfig> notificationTypes,
        IReadOnlyList<string> availableChannels,
        IReadOnlyDictionary<string, string> templates,
        IReadOnlyDictionary<string, IReadOnlyList<string>> defaultChannels,
        bool enabled,
        CancellationToken cancellationToken)
    {
        // Validar canais
        var validChannels = new[] { "Email", "Push", "InApp", "SMS" };
        if (availableChannels.Any(c => !validChannels.Contains(c, StringComparer.OrdinalIgnoreCase)))
        {
            return Result<NotificationConfig>.Failure("Invalid channel. Valid channels: Email, Push, InApp, SMS");
        }

        // Buscar config existente
        var existing = await _repository.GetByTerritoryIdAsync(territoryId, cancellationToken);
        
        NotificationConfig config;
        if (existing is null)
        {
            config = new NotificationConfig(
                Guid.NewGuid(),
                territoryId,
                notificationTypes,
                availableChannels,
                templates,
                defaultChannels,
                enabled,
                DateTime.UtcNow,
                DateTime.UtcNow);
        }
        else
        {
            existing.Update(
                notificationTypes,
                availableChannels,
                templates,
                defaultChannels,
                enabled,
                DateTime.UtcNow);
            config = existing;
        }

        await _repository.SaveAsync(config, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<NotificationConfig>.Success(config);
    }

    /// <summary>
    /// Lista tipos de notificação disponíveis.
    /// </summary>
    public async Task<Result<IReadOnlyList<string>>> GetAvailableTypesAsync(
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var configResult = await GetConfigAsync(territoryId, cancellationToken);
        if (configResult.IsFailure)
        {
            return Result<IReadOnlyList<string>>.Failure(configResult.Error ?? "Failed to get config");
        }

        var types = configResult.Value!.NotificationTypes
            .Where(kvp => kvp.Value.Enabled)
            .Select(kvp => kvp.Key)
            .ToList();

        return Result<IReadOnlyList<string>>.Success(types);
    }

    /// <summary>
    /// Obtém templates para um tipo de notificação.
    /// </summary>
    public async Task<Result<string?>> GetTemplateAsync(
        Guid? territoryId,
        string notificationType,
        CancellationToken cancellationToken)
    {
        var configResult = await GetConfigAsync(territoryId, cancellationToken);
        if (configResult.IsFailure)
        {
            return Result<string?>.Failure(configResult.Error ?? "Failed to get config");
        }

        configResult.Value!.Templates.TryGetValue(notificationType, out var template);
        return Result<string?>.Success(template);
    }

    private static NotificationConfig CreateDefaultConfig()
    {
        var defaultTypes = new Dictionary<string, NotificationTypeConfig>
        {
            ["post.created"] = new NotificationTypeConfig("post.created", true, new[] { "InApp" }),
            ["comment.created"] = new NotificationTypeConfig("comment.created", true, new[] { "InApp" }),
            ["event.created"] = new NotificationTypeConfig("event.created", true, new[] { "InApp", "Email" }),
            ["alert.created"] = new NotificationTypeConfig("alert.created", true, new[] { "InApp", "Push", "Email" }),
            ["marketplace.inquiry"] = new NotificationTypeConfig("marketplace.inquiry", true, new[] { "InApp" }),
            ["report.created"] = new NotificationTypeConfig("report.created", true, new[] { "InApp" }),
            ["membership.request"] = new NotificationTypeConfig("membership.request", true, new[] { "InApp", "Email" })
        };

        return new NotificationConfig(
            Guid.NewGuid(),
            null, // Global
            defaultTypes,
            new[] { "Email", "Push", "InApp", "SMS" },
            new Dictionary<string, string>(),
            new Dictionary<string, IReadOnlyList<string>>(),
            true,
            DateTime.UtcNow,
            DateTime.UtcNow);
    }
}
