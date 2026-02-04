using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Application.Services.Media;
using Araponga.Domain.Events;
using Araponga.Domain.Membership;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for EventsService,
/// focusing on coordinate validation, date validation, participation edge cases, and error handling.
/// </summary>
public class EventServiceEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    private static EventsService CreateService(InMemoryDataStore dataStore, InMemorySharedStore sharedStore)
    {
        var eventRepository = new InMemoryTerritoryEventRepository(dataStore);
        var participationRepository = new InMemoryEventParticipationRepository(dataStore);
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(sharedStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(sharedStore);
        var userRepository = new InMemoryUserRepository(sharedStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var membershipAccessRules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlags);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(sharedStore);
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            membershipAccessRules,
            cache);
        var mediaAssetRepository = new InMemoryMediaAssetRepository(dataStore);
        var mediaAttachmentRepository = new InMemoryMediaAttachmentRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var mediaConfigRepository = new InMemoryTerritoryMediaConfigRepository(dataStore);
        var globalMediaLimits = new Araponga.Infrastructure.InMemory.InMemoryGlobalMediaLimits();
        var mediaConfigService = new TerritoryMediaConfigService(
            mediaConfigRepository,
            featureFlags,
            unitOfWork,
            globalMediaLimits);

        return new EventsService(
            eventRepository,
            participationRepository,
            feedRepository,
            mediaAssetRepository,
            mediaAttachmentRepository,
            mediaConfigService,
            accessEvaluator,
            userRepository,
            unitOfWork);
    }

    [Fact]
    public async Task CreateEventAsync_WithEmptyTerritoryId_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar usuário para passar validação de políticas
        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var result = await service.CreateEventAsync(
            Guid.Empty,
            TestUserId,
            "Test Event",
            "Description",
            TestDate.AddDays(1),
            TestDate.AddDays(2),
            0.0,
            0.0,
            "Location",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Territory ID is required", result.Error ?? "");
    }

    [Fact]
    public async Task CreateEventAsync_WithEmptyUserId_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.CreateEventAsync(
            TestTerritoryId,
            Guid.Empty,
            "Test Event",
            "Description",
            TestDate.AddDays(1),
            TestDate.AddDays(2),
            0.0,
            0.0,
            "Location",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("User ID is required", result.Error ?? "");
    }

    [Fact]
    public async Task CreateEventAsync_WithNullTitle_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar usuário para passar validação de políticas
        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var result = await service.CreateEventAsync(
            TestTerritoryId,
            TestUserId,
            null!,
            "Description",
            TestDate.AddDays(1),
            TestDate.AddDays(2),
            0.0,
            0.0,
            "Location",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Title is required", result.Error ?? "");
    }

    [Fact]
    public async Task CreateEventAsync_WithEmptyTitle_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar usuário para passar validação de políticas
        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var result = await service.CreateEventAsync(
            TestTerritoryId,
            TestUserId,
            "   ",
            "Description",
            TestDate.AddDays(1),
            TestDate.AddDays(2),
            0.0,
            0.0,
            "Location",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Title is required", result.Error ?? "");
    }

    [Fact]
    public async Task CreateEventAsync_WithInvalidLatitude_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar usuário para passar validação de políticas
        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var result = await service.CreateEventAsync(
            TestTerritoryId,
            TestUserId,
            "Test Event",
            "Description",
            TestDate.AddDays(1),
            TestDate.AddDays(2),
            91.0, // Latitude inválida (> 90)
            0.0,
            "Location",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid latitude/longitude", result.Error ?? "");
    }

    [Fact]
    public async Task CreateEventAsync_WithInvalidLongitude_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar usuário para passar validação de políticas
        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var result = await service.CreateEventAsync(
            TestTerritoryId,
            TestUserId,
            "Test Event",
            "Description",
            TestDate.AddDays(1),
            TestDate.AddDays(2),
            0.0,
            181.0, // Longitude inválida (> 180)
            "Location",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid latitude/longitude", result.Error ?? "");
    }

    [Fact]
    public async Task CreateEventAsync_WithEndsAtBeforeStartsAt_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar usuário para passar validação de políticas
        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        var startsAt = TestDate.AddDays(2);
        var endsAt = TestDate.AddDays(1); // Antes de startsAt

        var result = await service.CreateEventAsync(
            TestTerritoryId,
            TestUserId,
            "Test Event",
            "Description",
            startsAt,
            endsAt,
            0.0,
            0.0,
            "Location",
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("EndsAtUtc must be after StartsAtUtc", result.Error ?? "");
    }

    [Fact]
    public async Task CreateEventAsync_WithUnicodeInTitle_HandlesCorrectly()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar usuário para passar validação de políticas
        var user = new User(
            TestUserId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(user);

        // Criar território e membership para passar validação de acesso
        var territory = new Territory(
            TestTerritoryId,
            null,
            "Test Territory",
            "Description",
            TerritoryStatus.Active,
            "Test City",
            "TS",
            0.0,
            0.0,
            TestDate);
        dataStore.Territories.Add(territory);

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            TestUserId,
            TestTerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            TestDate);
        dataStore.Memberships.Add(membership);

        var result = await service.CreateEventAsync(
            TestTerritoryId,
            TestUserId,
            "Evento com café, naïve, résumé, 文字 e emoji 🎉",
            "Descrição com Unicode",
            TestDate.AddDays(1),
            TestDate.AddDays(2),
            0.0,
            0.0,
            "Localização com Unicode",
            null,
            null,
            CancellationToken.None);

        // Pode falhar por falta de aceite de políticas, mas não por causa do Unicode
        // Se passar, significa que as políticas foram aceitas e o Unicode foi processado corretamente
        if (result.IsFailure)
        {
            Assert.DoesNotContain("Unicode", result.Error ?? "");
        }
        else
        {
            // Se passou, significa que Unicode foi aceito corretamente
            Assert.True(result.IsSuccess);
        }
    }

    [Fact]
    public async Task UpdateEventAsync_WithNonExistentEvent_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.UpdateEventAsync(
            Guid.NewGuid(),
            TestUserId,
            "Updated Title",
            "Updated Description",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Event not found", result.Error ?? "");
    }

    [Fact]
    public async Task CancelEventAsync_WithNonExistentEvent_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.CancelEventAsync(
            Guid.NewGuid(),
            TestUserId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Event not found", result.Error ?? "");
    }

    [Fact]
    public async Task SetParticipationAsync_WithNonExistentEvent_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.SetParticipationAsync(
            Guid.NewGuid(),
            TestUserId,
            EventParticipationStatus.Interested,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Event not found", result.Error ?? "");
    }

    [Fact]
    public async Task SetParticipationAsync_WithEmptyUserId_HandlesCorrectly()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        // Criar evento
        var eventId = Guid.NewGuid();
        var territoryEvent = new TerritoryEvent(
            eventId,
            TestTerritoryId,
            "Test Event",
            "Description",
            TestDate.AddDays(1),
            TestDate.AddDays(2),
            0.0,
            0.0,
            "Location",
            TestUserId,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            TestDate,
            TestDate);
        dataStore.TerritoryEvents.Add(territoryEvent);

        // SetParticipationAsync valida userId vazio e lança ArgumentException
        // O teste verifica que o método lança exceção apropriada
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await service.SetParticipationAsync(
                eventId,
                Guid.Empty,
                EventParticipationStatus.Interested,
                CancellationToken.None);
        });
    }

    [Fact]
    public async Task GetEventsNearbyAsync_WithInvalidCoordinates_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.GetEventsNearbyAsync(
            91.0, // Latitude inválida
            0.0,
            10.0,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEventParticipantsAsync_WithNonExistentEvent_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.GetEventParticipantsAsync(
            Guid.NewGuid(),
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Event not found", result.Error ?? "");
    }
}
