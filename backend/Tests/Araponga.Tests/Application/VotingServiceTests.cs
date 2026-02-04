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
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        // Usar territoryB e residentUser do InMemoryDataStore
        var territoryId = sharedStore.Territories[1].Id; // territoryB
        var userId = sharedStore.Users[0].Id; // residentUser

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
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        // Usar territoryB e residentUser do InMemoryDataStore
        var territoryId = sharedStore.Territories[1].Id; // territoryB
        var userId = sharedStore.Users[0].Id; // residentUser

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

    [Fact]
    public async Task CloseVotingAsync_WhenCreator_ReturnsSuccess()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        var territoryId = sharedStore.Territories[1].Id;
        var userId = sharedStore.Users[0].Id;

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepo.AddAsync(membership, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        var votingResult = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "Test Voting",
            "Test Description",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        Assert.True(votingResult.IsSuccess);
        var voting = votingResult.Value!;
        voting.Open();
        await votingRepo.UpdateAsync(voting, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Act
        var result = await service.CloseVotingAsync(voting.Id, userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedVoting = await votingRepo.GetByIdAsync(voting.Id, CancellationToken.None);
        Assert.NotNull(updatedVoting);
        Assert.Equal(VotingStatus.Closed, updatedVoting.Status);
    }

    [Fact]
    public async Task GetResultsAsync_WhenVotingHasVotes_ReturnsResults()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        var territoryId = sharedStore.Territories[1].Id;
        var userId = sharedStore.Users[0].Id;

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepo.AddAsync(membership, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        var votingResult = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "Results Test",
            "Test",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        Assert.True(votingResult.IsSuccess);
        var voting = votingResult.Value!;
        voting.Open();
        await votingRepo.UpdateAsync(voting, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Adicionar alguns votos
        await voteRepo.AddAsync(new Vote(Guid.NewGuid(), voting.Id, Guid.NewGuid(), "Option1", DateTime.UtcNow), CancellationToken.None);
        await voteRepo.AddAsync(new Vote(Guid.NewGuid(), voting.Id, Guid.NewGuid(), "Option1", DateTime.UtcNow), CancellationToken.None);
        await voteRepo.AddAsync(new Vote(Guid.NewGuid(), voting.Id, Guid.NewGuid(), "Option2", DateTime.UtcNow), CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Act
        var result = await service.GetResultsAsync(voting.Id, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value["Option1"]);
        Assert.Equal(1, result.Value["Option2"]);
    }

    [Fact]
    public async Task VoteAsync_WhenVotingClosed_ReturnsFailure()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        var territoryId = sharedStore.Territories[1].Id;
        var userId = sharedStore.Users[0].Id;

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepo.AddAsync(membership, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        var votingResult = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "Closed Test",
            "Test",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        Assert.True(votingResult.IsSuccess);
        var voting = votingResult.Value!;
        voting.Open();
        voting.Close(); // Fechar imediatamente
        await votingRepo.UpdateAsync(voting, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Act
        var result = await service.VoteAsync(voting.Id, userId, "Option1", CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not open", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task VoteAsync_WhenInvalidOption_ReturnsFailure()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        var territoryId = sharedStore.Territories[1].Id;
        var userId = sharedStore.Users[0].Id;

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepo.AddAsync(membership, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        var votingResult = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "Invalid Option Test",
            "Test",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        Assert.True(votingResult.IsSuccess);
        var voting = votingResult.Value!;
        voting.Open();
        await votingRepo.UpdateAsync(voting, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Act
        var result = await service.VoteAsync(voting.Id, userId, "InvalidOption", CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not valid", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListVotingsAsync_WhenTerritoryHasVotings_ReturnsList()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        var territoryId = sharedStore.Territories[1].Id;
        var userId = sharedStore.Users[0].Id;

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepo.AddAsync(membership, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Criar duas votações
        var voting1 = await service.CreateVotingAsync(
            territoryId, userId, VotingType.ThemePrioritization,
            "Voting 1", "Test", new[] { "A", "B" },
            VotingVisibility.AllMembers, null, null, CancellationToken.None);
        Assert.True(voting1.IsSuccess);

        var voting2 = await service.CreateVotingAsync(
            territoryId, userId, VotingType.ThemePrioritization,
            "Voting 2", "Test", new[] { "C", "D" },
            VotingVisibility.AllMembers, null, null, CancellationToken.None);
        Assert.True(voting2.IsSuccess);

        // Act
        var votings = await service.ListVotingsAsync(territoryId, null, null, CancellationToken.None);

        // Assert
        Assert.True(votings.Count >= 2);
    }

    [Fact]
    public async Task GetVotingAsync_WhenExists_ReturnsVoting()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        var territoryId = sharedStore.Territories[1].Id;
        var userId = sharedStore.Users[0].Id;

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepo.AddAsync(membership, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        var votingResult = await service.CreateVotingAsync(
            territoryId, userId, VotingType.ThemePrioritization,
            "Get Test", "Test", new[] { "A", "B" },
            VotingVisibility.AllMembers, null, null, CancellationToken.None);
        Assert.True(votingResult.IsSuccess);
        var votingId = votingResult.Value!.Id;

        // Act
        var result = await service.GetVotingAsync(votingId, userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(votingId, result.Value.Id);
    }

    [Fact]
    public async Task GetVotingAsync_WhenNotExists_ReturnsFailure()
    {
        // Arrange
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        // Act
        var result = await service.GetVotingAsync(Guid.NewGuid(), null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    private static AccessEvaluator CreateAccessEvaluator(InMemorySharedStore sharedStore)
    {
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(sharedStore);
        var permissionRepo = new InMemorySystemPermissionRepository(sharedStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(sharedStore);
        var userRepository = new InMemoryUserRepository(sharedStore);
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
