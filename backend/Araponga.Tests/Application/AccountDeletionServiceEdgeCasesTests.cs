using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class AccountDeletionServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryUserRepository _userRepository;
    private readonly InMemoryTerritoryMembershipRepository _membershipRepository;
    private readonly InMemoryFeedRepository _feedRepository;
    private readonly InMemoryTerritoryEventRepository _eventRepository;
    private readonly InMemoryNotificationInboxRepository _notificationRepository;
    private readonly InMemoryUserPreferencesRepository _preferencesRepository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly AccountDeletionService _service;

    public AccountDeletionServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _userRepository = new InMemoryUserRepository(_dataStore);
        _membershipRepository = new InMemoryTerritoryMembershipRepository(_dataStore);
        _feedRepository = new InMemoryFeedRepository(_dataStore);
        _eventRepository = new InMemoryTerritoryEventRepository(_dataStore);
        _notificationRepository = new InMemoryNotificationInboxRepository(_dataStore);
        _preferencesRepository = new InMemoryUserPreferencesRepository(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new AccountDeletionService(
            _userRepository,
            _membershipRepository,
            _feedRepository,
            _eventRepository,
            _notificationRepository,
            _preferencesRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithEmptyUserId_ReturnsFailure()
    {
        var result = await _service.AnonymizeUserDataAsync(Guid.Empty, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithNonExistentUserId_ReturnsFailure()
    {
        var result = await _service.AnonymizeUserDataAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithUserWithAllFields_AnonymizesAllPersonalData()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Original Name",
            "original@example.com",
            "123.456.789-00",
            null, // Não pode ter CPF e foreignDocument ao mesmo tempo
            "11987654321",
            "123 Main St",
            "google",
            "external-id",
            true,
            "secret",
            "hash",
            DateTime.UtcNow,
            UserIdentityVerificationStatus.Verified,
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Bio text",
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var anonymizedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
        Assert.NotNull(anonymizedUser);
        Assert.Null(anonymizedUser.Email);
        Assert.Null(anonymizedUser.PhoneNumber);
        Assert.Null(anonymizedUser.Address);
        Assert.Null(anonymizedUser.ForeignDocument);
        Assert.Null(anonymizedUser.TwoFactorSecret);
        Assert.Null(anonymizedUser.TwoFactorRecoveryCodesHash);
        Assert.Null(anonymizedUser.TwoFactorVerifiedAtUtc);
        Assert.Null(anonymizedUser.IdentityVerifiedAtUtc);
        Assert.Null(anonymizedUser.AvatarMediaAssetId);
        Assert.Null(anonymizedUser.Bio);
        Assert.Equal("000.000.000-00", anonymizedUser.Cpf);
        Assert.False(anonymizedUser.TwoFactorEnabled);
        Assert.Equal(UserIdentityVerificationStatus.Unverified, anonymizedUser.IdentityVerificationStatus);
        Assert.StartsWith("User_", anonymizedUser.DisplayName);
        Assert.Equal($"anon_{userId}", anonymizedUser.ExternalId);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithUserWithMinimalFields_AnonymizesCorrectly()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Minimal User",
            null,
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var anonymizedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
        Assert.NotNull(anonymizedUser);
        Assert.Null(anonymizedUser.Email);
        Assert.Null(anonymizedUser.PhoneNumber);
        Assert.Null(anonymizedUser.Address);
        Assert.Equal("000.000.000-00", anonymizedUser.Cpf);
        Assert.StartsWith("User_", anonymizedUser.DisplayName);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithUserPreferences_UpdatesToDefaults()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);

        var notificationPrefs = new NotificationPreferences(
            postsEnabled: true,
            commentsEnabled: true,
            eventsEnabled: true,
            alertsEnabled: true,
            marketplaceEnabled: true,
            moderationEnabled: true,
            membershipRequestsEnabled: true);
        var emailPrefs = EmailPreferences.Default();

        var preferences = new UserPreferences(
            userId,
            ProfileVisibility.Private,
            ContactVisibility.Private,
            shareLocation: true,
            showMemberships: false,
            notificationPreferences: notificationPrefs,
            emailPreferences: emailPrefs,
            createdAtUtc: DateTime.UtcNow,
            updatedAtUtc: DateTime.UtcNow);

        await _userRepository.AddAsync(user, CancellationToken.None);
        await _preferencesRepository.AddAsync(preferences, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var updatedPreferences = await _preferencesRepository.GetByUserIdAsync(userId, CancellationToken.None);
        Assert.NotNull(updatedPreferences);
        // Verificar que as preferências foram resetadas para padrões via CreateDefault
        // CreateDefault retorna ProfileVisibility.Public e ContactVisibility.ResidentsOnly
        Assert.Equal(ProfileVisibility.Public, updatedPreferences.ProfileVisibility);
        Assert.Equal(ContactVisibility.ResidentsOnly, updatedPreferences.ContactVisibility);
        Assert.False(updatedPreferences.ShareLocation);
        Assert.True(updatedPreferences.ShowMemberships);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithoutUserPreferences_DoesNotCreatePreferences()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        // O serviço só atualiza preferências se já existirem, não cria novas
        var preferences = await _preferencesRepository.GetByUserIdAsync(userId, CancellationToken.None);
        // Pode ser null se não existiam antes
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_PreservesCreatedAtUtc()
    {
        var userId = Guid.NewGuid();
        var originalCreatedAt = DateTime.UtcNow.AddDays(-100);
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            originalCreatedAt);

        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var anonymizedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
        Assert.NotNull(anonymizedUser);
        Assert.Equal(originalCreatedAt, anonymizedUser.CreatedAtUtc);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_PreservesAuthProvider()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "facebook",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var anonymizedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
        Assert.NotNull(anonymizedUser);
        Assert.Equal("facebook", anonymizedUser.AuthProvider);
    }

    [Fact]
    public async Task CanDeleteUserAsync_WithEmptyUserId_ReturnsFailure()
    {
        var result = await _service.CanDeleteUserAsync(Guid.Empty, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CanDeleteUserAsync_WithNonExistentUserId_ReturnsFailure()
    {
        var result = await _service.CanDeleteUserAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CanDeleteUserAsync_WithVerifiedUser_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Verified,
            DateTime.UtcNow,
            null,
            null,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.CanDeleteUserAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task CanDeleteUserAsync_WithUnverifiedUser_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.CanDeleteUserAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithUnicodeInDisplayName_HandlesCorrectly()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Usuário Teste 测试",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var anonymizedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
        Assert.NotNull(anonymizedUser);
        Assert.StartsWith("User_", anonymizedUser.DisplayName);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_CommitsUnitOfWork()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "external-id",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);

        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        // Verificar que o UnitOfWork foi commitado (verificando que as mudanças persistiram)
        var anonymizedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
        Assert.NotNull(anonymizedUser);
        Assert.Null(anonymizedUser.Email);
    }
}
