using Araponga.Modules.Assets.Domain;
using Araponga.Modules.Map.Domain;
using Araponga.Domain.Chat;
using Araponga.Domain.Email;
using Araponga.Domain.Events;
using Araponga.Domain.Feed;
using Araponga.Domain.Financial;
using Araponga.Domain.Health;
using Araponga.Modules.Marketplace.Domain;
using Araponga.Domain.Media;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Domain.Subscriptions;
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
                // Ignore invalid JSON; treat as null polygon
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
            EditCount = post.EditCount,
            TagsJson = post.Tags.Count > 0 ? JsonSerializer.Serialize(post.Tags) : null
        };
    }

    public static CommunityPost ToDomain(this CommunityPostRecord record)
    {
        IReadOnlyList<string>? tags = null;
        if (!string.IsNullOrWhiteSpace(record.TagsJson))
        {
            try
            {
                tags = JsonSerializer.Deserialize<List<string>>(record.TagsJson);
            }
            catch
            {
                // Se falhar ao deserializar, tags permanece null
            }
        }

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
            record.EditCount > int.MaxValue ? int.MaxValue : record.EditCount,
            tags);
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

    public static WorkItemRecord ToRecord(this Araponga.Modules.Moderation.Domain.Work.WorkItem item)
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

    public static Araponga.Modules.Moderation.Domain.Work.WorkItem ToDomain(this WorkItemRecord record)
    {
        return new Araponga.Modules.Moderation.Domain.Work.WorkItem(
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

    public static DocumentEvidenceRecord ToRecord(this Araponga.Modules.Moderation.Domain.Evidence.DocumentEvidence evidence)
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

    public static Araponga.Modules.Moderation.Domain.Evidence.DocumentEvidence ToDomain(this DocumentEvidenceRecord record)
    {
        return new Araponga.Modules.Moderation.Domain.Evidence.DocumentEvidence(
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

    private static readonly JsonSerializerOptions MediaConfigJsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static TerritoryMediaConfigRecord ToRecord(this TerritoryMediaConfig config)
    {
        return new TerritoryMediaConfigRecord
        {
            TerritoryId = config.TerritoryId,
            PostsJson = JsonSerializer.Serialize(config.Posts, MediaConfigJsonOptions),
            EventsJson = JsonSerializer.Serialize(config.Events, MediaConfigJsonOptions),
            MarketplaceJson = JsonSerializer.Serialize(config.Marketplace, MediaConfigJsonOptions),
            ChatJson = JsonSerializer.Serialize(config.Chat, MediaConfigJsonOptions),
            UpdatedAtUtc = config.UpdatedAtUtc,
            UpdatedByUserId = config.UpdatedByUserId
        };
    }

    public static TerritoryMediaConfig ToDomain(this TerritoryMediaConfigRecord record)
    {
        return new TerritoryMediaConfig
        {
            TerritoryId = record.TerritoryId,
            Posts = JsonSerializer.Deserialize<MediaContentConfig>(record.PostsJson, MediaConfigJsonOptions) ?? new MediaContentConfig(),
            Events = JsonSerializer.Deserialize<MediaContentConfig>(record.EventsJson, MediaConfigJsonOptions) ?? new MediaContentConfig(),
            Marketplace = JsonSerializer.Deserialize<MediaContentConfig>(record.MarketplaceJson, MediaConfigJsonOptions) ?? new MediaContentConfig(),
            Chat = JsonSerializer.Deserialize<MediaChatConfig>(record.ChatJson, MediaConfigJsonOptions) ?? new MediaChatConfig(),
            UpdatedAtUtc = record.UpdatedAtUtc,
            UpdatedByUserId = record.UpdatedByUserId
        };
    }

    public static UserMediaPreferencesRecord ToRecord(this UserMediaPreferences preferences)
    {
        return new UserMediaPreferencesRecord
        {
            UserId = preferences.UserId,
            ShowImages = preferences.ShowImages,
            ShowVideos = preferences.ShowVideos,
            ShowAudio = preferences.ShowAudio,
            AutoPlayVideos = preferences.AutoPlayVideos,
            AutoPlayAudio = preferences.AutoPlayAudio,
            UpdatedAtUtc = preferences.UpdatedAtUtc
        };
    }

    public static UserMediaPreferences ToDomain(this UserMediaPreferencesRecord record)
    {
        return new UserMediaPreferences
        {
            UserId = record.UserId,
            ShowImages = record.ShowImages,
            ShowVideos = record.ShowVideos,
            ShowAudio = record.ShowAudio,
            AutoPlayVideos = record.AutoPlayVideos,
            AutoPlayAudio = record.AutoPlayAudio,
            UpdatedAtUtc = record.UpdatedAtUtc
        };
    }

    private static readonly JsonSerializerOptions MediaStorageSettingsJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static MediaStorageConfigRecord ToRecord(this MediaStorageConfig config)
    {
        return new MediaStorageConfigRecord
        {
            Id = config.Id,
            Provider = (int)config.Provider,
            SettingsJson = JsonSerializer.Serialize(config.Settings, MediaStorageSettingsJsonOptions),
            IsActive = config.IsActive,
            Description = config.Description,
            CreatedAtUtc = config.CreatedAtUtc,
            CreatedByUserId = config.CreatedByUserId,
            UpdatedAtUtc = config.UpdatedAtUtc,
            UpdatedByUserId = config.UpdatedByUserId
        };
    }

    public static MediaStorageConfig ToDomain(this MediaStorageConfigRecord record)
    {
        var settings = JsonSerializer.Deserialize<MediaStorageSettings>(record.SettingsJson, MediaStorageSettingsJsonOptions)
            ?? new MediaStorageSettings();
        return new MediaStorageConfig(
            record.Id,
            (MediaStorageProvider)record.Provider,
            settings,
            record.IsActive,
            record.Description,
            record.CreatedAtUtc,
            record.CreatedByUserId,
            record.UpdatedAtUtc,
            record.UpdatedByUserId);
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

    // -----------------------
    // Policies
    // -----------------------

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

        // Atualizar UpdatedAtUtc usando reflection
        var updatedAtProp = typeof(TermsOfService).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (updatedAtProp?.SetMethod != null)
        {
            updatedAtProp.SetValue(terms, record.UpdatedAtUtc);
        }

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

        // Atualizar IsRevoked e RevokedAtUtc usando reflection se necessário
        if (record.IsRevoked)
        {
            var isRevokedProp = typeof(TermsAcceptance).GetProperty("IsRevoked", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var revokedAtProp = typeof(TermsAcceptance).GetProperty("RevokedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (isRevokedProp?.SetMethod != null)
            {
                isRevokedProp.SetValue(acceptance, true);
            }
            if (revokedAtProp?.SetMethod != null && record.RevokedAtUtc.HasValue)
            {
                revokedAtProp.SetValue(acceptance, record.RevokedAtUtc.Value);
            }
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

        // Atualizar UpdatedAtUtc usando reflection
        var updatedAtProp = typeof(PrivacyPolicy).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (updatedAtProp?.SetMethod != null)
        {
            updatedAtProp.SetValue(policy, record.UpdatedAtUtc);
        }

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

        // Atualizar IsRevoked e RevokedAtUtc usando reflection se necessário
        if (record.IsRevoked)
        {
            var isRevokedProp = typeof(PrivacyPolicyAcceptance).GetProperty("IsRevoked", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var revokedAtProp = typeof(PrivacyPolicyAcceptance).GetProperty("RevokedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (isRevokedProp?.SetMethod != null)
            {
                isRevokedProp.SetValue(acceptance, true);
            }
            if (revokedAtProp?.SetMethod != null && record.RevokedAtUtc.HasValue)
            {
                revokedAtProp.SetValue(acceptance, record.RevokedAtUtc.Value);
            }
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

    public static EmailQueueItemRecord ToRecord(this EmailQueueItem item)
    {
        return new EmailQueueItemRecord
        {
            Id = item.Id,
            To = item.To,
            Subject = item.Subject,
            Body = item.Body,
            IsHtml = item.IsHtml,
            TemplateName = item.TemplateName,
            TemplateDataJson = item.TemplateDataJson,
            Priority = (int)item.Priority,
            ScheduledFor = item.ScheduledFor,
            Attempts = item.Attempts,
            Status = (int)item.Status,
            CreatedAtUtc = item.CreatedAtUtc,
            ProcessedAtUtc = item.ProcessedAtUtc,
            ErrorMessage = item.ErrorMessage,
            NextRetryAtUtc = item.NextRetryAtUtc
        };
    }

    public static EmailQueueItem ToDomain(this EmailQueueItemRecord record)
    {
        // Usar construtor privado via reflection ou criar via método factory
        // Por enquanto, vamos criar usando o construtor público e depois restaurar estado
        var item = new EmailQueueItem(
            record.Id,
            record.To,
            record.Subject,
            record.Body ?? string.Empty,
            record.IsHtml,
            record.TemplateName,
            record.TemplateDataJson,
            (EmailQueuePriority)record.Priority,
            record.ScheduledFor);

        // Restaurar estado usando método público
        item.RestoreState(
            (EmailQueueStatus)record.Status,
            record.Attempts,
            record.ErrorMessage,
            record.NextRetryAtUtc,
            record.ProcessedAtUtc,
            record.CreatedAtUtc);

        return item;
    }

    public static VotingRecord ToRecord(this Domain.Governance.Voting voting)
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

    public static Domain.Governance.Voting ToDomain(this VotingRecord record)
    {
        var options = JsonSerializer.Deserialize<List<string>>(record.OptionsJson) ?? new List<string>();
        return new Domain.Governance.Voting(
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

    public static VoteRecord ToRecord(this Domain.Governance.Vote vote)
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

    public static Domain.Governance.Vote ToDomain(this VoteRecord record)
    {
        return new Domain.Governance.Vote(
            record.Id,
            record.VotingId,
            record.UserId,
            record.SelectedOption,
            record.CreatedAtUtc);
    }

    public static TerritoryModerationRuleRecord ToRecord(this Domain.Governance.TerritoryModerationRule rule)
    {
        return new TerritoryModerationRuleRecord
        {
            Id = rule.Id,
            TerritoryId = rule.TerritoryId,
            CreatedByVotingId = rule.CreatedByVotingId,
            RuleType = rule.RuleType,
            RuleJson = rule.RuleJson,
            IsActive = rule.IsActive,
            CreatedAtUtc = rule.CreatedAtUtc,
            UpdatedAtUtc = rule.UpdatedAtUtc
        };
    }

    public static Domain.Governance.TerritoryModerationRule ToDomain(this TerritoryModerationRuleRecord record)
    {
        return new Domain.Governance.TerritoryModerationRule(
            record.Id,
            record.TerritoryId,
            record.CreatedByVotingId,
            record.RuleType,
            record.RuleJson,
            record.IsActive,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    // ==================== Subscription Mappers ====================

    public static SubscriptionPlanRecord ToRecord(this SubscriptionPlan plan)
    {
        return new SubscriptionPlanRecord
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Tier = (int)plan.Tier,
            Scope = (int)plan.Scope,
            TerritoryId = plan.TerritoryId,
            PricePerCycle = plan.PricePerCycle,
            BillingCycle = plan.BillingCycle.HasValue ? (int)plan.BillingCycle.Value : null,
            CapabilitiesJson = JsonSerializer.Serialize(plan.Capabilities),
            LimitsJson = plan.Limits != null ? JsonSerializer.Serialize(plan.Limits) : null,
            IsDefault = plan.IsDefault,
            TrialDays = plan.TrialDays,
            IsActive = plan.IsActive,
            CreatedByUserId = plan.CreatedByUserId,
            StripePriceId = plan.StripePriceId,
            StripeProductId = plan.StripeProductId,
            CreatedAtUtc = plan.CreatedAtUtc,
            UpdatedAtUtc = plan.UpdatedAtUtc
        };
    }

    public static SubscriptionPlan ToDomain(this SubscriptionPlanRecord record)
    {
        var capabilities = JsonSerializer.Deserialize<List<FeatureCapability>>(record.CapabilitiesJson) ?? new List<FeatureCapability>();
        Dictionary<string, object>? limits = null;
        if (!string.IsNullOrWhiteSpace(record.LimitsJson))
        {
            try
            {
                limits = JsonSerializer.Deserialize<Dictionary<string, object>>(record.LimitsJson);
            }
            catch
            {
                // Se falhar, limits permanece null
            }
        }

        return new SubscriptionPlan(
            record.Id,
            record.Name,
            record.Description,
            (SubscriptionPlanTier)record.Tier,
            (PlanScope)record.Scope,
            record.TerritoryId,
            record.PricePerCycle,
            record.BillingCycle.HasValue ? (SubscriptionBillingCycle)record.BillingCycle.Value : null,
            capabilities,
            limits,
            record.IsDefault,
            record.TrialDays,
            record.CreatedByUserId,
            record.StripePriceId,
            record.StripeProductId);
    }

    public static SubscriptionRecord ToRecord(this Subscription subscription)
    {
        return new SubscriptionRecord
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            TerritoryId = subscription.TerritoryId,
            PlanId = subscription.PlanId,
            Status = (int)subscription.Status,
            CurrentPeriodStart = subscription.CurrentPeriodStart,
            CurrentPeriodEnd = subscription.CurrentPeriodEnd,
            TrialStart = subscription.TrialStart,
            TrialEnd = subscription.TrialEnd,
            CanceledAt = subscription.CanceledAt,
            CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
            StripeSubscriptionId = subscription.StripeSubscriptionId,
            StripeCustomerId = subscription.StripeCustomerId,
            CreatedAtUtc = subscription.CreatedAtUtc,
            UpdatedAtUtc = subscription.UpdatedAtUtc
        };
    }

    public static Subscription ToDomain(this SubscriptionRecord record)
    {
        return new Subscription(
            record.Id,
            record.UserId,
            record.TerritoryId,
            record.PlanId,
            (SubscriptionStatus)record.Status,
            record.CurrentPeriodStart,
            record.CurrentPeriodEnd,
            record.TrialStart,
            record.TrialEnd,
            record.StripeSubscriptionId,
            record.StripeCustomerId);
    }

    public static SubscriptionPaymentRecord ToRecord(this SubscriptionPayment payment)
    {
        return new SubscriptionPaymentRecord
        {
            Id = payment.Id,
            SubscriptionId = payment.SubscriptionId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = (int)payment.Status,
            PaymentDate = payment.PaymentDate,
            PeriodStart = payment.PeriodStart,
            PeriodEnd = payment.PeriodEnd,
            StripeInvoiceId = payment.StripeInvoiceId,
            StripePaymentIntentId = payment.StripePaymentIntentId,
            FailureReason = payment.FailureReason,
            CreatedAtUtc = payment.CreatedAtUtc,
            UpdatedAtUtc = payment.UpdatedAtUtc
        };
    }

    public static SubscriptionPayment ToDomain(this SubscriptionPaymentRecord record)
    {
        return new SubscriptionPayment(
            record.Id,
            record.SubscriptionId,
            record.Amount,
            record.Currency,
            (SubscriptionPaymentStatus)record.Status,
            record.PaymentDate,
            record.PeriodStart,
            record.PeriodEnd,
            record.StripeInvoiceId,
            record.StripePaymentIntentId,
            record.FailureReason);
    }

    public static CouponRecord ToRecord(this Coupon coupon)
    {
        return new CouponRecord
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Name = coupon.Name,
            Description = coupon.Description,
            DiscountType = (int)coupon.DiscountType,
            DiscountValue = coupon.DiscountValue,
            ValidFrom = coupon.ValidFrom,
            ValidUntil = coupon.ValidUntil,
            MaxUses = coupon.MaxUses,
            UsedCount = coupon.UsedCount,
            IsActive = coupon.IsActive,
            StripeCouponId = coupon.StripeCouponId,
            CreatedAtUtc = coupon.CreatedAtUtc,
            UpdatedAtUtc = coupon.UpdatedAtUtc
        };
    }

    public static Coupon ToDomain(this CouponRecord record)
    {
        var coupon = new Coupon(
            record.Id,
            record.Code,
            record.Name,
            record.Description,
            (CouponDiscountType)record.DiscountType,
            record.DiscountValue,
            record.ValidFrom,
            record.ValidUntil,
            record.MaxUses,
            record.StripeCouponId);

        // Restaurar estado do banco
        coupon.RestoreState(record.UsedCount, record.IsActive, record.CreatedAtUtc, record.UpdatedAtUtc);

        return coupon;
    }

    public static SubscriptionCouponRecord ToRecord(this SubscriptionCoupon subscriptionCoupon)
    {
        return new SubscriptionCouponRecord
        {
            Id = subscriptionCoupon.Id,
            SubscriptionId = subscriptionCoupon.SubscriptionId,
            CouponId = subscriptionCoupon.CouponId,
            AppliedAtUtc = subscriptionCoupon.AppliedAtUtc
        };
    }

    public static SubscriptionCoupon ToDomain(this SubscriptionCouponRecord record)
    {
        return new SubscriptionCoupon(
            record.Id,
            record.SubscriptionId,
            record.CouponId);
    }

    public static SubscriptionPlanHistoryRecord ToRecord(this SubscriptionPlanHistory history)
    {
        return new SubscriptionPlanHistoryRecord
        {
            Id = history.Id,
            PlanId = history.PlanId,
            ChangedByUserId = history.ChangedByUserId,
            ChangeType = (int)history.ChangeType,
            PreviousStateJson = history.PreviousState != null ? JsonSerializer.Serialize(history.PreviousState) : null,
            NewStateJson = history.NewState != null ? JsonSerializer.Serialize(history.NewState) : null,
            ChangeReason = history.ChangeReason,
            ChangedAtUtc = history.ChangedAtUtc
        };
    }

    public static SubscriptionPlanHistory ToDomain(this SubscriptionPlanHistoryRecord record)
    {
        Dictionary<string, object>? previousState = null;
        if (!string.IsNullOrWhiteSpace(record.PreviousStateJson))
        {
            try
            {
                previousState = JsonSerializer.Deserialize<Dictionary<string, object>>(record.PreviousStateJson);
            }
            catch
            {
                // Se falhar, previousState permanece null
            }
        }

        Dictionary<string, object>? newState = null;
        if (!string.IsNullOrWhiteSpace(record.NewStateJson))
        {
            try
            {
                newState = JsonSerializer.Deserialize<Dictionary<string, object>>(record.NewStateJson);
            }
            catch
            {
                // Se falhar, newState permanece null
            }
        }

        return new SubscriptionPlanHistory(
            record.Id,
            record.PlanId,
            record.ChangedByUserId,
            (SubscriptionPlanHistoryChangeType)record.ChangeType,
            previousState,
            newState,
            record.ChangeReason);
    }
}
