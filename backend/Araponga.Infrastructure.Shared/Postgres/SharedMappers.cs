using System.Reflection;
using System.Text.Json;
using Araponga.Domain.Configuration;
using Araponga.Domain.Governance;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Shared.Postgres.Entities;

namespace Araponga.Infrastructure.Shared.Postgres;

/// <summary>
/// Mapeamento entre entidades (Records) de Shared e agregados de domínio.
/// Fonte da verdade para entidades está em Infrastructure.Shared.
/// </summary>
public static class SharedMappers
{
    public static TerritoryRecord ToRecord(this Territory territory)
    {
        return new TerritoryRecord
        {
            Id = territory.Id,
            ParentTerritoryId = territory.ParentTerritoryId,
            Name = territory.Name,
            Description = territory.Description,
            Status = territory.Status,
            City = territory.City,
            State = territory.State,
            Latitude = territory.Latitude,
            Longitude = territory.Longitude,
            CreatedAtUtc = territory.CreatedAtUtc,
            RadiusKm = territory.RadiusKm,
            BoundaryPolygonJson = territory.BoundaryPolygon is null or { Count: 0 }
                ? null
                : JsonSerializer.Serialize(territory.BoundaryPolygon),
        };
    }

    public static Territory ToDomain(this TerritoryRecord record)
    {
        IReadOnlyList<TerritoryBoundaryPoint>? boundaryPolygon = null;
        if (!string.IsNullOrWhiteSpace(record.BoundaryPolygonJson))
        {
            try
            {
                var list = JsonSerializer.Deserialize<List<TerritoryBoundaryPoint>>(record.BoundaryPolygonJson);
                if (list is { Count: >= 3 })
                    boundaryPolygon = list;
            }
            catch
            {
                // Ignore invalid JSON
            }
        }

        return new Territory(
            record.Id,
            record.ParentTerritoryId,
            record.Name,
            record.Description,
            record.Status,
            record.City,
            record.State,
            record.Latitude,
            record.Longitude,
            record.CreatedAtUtc,
            record.RadiusKm,
            boundaryPolygon);
    }

    public static UserRecord ToRecord(this User user)
    {
        return new UserRecord
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Cpf = user.Cpf,
            ForeignDocument = user.ForeignDocument,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            AuthProvider = user.AuthProvider,
            ExternalId = user.ExternalId,
            TwoFactorEnabled = user.TwoFactorEnabled,
            TwoFactorSecret = user.TwoFactorSecret,
            TwoFactorRecoveryCodesHash = user.TwoFactorRecoveryCodesHash,
            TwoFactorVerifiedAtUtc = user.TwoFactorVerifiedAtUtc,
            IdentityVerificationStatus = user.IdentityVerificationStatus,
            IdentityVerifiedAtUtc = user.IdentityVerifiedAtUtc,
            AvatarMediaAssetId = user.AvatarMediaAssetId,
            Bio = user.Bio,
            PasswordHash = user.PasswordHash,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }

    public static User ToDomain(this UserRecord record)
    {
        return new User(
            record.Id,
            record.DisplayName,
            record.Email,
            record.Cpf,
            record.ForeignDocument,
            record.PhoneNumber,
            record.Address,
            record.AuthProvider,
            record.ExternalId,
            record.TwoFactorEnabled,
            record.TwoFactorSecret,
            record.TwoFactorRecoveryCodesHash,
            record.TwoFactorVerifiedAtUtc,
            record.IdentityVerificationStatus,
            record.IdentityVerifiedAtUtc,
            record.AvatarMediaAssetId,
            record.Bio,
            record.PasswordHash,
            record.CreatedAtUtc);
    }

    public static TerritoryMembershipRecord ToRecord(this TerritoryMembership membership)
    {
        return new TerritoryMembershipRecord
        {
            Id = membership.Id,
            UserId = membership.UserId,
            TerritoryId = membership.TerritoryId,
            Role = membership.Role,
            ResidencyVerification = membership.ResidencyVerification,
            LastGeoVerifiedAtUtc = membership.LastGeoVerifiedAtUtc,
            LastDocumentVerifiedAtUtc = membership.LastDocumentVerifiedAtUtc,
            CreatedAtUtc = membership.CreatedAtUtc,
            // Garante valor não nulo no INSERT; coluna NOT NULL e IsRowVersion() pode fazer o EF omitir no insert.
            RowVersion = new byte[8]
        };
    }

    public static TerritoryMembership ToDomain(this TerritoryMembershipRecord record)
    {
        return new TerritoryMembership(
            record.Id,
            record.UserId,
            record.TerritoryId,
            record.Role,
            record.ResidencyVerification,
            record.LastGeoVerifiedAtUtc,
            record.LastDocumentVerifiedAtUtc,
            record.CreatedAtUtc);
    }

    public static TerritoryJoinRequestRecord ToRecord(this TerritoryJoinRequest request)
    {
        return new TerritoryJoinRequestRecord
        {
            Id = request.Id,
            TerritoryId = request.TerritoryId,
            RequesterUserId = request.RequesterUserId,
            Message = request.Message,
            Status = request.Status,
            CreatedAtUtc = request.CreatedAtUtc,
            ExpiresAtUtc = request.ExpiresAtUtc,
            DecidedAtUtc = request.DecidedAtUtc,
            DecidedByUserId = request.DecidedByUserId
        };
    }

    public static TerritoryJoinRequest ToDomain(this TerritoryJoinRequestRecord record)
    {
        return new TerritoryJoinRequest(
            record.Id,
            record.TerritoryId,
            record.RequesterUserId,
            record.Message,
            record.Status,
            record.CreatedAtUtc,
            record.ExpiresAtUtc,
            record.DecidedAtUtc,
            record.DecidedByUserId);
    }

    public static TerritoryJoinRequestRecipientRecord ToRecord(this TerritoryJoinRequestRecipient recipient)
    {
        return new TerritoryJoinRequestRecipientRecord
        {
            JoinRequestId = recipient.JoinRequestId,
            RecipientUserId = recipient.RecipientUserId,
            CreatedAtUtc = recipient.CreatedAtUtc,
            RespondedAtUtc = recipient.RespondedAtUtc
        };
    }

    public static TerritoryJoinRequestRecipient ToDomain(this TerritoryJoinRequestRecipientRecord record)
    {
        return new TerritoryJoinRequestRecipient(
            record.JoinRequestId,
            record.RecipientUserId,
            record.CreatedAtUtc,
            record.RespondedAtUtc);
    }

    public static UserPreferencesRecord ToRecord(this UserPreferences preferences)
    {
        return new UserPreferencesRecord
        {
            UserId = preferences.UserId,
            ProfileVisibility = preferences.ProfileVisibility,
            ContactVisibility = preferences.ContactVisibility,
            ShareLocation = preferences.ShareLocation,
            ShowMemberships = preferences.ShowMemberships,
            NotificationsPostsEnabled = preferences.NotificationPreferences.PostsEnabled,
            NotificationsCommentsEnabled = preferences.NotificationPreferences.CommentsEnabled,
            NotificationsEventsEnabled = preferences.NotificationPreferences.EventsEnabled,
            NotificationsAlertsEnabled = preferences.NotificationPreferences.AlertsEnabled,
            NotificationsMarketplaceEnabled = preferences.NotificationPreferences.MarketplaceEnabled,
            NotificationsModerationEnabled = preferences.NotificationPreferences.ModerationEnabled,
            NotificationsMembershipRequestsEnabled = preferences.NotificationPreferences.MembershipRequestsEnabled,
            EmailReceiveEmails = preferences.EmailPreferences.ReceiveEmails,
            EmailFrequency = (int)preferences.EmailPreferences.EmailFrequency,
            EmailTypes = (int)preferences.EmailPreferences.EmailTypes,
            CreatedAtUtc = preferences.CreatedAtUtc,
            UpdatedAtUtc = preferences.UpdatedAtUtc
        };
    }

    public static UserPreferences ToDomain(this UserPreferencesRecord record)
    {
        var notificationPreferences = new NotificationPreferences(
            record.NotificationsPostsEnabled,
            record.NotificationsCommentsEnabled,
            record.NotificationsEventsEnabled,
            record.NotificationsAlertsEnabled,
            record.NotificationsMarketplaceEnabled,
            record.NotificationsModerationEnabled,
            record.NotificationsMembershipRequestsEnabled);
        var emailPreferences = new EmailPreferences(
            record.EmailReceiveEmails,
            (EmailFrequency)record.EmailFrequency,
            (EmailTypes)record.EmailTypes);
        return new UserPreferences(
            record.UserId,
            record.ProfileVisibility,
            record.ContactVisibility,
            record.ShareLocation,
            record.ShowMemberships,
            notificationPreferences,
            emailPreferences,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static UserInterestRecord ToRecord(this UserInterest interest)
    {
        return new UserInterestRecord
        {
            Id = interest.Id,
            UserId = interest.UserId,
            InterestTag = interest.InterestTag,
            CreatedAtUtc = interest.CreatedAtUtc
        };
    }

    public static UserInterest ToDomain(this UserInterestRecord record)
    {
        return new UserInterest(record.Id, record.UserId, record.InterestTag, record.CreatedAtUtc);
    }

    public static TerritoryCharacterizationRecord ToRecord(this TerritoryCharacterization characterization)
    {
        return new TerritoryCharacterizationRecord
        {
            TerritoryId = characterization.TerritoryId,
            TagsJson = JsonSerializer.Serialize(characterization.Tags),
            UpdatedAtUtc = characterization.UpdatedAtUtc
        };
    }

    public static TerritoryCharacterization ToDomain(this TerritoryCharacterizationRecord record)
    {
        var tags = JsonSerializer.Deserialize<List<string>>(record.TagsJson) ?? new List<string>();
        return new TerritoryCharacterization(record.TerritoryId, tags, record.UpdatedAtUtc);
    }

    public static MembershipSettingsRecord ToRecord(this MembershipSettings settings)
    {
        return new MembershipSettingsRecord
        {
            MembershipId = settings.MembershipId,
            MarketplaceOptIn = settings.MarketplaceOptIn,
            CreatedAtUtc = settings.CreatedAtUtc,
            UpdatedAtUtc = settings.UpdatedAtUtc
        };
    }

    public static MembershipSettings ToDomain(this MembershipSettingsRecord record)
    {
        return new MembershipSettings(
            record.MembershipId,
            record.MarketplaceOptIn,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static MembershipCapabilityRecord ToRecord(this MembershipCapability capability)
    {
        return new MembershipCapabilityRecord
        {
            Id = capability.Id,
            MembershipId = capability.MembershipId,
            CapabilityType = capability.CapabilityType,
            GrantedAtUtc = capability.GrantedAtUtc,
            RevokedAtUtc = capability.RevokedAtUtc,
            GrantedByUserId = capability.GrantedByUserId,
            GrantedByMembershipId = capability.GrantedByMembershipId,
            Reason = capability.Reason
        };
    }

    public static MembershipCapability ToDomain(this MembershipCapabilityRecord record)
    {
        return new MembershipCapability(
            record.Id,
            record.MembershipId,
            record.CapabilityType,
            record.GrantedAtUtc,
            record.GrantedByUserId,
            record.GrantedByMembershipId,
            record.Reason);
    }

    public static VotingRecord ToRecord(this Voting voting)
    {
        return new VotingRecord
        {
            Id = voting.Id,
            TerritoryId = voting.TerritoryId,
            CreatedByUserId = voting.CreatedByUserId,
            Type = voting.Type,
            Title = voting.Title,
            Description = voting.Description,
            OptionsJson = JsonSerializer.Serialize(voting.Options),
            Visibility = voting.Visibility,
            Status = voting.Status,
            StartsAtUtc = voting.StartsAtUtc,
            EndsAtUtc = voting.EndsAtUtc,
            CreatedAtUtc = voting.CreatedAtUtc,
            UpdatedAtUtc = voting.UpdatedAtUtc
        };
    }

    public static Voting ToDomain(this VotingRecord record)
    {
        var options = JsonSerializer.Deserialize<List<string>>(record.OptionsJson) ?? new List<string>();
        return new Voting(
            record.Id,
            record.TerritoryId,
            record.CreatedByUserId,
            record.Type,
            record.Title,
            record.Description,
            options,
            record.Visibility,
            record.Status,
            record.StartsAtUtc,
            record.EndsAtUtc,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static VoteRecord ToRecord(this Vote vote)
    {
        return new VoteRecord
        {
            Id = vote.Id,
            VotingId = vote.VotingId,
            UserId = vote.UserId,
            SelectedOption = vote.SelectedOption,
            CreatedAtUtc = vote.CreatedAtUtc
        };
    }

    public static Vote ToDomain(this VoteRecord record)
    {
        return new Vote(
            record.Id,
            record.VotingId,
            record.UserId,
            record.SelectedOption,
            record.CreatedAtUtc);
    }

    public static SystemPermissionRecord ToRecord(this SystemPermission permission)
    {
        return new SystemPermissionRecord
        {
            Id = permission.Id,
            UserId = permission.UserId,
            PermissionType = permission.PermissionType,
            GrantedAtUtc = permission.GrantedAtUtc,
            GrantedByUserId = permission.GrantedByUserId,
            RevokedAtUtc = permission.RevokedAtUtc,
            RevokedByUserId = permission.RevokedByUserId
        };
    }

    public static SystemPermission ToDomain(this SystemPermissionRecord record)
    {
        return new SystemPermission(
            record.Id,
            record.UserId,
            record.PermissionType,
            record.GrantedAtUtc,
            record.GrantedByUserId,
            record.RevokedAtUtc,
            record.RevokedByUserId);
    }

    public static SystemConfigRecord ToRecord(this SystemConfig config)
    {
        return new SystemConfigRecord
        {
            Id = config.Id,
            Key = config.Key,
            Value = config.Value,
            Category = config.Category,
            Description = config.Description,
            CreatedAtUtc = config.CreatedAtUtc,
            CreatedByUserId = config.CreatedByUserId,
            UpdatedAtUtc = config.UpdatedAtUtc,
            UpdatedByUserId = config.UpdatedByUserId
        };
    }

    public static SystemConfig ToDomain(this SystemConfigRecord record)
    {
        return new SystemConfig(
            record.Id,
            record.Key,
            record.Value,
            record.Category,
            record.Description,
            record.CreatedAtUtc,
            record.CreatedByUserId,
            record.UpdatedAtUtc,
            record.UpdatedByUserId);
    }

    public static TermsOfServiceRecord ToRecord(this TermsOfService terms)
    {
        return new TermsOfServiceRecord
        {
            Id = terms.Id,
            Version = terms.Version,
            Title = terms.Title,
            Content = terms.Content,
            EffectiveDate = terms.EffectiveDate,
            ExpirationDate = terms.ExpirationDate,
            IsActive = terms.IsActive,
            RequiredRoles = terms.RequiredRoles,
            RequiredCapabilities = terms.RequiredCapabilities,
            RequiredSystemPermissions = terms.RequiredSystemPermissions,
            CreatedAtUtc = terms.CreatedAtUtc,
            UpdatedAtUtc = terms.UpdatedAtUtc
        };
    }

    public static TermsOfService ToDomain(this TermsOfServiceRecord record)
    {
        var terms = new TermsOfService(
            record.Id,
            record.Version,
            record.Title,
            record.Content,
            record.EffectiveDate,
            record.ExpirationDate,
            record.IsActive,
            record.RequiredRoles,
            record.RequiredCapabilities,
            record.RequiredSystemPermissions,
            record.CreatedAtUtc);
        SetProperty(terms, "UpdatedAtUtc", record.UpdatedAtUtc);
        return terms;
    }

    public static TermsAcceptanceRecord ToRecord(this TermsAcceptance acceptance)
    {
        return new TermsAcceptanceRecord
        {
            Id = acceptance.Id,
            UserId = acceptance.UserId,
            TermsOfServiceId = acceptance.TermsOfServiceId,
            AcceptedAtUtc = acceptance.AcceptedAtUtc,
            IpAddress = acceptance.IpAddress,
            UserAgent = acceptance.UserAgent,
            AcceptedVersion = acceptance.AcceptedVersion,
            IsRevoked = acceptance.IsRevoked,
            RevokedAtUtc = acceptance.RevokedAtUtc
        };
    }

    public static TermsAcceptance ToDomain(this TermsAcceptanceRecord record)
    {
        var acceptance = new TermsAcceptance(
            record.Id,
            record.UserId,
            record.TermsOfServiceId,
            record.AcceptedAtUtc,
            record.AcceptedVersion,
            record.IpAddress,
            record.UserAgent);
        if (record.IsRevoked)
        {
            SetProperty(acceptance, "IsRevoked", true);
            if (record.RevokedAtUtc.HasValue)
                SetProperty(acceptance, "RevokedAtUtc", record.RevokedAtUtc.Value);
        }
        return acceptance;
    }

    public static PrivacyPolicyRecord ToRecord(this PrivacyPolicy policy)
    {
        return new PrivacyPolicyRecord
        {
            Id = policy.Id,
            Version = policy.Version,
            Title = policy.Title,
            Content = policy.Content,
            EffectiveDate = policy.EffectiveDate,
            ExpirationDate = policy.ExpirationDate,
            IsActive = policy.IsActive,
            RequiredRoles = policy.RequiredRoles,
            RequiredCapabilities = policy.RequiredCapabilities,
            RequiredSystemPermissions = policy.RequiredSystemPermissions,
            CreatedAtUtc = policy.CreatedAtUtc,
            UpdatedAtUtc = policy.UpdatedAtUtc
        };
    }

    public static PrivacyPolicy ToDomain(this PrivacyPolicyRecord record)
    {
        var policy = new PrivacyPolicy(
            record.Id,
            record.Version,
            record.Title,
            record.Content,
            record.EffectiveDate,
            record.ExpirationDate,
            record.IsActive,
            record.RequiredRoles,
            record.RequiredCapabilities,
            record.RequiredSystemPermissions,
            record.CreatedAtUtc);
        SetProperty(policy, "UpdatedAtUtc", record.UpdatedAtUtc);
        return policy;
    }

    public static PrivacyPolicyAcceptanceRecord ToRecord(this PrivacyPolicyAcceptance acceptance)
    {
        return new PrivacyPolicyAcceptanceRecord
        {
            Id = acceptance.Id,
            UserId = acceptance.UserId,
            PrivacyPolicyId = acceptance.PrivacyPolicyId,
            AcceptedAtUtc = acceptance.AcceptedAtUtc,
            IpAddress = acceptance.IpAddress,
            UserAgent = acceptance.UserAgent,
            AcceptedVersion = acceptance.AcceptedVersion,
            IsRevoked = acceptance.IsRevoked,
            RevokedAtUtc = acceptance.RevokedAtUtc
        };
    }

    public static PrivacyPolicyAcceptance ToDomain(this PrivacyPolicyAcceptanceRecord record)
    {
        var acceptance = new PrivacyPolicyAcceptance(
            record.Id,
            record.UserId,
            record.PrivacyPolicyId,
            record.AcceptedAtUtc,
            record.AcceptedVersion,
            record.IpAddress,
            record.UserAgent);
        if (record.IsRevoked)
        {
            SetProperty(acceptance, "IsRevoked", true);
            if (record.RevokedAtUtc.HasValue)
                SetProperty(acceptance, "RevokedAtUtc", record.RevokedAtUtc.Value);
        }
        return acceptance;
    }

    public static UserDeviceRecord ToRecord(this UserDevice device)
    {
        return new UserDeviceRecord
        {
            Id = device.Id,
            UserId = device.UserId,
            DeviceToken = device.DeviceToken,
            Platform = device.Platform,
            DeviceName = device.DeviceName,
            RegisteredAtUtc = device.RegisteredAtUtc,
            LastUsedAtUtc = device.LastUsedAtUtc,
            IsActive = device.IsActive
        };
    }

    public static UserDevice ToDomain(this UserDeviceRecord record)
    {
        var device = new UserDevice(
            record.Id,
            record.UserId,
            record.DeviceToken,
            record.Platform,
            record.DeviceName,
            record.RegisteredAtUtc);
        if (record.LastUsedAtUtc.HasValue)
            SetProperty(device, "LastUsedAtUtc", record.LastUsedAtUtc.Value);
        if (!record.IsActive)
            device.MarkAsInactive();
        return device;
    }

    private static void SetProperty<T>(T target, string propertyName, object value)
    {
        var prop = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (prop?.SetMethod != null)
            prop.SetValue(target, value);
    }
}
