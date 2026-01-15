using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class JoinRequestServiceTests
{
    private static async Task EnsureTestUserExists(InMemoryDataStore dataStore, Guid userId, string displayName, string cpf)
    {
        var userRepository = new InMemoryUserRepository(dataStore);
        var existing = await userRepository.GetByIdAsync(userId, CancellationToken.None);
        if (existing is null)
        {
            var user = new User(userId, displayName, null, cpf, null, null, null, "test", $"test-{userId}", DateTime.UtcNow);
            await userRepository.AddAsync(user, CancellationToken.None);
        }
    }

    private static JoinRequestService CreateService(InMemoryDataStore dataStore)
    {
        var joinRequestRepository = new InMemoryTerritoryJoinRequestRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var membershipSettingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var featureFlagService = new InMemoryFeatureFlagService();
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            new InMemoryMembershipCapabilityRepository(dataStore),
            new InMemorySystemPermissionRepository(dataStore),
            new MembershipAccessRules(
                membershipRepository,
                membershipSettingsRepository,
                userRepository,
                featureFlagService),
            CacheTestHelper.CreateDistributedCacheService());
        var unitOfWork = new InMemoryUnitOfWork();
        return new JoinRequestService(
            joinRequestRepository,
            membershipRepository,
            userRepository,
            accessEvaluator,
            unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ReturnsError_WhenNoRecipients()
    {
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory1;

        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            Array.Empty<Guid>(),
            null,
            CancellationToken.None);

        Assert.False(created);
        Assert.Equal("Recipients are required.", error);
        Assert.Null(request);
    }

    [Fact]
    public async Task CreateAsync_ReturnsError_WhenDuplicateRecipients()
    {
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory1;
        var recipientId = TestIds.ResidentUser;

        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId, recipientId },
            null,
            CancellationToken.None);

        Assert.False(created);
        Assert.Equal("Recipients must be unique.", error);
        Assert.Null(request);
    }

    [Fact]
    public async Task CreateAsync_ReturnsError_WhenRequesterIsRecipient()
    {
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory1;

        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { requesterId },
            null,
            CancellationToken.None);

        Assert.False(created);
        Assert.Equal("Requester cannot be a recipient.", error);
        Assert.Null(request);
    }

    [Fact]
    public async Task CreateAsync_ReturnsError_WhenRequesterIsAlreadyConfirmedResident()
    {
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);
        var requesterId = TestIds.ResidentUser; // Já é resident confirmado
        var territoryId = TestIds.Territory2; // Territory2 tem membership pré-populado
        var recipientId = TestIds.CuratorUser;

        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId },
            "Please approve my request",
            CancellationToken.None);

        Assert.False(created);
        Assert.Equal("Requester is already a confirmed resident.", error);
        Assert.Null(request);
    }

    [Fact]
    public async Task CreateAsync_ReturnsError_WhenRecipientNotFound()
    {
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory1;
        var nonExistentRecipientId = Guid.NewGuid();

        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { nonExistentRecipientId },
            null,
            CancellationToken.None);

        Assert.False(created);
        Assert.Equal("Recipient not found.", error);
        Assert.Null(request);
    }

    [Fact]
    public async Task CreateAsync_ReturnsError_WhenRecipientIsNotResidentOrCurator()
    {
        var dataStore = new InMemoryDataStore();
        await EnsureTestUserExists(dataStore, TestIds.TestUserId1, "Test User 1", "111.111.111-11");
        await EnsureTestUserExists(dataStore, TestIds.TestUserId3, "Visitor", "999.999.999-99");
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory2; // Territory2 tem ResidentUser como resident (para usar como recipient válido em outros testes)
        var visitorId = TestIds.TestUserId3; // Este não é resident nem curator

        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { visitorId },
            null,
            CancellationToken.None);

        Assert.False(created);
        Assert.Equal("Recipient is not a confirmed resident or curator.", error);
        Assert.Null(request);
    }

    [Fact]
    public async Task CreateAsync_ReturnsExisting_WhenPendingRequestExists()
    {
        var dataStore = new InMemoryDataStore();
        await EnsureTestUserExists(dataStore, TestIds.TestUserId1, "Test User 1", "111.111.111-11");
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory2; // Territory2 tem ResidentUser como resident
        var recipientId = TestIds.ResidentUser;

        // Criar primeiro request
        var (created1, error1, request1) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId },
            "First request",
            CancellationToken.None);
        Assert.True(created1);

        // Tentar criar segundo request (deve retornar o existente)
        var (created2, error2, request2) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId },
            "Second request",
            CancellationToken.None);

        Assert.False(created2);
        Assert.Null(error2);
        Assert.NotNull(request2);
        Assert.Equal(request1!.Id, request2!.Id);
    }

    [Fact]
    public async Task CreateAsync_CreatesRequest_WhenValid()
    {
        var dataStore = new InMemoryDataStore();
        await EnsureTestUserExists(dataStore, TestIds.TestUserId1, "Test User 1", "111.111.111-11");
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory2; // Territory2 tem ResidentUser como resident
        var recipientId = TestIds.ResidentUser;

        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId },
            "Please approve my request",
            CancellationToken.None);

        Assert.True(created);
        Assert.Null(error);
        Assert.NotNull(request);
        Assert.Equal(requesterId, request!.RequesterUserId);
        Assert.Equal(territoryId, request.TerritoryId);
        Assert.Equal("Please approve my request", request.Message);
        Assert.Equal(TerritoryJoinRequestStatus.Pending, request.Status);
    }

    [Fact]
    public async Task ApproveAsync_ReturnsError_WhenRequestNotFound()
    {
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);
        var nonExistentRequestId = Guid.NewGuid();
        var actorId = TestIds.ResidentUser; // Este usuário já existe no dataStore

        var result = await service.ApproveAsync(
            nonExistentRequestId,
            actorId,
            false,
            CancellationToken.None);

        Assert.False(result.Found);
        Assert.False(result.Forbidden);
        Assert.Null(result.Request);
        Assert.False(result.Updated);
    }

    [Fact]
    public async Task ApproveAsync_ReturnsUnauthorized_WhenActorIsNotRecipientOrCurator()
    {
        var dataStore = new InMemoryDataStore();
        await EnsureTestUserExists(dataStore, TestIds.TestUserId1, "Test User 1", "111.111.111-11");
        await EnsureTestUserExists(dataStore, TestIds.TestUserId3, "Test User 3", "333.333.333-33");
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory2; // Territory2 tem ResidentUser como resident
        var recipientId = TestIds.ResidentUser;
        var unauthorizedActorId = TestIds.TestUserId3;

        // Criar request
        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId },
            "Test request",
            CancellationToken.None);
        Assert.True(created);

        // Tentar aprovar com usuário não autorizado
        var result = await service.ApproveAsync(
            request!.Id,
            unauthorizedActorId,
            false,
            CancellationToken.None);

        Assert.True(result.Found);
        Assert.True(result.Forbidden); // Forbidden = true significa que NÃO está autorizado
        Assert.NotNull(result.Request);
        Assert.False(result.Updated);
    }

    [Fact]
    public async Task ApproveAsync_ReturnsError_WhenRequestNotPending()
    {
        var dataStore = new InMemoryDataStore();
        await EnsureTestUserExists(dataStore, TestIds.TestUserId1, "Test User 1", "111.111.111-11");
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory2; // Territory2 tem ResidentUser como resident
        var recipientId = TestIds.ResidentUser;

        // Criar e aprovar request
        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId },
            "Test request",
            CancellationToken.None);
        Assert.True(created);
        Assert.NotNull(request);
        var requestId = request!.Id;

        // Verificar que o request foi criado corretamente
        var verifyRequest = await service.GetByIdAsync(requestId, CancellationToken.None);
        Assert.NotNull(verifyRequest);
        Assert.Equal(TerritoryJoinRequestStatus.Pending, verifyRequest!.Status);

        var approveResult1 = await service.ApproveAsync(requestId, recipientId, false, CancellationToken.None);
        Assert.True(approveResult1.Found, $"Request {requestId} should be found. Result: Found={approveResult1.Found}, Forbidden={approveResult1.Forbidden}, Updated={approveResult1.Updated}");
        Assert.True(approveResult1.Updated);

        // Tentar aprovar novamente (request não está mais Pending)
        var result = await service.ApproveAsync(
            request.Id,
            recipientId,
            false,
            CancellationToken.None);

        Assert.True(result.Found);
        Assert.False(result.Forbidden);
        Assert.NotNull(result.Request);
        Assert.False(result.Updated);
        Assert.NotEqual(TerritoryJoinRequestStatus.Pending, result.Request!.Status);
    }

    [Fact]
    public async Task RejectAsync_RejectsRequest_WhenValid()
    {
        var dataStore = new InMemoryDataStore();
        await EnsureTestUserExists(dataStore, TestIds.TestUserId1, "Test User 1", "111.111.111-11");
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory2; // Territory2 tem ResidentUser como resident
        var recipientId = TestIds.ResidentUser;

        // Criar request
        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId },
            "Test request",
            CancellationToken.None);
        Assert.True(created);

        // Rejeitar
        var result = await service.RejectAsync(
            request!.Id,
            recipientId,
            false,
            CancellationToken.None);

        Assert.True(result.Found);
        Assert.False(result.Forbidden);
        Assert.NotNull(result.Request);
        Assert.True(result.Updated);
        Assert.Equal(TerritoryJoinRequestStatus.Rejected, result.Request.Status);
    }

    [Fact]
    public async Task CancelAsync_ReturnsError_WhenRequesterIsNotOwner()
    {
        var dataStore = new InMemoryDataStore();
        await EnsureTestUserExists(dataStore, TestIds.TestUserId1, "Test User 1", "111.111.111-11");
        await EnsureTestUserExists(dataStore, TestIds.TestUserId3, "Test User 3", "333.333.333-33");
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory2; // Territory2 tem ResidentUser como resident
        var recipientId = TestIds.ResidentUser;
        var otherUserId = TestIds.TestUserId3;

        // Criar request
        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId },
            "Test request",
            CancellationToken.None);
        Assert.True(created);

        // Tentar cancelar com outro usuário
        var result = await service.CancelAsync(
            request!.Id,
            otherUserId,
            CancellationToken.None);

        Assert.True(result.Found);
        Assert.True(result.Forbidden); // Forbidden = true significa que NÃO está autorizado
        Assert.NotNull(result.Request);
        Assert.False(result.Updated);
    }

    [Fact]
    public async Task CancelAsync_CancelsRequest_WhenValid()
    {
        var dataStore = new InMemoryDataStore();
        await EnsureTestUserExists(dataStore, TestIds.TestUserId1, "Test User 1", "111.111.111-11");
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory2; // Territory2 tem ResidentUser como resident
        var recipientId = TestIds.ResidentUser;

        // Criar request
        var (created, error, request) = await service.CreateAsync(
            requesterId,
            territoryId,
            new[] { recipientId },
            "Test request",
            CancellationToken.None);
        Assert.True(created);

        // Cancelar
        var result = await service.CancelAsync(
            request!.Id,
            requesterId,
            CancellationToken.None);

        Assert.True(result.Found);
        Assert.False(result.Forbidden);
        Assert.NotNull(result.Request);
        Assert.True(result.Updated);
        Assert.Equal(TerritoryJoinRequestStatus.Canceled, result.Request.Status);
    }

    [Fact]
    public async Task ListIncomingPagedAsync_ReturnsPagedResults()
    {
        var dataStore = new InMemoryDataStore();
        await EnsureTestUserExists(dataStore, TestIds.TestUserId1, "Test User 1", "111.111.111-11");
        await EnsureTestUserExists(dataStore, TestIds.TestUserId3, "Test User 3", "333.333.333-33");
        var service = CreateService(dataStore);
        var requesterId = TestIds.TestUserId1;
        var territoryId = TestIds.Territory2; // Territory2 tem ResidentUser como resident
        var recipientId = TestIds.ResidentUser;

        // Criar alguns requests
        await service.CreateAsync(requesterId, territoryId, new[] { recipientId }, "Request 1", CancellationToken.None);
        await service.CreateAsync(TestIds.TestUserId3, territoryId, new[] { recipientId }, "Request 2", CancellationToken.None);

        var pagination = new PaginationParameters(1, 10);
        var result = await service.ListIncomingPagedAsync(
            recipientId,
            pagination,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.TotalCount >= 2);
        Assert.True(result.Items.Count <= 10);
    }
}
