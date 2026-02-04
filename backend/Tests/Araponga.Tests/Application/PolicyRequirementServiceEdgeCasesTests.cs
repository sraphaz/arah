using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for PolicyRequirementService (GetRequiredPoliciesForUser empty, GetRequiredPoliciesForRole).
/// </summary>
public sealed class PolicyRequirementServiceEdgeCasesTests
{
    [Fact]
    public async Task GetRequiredPoliciesForUserAsync_WhenUserHasNoMemberships_ReturnsEmpty()
    {
        var sharedStore = new InMemorySharedStore();
        var termsRepo = new InMemoryTermsOfServiceRepository(sharedStore);
        var privacyRepo = new InMemoryPrivacyPolicyRepository(sharedStore);
        var termsService = new TermsOfServiceService(termsRepo);
        var privacyService = new PrivacyPolicyService(privacyRepo);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(sharedStore);
        var permissionRepo = new InMemorySystemPermissionRepository(sharedStore);
        var svc = new PolicyRequirementService(
            termsService,
            privacyService,
            membershipRepo,
            capabilityRepo,
            permissionRepo);
        var userId = Guid.NewGuid();

        var result = await svc.GetRequiredPoliciesForUserAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.RequiredTerms);
        Assert.NotNull(result.RequiredPrivacyPolicies);
        Assert.Empty(result.RequiredTerms);
        Assert.Empty(result.RequiredPrivacyPolicies);
    }

    [Fact]
    public async Task GetRequiredPoliciesForRoleAsync_ReturnsResult()
    {
        var sharedStore = new InMemorySharedStore();
        var termsRepo = new InMemoryTermsOfServiceRepository(sharedStore);
        var privacyRepo = new InMemoryPrivacyPolicyRepository(sharedStore);
        var termsService = new TermsOfServiceService(termsRepo);
        var privacyService = new PrivacyPolicyService(privacyRepo);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(sharedStore);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(sharedStore);
        var permissionRepo = new InMemorySystemPermissionRepository(sharedStore);
        var svc = new PolicyRequirementService(
            termsService,
            privacyService,
            membershipRepo,
            capabilityRepo,
            permissionRepo);

        var result = await svc.GetRequiredPoliciesForRoleAsync(MembershipRole.Visitor, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.RequiredTerms);
        Assert.NotNull(result.RequiredPrivacyPolicies);
    }
}
