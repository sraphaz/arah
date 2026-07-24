using Arah.Domain.Media;
using Arah.Domain.Users;
using Arah.Infrastructure.Postgres.Entities;
using System.Text.Json;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
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
}
