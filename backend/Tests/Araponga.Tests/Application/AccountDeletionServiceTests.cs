using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class AccountDeletionServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITerritoryMembershipRepository> _membershipRepositoryMock;
    private readonly Mock<IFeedRepository> _feedRepositoryMock;
    private readonly Mock<ITerritoryEventRepository> _eventRepositoryMock;
    private readonly Mock<INotificationInboxRepository> _notificationRepositoryMock;
    private readonly Mock<IUserPreferencesRepository> _preferencesRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AccountDeletionService _service;

    public AccountDeletionServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _membershipRepositoryMock = new Mock<ITerritoryMembershipRepository>();
        _feedRepositoryMock = new Mock<IFeedRepository>();
        _eventRepositoryMock = new Mock<ITerritoryEventRepository>();
        _notificationRepositoryMock = new Mock<INotificationInboxRepository>();
        _preferencesRepositoryMock = new Mock<IUserPreferencesRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new AccountDeletionService(
            _userRepositoryMock.Object,
            _membershipRepositoryMock.Object,
            _feedRepositoryMock.Object,
            _eventRepositoryMock.Object,
            _notificationRepositoryMock.Object,
            _preferencesRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WhenUserNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AnonymizeUserDataAsync_WhenUserExists_AnonymizesPersonalData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            "1234567890",
            "Test Address",
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
            null,
            DateTime.UtcNow);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _preferencesRepositoryMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreferences?)null);

        // Act
        var result = await _service.AnonymizeUserDataAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u =>
            u.Id == userId &&
            u.Email == null &&
            u.PhoneNumber == null &&
            u.Address == null &&
            u.DisplayName.StartsWith("User_") &&
            u.Cpf == "000.000.000-00"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CanDeleteUserAsync_WhenUserNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.CanDeleteUserAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CanDeleteUserAsync_WhenUserExists_ReturnsSuccess()
    {
        // Arrange
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
            null,
            DateTime.UtcNow);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.CanDeleteUserAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
}
