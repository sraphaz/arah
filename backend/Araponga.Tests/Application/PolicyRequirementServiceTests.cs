using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Users;
using Moq;
using System.Text.Json;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class PolicyRequirementServiceTests
{
    private readonly Mock<ITermsOfServiceRepository> _termsRepositoryMock;
    private readonly Mock<IPrivacyPolicyRepository> _privacyRepositoryMock;
    private readonly Mock<ITerritoryMembershipRepository> _membershipRepositoryMock;
    private readonly Mock<IMembershipCapabilityRepository> _capabilityRepositoryMock;
    private readonly Mock<ISystemPermissionRepository> _systemPermissionRepositoryMock;
    private readonly TermsOfServiceService _termsService;
    private readonly PrivacyPolicyService _privacyService;
    private readonly PolicyRequirementService _service;

    public PolicyRequirementServiceTests()
    {
        _termsRepositoryMock = new Mock<ITermsOfServiceRepository>();
        _privacyRepositoryMock = new Mock<IPrivacyPolicyRepository>();
        _membershipRepositoryMock = new Mock<ITerritoryMembershipRepository>();
        _capabilityRepositoryMock = new Mock<IMembershipCapabilityRepository>();
        _systemPermissionRepositoryMock = new Mock<ISystemPermissionRepository>();
        
        _termsService = new TermsOfServiceService(_termsRepositoryMock.Object);
        _privacyService = new PrivacyPolicyService(_privacyRepositoryMock.Object);
        
        _service = new PolicyRequirementService(
            _termsService,
            _privacyService,
            _membershipRepositoryMock.Object,
            _capabilityRepositoryMock.Object,
            _systemPermissionRepositoryMock.Object);
    }

    [Fact]
    public async Task GetRequiredPoliciesForUserAsync_WhenUserHasNoMemberships_ReturnsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _membershipRepositoryMock.Setup(r => r.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TerritoryMembership>());
        _systemPermissionRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<SystemPermission>());
        _termsRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TermsOfService>());
        _privacyRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<PrivacyPolicy>());

        // Act
        var result = await _service.GetRequiredPoliciesForUserAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.RequiredTerms);
        Assert.Empty(result.RequiredPrivacyPolicies);
    }

    [Fact]
    public async Task GetRequiredPoliciesForRoleAsync_ReturnsPoliciesForRole()
    {
        // Arrange
        var role = MembershipRole.Resident;
        var terms = new List<TermsOfService>
        {
            CreateTermsForRole(role)
        };
        var policies = new List<PrivacyPolicy>
        {
            CreatePolicyForRole(role)
        };
        _termsRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(terms);
        _privacyRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(policies);

        // Act
        var result = await _service.GetRequiredPoliciesForRoleAsync(role, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.RequiredTerms);
        Assert.Single(result.RequiredPrivacyPolicies);
    }

    [Fact]
    public async Task GetRequiredPoliciesForCapabilityAsync_ReturnsPoliciesForCapability()
    {
        // Arrange
        var capability = MembershipCapabilityType.Curator;
        var terms = new List<TermsOfService>
        {
            CreateTermsForCapability(capability)
        };
        var policies = new List<PrivacyPolicy>
        {
            CreatePolicyForCapability(capability)
        };
        _termsRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(terms);
        _privacyRepositoryMock.Setup(r => r.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(policies);

        // Act
        var result = await _service.GetRequiredPoliciesForCapabilityAsync(capability, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.RequiredTerms);
        Assert.Single(result.RequiredPrivacyPolicies);
    }

    private static TermsOfService CreateTermsForRole(MembershipRole role)
    {
        var requiredRoles = JsonSerializer.Serialize(new List<int> { (int)role });
        return new TermsOfService(
            Guid.NewGuid(),
            "1.0",
            "Test Terms",
            "Content",
            DateTime.UtcNow.AddDays(-1),
            null,
            true,
            requiredRoles,
            null,
            null,
            DateTime.UtcNow);
    }

    private static TermsOfService CreateTermsForCapability(MembershipCapabilityType capability)
    {
        var requiredCapabilities = JsonSerializer.Serialize(new List<int> { (int)capability });
        return new TermsOfService(
            Guid.NewGuid(),
            "1.0",
            "Test Terms",
            "Content",
            DateTime.UtcNow.AddDays(-1),
            null,
            true,
            null,
            requiredCapabilities,
            null,
            DateTime.UtcNow);
    }

    private static PrivacyPolicy CreatePolicyForRole(MembershipRole role)
    {
        var requiredRoles = JsonSerializer.Serialize(new List<int> { (int)role });
        return new PrivacyPolicy(
            Guid.NewGuid(),
            "1.0",
            "Test Policy",
            "Content",
            DateTime.UtcNow.AddDays(-1),
            null,
            true,
            requiredRoles,
            null,
            null,
            DateTime.UtcNow);
    }

    private static PrivacyPolicy CreatePolicyForCapability(MembershipCapabilityType capability)
    {
        var requiredCapabilities = JsonSerializer.Serialize(new List<int> { (int)capability });
        return new PrivacyPolicy(
            Guid.NewGuid(),
            "1.0",
            "Test Policy",
            "Content",
            DateTime.UtcNow.AddDays(-1),
            null,
            true,
            null,
            requiredCapabilities,
            null,
            DateTime.UtcNow);
    }
}
