using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for AccountDeletionService,
/// focusing on null/empty inputs, non-existent users, and anonymization edge cases.
/// </summary>
public sealed class AccountDeletionServiceEdgeCasesTests
{
    private readonly InMemorySharedStore _sharedStore;
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
        _sharedStore = new InMemorySharedStore();
        _dataStore = new InMemoryDataStore();
        _userRepository = new InMemoryUserRepository(_sharedStore);
        _membershipRepository = new InMemoryTerritoryMembershipRepository(_sharedStore);
        _feedRepository = new InMemoryFeedRepository(_dataStore);
        _eventRepository = new InMemoryTerritoryEventRepository(_dataStore);
        _notificationRepository = new InMemoryNotificationInboxRepository(_dataStore);
        _preferencesRepository = new InMemoryUserPreferencesRepository(_sharedStore);
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
    public async Task AnonymizeUserDataAsync_WithEmptyGuid_ReturnsFailure()
    {
        var result = await _service.AnonymizeUserDataAsync(Guid.Empty, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "");
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithNonExistentUser_ReturnsFailure()
    {
        var result = await _service.AnonymizeUserDataAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "");
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithExistingUser_AnonymizesData()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            "(11) 90000-0000",
            "Rua Teste",
            "google",
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var anonymizedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
        Assert.NotNull(anonymizedUser);
        Assert.Null(anonymizedUser!.Email);
        Assert.Equal("000.000.000-00", anonymizedUser.Cpf);
        Assert.Null(anonymizedUser.PhoneNumber);
        Assert.Null(anonymizedUser.Address);
        Assert.Contains("User_", anonymizedUser.DisplayName);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithUserWithPreferences_ResetsPreferences()
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
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        var preferences = await _preferencesRepository.GetOrCreateDefaultAsync(userId, CancellationToken.None);
        preferences.UpdatePrivacy(ProfileVisibility.Private, ContactVisibility.Private, false, false, DateTime.UtcNow);
        await _preferencesRepository.UpdateAsync(preferences, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var updatedPreferences = await _preferencesRepository.GetByUserIdAsync(userId, CancellationToken.None);
        Assert.NotNull(updatedPreferences);
        Assert.Equal(ProfileVisibility.Public, updatedPreferences!.ProfileVisibility);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_PreservesCreatedAtUtc()
    {
        var userId = Guid.NewGuid();
        var originalCreatedAt = DateTime.UtcNow.AddYears(-1);
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "test-external-id",
            originalCreatedAt);
        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var anonymizedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
        Assert.NotNull(anonymizedUser);
        Assert.Equal(originalCreatedAt, anonymizedUser!.CreatedAtUtc);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithUnicodeData_HandlesCorrectly()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Usuário com acentuação: café, naïve, 文字",
            "test+café@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var anonymizedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None);
        Assert.NotNull(anonymizedUser);
        Assert.Null(anonymizedUser!.Email);
        Assert.Contains("User_", anonymizedUser.DisplayName);
    }

    [Fact]
    public async Task CanDeleteUserAsync_WithEmptyGuid_ReturnsFailure()
    {
        var result = await _service.CanDeleteUserAsync(Guid.Empty, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "");
    }

    [Fact]
    public async Task CanDeleteUserAsync_WithNonExistentUser_ReturnsFailure()
    {
        var result = await _service.CanDeleteUserAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "");
    }

    [Fact]
    public async Task CanDeleteUserAsync_WithExistingUser_ReturnsSuccess()
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
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.CanDeleteUserAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WithUserWithoutPreferences_HandlesGracefully()
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
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}
