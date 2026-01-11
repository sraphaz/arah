using Araponga.Domain.Feed;
using Araponga.Domain.Health;
using Araponga.Domain.Map;
using Araponga.Domain.Social;
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
            Provider = user.Provider,
            ExternalId = user.ExternalId,
            Role = user.Role,
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
            record.Provider,
            record.ExternalId,
            record.Role,
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
            VerificationStatus = membership.VerificationStatus,
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
            record.VerificationStatus,
            record.CreatedAtUtc);
    }

    public static UserTerritoryRecord ToRecord(this UserTerritory membership)
    {
        return new UserTerritoryRecord
        {
            Id = membership.Id,
            UserId = membership.UserId,
            TerritoryId = membership.TerritoryId,
            Status = membership.Status,
            CreatedAtUtc = membership.CreatedAtUtc
        };
    }

    public static UserTerritory ToDomain(this UserTerritoryRecord record)
    {
        return new UserTerritory(
            record.Id,
            record.UserId,
            record.TerritoryId,
            record.Status,
            record.CreatedAtUtc);
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
            record.CreatedAtUtc);
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

    public static MapEntityRecord ToRecord(this MapEntity entity)
    {
        return new MapEntityRecord
        {
            Id = entity.Id,
            TerritoryId = entity.TerritoryId,
            CreatedByUserId = entity.CreatedByUserId,
            Name = entity.Name,
            Category = entity.Category,
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
            record.Status,
            record.Visibility,
            record.ConfirmationCount,
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
}
