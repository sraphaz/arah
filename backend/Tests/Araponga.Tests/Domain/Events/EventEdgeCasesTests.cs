using Araponga.Domain.Events;
using Araponga.Domain.Geo;
using Araponga.Domain.Membership;
using Xunit;

namespace Araponga.Tests.Domain.Events;

/// <summary>
/// Edge case tests for TerritoryEvent and EventParticipation domain entities,
/// focusing on invalid coordinates, extreme dates, capacity validation, status transitions, and Unicode.
/// </summary>
public class EventEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    // TerritoryEvent edge cases
    [Fact]
    public void TerritoryEvent_Constructor_WithValidData_CreatesSuccessfully()
    {
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            "Description",
            TestDate.AddDays(1),
            TestDate.AddDays(2),
            -23.5505,
            -46.6333,
            "São Paulo",
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Equal("Test Event", evt.Title);
        Assert.Equal(-23.5505, evt.Latitude);
        Assert.Equal(-46.6333, evt.Longitude);
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithEmptyTerritoryId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TerritoryEvent(
            Guid.NewGuid(),
            Guid.Empty,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate));
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithEmptyCreatedByUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            Guid.Empty,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate));
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithNullTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            null!,
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate));
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithEmptyTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "   ",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate));
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithUnicodeTitle_TrimsAndStores()
    {
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "  Evento Café & Cia 🎉  ",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Equal("Evento Café & Cia 🎉", evt.Title);
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithUnicodeDescription_TrimsAndStores()
    {
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            "  Descrição com café, naïve, résumé, 文字 e emoji 🎉  ",
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Contains("café", evt.Description!);
        Assert.Contains("文字", evt.Description!);
        Assert.Contains("🎉", evt.Description!);
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithInvalidLatitude_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            91,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate));
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithInvalidLongitude_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            0,
            181,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate));
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithEndsAtBeforeStartsAt_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(2),
            TestDate.AddDays(1),
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate));
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithEndsAtEqualToStartsAt_Allows()
    {
        // Nota: O construtor permite EndsAtUtc == StartsAtUtc, apenas valida se EndsAtUtc < StartsAtUtc
        var startDate = TestDate.AddDays(1);
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            startDate,
            startDate,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Equal(startDate, evt.StartsAtUtc);
        Assert.Equal(startDate, evt.EndsAtUtc);
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithPastStartDate_Allows()
    {
        var pastDate = TestDate.AddDays(-1);
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Past Event",
            null,
            pastDate,
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Equal(pastDate, evt.StartsAtUtc);
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithFutureStartDate_Allows()
    {
        var futureDate = TestDate.AddYears(10);
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Future Event",
            null,
            futureDate,
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Equal(futureDate, evt.StartsAtUtc);
    }

    [Fact]
    public void TerritoryEvent_Update_WithNullTitle_ThrowsArgumentException()
    {
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Throws<ArgumentException>(() => evt.Update(
            null!,
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestDate.AddHours(1)));
    }

    [Fact]
    public void TerritoryEvent_Update_WithEmptyTitle_ThrowsArgumentException()
    {
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Throws<ArgumentException>(() => evt.Update(
            "   ",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestDate.AddHours(1)));
    }

    [Fact]
    public void TerritoryEvent_Update_WithInvalidLatitude_ThrowsArgumentException()
    {
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Throws<ArgumentException>(() => evt.Update(
            "Updated",
            null,
            TestDate.AddDays(1),
            null,
            91,
            0,
            null,
            TestDate.AddHours(1)));
    }

    [Fact]
    public void TerritoryEvent_Update_WithEndsAtBeforeStartsAt_ThrowsArgumentException()
    {
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        Assert.Throws<ArgumentException>(() => evt.Update(
            "Updated",
            null,
            TestDate.AddDays(2),
            TestDate.AddDays(1),
            0,
            0,
            null,
            TestDate.AddHours(1)));
    }

    [Fact]
    public void TerritoryEvent_Cancel_UpdatesStatus()
    {
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        evt.Cancel(TestDate.AddHours(1));

        Assert.Equal(EventStatus.Canceled, evt.Status);
    }

    [Fact]
    public void TerritoryEvent_Update_WithUnicodeLocationLabel_TrimsAndStores()
    {
        var evt = new TerritoryEvent(
            Guid.NewGuid(),
            TestTerritoryId,
            "Test Event",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            null,
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);

        evt.Update(
            "Updated",
            null,
            TestDate.AddDays(1),
            null,
            0,
            0,
            "  Localização: Café & Cia 🏪  ",
            TestDate.AddHours(1));

        Assert.Equal("Localização: Café & Cia 🏪", evt.LocationLabel);
    }

    [Fact]
    public void TerritoryEvent_Constructor_WithAllEventStatuses_CreatesSuccessfully()
    {
        var statuses = new[] { EventStatus.Scheduled, EventStatus.Canceled, EventStatus.Finished };

        foreach (var status in statuses)
        {
            var evt = new TerritoryEvent(
                Guid.NewGuid(),
                TestTerritoryId,
                "Test Event",
                null,
                TestDate.AddDays(1),
                null,
                0,
                0,
                null,
                TestUserId,
                MembershipRole.Resident,
                status,
                TestDate,
                TestDate);

            Assert.Equal(status, evt.Status);
        }
    }

    // EventParticipation edge cases
    [Fact]
    public void EventParticipation_Constructor_WithValidData_CreatesSuccessfully()
    {
        var participation = new EventParticipation(
            Guid.NewGuid(),
            TestUserId,
            EventParticipationStatus.Interested,
            TestDate,
            TestDate);

        Assert.Equal(TestUserId, participation.UserId);
        Assert.Equal(EventParticipationStatus.Interested, participation.Status);
    }

    [Fact]
    public void EventParticipation_Constructor_WithEmptyEventId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new EventParticipation(
            Guid.Empty,
            TestUserId,
            EventParticipationStatus.Interested,
            TestDate,
            TestDate));
    }

    [Fact]
    public void EventParticipation_Constructor_WithEmptyUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new EventParticipation(
            Guid.NewGuid(),
            Guid.Empty,
            EventParticipationStatus.Interested,
            TestDate,
            TestDate));
    }

    [Fact]
    public void EventParticipation_UpdateStatus_WithAllStatuses_UpdatesSuccessfully()
    {
        var participation = new EventParticipation(
            Guid.NewGuid(),
            TestUserId,
            EventParticipationStatus.Interested,
            TestDate,
            TestDate);

        var statuses = new[]
        {
            EventParticipationStatus.Interested,
            EventParticipationStatus.Confirmed
        };

        foreach (var status in statuses)
        {
            participation.UpdateStatus(status, TestDate.AddHours(1));
            Assert.Equal(status, participation.Status);
        }
    }

    [Fact]
    public void EventParticipation_UpdateStatus_UpdatesTimestamp()
    {
        var participation = new EventParticipation(
            Guid.NewGuid(),
            TestUserId,
            EventParticipationStatus.Interested,
            TestDate,
            TestDate);

        var newDate = TestDate.AddHours(2);
        participation.UpdateStatus(EventParticipationStatus.Confirmed, newDate);

        Assert.Equal(newDate, participation.UpdatedAtUtc);
    }

    [Fact]
    public void EventParticipation_UpdateStatus_FromConfirmedToInterested_UpdatesSuccessfully()
    {
        var participation = new EventParticipation(
            Guid.NewGuid(),
            TestUserId,
            EventParticipationStatus.Confirmed,
            TestDate,
            TestDate);

        participation.UpdateStatus(EventParticipationStatus.Interested, TestDate.AddHours(1));

        Assert.Equal(EventParticipationStatus.Interested, participation.Status);
    }
}
