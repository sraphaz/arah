using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Governance;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for VotingService,
/// focusing on validation (empty title, description, options), GetResults not found, List empty.
/// </summary>
public sealed class VotingServiceEdgeCasesTests
{
    [Fact]
    public async Task CreateVotingAsync_WithEmptyTitle_ReturnsFailure()
    {
        var (service, territoryId, userId) = await CreateServiceAndSetupAsync();

        var result = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "",
            "Description",
            new[] { "A", "B" },
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Title", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateVotingAsync_WithEmptyDescription_ReturnsFailure()
    {
        var (service, territoryId, userId) = await CreateServiceAndSetupAsync();

        var result = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "Title",
            "   ",
            new[] { "A", "B" },
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Description", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateVotingAsync_WithNullOptions_ReturnsFailure()
    {
        var (service, territoryId, userId) = await CreateServiceAndSetupAsync();

        var result = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "Title",
            "Desc",
            null!,
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("options", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateVotingAsync_WithSingleOption_ReturnsFailure()
    {
        var (service, territoryId, userId) = await CreateServiceAndSetupAsync();

        var result = await service.CreateVotingAsync(
            territoryId,
            userId,
            VotingType.ThemePrioritization,
            "Title",
            "Desc",
            new[] { "Only" },
            VotingVisibility.AllMembers,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("2", result.Error ?? "");
    }

    [Fact]
    public async Task GetResultsAsync_WhenVotingNotFound_ReturnsFailure()
    {
        var (service, _, _) = await CreateServiceAndSetupAsync();

        var result = await service.GetResultsAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListVotingsAsync_WhenTerritoryHasNoVotings_ReturnsEmpty()
    {
        var (service, territoryId, _) = await CreateServiceAndSetupAsync();
        var emptyTerritoryId = Guid.NewGuid();

        var list = await service.ListVotingsAsync(emptyTerritoryId, null, null, CancellationToken.None);

        Assert.NotNull(list);
        Assert.Empty(list);
    }

    [Fact]
    public async Task GetVotingAsync_WhenNotFound_ReturnsFailure()
    {
        var (service, _, _) = await CreateServiceAndSetupAsync();

        var result = await service.GetVotingAsync(Guid.NewGuid(), null, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<(VotingService Service, Guid TerritoryId, Guid UserId)> CreateServiceAndSetupAsync()
    {
        var dataStore = new InMemoryDataStore();
        var votingRepo = new InMemoryVotingRepository(dataStore);
        var voteRepo = new InMemoryVoteRepository(dataStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(dataStore);
        var accessEvaluator = CreateAccessEvaluator(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        var territoryId = dataStore.Territories[1].Id;
        var userId = dataStore.Users[0].Id;

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

        return (service, territoryId, userId);
    }

    private static AccessEvaluator CreateAccessEvaluator(InMemoryDataStore dataStore)
    {
        var membershipRepo = new InMemoryTerritoryMembershipRepository(dataStore);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(dataStore);
        var permissionRepo = new InMemorySystemPermissionRepository(dataStore);
        var settingsRepo = new InMemoryMembershipSettingsRepository(dataStore);
        var userRepo = new InMemoryUserRepository(dataStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var accessRules = new MembershipAccessRules(
            membershipRepo,
            settingsRepo,
            userRepo,
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
