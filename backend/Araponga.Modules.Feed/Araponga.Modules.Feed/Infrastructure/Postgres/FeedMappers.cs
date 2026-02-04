using Araponga.Domain.Feed;
using Araponga.Modules.Feed.Infrastructure.Postgres.Entities;
using Araponga.Modules.Map.Domain;
using System.Text.Json;

namespace Araponga.Modules.Feed.Infrastructure.Postgres;

public static class FeedMappers
{
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

    public static PostGeoAnchor ToPostGeoAnchorDomain(this PostGeoAnchorRecord record)
    {
        return new PostGeoAnchor(
            record.Id,
            record.PostId,
            record.Latitude,
            record.Longitude,
            record.Type,
            record.CreatedAtUtc);
    }

    public static PostAssetRecord ToRecord(this PostAsset postAsset)
    {
        return new PostAssetRecord
        {
            PostId = postAsset.PostId,
            AssetId = postAsset.AssetId
        };
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

    public static PostComment ToDomain(this PostCommentRecord record)
    {
        return new PostComment(
            record.Id,
            record.PostId,
            record.UserId,
            record.Content,
            record.CreatedAtUtc);
    }
}
