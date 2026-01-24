using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Email;
using Araponga.Domain.Events;
using Araponga.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Araponga.Infrastructure.Email;

/// <summary>
/// Background worker que verifica eventos que começam em 24 horas e envia lembretes por email.
/// </summary>
public sealed class EventReminderWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EventReminderWorker> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromHours(1); // Verifica a cada hora

    public EventReminderWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<EventReminderWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EventReminderWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessEventRemindersAsync(stoppingToken);
                await Task.Delay(_processingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EventReminderWorker main loop");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("EventReminderWorker stopped");
    }

    private async Task ProcessEventRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var eventRepository = scope.ServiceProvider.GetRequiredService<ITerritoryEventRepository>();
        var participationRepository = scope.ServiceProvider.GetRequiredService<IEventParticipationRepository>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var userPreferencesRepository = scope.ServiceProvider.GetRequiredService<IUserPreferencesRepository>();
        var emailQueueService = scope.ServiceProvider.GetRequiredService<EmailQueueService>();
        var configuration = scope.ServiceProvider.GetService<IConfiguration>();
        var baseUrl = configuration?["BaseUrl"] ?? "https://araponga.com";

        // Calcular janela de 24 horas: agora + 24h ± 1 hora (para pegar eventos que começam entre 23h e 25h)
        var now = DateTime.UtcNow;
        var reminderWindowStart = now.AddHours(23);
        var reminderWindowEnd = now.AddHours(25);

        // Buscar eventos agendados que começam na janela de 24 horas usando bounding box global
        // (latitude -90 a 90, longitude -180 a 180 cobre todo o globo)
        var events = await eventRepository.ListByBoundingBoxAsync(
            -90, 90, -180, 180,
            reminderWindowStart,
            reminderWindowEnd,
            null, // Todos os territórios
            cancellationToken);

        // Filtrar apenas eventos agendados
        events = events.Where(e => e.Status == EventStatus.Scheduled).ToList();

        if (events.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Found {Count} events starting in 24 hours", events.Count);

        var emailsSent = 0;
        foreach (var evt in events)
        {
            try
            {
                // Buscar participantes interessados ou confirmados
                var interestedParticipants = await participationRepository.ListByEventIdAsync(
                    evt.Id,
                    EventParticipationStatus.Interested,
                    cancellationToken);
                var confirmedParticipants = await participationRepository.ListByEventIdAsync(
                    evt.Id,
                    EventParticipationStatus.Confirmed,
                    cancellationToken);

                var allParticipantIds = interestedParticipants
                    .Select(p => p.UserId)
                    .Concat(confirmedParticipants.Select(p => p.UserId))
                    .Distinct()
                    .ToList();

                if (allParticipantIds.Count == 0)
                {
                    continue;
                }

                // Buscar usuários e suas preferências
                var users = await userRepository.ListByIdsAsync(allParticipantIds, cancellationToken);
                var preferencesLookup = new Dictionary<Guid, UserPreferences>();

                // Buscar preferências uma por uma (não há método batch)
                foreach (var userId in users.Select(u => u.Id))
                {
                    var prefs = await userPreferencesRepository.GetByUserIdAsync(userId, cancellationToken);
                    if (prefs is not null)
                    {
                        preferencesLookup[userId] = prefs;
                    }
                }

                foreach (var user in users)
                {
                    if (string.IsNullOrWhiteSpace(user.Email))
                    {
                        continue;
                    }

                    // Verificar preferências de email
                    if (!preferencesLookup.TryGetValue(user.Id, out var userPrefs))
                    {
                        continue;
                    }

                    var emailPrefs = userPrefs.EmailPreferences;
                    if (!emailPrefs.ReceiveEmails)
                    {
                        continue;
                    }

                    if (!emailPrefs.EmailTypes.HasFlag(EmailTypes.Events))
                    {
                        continue;
                    }

                    // Criar template data
                    var templateData = new EventReminderEmailTemplateData
                    {
                        UserName = user.DisplayName,
                        BaseUrl = baseUrl,
                        EventName = evt.Title,
                        EventDate = evt.StartsAtUtc,
                        Location = evt.LocationLabel ?? $"{evt.Latitude:F6}, {evt.Longitude:F6}",
                        EventLink = $"{baseUrl}/events/{evt.Id}"
                    };

                    var emailMessage = new EmailMessage
                    {
                        To = user.Email,
                        Subject = $"Lembrete: {evt.Title} em 24 horas",
                        TemplateName = "event-reminder",
                        TemplateData = templateData,
                        Body = string.Empty
                    };

                    await emailQueueService.EnqueueEmailAsync(
                        emailMessage,
                        EmailQueuePriority.Normal,
                        cancellationToken: cancellationToken);

                    emailsSent++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing reminder for event {EventId}", evt.Id);
            }
        }
    }
}
