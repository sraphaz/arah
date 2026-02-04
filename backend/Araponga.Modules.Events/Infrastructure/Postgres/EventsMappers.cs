using Araponga.Application.Models;
using Araponga.Domain.Events;
using Araponga.Modules.Events.Infrastructure.Postgres.Entities;

namespace Araponga.Modules.Events.Infrastructure.Postgres;

public static class EventsMappers
{
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
}
