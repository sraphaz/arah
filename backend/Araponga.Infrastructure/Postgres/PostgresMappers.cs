using Araponga.Domain.Assets;
using Araponga.Domain.Events;
using Araponga.Domain.Feed;
using Araponga.Domain.Health;
using Araponga.Domain.Map;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Membership;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Postgres.Entities;

namespace Araponga.Infrastructure.Postgres;

public static class PostgresMappers
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
            CreatedAtUtc = territory.CreatedAtUtc
        };
    }

    public static Territory ToDomain(this TerritoryRecord record)
    {
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
            record.CreatedAtUtc);
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
            CreatedAtUtc = membership.CreatedAtUtc
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

    public static TerritoryStoreRecord ToRecord(this Store store)
    {
        return new TerritoryStoreRecord
        {
            Id = store.Id,
            TerritoryId = store.TerritoryId,
            OwnerUserId = store.OwnerUserId,
            DisplayName = store.DisplayName,
            Description = store.Description,
            Status = store.Status,
            PaymentsEnabled = store.PaymentsEnabled,
            ContactVisibility = store.ContactVisibility,
            Phone = store.Phone,
            Whatsapp = store.Whatsapp,
            Email = store.Email,
            Instagram = store.Instagram,
            Website = store.Website,
            PreferredContactMethod = store.PreferredContactMethod,
            CreatedAtUtc = store.CreatedAtUtc,
            UpdatedAtUtc = store.UpdatedAtUtc
        };
    }

    public static Store ToDomain(this TerritoryStoreRecord record)
    {
        return new Store(
            record.Id,
            record.TerritoryId,
            record.OwnerUserId,
            record.DisplayName,
            record.Description,
            record.Status,
            record.PaymentsEnabled,
            record.ContactVisibility,
            record.Phone,
            record.Whatsapp,
            record.Email,
            record.Instagram,
            record.Website,
            record.PreferredContactMethod,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static StoreItemRecord ToRecord(this StoreItem item)
    {
        return new StoreItemRecord
        {
            Id = item.Id,
            TerritoryId = item.TerritoryId,
            StoreId = item.StoreId,
            Type = item.Type,
            Title = item.Title,
            Description = item.Description,
            Category = item.Category,
            Tags = item.Tags,
            PricingType = item.PricingType,
            PriceAmount = item.PriceAmount,
            Currency = item.Currency,
            Unit = item.Unit,
            Latitude = item.Latitude,
            Longitude = item.Longitude,
            Status = item.Status,
            CreatedAtUtc = item.CreatedAtUtc,
            UpdatedAtUtc = item.UpdatedAtUtc
        };
    }

    public static StoreItem ToDomain(this StoreItemRecord record)
    {
        return new StoreItem(
            record.Id,
            record.TerritoryId,
            record.StoreId,
            record.Type,
            record.Title,
            record.Description,
            record.Category,
            record.Tags,
            record.PricingType,
            record.PriceAmount,
            record.Currency,
            record.Unit,
            record.Latitude,
            record.Longitude,
            record.Status,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static ItemInquiryRecord ToRecord(this ItemInquiry inquiry)
    {
        return new ItemInquiryRecord
        {
            Id = inquiry.Id,
            TerritoryId = inquiry.TerritoryId,
            ItemId = inquiry.ItemId,
            StoreId = inquiry.StoreId,
            FromUserId = inquiry.FromUserId,
            Message = inquiry.Message,
            Status = inquiry.Status,
            BatchId = inquiry.BatchId,
            CreatedAtUtc = inquiry.CreatedAtUtc
        };
    }

    public static ItemInquiry ToDomain(this ItemInquiryRecord record)
    {
        return new ItemInquiry(
            record.Id,
            record.TerritoryId,
            record.ItemId,
            record.StoreId,
            record.FromUserId,
            record.Message,
            record.Status,
            record.BatchId,
            record.CreatedAtUtc);
    }

    public static CartRecord ToRecord(this Cart cart)
    {
        return new CartRecord
        {
            Id = cart.Id,
            TerritoryId = cart.TerritoryId,
            UserId = cart.UserId,
            CreatedAtUtc = cart.CreatedAtUtc,
            UpdatedAtUtc = cart.UpdatedAtUtc
        };
    }

    public static Cart ToDomain(this CartRecord record)
    {
        return new Cart(
            record.Id,
            record.TerritoryId,
            record.UserId,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static CartItemRecord ToRecord(this CartItem item)
    {
        return new CartItemRecord
        {
            Id = item.Id,
            CartId = item.CartId,
            ItemId = item.ItemId,
            Quantity = item.Quantity,
            Notes = item.Notes,
            CreatedAtUtc = item.CreatedAtUtc,
            UpdatedAtUtc = item.UpdatedAtUtc
        };
    }

    public static CartItem ToDomain(this CartItemRecord record)
    {
        return new CartItem(
            record.Id,
            record.CartId,
            record.ItemId,
            record.Quantity,
            record.Notes,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static CheckoutRecord ToRecord(this Checkout checkout)
    {
        return new CheckoutRecord
        {
            Id = checkout.Id,
            TerritoryId = checkout.TerritoryId,
            BuyerUserId = checkout.BuyerUserId,
            StoreId = checkout.StoreId,
            Status = checkout.Status,
            Currency = checkout.Currency,
            ItemsSubtotalAmount = checkout.ItemsSubtotalAmount,
            PlatformFeeAmount = checkout.PlatformFeeAmount,
            TotalAmount = checkout.TotalAmount,
            CreatedAtUtc = checkout.CreatedAtUtc,
            UpdatedAtUtc = checkout.UpdatedAtUtc
        };
    }

    public static Checkout ToDomain(this CheckoutRecord record)
    {
        return new Checkout(
            record.Id,
            record.TerritoryId,
            record.BuyerUserId,
            record.StoreId,
            record.Status,
            record.Currency,
            record.ItemsSubtotalAmount,
            record.PlatformFeeAmount,
            record.TotalAmount,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static CheckoutItemRecord ToRecord(this CheckoutItem item)
    {
        return new CheckoutItemRecord
        {
            Id = item.Id,
            CheckoutId = item.CheckoutId,
            ItemId = item.ItemId,
            ItemType = item.ItemType,
            TitleSnapshot = item.TitleSnapshot,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineSubtotal = item.LineSubtotal,
            PlatformFeeLine = item.PlatformFeeLine,
            LineTotal = item.LineTotal,
            CreatedAtUtc = item.CreatedAtUtc
        };
    }

    public static CheckoutItem ToDomain(this CheckoutItemRecord record)
    {
        return new CheckoutItem(
            record.Id,
            record.CheckoutId,
            record.ItemId,
            record.ItemType,
            record.TitleSnapshot,
            record.Quantity,
            record.UnitPrice,
            record.LineSubtotal,
            record.PlatformFeeLine,
            record.LineTotal,
            record.CreatedAtUtc);
    }

    public static PlatformFeeConfigRecord ToRecord(this PlatformFeeConfig config)
    {
        return new PlatformFeeConfigRecord
        {
            Id = config.Id,
            TerritoryId = config.TerritoryId,
            ItemType = config.ItemType,
            FeeMode = config.FeeMode,
            FeeValue = config.FeeValue,
            Currency = config.Currency,
            IsActive = config.IsActive,
            CreatedAtUtc = config.CreatedAtUtc,
            UpdatedAtUtc = config.UpdatedAtUtc
        };
    }

    public static PlatformFeeConfig ToDomain(this PlatformFeeConfigRecord record)
    {
        return new PlatformFeeConfig(
            record.Id,
            record.TerritoryId,
            record.ItemType,
            record.FeeMode,
            record.FeeValue,
            record.Currency,
            record.IsActive,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
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

    public static CommunityPostRecord ToRecord(this CommunityPost post)
    {
        return new CommunityPostRecord
        {
            Id = post.Id,
            TerritoryId = post.TerritoryId,
            AuthorUserId = post.AuthorUserId,
            Title = post.Title,
            Content = post.Content,
            Type = post.Type,
            Visibility = post.Visibility,
            Status = post.Status,
            MapEntityId = post.MapEntityId,
            ReferenceType = post.ReferenceType,
            ReferenceId = post.ReferenceId,
            CreatedAtUtc = post.CreatedAtUtc
        };
    }

    public static CommunityPost ToDomain(this CommunityPostRecord record)
    {
        return new CommunityPost(
            record.Id,
            record.TerritoryId,
            record.AuthorUserId,
            record.Title,
            record.Content,
            record.Type,
            record.Visibility,
            record.Status,
            record.MapEntityId,
            record.CreatedAtUtc,
            record.ReferenceType,
            record.ReferenceId);
    }

    public static PostCommentRecord ToRecord(this PostComment comment)
    {
        return new PostCommentRecord
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAtUtc = comment.CreatedAtUtc
        };
    }

    public static TerritoryEventRecord ToRecord(this TerritoryEvent territoryEvent)
    {
        return new TerritoryEventRecord
        {
            Id = territoryEvent.Id,
            TerritoryId = territoryEvent.TerritoryId,
            Title = territoryEvent.Title,
            Description = territoryEvent.Description,
            StartsAtUtc = territoryEvent.StartsAtUtc,
            EndsAtUtc = territoryEvent.EndsAtUtc,
            Latitude = territoryEvent.Latitude,
            Longitude = territoryEvent.Longitude,
            LocationLabel = territoryEvent.LocationLabel,
            CreatedByUserId = territoryEvent.CreatedByUserId,
            CreatedByMembership = territoryEvent.CreatedByMembership,
            Status = territoryEvent.Status,
            CreatedAtUtc = territoryEvent.CreatedAtUtc,
            UpdatedAtUtc = territoryEvent.UpdatedAtUtc
        };
    }

    public static TerritoryEvent ToDomain(this TerritoryEventRecord record)
    {
        return new TerritoryEvent(
            record.Id,
            record.TerritoryId,
            record.Title,
            record.Description,
            record.StartsAtUtc,
            record.EndsAtUtc,
            record.Latitude,
            record.Longitude,
            record.LocationLabel,
            record.CreatedByUserId,
            record.CreatedByMembership,
            record.Status,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static EventParticipationRecord ToRecord(this EventParticipation participation)
    {
        return new EventParticipationRecord
        {
            EventId = participation.EventId,
            UserId = participation.UserId,
            Status = participation.Status,
            CreatedAtUtc = participation.CreatedAtUtc,
            UpdatedAtUtc = participation.UpdatedAtUtc
        };
    }

    public static PostComment ToDomain(this PostCommentRecord record)
    {
        return new PostComment(
            record.Id,
            record.PostId,
            record.UserId,
            record.Content,
            record.CreatedAtUtc);
    }

    public static MapEntityRecord ToRecord(this MapEntity entity)
    {
        return new MapEntityRecord
        {
            Id = entity.Id,
            TerritoryId = entity.TerritoryId,
            CreatedByUserId = entity.CreatedByUserId,
            Name = entity.Name,
            Category = entity.Category,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            Status = entity.Status,
            Visibility = entity.Visibility,
            ConfirmationCount = entity.ConfirmationCount,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }

    public static MapEntity ToDomain(this MapEntityRecord record)
    {
        return new MapEntity(
            record.Id,
            record.TerritoryId,
            record.CreatedByUserId,
            record.Name,
            record.Category,
            record.Latitude,
            record.Longitude,
            record.Status,
            record.Visibility,
            record.ConfirmationCount,
            record.CreatedAtUtc);
    }

    public static PostGeoAnchorRecord ToRecord(this PostGeoAnchor anchor)
    {
        return new PostGeoAnchorRecord
        {
            Id = anchor.Id,
            PostId = anchor.PostId,
            Latitude = anchor.Latitude,
            Longitude = anchor.Longitude,
            Type = anchor.Type,
            CreatedAtUtc = anchor.CreatedAtUtc
        };
    }

    public static PostGeoAnchor ToDomain(this PostGeoAnchorRecord record)
    {
        return new PostGeoAnchor(
            record.Id,
            record.PostId,
            record.Latitude,
            record.Longitude,
            record.Type,
            record.CreatedAtUtc);
    }

    public static HealthAlertRecord ToRecord(this HealthAlert alert)
    {
        return new HealthAlertRecord
        {
            Id = alert.Id,
            TerritoryId = alert.TerritoryId,
            ReporterUserId = alert.ReporterUserId,
            Title = alert.Title,
            Description = alert.Description,
            Status = alert.Status,
            CreatedAtUtc = alert.CreatedAtUtc
        };
    }

    public static HealthAlert ToDomain(this HealthAlertRecord record)
    {
        return new HealthAlert(
            record.Id,
            record.TerritoryId,
            record.ReporterUserId,
            record.Title,
            record.Description,
            record.Status,
            record.CreatedAtUtc);
    }

    public static TerritoryAssetRecord ToRecord(this TerritoryAsset asset)
    {
        return new TerritoryAssetRecord
        {
            Id = asset.Id,
            TerritoryId = asset.TerritoryId,
            Type = asset.Type,
            Name = asset.Name,
            Description = asset.Description,
            Status = asset.Status,
            CreatedByUserId = asset.CreatedByUserId,
            CreatedAtUtc = asset.CreatedAtUtc,
            UpdatedByUserId = asset.UpdatedByUserId,
            UpdatedAtUtc = asset.UpdatedAtUtc,
            ArchivedByUserId = asset.ArchivedByUserId,
            ArchivedAtUtc = asset.ArchivedAtUtc,
            ArchiveReason = asset.ArchiveReason
        };
    }

    public static TerritoryAsset ToDomain(this TerritoryAssetRecord record)
    {
        return new TerritoryAsset(
            record.Id,
            record.TerritoryId,
            record.Type,
            record.Name,
            record.Description,
            record.Status,
            record.CreatedByUserId,
            record.CreatedAtUtc,
            record.UpdatedByUserId,
            record.UpdatedAtUtc,
            record.ArchivedByUserId,
            record.ArchivedAtUtc,
            record.ArchiveReason);
    }

    public static AssetGeoAnchorRecord ToRecord(this AssetGeoAnchor anchor)
    {
        return new AssetGeoAnchorRecord
        {
            Id = anchor.Id,
            AssetId = anchor.AssetId,
            Latitude = anchor.Latitude,
            Longitude = anchor.Longitude,
            CreatedAtUtc = anchor.CreatedAtUtc
        };
    }

    public static AssetGeoAnchor ToDomain(this AssetGeoAnchorRecord record)
    {
        return new AssetGeoAnchor(
            record.Id,
            record.AssetId,
            record.Latitude,
            record.Longitude,
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

        return new UserPreferences(
            record.UserId,
            record.ProfileVisibility,
            record.ContactVisibility,
            record.ShareLocation,
            record.ShowMemberships,
            notificationPreferences,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
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
}
