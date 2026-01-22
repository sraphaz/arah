using System.Text.Json;
using System.Text.Json.Serialization;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        var emailQueueService = scope.ServiceProvider.GetService<EmailQueueService>();
        var userRepository = scope.ServiceProvider.GetService<IUserRepository>();
        var configuration = scope.ServiceProvider.GetService<Microsoft.Extensions.Configuration.IConfiguration>();
        var baseUrl = configuration?["BaseUrl"] ?? "https://araponga.com";

        foreach (var message in messages)
        {
            try
            {
                await HandleMessageAsync(
                    message,
                    inboxRepository,
                    preferencesRepository,
                    emailQueueService,
                    userRepository,
                    baseUrl,
                    stoppingToken);
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
        EmailQueueService? emailQueueService,
        IUserRepository? userRepository,
        string baseUrl,
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

            // Enfileirar email se apropriado
            if (emailQueueService != null && EmailNotificationMapper.ShouldSendEmail(payload.Kind))
            {
                // Verificar preferências de email do usuário
                var userPrefs = await preferencesRepository.GetByUserIdAsync(recipient, cancellationToken);
                if (userPrefs != null && userPrefs.EmailPreferences.ReceiveEmails)
                {
                    // Verificar se o tipo de email está habilitado
                    var emailType = GetEmailTypeForNotification(payload.Kind);
                    if (emailType != null && userPrefs.EmailPreferences.EmailTypes.HasFlag(emailType.Value))
                    {
                        await EnqueueEmailForNotificationAsync(
                            emailQueueService,
                            userRepository,
                            recipient,
                            payload,
                            baseUrl,
                            cancellationToken);
                    }
                }
            }
        }
    }

    private static EmailTypes? GetEmailTypeForNotification(string notificationKind)
    {
        return notificationKind switch
        {
            "event.created" or "event.reminder" => EmailTypes.Events,
            "marketplace.order.confirmed" => EmailTypes.Marketplace,
            "alert.critical" => EmailTypes.CriticalAlerts,
            _ => null
        };
    }

    private static async Task EnqueueEmailForNotificationAsync(
        EmailQueueService emailQueueService,
        IUserRepository? userRepository,
        Guid userId,
        NotificationDispatchPayload payload,
        string baseUrl,
        CancellationToken cancellationToken)
    {
        try
        {
            // Buscar usuário para obter email e nome
            var user = userRepository != null
                ? await userRepository.GetByIdAsync(userId, cancellationToken)
                : null;

            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return; // Usuário não encontrado ou sem email
            }

            var templateName = EmailNotificationMapper.GetEmailTemplate(payload.Kind);
            if (templateName == null)
            {
                return; // Não há template para este tipo de notificação
            }

            var baseTemplateData = EmailNotificationMapper.CreateTemplateData(payload.Kind, payload, baseUrl);
            if (baseTemplateData == null)
            {
                return; // Não foi possível criar dados do template
            }

            // Criar nova instância com UserName atualizado
            object? templateData = baseTemplateData switch
            {
                EventReminderEmailTemplateData eventData => new EventReminderEmailTemplateData
                {
                    UserName = user.DisplayName,
                    BaseUrl = eventData.BaseUrl,
                    EventName = eventData.EventName,
                    EventDate = eventData.EventDate,
                    Location = eventData.Location,
                    EventLink = eventData.EventLink
                },
                MarketplaceOrderEmailTemplateData orderData => new MarketplaceOrderEmailTemplateData
                {
                    UserName = user.DisplayName,
                    BaseUrl = orderData.BaseUrl,
                    OrderId = orderData.OrderId,
                    Items = orderData.Items,
                    Total = orderData.Total,
                    SellerName = orderData.SellerName,
                    OrderLink = orderData.OrderLink
                },
                CriticalAlertEmailTemplateData alertData => new CriticalAlertEmailTemplateData
                {
                    UserName = user.DisplayName,
                    BaseUrl = alertData.BaseUrl,
                    AlertType = alertData.AlertType,
                    AlertTitle = alertData.AlertTitle,
                    AlertDescription = alertData.AlertDescription,
                    RecommendedActions = alertData.RecommendedActions,
                    MoreInfoLink = alertData.MoreInfoLink
                },
                _ => baseTemplateData
            };

            var priority = EmailNotificationMapper.GetEmailPriority(payload.Kind);

            var emailMessage = new EmailMessage
            {
                To = user.Email,
                Subject = payload.Title,
                Body = string.Empty, // Não usado quando TemplateName é fornecido
                TemplateName = templateName,
                TemplateData = templateData,
                IsHtml = true
            };

            await emailQueueService.EnqueueEmailAsync(emailMessage, priority, null, cancellationToken);
        }
        catch (Exception)
        {
            // Log mas não falha o processamento da notificação in-app
            // O email é opcional, a notificação in-app é essencial
            // Exceções são silenciadas para não quebrar o fluxo de notificações
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
