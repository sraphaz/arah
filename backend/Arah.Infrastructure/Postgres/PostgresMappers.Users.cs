using Arah.Domain.Email;
using Arah.Domain.Users;
using Arah.Infrastructure.Postgres.Entities;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
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

        // Atualizar LastUsedAtUtc e IsActive usando reflection
        var lastUsedProp = typeof(UserDevice).GetProperty("LastUsedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (lastUsedProp?.SetMethod != null && record.LastUsedAtUtc.HasValue)
        {
            lastUsedProp.SetValue(device, record.LastUsedAtUtc.Value);
        }

        if (!record.IsActive)
        {
            device.MarkAsInactive();
        }

        return device;
    }
}
