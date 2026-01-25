using Araponga.Application.Exceptions;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for UserProfileService,
/// focusing on GetProfile (NotFound), visibility (Public/Private/ResidentsOnly), and permissions.
/// </summary>
public sealed class UserProfileServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryUserRepository _userRepository;
    private readonly InMemoryUserInterestRepository _interestRepository;
    private readonly InMemoryUserPreferencesRepository _preferencesRepository;
    private readonly InMemoryTerritoryMembershipRepository _membershipRepository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly UserProfileService _service;

    public UserProfileServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _userRepository = new InMemoryUserRepository(_dataStore);
        _interestRepository = new InMemoryUserInterestRepository(_dataStore);
        _preferencesRepository = new InMemoryUserPreferencesRepository(_dataStore);
        _membershipRepository = new InMemoryTerritoryMembershipRepository(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new UserProfileService(
            _userRepository,
            _interestRepository,
            _unitOfWork,
            preferencesRepository: _preferencesRepository,
            membershipRepository: _membershipRepository);
    }

    [Fact]
    public async Task GetProfileAsync_WithNonExistentUser_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetProfileAsync(userId, CancellationToken.None));

        Assert.Contains("User", ex.Message);
        Assert.Contains(userId.ToString(), ex.Message);
    }

    [Fact]
    public async Task GetProfileAsync_WithViewer_WhenUserNotFound_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        var viewerId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetProfileAsync(userId, viewerId, CancellationToken.None));

        Assert.Contains("User", ex.Message);
    }

    [Fact]
    public async Task GetProfileAsync_WhenViewerIsSelf_ReturnsUser()
    {
        var user = await CreateAndAddUserAsync("Test User", "a@b.com", "111.111.111-11");

        var profile = await _service.GetProfileAsync(user.Id, user.Id, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Equal(user.Id, profile.Id);
        Assert.Equal("Test User", profile.DisplayName);
    }

    [Fact]
    public async Task GetProfileAsync_WhenProfilePublic_AllowsAnyViewer()
    {
        var user = await CreateAndAddUserAsync("Public User", "p@b.com", "222.222.222-22");
        await SetProfileVisibilityAsync(user.Id, ProfileVisibility.Public);

        var viewerId = Guid.NewGuid();
        var profile = await _service.GetProfileAsync(user.Id, viewerId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Equal(user.Id, profile.Id);
    }

    [Fact]
    public async Task GetProfileAsync_WhenProfilePrivate_ThrowsForbiddenForOtherViewer()
    {
        var user = await CreateAndAddUserAsync("Private User", "pv@b.com", "333.333.333-33");
        await SetProfileVisibilityAsync(user.Id, ProfileVisibility.Private);

        var viewerId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.GetProfileAsync(user.Id, viewerId, CancellationToken.None));

        Assert.Contains("privado", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetProfileAsync_WhenProfilePrivate_AllowsSelf()
    {
        var user = await CreateAndAddUserAsync("Private Self", "ps@b.com", "444.444.444-44");
        await SetProfileVisibilityAsync(user.Id, ProfileVisibility.Private);

        var profile = await _service.GetProfileAsync(user.Id, user.Id, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Equal(user.Id, profile.Id);
    }

    [Fact]
    public async Task GetProfileAsync_WhenNoPreferences_DefaultsToPublic()
    {
        var user = await CreateAndAddUserAsync("No Prefs", "nop@b.com", "555.555.555-55");
        // No preferences set -> default Public

        var viewerId = Guid.NewGuid();
        var profile = await _service.GetProfileAsync(user.Id, viewerId, CancellationToken.None);

        Assert.NotNull(profile);
        Assert.Equal(user.Id, profile.Id);
    }

    private async Task<User> CreateAndAddUserAsync(string displayName, string email, string cpf)
    {
        var id = Guid.NewGuid();
        var user = new User(
            id,
            displayName,
            email,
            cpf,
            null,
            null,
            null,
            "google",
            "ext-" + id.ToString("N")[..8],
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);
        return user;
    }

    private async Task SetProfileVisibilityAsync(Guid userId, ProfileVisibility visibility)
    {
        var prefs = await _preferencesRepository.GetOrCreateDefaultAsync(userId, CancellationToken.None);
        prefs.UpdatePrivacy(visibility, prefs.ContactVisibility, false, false, DateTime.UtcNow);
        await _preferencesRepository.UpdateAsync(prefs, CancellationToken.None);
    }
}
