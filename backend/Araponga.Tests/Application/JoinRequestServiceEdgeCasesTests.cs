using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for JoinRequestService,
/// focusing on status transitions, territory validation, and error handling.
/// </summary>
public class JoinRequestServiceEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private static readonly Guid TestRecipientId = Guid.NewGuid();

    private static JoinRequestService CreateService(InMemoryDataStore dataStore, InMemorySharedStore sharedStore)
    {
        var joinRequestRepository = new InMemoryTerritoryJoinRequestRepository(sharedStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var userRepository = new InMemoryUserRepository(sharedStore);
        var membershipSettingsRepository = new InMemoryMembershipSettingsRepository(sharedStore);
        var featureFlagService = new InMemoryFeatureFlagService();
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            new InMemoryMembershipCapabilityRepository(sharedStore),
            new InMemorySystemPermissionRepository(sharedStore),
            new MembershipAccessRules(
                membershipRepository,
                membershipSettingsRepository,
                userRepository,
                featureFlagService),
            CacheTestHelper.CreateDistributedCacheService());
        var unitOfWork = new InMemoryUnitOfWork();
        var settingsRepository = new InMemoryMembershipSettingsRepository(sharedStore);
        return new JoinRequestService(
            joinRequestRepository,
            membershipRepository,
            settingsRepository,
            userRepository,
            accessEvaluator,
            unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyTerritoryId_HandlesCorrectly()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var requester = new User(
            TestUserId,
            "Test Requester",
            "requester@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(requester);

        var recipient = new User(
            TestRecipientId,
            "Test Recipient",
            "recipient@example.com",
            "987.654.321-00",
            null,
            null,
            null,
            "test",
            $"test-{TestRecipientId}",
            TestDate);
        sharedStore.Users.Add(recipient);

        // Criar território e membership para recipient
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
        sharedStore.Territories.Add(territory);

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            TestRecipientId,
            TestTerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            TestDate);
        sharedStore.Memberships.Add(membership);

        var (created, error, request) = await service.CreateAsync(
            TestUserId,
            Guid.Empty, // TerritoryId vazio
            new[] { TestRecipientId },
            "Please approve",
            CancellationToken.None);

        // Deve falhar porque territoryId vazio não existe
        Assert.False(created);
        Assert.NotNull(error);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentRecipient_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var requester = new User(
            TestUserId,
            "Test Requester",
            "requester@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(requester);

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
        sharedStore.Territories.Add(territory);

        var (created, error, request) = await service.CreateAsync(
            TestUserId,
            TestTerritoryId,
            new[] { Guid.NewGuid() }, // Recipient não existe
            "Please approve",
            CancellationToken.None);

        Assert.False(created);
        Assert.Contains("Recipient not found", error ?? "");
    }

    [Fact]
    public async Task CreateAsync_WithRecipientNotResidentOrCurator_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var requester = new User(
            TestUserId,
            "Test Requester",
            "requester@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(requester);

        var recipient = new User(
            TestRecipientId,
            "Test Recipient",
            "recipient@example.com",
            "987.654.321-00",
            null,
            null,
            null,
            "test",
            $"test-{TestRecipientId}",
            TestDate);
        sharedStore.Users.Add(recipient);

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
        sharedStore.Territories.Add(territory);

        // Recipient não é resident nem curator
        var (created, error, request) = await service.CreateAsync(
            TestUserId,
            TestTerritoryId,
            new[] { TestRecipientId },
            "Please approve",
            CancellationToken.None);

        Assert.False(created);
        Assert.Contains("Recipient is not a confirmed resident or curator", error ?? "");
    }

    [Fact]
    public async Task ApproveAsync_WithNonExistentRequest_ReturnsNotFound()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.ApproveAsync(
            Guid.NewGuid(),
            TestUserId,
            false,
            CancellationToken.None);

        Assert.False(result.Found);
        Assert.False(result.Updated);
        Assert.Null(result.Request);
    }

    [Fact]
    public async Task ApproveAsync_WithAlreadyApprovedRequest_HandlesCorrectly()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var requester = new User(
            TestUserId,
            "Test Requester",
            "requester@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(requester);

        var recipient = new User(
            TestRecipientId,
            "Test Recipient",
            "recipient@example.com",
            "987.654.321-00",
            null,
            null,
            null,
            "test",
            $"test-{TestRecipientId}",
            TestDate);
        sharedStore.Users.Add(recipient);

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
        sharedStore.Territories.Add(territory);

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            TestRecipientId,
            TestTerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            TestDate);
        sharedStore.Memberships.Add(membership);

        // Criar request
        var requestId = Guid.NewGuid();
        var request = new TerritoryJoinRequest(
            requestId,
            TestTerritoryId,
            TestUserId,
            "Please approve",
            TerritoryJoinRequestStatus.Approved, // Já aprovado
            TestDate,
            null, // expiresAtUtc
            TestDate, // decidedAtUtc
            TestRecipientId); // decidedByUserId
        sharedStore.TerritoryJoinRequests.Add(request);

        var recipientEntity = new TerritoryJoinRequestRecipient(
            requestId,
            TestRecipientId,
            TestDate,
            null);
        sharedStore.TerritoryJoinRequestRecipients.Add(recipientEntity);

        var result = await service.ApproveAsync(
            requestId,
            TestRecipientId,
            false,
            CancellationToken.None);

        // Pode retornar Found mas não Updated se já estava aprovado
        Assert.True(result.Found);
    }

    [Fact]
    public async Task RejectAsync_WithNonExistentRequest_ReturnsNotFound()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.RejectAsync(
            Guid.NewGuid(),
            TestUserId,
            false, // isCurator
            CancellationToken.None);

        Assert.False(result.Found);
        Assert.False(result.Updated);
        Assert.Null(result.Request);
    }

    [Fact]
    public async Task RejectAsync_WithEmptyReason_HandlesCorrectly()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var requester = new User(
            TestUserId,
            "Test Requester",
            "requester@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            $"test-{TestUserId}",
            TestDate);
        sharedStore.Users.Add(requester);

        var recipient = new User(
            TestRecipientId,
            "Test Recipient",
            "recipient@example.com",
            "987.654.321-00",
            null,
            null,
            null,
            "test",
            $"test-{TestRecipientId}",
            TestDate);
        sharedStore.Users.Add(recipient);

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
        sharedStore.Territories.Add(territory);

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            TestRecipientId,
            TestTerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            TestDate);
        sharedStore.Memberships.Add(membership);

        // Criar request pendente
        var requestId = Guid.NewGuid();
        var request = new TerritoryJoinRequest(
            requestId,
            TestTerritoryId,
            TestUserId,
            "Please approve",
            TerritoryJoinRequestStatus.Pending,
            TestDate,
            null,
            null,
            null);
        sharedStore.TerritoryJoinRequests.Add(request);

        var recipientEntity = new TerritoryJoinRequestRecipient(
            requestId,
            TestRecipientId,
            TestDate,
            null);
        sharedStore.TerritoryJoinRequestRecipients.Add(recipientEntity);

        var result = await service.RejectAsync(
            requestId,
            TestRecipientId,
            false, // isCurator
            CancellationToken.None);

        // Deve processar mesmo com reason vazio
        Assert.True(result.Found);
    }

    [Fact]
    public async Task ListIncomingAsync_WithNonExistentUser_ReturnsEmpty()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var service = CreateService(dataStore, sharedStore);

        var result = await service.ListIncomingAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.Empty(result);
    }
}
