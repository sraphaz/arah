using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Governance;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class VotingServiceTests
{
    [Fact]
    public async Task CreateVotingAsync_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var votingRepo = new InMemoryVotingRepository(dataStore);
        var voteRepo = new InMemoryVoteRepository(dataStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(dataStore);
        var accessEvaluator = CreateAccessEvaluator(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        // Usar territoryB e residentUser do InMemoryDataStore
        var territoryId = dataStore.Territories[1].Id; // territoryB
        var userId = dataStore.Users[0].Id; // residentUser

        // Criar membership explicitamente para garantir que existe
        var newMembership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepo.AddAsync(newMembership, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Verificar que o membership foi criado
        var membership = await membershipRepo.GetByUserAndTerritoryAsync(userId, territoryId, CancellationToken.None);
        Assert.NotNull(membership);
        Assert.Equal(MembershipRole.Resident, membership.Role);

        // Act
        var result = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "Priorizar temas",
            "Qual tema deve ter prioridade?",
            new[] { "Meio Ambiente", "Eventos" },
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        // Assert
        if (!result.IsSuccess)
        {
            var errorMsg = result.Error ?? "Unknown error";
            Assert.Fail($"Expected success but got failure: {errorMsg}");
        }
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task VoteAsync_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var votingRepo = new InMemoryVotingRepository(dataStore);
        var voteRepo = new InMemoryVoteRepository(dataStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(dataStore);
        var accessEvaluator = CreateAccessEvaluator(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        // Usar territoryB e residentUser do InMemoryDataStore
        var territoryId = dataStore.Territories[1].Id; // territoryB
        var userId = dataStore.Users[0].Id; // residentUser

        // Criar membership explicitamente para garantir que existe
        var newMembership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepo.AddAsync(newMembership, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Verificar que o membership foi criado
        var membership = await membershipRepo.GetByUserAndTerritoryAsync(userId, territoryId, CancellationToken.None);
        Assert.NotNull(membership);
        Assert.Equal(MembershipRole.Resident, membership.Role);

        var votingResult = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "Test",
            "Test",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        Assert.True(votingResult.IsSuccess);
        var voting = votingResult.Value!;

        // Abrir votação e salvar
        voting.Open();
        await votingRepo.UpdateAsync(voting, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Act
        var result = await service.VoteAsync(
            voting.Id,
            userId,
            "Option1",
            CancellationToken.None);

        // Assert
        if (!result.IsSuccess)
        {
            var errorMsg = result.Error ?? "Unknown error";
            Assert.Fail($"Expected success but got failure: {errorMsg}");
        }
    }

    private static AccessEvaluator CreateAccessEvaluator(InMemoryDataStore dataStore)
    {
        var membershipRepo = new InMemoryTerritoryMembershipRepository(dataStore);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(dataStore);
        var permissionRepo = new InMemorySystemPermissionRepository(dataStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var accessRules = new MembershipAccessRules(
            membershipRepo,
            settingsRepository,
            userRepository,
            featureFlags);
        var cache = CacheTestHelper.CreateDistributedCacheService();
        return new AccessEvaluator(
            membershipRepo,
            capabilityRepo,
            permissionRepo,
            accessRules,
            cache);
    }
}
