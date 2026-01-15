using System.Text.Json;
using System.Text.Json.Serialization;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Araponga.Infrastructure.Outbox;

public sealed class OutboxDispatcherWorker : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        MaxDepth = 64,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
    private static readonly TimeSpan[] RetrySchedule =
    {
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(2),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromMinutes(30)
    };
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxDispatcherWorker> _logger;
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(5);

    public OutboxDispatcherWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxDispatcherWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessBatchAsync(stoppingToken);
            await Task.Delay(_pollInterval, stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ArapongaDbContext>();
        var inboxRepository = scope.ServiceProvider.GetRequiredService<INotificationInboxRepository>();
        var now = DateTime.UtcNow;

        var messages = await dbContext.OutboxMessages
            .Where(message => message.ProcessedAtUtc == null &&
                              (message.ProcessAfterUtc == null || message.ProcessAfterUtc <= now))
            .OrderBy(message => message.OccurredAtUtc)
            .Take(20)
            .ToListAsync(stoppingToken);

        if (messages.Count == 0)
        {
            return;
        }

        var preferencesRepository = scope.ServiceProvider.GetRequiredService<IUserPreferencesRepository>();

        foreach (var message in messages)
        {
            try
            {
                await HandleMessageAsync(message, inboxRepository, preferencesRepository, stoppingToken);
                message.ProcessedAtUtc = DateTime.UtcNow;
                message.LastError = null;
                message.ProcessAfterUtc = null;
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                message.ProcessedAtUtc = DateTime.UtcNow;
                message.LastError = null;
                message.ProcessAfterUtc = null;
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                message.Attempts++;
                message.LastError = ex.Message;
                message.ProcessAfterUtc = DateTime.UtcNow.Add(GetBackoffDelay(message.Attempts));
                await dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogWarning(ex, "Outbox processing failed for message {MessageId}", message.Id);
            }
        }
    }

    private static async Task HandleMessageAsync(
        Infrastructure.Postgres.Entities.OutboxMessageRecord message,
        INotificationInboxRepository inboxRepository,
        IUserPreferencesRepository preferencesRepository,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(message.Type, "notification.dispatch", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var payload = JsonSerializer.Deserialize<NotificationDispatchPayload>(message.PayloadJson, JsonOptions);
        if (payload is null)
        {
            throw new InvalidOperationException("Outbox payload is invalid.");
        }

        var dataJson = payload.Data is null
            ? null
            : JsonSerializer.Serialize(payload.Data, JsonOptions);

        foreach (var recipient in payload.Recipients.Distinct())
        {
            // Verificar preferências do usuário antes de enviar notificação
            var preferences = await preferencesRepository.GetByUserIdAsync(recipient, cancellationToken);
            if (preferences is not null)
            {
                var shouldNotify = payload.Kind switch
                {
                    "post.created" => preferences.NotificationPreferences.PostsEnabled,
                    "comment.created" => preferences.NotificationPreferences.CommentsEnabled,
                    "event.created" => preferences.NotificationPreferences.EventsEnabled,
                    "alert.created" => preferences.NotificationPreferences.AlertsEnabled,
                    "marketplace.inquiry" => preferences.NotificationPreferences.MarketplaceEnabled,
                    "report.created" => preferences.NotificationPreferences.ModerationEnabled,
                    "membership.request" => preferences.NotificationPreferences.MembershipRequestsEnabled,
                    _ => true // Notificações do sistema sempre habilitadas
                };

                if (!shouldNotify)
                {
                    continue; // Pular este destinatário
                }
            }

            var notification = new UserNotification(
                Guid.NewGuid(),
                recipient,
                payload.Title,
                payload.Body,
                payload.Kind,
                dataJson,
                DateTime.UtcNow,
                null,
                message.Id);

            await inboxRepository.AddAsync(notification, cancellationToken);
        }
    }

    private static TimeSpan GetBackoffDelay(int attempts)
    {
        if (attempts <= 0)
        {
            return RetrySchedule[0];
        }

        return attempts <= RetrySchedule.Length
            ? RetrySchedule[attempts - 1]
            : RetrySchedule[^1];
    }

    private static bool IsUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException &&
               postgresException.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}
