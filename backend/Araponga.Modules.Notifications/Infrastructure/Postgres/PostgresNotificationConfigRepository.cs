using Araponga.Application.Interfaces.Notifications;
using Araponga.Domain.Notifications;
using Araponga.Modules.Notifications.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Araponga.Modules.Notifications.Infrastructure.Postgres;

public sealed class PostgresNotificationConfigRepository : INotificationConfigRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly NotificationsDbContext _dbContext;

    public PostgresNotificationConfigRepository(NotificationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NotificationConfig?> GetByTerritoryIdAsync(Guid? territoryId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.NotificationConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.TerritoryId == territoryId,
                cancellationToken);

        return record is null ? null : ToDomain(record);
    }

    public async Task<NotificationConfig?> GetGlobalAsync(CancellationToken cancellationToken)
    {
        return await GetByTerritoryIdAsync(null, cancellationToken);
    }

    public async Task SaveAsync(NotificationConfig config, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.NotificationConfigs
            .FirstOrDefaultAsync(
                c => c.Id == config.Id,
                cancellationToken);

        if (existing is null)
        {
            var record = ToRecord(config);
            _dbContext.NotificationConfigs.Add(record);
        }
        else
        {
            UpdateRecord(existing, config);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationConfig>> ListTerritorialAsync(CancellationToken cancellationToken)
    {
        var records = await _dbContext.NotificationConfigs
            .AsNoTracking()
            .Where(c => c.TerritoryId != null)
            .ToListAsync(cancellationToken);

        return records.Select(ToDomain).ToList();
    }

    private static NotificationConfigRecord ToRecord(NotificationConfig config)
    {
        // Serializar NotificationTypeConfig manualmente
        var notificationTypesDict = new Dictionary<string, object>();
        foreach (var kvp in config.NotificationTypes)
        {
            notificationTypesDict[kvp.Key] = new
            {
                type = kvp.Value.Type,
                enabled = kvp.Value.Enabled,
                allowedChannels = kvp.Value.AllowedChannels
            };
        }

        return new NotificationConfigRecord
        {
            Id = config.Id,
            TerritoryId = config.TerritoryId,
            NotificationTypesJson = JsonSerializer.Serialize(notificationTypesDict, JsonOptions),
            AvailableChannelsJson = JsonSerializer.Serialize(config.AvailableChannels, JsonOptions),
            TemplatesJson = JsonSerializer.Serialize(config.Templates, JsonOptions),
            DefaultChannelsJson = JsonSerializer.Serialize(config.DefaultChannels, JsonOptions),
            Enabled = config.Enabled,
            CreatedAtUtc = config.CreatedAtUtc,
            UpdatedAtUtc = config.UpdatedAtUtc
        };
    }

    private static void UpdateRecord(NotificationConfigRecord record, NotificationConfig config)
    {
        // Serializar NotificationTypeConfig manualmente
        var notificationTypesDict = new Dictionary<string, object>();
        foreach (var kvp in config.NotificationTypes)
        {
            notificationTypesDict[kvp.Key] = new
            {
                type = kvp.Value.Type,
                enabled = kvp.Value.Enabled,
                allowedChannels = kvp.Value.AllowedChannels
            };
        }

        record.NotificationTypesJson = JsonSerializer.Serialize(notificationTypesDict, JsonOptions);
        record.AvailableChannelsJson = JsonSerializer.Serialize(config.AvailableChannels, JsonOptions);
        record.TemplatesJson = JsonSerializer.Serialize(config.Templates, JsonOptions);
        record.DefaultChannelsJson = JsonSerializer.Serialize(config.DefaultChannels, JsonOptions);
        record.Enabled = config.Enabled;
        record.UpdatedAtUtc = config.UpdatedAtUtc;
    }

    private static NotificationConfig ToDomain(NotificationConfigRecord record)
    {
        Dictionary<string, NotificationTypeConfig> notificationTypes = new();
        try
        {
            // Deserializar como Dictionary<string, object> primeiro, depois converter
            var rawTypes = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                record.NotificationTypesJson, JsonOptions);

            if (rawTypes != null)
            {
                foreach (var kvp in rawTypes)
                {
                    try
                    {
                        var type = kvp.Value.GetProperty("type").GetString() ?? kvp.Key;
                        var enabled = kvp.Value.TryGetProperty("enabled", out var enabledProp) && enabledProp.GetBoolean();
                        var channels = kvp.Value.TryGetProperty("allowedChannels", out var channelsProp)
                            ? channelsProp.EnumerateArray().Select(e => e.GetString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList()
                            : new List<string>();

                        notificationTypes[kvp.Key] = new NotificationTypeConfig(type, enabled, channels);
                    }
                    catch
                    {
                        // Ignorar tipos inválidos
                    }
                }
            }
        }
        catch
        {
            // Se falhar, usar vazio
        }

        List<string>? availableChannels = null;
        try
        {
            availableChannels = JsonSerializer.Deserialize<List<string>>(
                record.AvailableChannelsJson, JsonOptions);
        }
        catch
        {
            // Se falhar, usar vazio
        }
        availableChannels ??= new List<string>();

        Dictionary<string, string>? templates = null;
        try
        {
            templates = JsonSerializer.Deserialize<Dictionary<string, string>>(
                record.TemplatesJson, JsonOptions);
        }
        catch
        {
            // Se falhar, usar vazio
        }
        templates ??= new Dictionary<string, string>();

        Dictionary<string, List<string>>? defaultChannels = null;
        try
        {
            defaultChannels = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(
                record.DefaultChannelsJson, JsonOptions);
        }
        catch
        {
            // Se falhar, usar vazio
        }
        defaultChannels ??= new Dictionary<string, List<string>>();

        return new NotificationConfig(
            record.Id,
            record.TerritoryId,
            notificationTypes,
            availableChannels,
            templates,
            defaultChannels.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<string>)kvp.Value),
            record.Enabled,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }
}
