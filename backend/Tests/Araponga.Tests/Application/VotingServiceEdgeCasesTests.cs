using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Governance;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
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
        var sharedStore = new InMemorySharedStore();
        var votingRepo = new InMemoryVotingRepository(sharedStore);
        var voteRepo = new InMemoryVoteRepository(sharedStore);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var accessEvaluator = CreateAccessEvaluator(sharedStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new VotingService(
            votingRepo, voteRepo, membershipRepo, accessEvaluator, unitOfWork);

        var territoryId = sharedStore.Territories.Count > 1 ? sharedStore.Territories[1].Id : sharedStore.Territories[0].Id;
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

        return (service, territoryId, userId);
    }

    private static AccessEvaluator CreateAccessEvaluator(InMemorySharedStore sharedStore)
    {
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(sharedStore);
        var permissionRepo = new InMemorySystemPermissionRepository(sharedStore);
        var settingsRepo = new InMemoryMembershipSettingsRepository(sharedStore);
        var userRepo = new InMemoryUserRepository(sharedStore);
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
