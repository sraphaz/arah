using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class PushNotificationServiceTests
{
    private readonly Mock<IUserDeviceRepository> _deviceRepositoryMock;
    private readonly Mock<IPushNotificationProvider> _pushProviderMock;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<PushNotificationService>> _loggerMock;
    private readonly PushNotificationService _service;

    public PushNotificationServiceTests()
    {
        _deviceRepositoryMock = new Mock<IUserDeviceRepository>();
        _pushProviderMock = new Mock<IPushNotificationProvider>();
        _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<PushNotificationService>>();
        _service = new PushNotificationService(
            _deviceRepositoryMock.Object,
            _pushProviderMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterDeviceAsync_WhenTokenIsEmpty_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _service.RegisterDeviceAsync(userId, "", "IOS", null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("required", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegisterDeviceAsync_WhenPlatformIsEmpty_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _service.RegisterDeviceAsync(userId, "token123", "", null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("required", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegisterDeviceAsync_WhenDeviceExists_UpdatesExisting()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var existingDevice = new UserDevice(
            deviceId,
            userId,
            "token123",
            "IOS",
            "iPhone",
            DateTime.UtcNow.AddDays(-5));

        _deviceRepositoryMock.Setup(r => r.GetByTokenAsync("token123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingDevice);

        // Act
        var result = await _service.RegisterDeviceAsync(userId, "token123", "IOS", "iPhone Updated", CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(deviceId, result.Value.Id);
        _deviceRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserDevice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterDeviceAsync_WhenDeviceDoesNotExist_CreatesNew()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _deviceRepositoryMock.Setup(r => r.GetByTokenAsync("token123", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDevice?)null);

        // Act
        var result = await _service.RegisterDeviceAsync(userId, "token123", "ANDROID", "Pixel 5", CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal("token123", result.Value.DeviceToken);
        Assert.Equal("ANDROID", result.Value.Platform);
        Assert.Equal("Pixel 5", result.Value.DeviceName);
        _deviceRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserDevice>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListDevicesAsync_ReturnsActiveDevices()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var devices = new List<UserDevice>
        {
            new UserDevice(Guid.NewGuid(), userId, "token1", "IOS", "iPhone", DateTime.UtcNow),
            new UserDevice(Guid.NewGuid(), userId, "token2", "ANDROID", "Pixel", DateTime.UtcNow)
        };

        _deviceRepositoryMock.Setup(r => r.ListActiveByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);

        // Act
        var result = await _service.ListDevicesAsync(userId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task GetDeviceAsync_WhenDeviceNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        _deviceRepositoryMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDevice?)null);

        // Act
        var result = await _service.GetDeviceAsync(userId, deviceId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetDeviceAsync_WhenDeviceBelongsToDifferentUser_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var device = new UserDevice(deviceId, otherUserId, "token", "IOS", null, DateTime.UtcNow);

        _deviceRepositoryMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        // Act
        var result = await _service.GetDeviceAsync(userId, deviceId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("belong", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetDeviceAsync_WhenValid_ReturnsDevice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var device = new UserDevice(deviceId, userId, "token", "IOS", "iPhone", DateTime.UtcNow);

        _deviceRepositoryMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        // Act
        var result = await _service.GetDeviceAsync(userId, deviceId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(deviceId, result.Value.Id);
        Assert.Equal(userId, result.Value.UserId);
    }

    [Fact]
    public async Task UnregisterDeviceAsync_WhenDeviceNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        _deviceRepositoryMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDevice?)null);

        // Act
        var result = await _service.UnregisterDeviceAsync(userId, deviceId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UnregisterDeviceAsync_WhenValid_DeletesDevice()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var device = new UserDevice(deviceId, userId, "token", "IOS", null, DateTime.UtcNow);

        _deviceRepositoryMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        // Act
        var result = await _service.UnregisterDeviceAsync(userId, deviceId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _deviceRepositoryMock.Verify(r => r.DeleteAsync(deviceId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendToUserAsync_WhenProviderNotConfigured_ReturnsZero()
    {
        // Arrange
        var serviceWithoutProvider = new PushNotificationService(
            _deviceRepositoryMock.Object,
            null,
            _loggerMock.Object);
        var userId = Guid.NewGuid();

        // Act
        var result = await serviceWithoutProvider.SendToUserAsync(userId, "Title", "Body", null, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public async Task SendToUserAsync_WhenNoDevices_ReturnsZero()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _deviceRepositoryMock.Setup(r => r.ListActiveByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<UserDevice>());

        // Act
        var result = await _service.SendToUserAsync(userId, "Title", "Body", null, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public async Task SendToUserAsync_WhenValid_SendsNotification()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var devices = new List<UserDevice>
        {
            new UserDevice(Guid.NewGuid(), userId, "token1", "IOS", null, DateTime.UtcNow)
        };

        _deviceRepositoryMock.Setup(r => r.ListActiveByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);
        _pushProviderMock.Setup(p => p.SendBatchAsync(
            It.IsAny<IReadOnlyCollection<(string DeviceToken, string Platform)>>(),
            "Title",
            "Body",
            null,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _service.SendToUserAsync(userId, "Title", "Body", null, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
        _pushProviderMock.Verify(p => p.SendBatchAsync(
            It.IsAny<IReadOnlyCollection<(string DeviceToken, string Platform)>>(),
            "Title",
            "Body",
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
