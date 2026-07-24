using Arah.Domain.Social.JoinRequests;
using Arah.Infrastructure.Postgres.Entities;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
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
}
