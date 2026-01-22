using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Events;
using Araponga.Domain.Feed;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Users;
using System.Text.Json;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para exportação de dados do usuário (LGPD).
/// </summary>
public sealed class DataExportService
{
    private readonly IUserRepository _userRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly ITerritoryEventRepository _eventRepository;
    private readonly IEventParticipationRepository _eventParticipationRepository;
    private readonly INotificationInboxRepository _notificationRepository;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ITermsAcceptanceRepository _termsAcceptanceRepository;
    private readonly IPrivacyPolicyAcceptanceRepository _privacyAcceptanceRepository;

    public DataExportService(
        IUserRepository userRepository,
        ITerritoryMembershipRepository membershipRepository,
        IFeedRepository feedRepository,
        ITerritoryEventRepository eventRepository,
        IEventParticipationRepository eventParticipationRepository,
        INotificationInboxRepository notificationRepository,
        IUserPreferencesRepository preferencesRepository,
        ITermsAcceptanceRepository termsAcceptanceRepository,
        IPrivacyPolicyAcceptanceRepository privacyAcceptanceRepository)
    {
        _userRepository = userRepository;
        _membershipRepository = membershipRepository;
        _feedRepository = feedRepository;
        _eventRepository = eventRepository;
        _eventParticipationRepository = eventParticipationRepository;
        _notificationRepository = notificationRepository;
        _preferencesRepository = preferencesRepository;
        _termsAcceptanceRepository = termsAcceptanceRepository;
        _privacyAcceptanceRepository = privacyAcceptanceRepository;
    }

    /// <summary>
    /// Exporta todos os dados do usuário em formato JSON (LGPD).
    /// </summary>
    public async Task<Result<UserDataExport>> ExportUserDataAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<UserDataExport>.Failure("User not found.");
        }

        // Coletar todos os dados do usuário
        var memberships = await _membershipRepository.ListByUserAsync(userId, cancellationToken);
        var posts = await _feedRepository.ListByAuthorAsync(userId, cancellationToken);
        var events = await _eventRepository.ListByAuthorPagedAsync(userId, 0, int.MaxValue, cancellationToken);
        var eventParticipations = await _eventParticipationRepository.GetByUserIdAsync(userId, cancellationToken);
        
        // Notificações (limitado a 1000 mais recentes para evitar arquivo muito grande)
        var notifications = await _notificationRepository.ListByUserAsync(userId, 0, 1000, cancellationToken);
        
        var preferences = await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken);
        var termsAcceptances = await _termsAcceptanceRepository.GetByUserIdAsync(userId, cancellationToken);
        var privacyAcceptances = await _privacyAcceptanceRepository.GetByUserIdAsync(userId, cancellationToken);

        var export = new UserDataExport
        {
            User = new UserExportData
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                CreatedAtUtc = user.CreatedAtUtc
            },
            Memberships = memberships.Select(m => new MembershipExportData
            {
                Id = m.Id,
                TerritoryId = m.TerritoryId,
                Role = m.Role.ToString(),
                ResidencyVerification = m.ResidencyVerification.ToString(),
                CreatedAtUtc = m.CreatedAtUtc,
                LastGeoVerifiedAtUtc = m.LastGeoVerifiedAtUtc,
                LastDocumentVerifiedAtUtc = m.LastDocumentVerifiedAtUtc
            }).ToList(),
            Posts = posts.Select(p => new PostExportData
            {
                Id = p.Id,
                TerritoryId = p.TerritoryId,
                Title = p.Title,
                Content = p.Content,
                Type = p.Type.ToString(),
                Visibility = p.Visibility.ToString(),
                Status = p.Status.ToString(),
                CreatedAtUtc = p.CreatedAtUtc,
                EditedAtUtc = p.EditedAtUtc,
                EditCount = p.EditCount
            }).ToList(),
            Events = events.Select(e => new EventExportData
            {
                Id = e.Id,
                TerritoryId = e.TerritoryId,
                Title = e.Title,
                Description = e.Description,
                StartsAtUtc = e.StartsAtUtc,
                EndsAtUtc = e.EndsAtUtc,
                CreatedAtUtc = e.CreatedAtUtc
            }).ToList(),
            EventParticipations = eventParticipations.Select(ep => new EventParticipationExportData
            {
                EventId = ep.EventId,
                Status = ep.Status.ToString(),
                CreatedAtUtc = ep.CreatedAtUtc,
                UpdatedAtUtc = ep.UpdatedAtUtc
            }).ToList(),
            Notifications = notifications.Select(n => new NotificationExportData
            {
                Id = n.Id,
                Kind = n.Kind,
                Title = n.Title,
                Body = n.Body,
                DataJson = n.DataJson,
                CreatedAtUtc = n.CreatedAtUtc,
                ReadAtUtc = n.ReadAtUtc
            }).ToList(),
            Preferences = preferences is not null ? new PreferencesExportData
            {
                ProfileVisibility = preferences.ProfileVisibility.ToString(),
                ContactVisibility = preferences.ContactVisibility.ToString(),
                ShareLocation = preferences.ShareLocation,
                ShowMemberships = preferences.ShowMemberships,
                NotificationPreferences = new NotificationPreferencesExportData
                {
                    PostsEnabled = preferences.NotificationPreferences.PostsEnabled,
                    CommentsEnabled = preferences.NotificationPreferences.CommentsEnabled,
                    EventsEnabled = preferences.NotificationPreferences.EventsEnabled,
                    AlertsEnabled = preferences.NotificationPreferences.AlertsEnabled,
                    MarketplaceEnabled = preferences.NotificationPreferences.MarketplaceEnabled,
                    ModerationEnabled = preferences.NotificationPreferences.ModerationEnabled,
                    MembershipRequestsEnabled = preferences.NotificationPreferences.MembershipRequestsEnabled
                },
                CreatedAtUtc = preferences.CreatedAtUtc,
                UpdatedAtUtc = preferences.UpdatedAtUtc
            } : null,
            TermsAcceptances = termsAcceptances.Select(ta => new TermsAcceptanceExportData
            {
                Id = ta.Id,
                TermsOfServiceId = ta.TermsOfServiceId,
                AcceptedVersion = ta.AcceptedVersion,
                AcceptedAtUtc = ta.AcceptedAtUtc,
                IsRevoked = ta.IsRevoked,
                RevokedAtUtc = ta.RevokedAtUtc
            }).ToList(),
            PrivacyPolicyAcceptances = privacyAcceptances.Select(pa => new PrivacyPolicyAcceptanceExportData
            {
                Id = pa.Id,
                PrivacyPolicyId = pa.PrivacyPolicyId,
                AcceptedVersion = pa.AcceptedVersion,
                AcceptedAtUtc = pa.AcceptedAtUtc,
                IsRevoked = pa.IsRevoked,
                RevokedAtUtc = pa.RevokedAtUtc
            }).ToList(),
            ExportedAtUtc = DateTime.UtcNow
        };

        return Result<UserDataExport>.Success(export);
    }

    /// <summary>
    /// Serializa os dados exportados para JSON.
    /// </summary>
    public string SerializeToJson(UserDataExport export)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return JsonSerializer.Serialize(export, options);
    }
}
