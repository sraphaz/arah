using Araponga.Domain.Assets;
using Araponga.Domain.Chat;
using Araponga.Domain.Events;
using Araponga.Domain.Feed;
using Araponga.Domain.Financial;
using Araponga.Domain.Health;
using Araponga.Domain.Map;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Media;
using Araponga.Domain.Membership;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Postgres.Entities;
using System.Text.Json;

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
            CreatedAtUtc = post.CreatedAtUtc,
            EditedAtUtc = post.EditedAtUtc,
            EditCount = post.EditCount
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
            record.ReferenceId,
            record.EditedAtUtc,
            record.EditCount > int.MaxValue ? int.MaxValue : record.EditCount);
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
        const int maxInt32 = int.MaxValue;
        var confirmationCount = record.ConfirmationCount > maxInt32 ? maxInt32 : record.ConfirmationCount;
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
            confirmationCount,
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

    public static SystemConfigRecord ToRecord(this Araponga.Domain.Configuration.SystemConfig config)
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

    public static Araponga.Domain.Configuration.SystemConfig ToDomain(this SystemConfigRecord record)
    {
        return new Araponga.Domain.Configuration.SystemConfig(
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

    public static WorkItemRecord ToRecord(this Araponga.Domain.Work.WorkItem item)
    {
        return new WorkItemRecord
        {
            Id = item.Id,
            Type = item.Type,
            Status = item.Status,
            TerritoryId = item.TerritoryId,
            CreatedByUserId = item.CreatedByUserId,
            CreatedAtUtc = item.CreatedAtUtc,
            RequiredSystemPermission = item.RequiredSystemPermission,
            RequiredCapability = item.RequiredCapability,
            SubjectType = item.SubjectType,
            SubjectId = item.SubjectId,
            PayloadJson = item.PayloadJson,
            Outcome = item.Outcome,
            CompletedAtUtc = item.CompletedAtUtc,
            CompletedByUserId = item.CompletedByUserId,
            CompletionNotes = item.CompletionNotes
        };
    }

    public static Araponga.Domain.Work.WorkItem ToDomain(this WorkItemRecord record)
    {
        return new Araponga.Domain.Work.WorkItem(
            record.Id,
            record.Type,
            record.Status,
            record.TerritoryId,
            record.CreatedByUserId,
            record.CreatedAtUtc,
            record.RequiredSystemPermission,
            record.RequiredCapability,
            record.SubjectType,
            record.SubjectId,
            record.PayloadJson,
            record.Outcome,
            record.CompletedAtUtc,
            record.CompletedByUserId,
            record.CompletionNotes);
    }

    public static DocumentEvidenceRecord ToRecord(this Araponga.Domain.Evidence.DocumentEvidence evidence)
    {
        return new DocumentEvidenceRecord
        {
            Id = evidence.Id,
            UserId = evidence.UserId,
            TerritoryId = evidence.TerritoryId,
            Kind = evidence.Kind,
            StorageProvider = evidence.StorageProvider,
            StorageKey = evidence.StorageKey,
            ContentType = evidence.ContentType,
            SizeBytes = evidence.SizeBytes,
            Sha256 = evidence.Sha256,
            OriginalFileName = evidence.OriginalFileName,
            CreatedAtUtc = evidence.CreatedAtUtc
        };
    }

    public static Araponga.Domain.Evidence.DocumentEvidence ToDomain(this DocumentEvidenceRecord record)
    {
        return new Araponga.Domain.Evidence.DocumentEvidence(
            record.Id,
            record.UserId,
            record.TerritoryId,
            record.Kind,
            record.StorageProvider,
            record.StorageKey,
            record.ContentType,
            record.SizeBytes,
            record.Sha256,
            record.OriginalFileName,
            record.CreatedAtUtc);
    }

    // -----------------------
    // Chat
    // -----------------------
    public static ChatConversationRecord ToRecord(this ChatConversation conversation)
    {
        return new ChatConversationRecord
        {
            Id = conversation.Id,
            TerritoryId = conversation.TerritoryId,
            Kind = conversation.Kind,
            Status = conversation.Status,
            Name = conversation.Name,
            CreatedByUserId = conversation.CreatedByUserId,
            CreatedAtUtc = conversation.CreatedAtUtc,
            ApprovedByUserId = conversation.ApprovedByUserId,
            ApprovedAtUtc = conversation.ApprovedAtUtc,
            LockedByUserId = conversation.LockedByUserId,
            LockedAtUtc = conversation.LockedAtUtc,
            DisabledByUserId = conversation.DisabledByUserId,
            DisabledAtUtc = conversation.DisabledAtUtc
        };
    }

    public static ChatConversation ToDomain(this ChatConversationRecord record)
    {
        return new ChatConversation(
            record.Id,
            record.Kind,
            record.Status,
            record.TerritoryId,
            record.Name,
            record.CreatedByUserId,
            record.CreatedAtUtc,
            record.ApprovedByUserId,
            record.ApprovedAtUtc,
            record.LockedAtUtc,
            record.LockedByUserId,
            record.DisabledAtUtc,
            record.DisabledByUserId);
    }

    public static ChatConversationParticipantRecord ToRecord(this ConversationParticipant participant)
    {
        return new ChatConversationParticipantRecord
        {
            ConversationId = participant.ConversationId,
            UserId = participant.UserId,
            Role = participant.Role,
            JoinedAtUtc = participant.JoinedAtUtc,
            LeftAtUtc = participant.LeftAtUtc,
            MutedUntilUtc = participant.MutedUntilUtc,
            LastReadMessageId = participant.LastReadMessageId,
            LastReadAtUtc = participant.LastReadAtUtc
        };
    }

    public static ConversationParticipant ToDomain(this ChatConversationParticipantRecord record)
    {
        return new ConversationParticipant(
            record.ConversationId,
            record.UserId,
            record.Role,
            record.JoinedAtUtc,
            record.LeftAtUtc,
            record.MutedUntilUtc,
            record.LastReadMessageId,
            record.LastReadAtUtc);
    }

    public static ChatMessageRecord ToRecord(this ChatMessage message)
    {
        return new ChatMessageRecord
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderUserId = message.SenderUserId,
            ContentType = message.ContentType,
            Text = message.Text,
            PayloadJson = message.PayloadJson,
            CreatedAtUtc = message.CreatedAtUtc,
            EditedAtUtc = message.EditedAtUtc,
            DeletedAtUtc = message.DeletedAtUtc,
            DeletedByUserId = message.DeletedByUserId
        };
    }

    public static ChatMessage ToDomain(this ChatMessageRecord record)
    {
        return new ChatMessage(
            record.Id,
            record.ConversationId,
            record.SenderUserId,
            record.ContentType,
            record.Text,
            record.PayloadJson,
            record.CreatedAtUtc,
            record.EditedAtUtc,
            record.DeletedAtUtc,
            record.DeletedByUserId);
    }

    // -----------------------
    // Financial
    // -----------------------
    public static FinancialTransactionRecord ToRecord(this FinancialTransaction transaction)
    {
        return new FinancialTransactionRecord
        {
            Id = transaction.Id,
            TerritoryId = transaction.TerritoryId,
            Type = (int)transaction.Type,
            Status = (int)transaction.Status,
            AmountInCents = transaction.AmountInCents,
            Currency = transaction.Currency,
            Description = transaction.Description,
            RelatedEntityId = transaction.RelatedEntityId,
            RelatedEntityType = transaction.RelatedEntityType,
            RelatedTransactionIdsJson = JsonSerializer.Serialize(transaction.RelatedTransactionIds),
            MetadataJson = transaction.Metadata != null ? JsonSerializer.Serialize(transaction.Metadata) : null,
            CreatedAtUtc = transaction.CreatedAtUtc,
            UpdatedAtUtc = transaction.UpdatedAtUtc
        };
    }

    public static FinancialTransaction ToDomain(this FinancialTransactionRecord record)
    {
        var transaction = new FinancialTransaction(
            record.Id,
            record.TerritoryId,
            (TransactionType)record.Type,
            record.AmountInCents,
            record.Currency,
            record.Description,
            record.RelatedEntityId,
            record.RelatedEntityType,
            record.MetadataJson != null ? JsonSerializer.Deserialize<Dictionary<string, string>>(record.MetadataJson) : null);

        // Set status and related transaction IDs using reflection or public setters
        // Since we can't modify private setters, we'll need to use reflection or add a method
        var relatedIds = JsonSerializer.Deserialize<List<Guid>>(record.RelatedTransactionIdsJson) ?? new List<Guid>();
        foreach (var id in relatedIds)
        {
            transaction.AddRelatedTransaction(id);
        }

        // Restore status and timestamps using reflection
        var statusProp = typeof(FinancialTransaction).GetProperty("Status", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(FinancialTransaction).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(FinancialTransaction).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (statusProp?.SetMethod != null) statusProp.SetValue(transaction, (TransactionStatus)record.Status);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(transaction, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(transaction, record.UpdatedAtUtc);

        return transaction;
    }

    public static TransactionStatusHistoryRecord ToRecord(this TransactionStatusHistory history)
    {
        return new TransactionStatusHistoryRecord
        {
            Id = history.Id,
            FinancialTransactionId = history.FinancialTransactionId,
            PreviousStatus = (int)history.PreviousStatus,
            NewStatus = (int)history.NewStatus,
            ChangedByUserId = history.ChangedByUserId,
            Reason = history.Reason,
            ChangedAtUtc = history.ChangedAtUtc
        };
    }

    public static TransactionStatusHistory ToDomain(this TransactionStatusHistoryRecord record)
    {
        return new TransactionStatusHistory(
            record.Id,
            record.FinancialTransactionId,
            (TransactionStatus)record.PreviousStatus,
            (TransactionStatus)record.NewStatus,
            record.ChangedByUserId,
            record.Reason);
    }

    public static SellerBalanceRecord ToRecord(this SellerBalance balance)
    {
        return new SellerBalanceRecord
        {
            Id = balance.Id,
            TerritoryId = balance.TerritoryId,
            SellerUserId = balance.SellerUserId,
            PendingAmountInCents = balance.PendingAmountInCents,
            ReadyForPayoutAmountInCents = balance.ReadyForPayoutAmountInCents,
            PaidAmountInCents = balance.PaidAmountInCents,
            Currency = balance.Currency,
            CreatedAtUtc = balance.CreatedAtUtc,
            UpdatedAtUtc = balance.UpdatedAtUtc
        };
    }

    public static SellerBalance ToDomain(this SellerBalanceRecord record)
    {
        var balance = new SellerBalance(
            record.Id,
            record.TerritoryId,
            record.SellerUserId,
            record.Currency);

        // Restore state using reflection
        var pendingProp = typeof(SellerBalance).GetProperty("PendingAmountInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var readyProp = typeof(SellerBalance).GetProperty("ReadyForPayoutAmountInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var paidProp = typeof(SellerBalance).GetProperty("PaidAmountInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(SellerBalance).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(SellerBalance).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (pendingProp?.SetMethod != null) pendingProp.SetValue(balance, record.PendingAmountInCents);
        if (readyProp?.SetMethod != null) readyProp.SetValue(balance, record.ReadyForPayoutAmountInCents);
        if (paidProp?.SetMethod != null) paidProp.SetValue(balance, record.PaidAmountInCents);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(balance, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(balance, record.UpdatedAtUtc);

        return balance;
    }

    public static SellerTransactionRecord ToRecord(this SellerTransaction transaction)
    {
        return new SellerTransactionRecord
        {
            Id = transaction.Id,
            TerritoryId = transaction.TerritoryId,
            StoreId = transaction.StoreId,
            CheckoutId = transaction.CheckoutId,
            SellerUserId = transaction.SellerUserId,
            GrossAmountInCents = transaction.GrossAmountInCents,
            PlatformFeeInCents = transaction.PlatformFeeInCents,
            NetAmountInCents = transaction.NetAmountInCents,
            Currency = transaction.Currency,
            Status = (int)transaction.Status,
            PayoutId = transaction.PayoutId,
            ReadyForPayoutAtUtc = transaction.ReadyForPayoutAtUtc,
            PaidAtUtc = transaction.PaidAtUtc,
            FinancialTransactionId = transaction.FinancialTransactionId,
            CreatedAtUtc = transaction.CreatedAtUtc,
            UpdatedAtUtc = transaction.UpdatedAtUtc
        };
    }

    public static SellerTransaction ToDomain(this SellerTransactionRecord record)
    {
        var transaction = new SellerTransaction(
            record.Id,
            record.TerritoryId,
            record.StoreId,
            record.CheckoutId,
            record.SellerUserId,
            record.GrossAmountInCents,
            record.PlatformFeeInCents,
            record.Currency);

        // Restore state using reflection
        var statusProp = typeof(SellerTransaction).GetProperty("Status", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var payoutIdProp = typeof(SellerTransaction).GetProperty("PayoutId", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var readyForPayoutAtProp = typeof(SellerTransaction).GetProperty("ReadyForPayoutAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var paidAtProp = typeof(SellerTransaction).GetProperty("PaidAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var financialTransactionIdProp = typeof(SellerTransaction).GetProperty("FinancialTransactionId", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(SellerTransaction).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(SellerTransaction).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (statusProp?.SetMethod != null) statusProp.SetValue(transaction, (SellerTransactionStatus)record.Status);
        if (payoutIdProp?.SetMethod != null) payoutIdProp.SetValue(transaction, record.PayoutId);
        if (readyForPayoutAtProp?.SetMethod != null) readyForPayoutAtProp.SetValue(transaction, record.ReadyForPayoutAtUtc);
        if (paidAtProp?.SetMethod != null) paidAtProp.SetValue(transaction, record.PaidAtUtc);
        if (financialTransactionIdProp?.SetMethod != null) financialTransactionIdProp.SetValue(transaction, record.FinancialTransactionId);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(transaction, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(transaction, record.UpdatedAtUtc);

        return transaction;
    }

    public static PlatformFinancialBalanceRecord ToRecord(this PlatformFinancialBalance balance)
    {
        return new PlatformFinancialBalanceRecord
        {
            Id = balance.Id,
            TerritoryId = balance.TerritoryId,
            TotalRevenueInCents = balance.TotalRevenueInCents,
            TotalExpensesInCents = balance.TotalExpensesInCents,
            NetBalanceInCents = balance.NetBalanceInCents,
            Currency = balance.Currency,
            CreatedAtUtc = balance.CreatedAtUtc,
            UpdatedAtUtc = balance.UpdatedAtUtc
        };
    }

    public static PlatformFinancialBalance ToDomain(this PlatformFinancialBalanceRecord record)
    {
        var balance = new PlatformFinancialBalance(
            record.Id,
            record.TerritoryId,
            record.Currency);

        // Restore state using reflection
        var revenueProp = typeof(PlatformFinancialBalance).GetProperty("TotalRevenueInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var expenseProp = typeof(PlatformFinancialBalance).GetProperty("TotalExpensesInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var netBalanceProp = typeof(PlatformFinancialBalance).GetProperty("NetBalanceInCents", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(PlatformFinancialBalance).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(PlatformFinancialBalance).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (revenueProp?.SetMethod != null) revenueProp.SetValue(balance, record.TotalRevenueInCents);
        if (expenseProp?.SetMethod != null) expenseProp.SetValue(balance, record.TotalExpensesInCents);
        if (netBalanceProp?.SetMethod != null) netBalanceProp.SetValue(balance, record.NetBalanceInCents);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(balance, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(balance, record.UpdatedAtUtc);

        return balance;
    }

    public static PlatformRevenueTransactionRecord ToRecord(this PlatformRevenueTransaction transaction)
    {
        return new PlatformRevenueTransactionRecord
        {
            Id = transaction.Id,
            TerritoryId = transaction.TerritoryId,
            CheckoutId = transaction.CheckoutId,
            FeeAmountInCents = transaction.FeeAmountInCents,
            Currency = transaction.Currency,
            FinancialTransactionId = transaction.FinancialTransactionId,
            CreatedAtUtc = transaction.CreatedAtUtc
        };
    }

    public static PlatformRevenueTransaction ToDomain(this PlatformRevenueTransactionRecord record)
    {
        var transaction = new PlatformRevenueTransaction(
            record.Id,
            record.TerritoryId,
            record.CheckoutId,
            record.FeeAmountInCents,
            record.Currency);

        if (record.FinancialTransactionId.HasValue)
        {
            transaction.SetFinancialTransactionId(record.FinancialTransactionId.Value);
        }

        return transaction;
    }

    public static PlatformExpenseTransactionRecord ToRecord(this PlatformExpenseTransaction transaction)
    {
        return new PlatformExpenseTransactionRecord
        {
            Id = transaction.Id,
            TerritoryId = transaction.TerritoryId,
            SellerTransactionId = transaction.SellerTransactionId,
            PayoutAmountInCents = transaction.PayoutAmountInCents,
            Currency = transaction.Currency,
            PayoutId = transaction.PayoutId,
            FinancialTransactionId = transaction.FinancialTransactionId,
            CreatedAtUtc = transaction.CreatedAtUtc
        };
    }

    public static PlatformExpenseTransaction ToDomain(this PlatformExpenseTransactionRecord record)
    {
        var transaction = new PlatformExpenseTransaction(
            record.Id,
            record.TerritoryId,
            record.SellerTransactionId,
            record.PayoutAmountInCents,
            record.Currency,
            record.PayoutId);

        if (record.FinancialTransactionId.HasValue)
        {
            transaction.SetFinancialTransactionId(record.FinancialTransactionId.Value);
        }

        return transaction;
    }

    public static ReconciliationRecordRecord ToRecord(this ReconciliationRecord record)
    {
        return new ReconciliationRecordRecord
        {
            Id = record.Id,
            TerritoryId = record.TerritoryId,
            ReconciliationDate = record.ReconciliationDate,
            ExpectedAmountInCents = record.ExpectedAmountInCents,
            ActualAmountInCents = record.ActualAmountInCents,
            DifferenceInCents = record.DifferenceInCents,
            Currency = record.Currency,
            Status = (int)record.Status,
            Notes = record.Notes,
            ReconciledByUserId = record.ReconciledByUserId,
            CreatedAtUtc = record.CreatedAtUtc,
            UpdatedAtUtc = record.UpdatedAtUtc
        };
    }

    public static ReconciliationRecord ToDomain(this ReconciliationRecordRecord record)
    {
        var reconciliation = new ReconciliationRecord(
            record.Id,
            record.TerritoryId,
            record.ReconciliationDate,
            record.ExpectedAmountInCents,
            record.ActualAmountInCents,
            record.Currency,
            record.ReconciledByUserId,
            record.Notes);

        // Restore status and timestamps using reflection
        var statusProp = typeof(ReconciliationRecord).GetProperty("Status", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(ReconciliationRecord).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(ReconciliationRecord).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (statusProp?.SetMethod != null) statusProp.SetValue(reconciliation, (ReconciliationStatus)record.Status);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(reconciliation, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(reconciliation, record.UpdatedAtUtc);

        return reconciliation;
    }

    public static TerritoryPayoutConfigRecord ToRecord(this TerritoryPayoutConfig config)
    {
        return new TerritoryPayoutConfigRecord
        {
            Id = config.Id,
            TerritoryId = config.TerritoryId,
            RetentionPeriodDays = config.RetentionPeriodDays,
            MinimumPayoutAmountInCents = config.MinimumPayoutAmountInCents,
            MaximumPayoutAmountInCents = config.MaximumPayoutAmountInCents,
            Frequency = (int)config.Frequency,
            AutoPayoutEnabled = config.AutoPayoutEnabled,
            RequiresApproval = config.RequiresApproval,
            Currency = config.Currency,
            IsActive = config.IsActive,
            CreatedAtUtc = config.CreatedAtUtc,
            UpdatedAtUtc = config.UpdatedAtUtc
        };
    }

    public static TerritoryPayoutConfig ToDomain(this TerritoryPayoutConfigRecord record)
    {
        var config = new TerritoryPayoutConfig(
            record.Id,
            record.TerritoryId,
            record.RetentionPeriodDays,
            record.MinimumPayoutAmountInCents,
            record.MaximumPayoutAmountInCents,
            (PayoutFrequency)record.Frequency,
            record.AutoPayoutEnabled,
            record.RequiresApproval,
            record.Currency);

        // Restore state using reflection
        var isActiveProp = typeof(TerritoryPayoutConfig).GetProperty("IsActive", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var createdAtProp = typeof(TerritoryPayoutConfig).GetProperty("CreatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var updatedAtProp = typeof(TerritoryPayoutConfig).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (isActiveProp?.SetMethod != null) isActiveProp.SetValue(config, record.IsActive);
        if (createdAtProp?.SetMethod != null) createdAtProp.SetValue(config, record.CreatedAtUtc);
        if (updatedAtProp?.SetMethod != null) updatedAtProp.SetValue(config, record.UpdatedAtUtc);

        return config;
    }

    public static MediaAssetRecord ToRecord(this MediaAsset mediaAsset)
    {
        return new MediaAssetRecord
        {
            Id = mediaAsset.Id,
            UploadedByUserId = mediaAsset.UploadedByUserId,
            MediaType = mediaAsset.MediaType,
            MimeType = mediaAsset.MimeType,
            StorageKey = mediaAsset.StorageKey,
            SizeBytes = mediaAsset.SizeBytes,
            WidthPx = mediaAsset.WidthPx,
            HeightPx = mediaAsset.HeightPx,
            Checksum = mediaAsset.Checksum,
            CreatedAtUtc = mediaAsset.CreatedAtUtc,
            DeletedByUserId = mediaAsset.DeletedByUserId,
            DeletedAtUtc = mediaAsset.DeletedAtUtc
        };
    }

    public static MediaAsset ToDomain(this MediaAssetRecord record)
    {
        return new MediaAsset(
            record.Id,
            record.UploadedByUserId,
            record.MediaType,
            record.MimeType,
            record.StorageKey,
            record.SizeBytes,
            record.WidthPx,
            record.HeightPx,
            record.Checksum,
            record.CreatedAtUtc,
            record.DeletedByUserId,
            record.DeletedAtUtc);
    }

    public static MediaAttachmentRecord ToRecord(this MediaAttachment attachment)
    {
        return new MediaAttachmentRecord
        {
            Id = attachment.Id,
            MediaAssetId = attachment.MediaAssetId,
            OwnerType = attachment.OwnerType,
            OwnerId = attachment.OwnerId,
            DisplayOrder = attachment.DisplayOrder,
            CreatedAtUtc = attachment.CreatedAtUtc
        };
    }

    public static MediaAttachment ToDomain(this MediaAttachmentRecord record)
    {
        return new MediaAttachment(
            record.Id,
            record.MediaAssetId,
            record.OwnerType,
            record.OwnerId,
            record.DisplayOrder,
            record.CreatedAtUtc);
    }

    // -----------------------
    // Marketplace Ratings
    // -----------------------
    public static StoreRatingRecord ToRecord(this StoreRating rating)
    {
        return new StoreRatingRecord
        {
            Id = rating.Id,
            StoreId = rating.StoreId,
            UserId = rating.UserId,
            Rating = rating.Rating,
            Comment = rating.Comment,
            CreatedAtUtc = rating.CreatedAtUtc,
            UpdatedAtUtc = rating.UpdatedAtUtc
        };
    }

    public static StoreRating ToDomain(this StoreRatingRecord record)
    {
        return new StoreRating(
            record.Id,
            record.StoreId,
            record.UserId,
            record.Rating,
            record.Comment,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static StoreItemRatingRecord ToRecord(this StoreItemRating rating)
    {
        return new StoreItemRatingRecord
        {
            Id = rating.Id,
            StoreItemId = rating.StoreItemId,
            UserId = rating.UserId,
            Rating = rating.Rating,
            Comment = rating.Comment,
            CreatedAtUtc = rating.CreatedAtUtc,
            UpdatedAtUtc = rating.UpdatedAtUtc
        };
    }

    public static StoreItemRating ToDomain(this StoreItemRatingRecord record)
    {
        return new StoreItemRating(
            record.Id,
            record.StoreItemId,
            record.UserId,
            record.Rating,
            record.Comment,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static StoreRatingResponseRecord ToRecord(this StoreRatingResponse response)
    {
        return new StoreRatingResponseRecord
        {
            Id = response.Id,
            RatingId = response.RatingId,
            StoreId = response.StoreId,
            ResponseText = response.ResponseText,
            CreatedAtUtc = response.CreatedAtUtc
        };
    }

    public static StoreRatingResponse ToDomain(this StoreRatingResponseRecord record)
    {
        return new StoreRatingResponse(
            record.Id,
            record.RatingId,
            record.StoreId,
            record.ResponseText,
            record.CreatedAtUtc);
    }
}
