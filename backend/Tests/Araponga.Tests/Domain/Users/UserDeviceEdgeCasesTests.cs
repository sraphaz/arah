using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain.Users;

public sealed class UserDeviceEdgeCasesTests
{
    [Fact]
    public void UserDevice_WithNullDeviceToken_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new UserDevice(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null!,
                "ios",
                "iPhone",
                DateTime.UtcNow));

        Assert.Contains("Device token", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserDevice_WithEmptyDeviceToken_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new UserDevice(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "",
                "ios",
                "iPhone",
                DateTime.UtcNow));

        Assert.Contains("Device token", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserDevice_WithWhitespaceDeviceToken_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new UserDevice(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "   ",
                "ios",
                "iPhone",
                DateTime.UtcNow));

        Assert.Contains("Device token", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserDevice_WithNullPlatform_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new UserDevice(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "token123",
                null!,
                "iPhone",
                DateTime.UtcNow));

        Assert.Contains("Platform", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserDevice_WithEmptyPlatform_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new UserDevice(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "token123",
                "",
                "iPhone",
                DateTime.UtcNow));

        Assert.Contains("Platform", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserDevice_WithNullDeviceName_DoesNotThrow()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            null,
            DateTime.UtcNow);

        Assert.Null(device.DeviceName);
    }

    [Fact]
    public void UserDevice_InitializesAsActive()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone",
            DateTime.UtcNow);

        Assert.True(device.IsActive);
    }

    [Fact]
    public void UserDevice_InitializesLastUsedAtUtc_ToRegisteredAtUtc()
    {
        var registeredAt = DateTime.UtcNow;
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone",
            registeredAt);

        Assert.NotNull(device.LastUsedAtUtc);
        Assert.Equal(registeredAt, device.LastUsedAtUtc.Value, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UserDevice_UpdateToken_WithNullToken_ThrowsArgumentException()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone",
            DateTime.UtcNow);

        var ex = Assert.Throws<ArgumentException>(() => device.UpdateToken(null!));
        Assert.Contains("Device token", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserDevice_UpdateToken_WithEmptyToken_ThrowsArgumentException()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone",
            DateTime.UtcNow);

        var ex = Assert.Throws<ArgumentException>(() => device.UpdateToken(""));
        Assert.Contains("Device token", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserDevice_UpdateToken_UpdatesTokenAndLastUsedAt()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone",
            DateTime.UtcNow);

        var beforeUpdate = device.LastUsedAtUtc;
        Thread.Sleep(100); // Ensure time difference

        device.UpdateToken("newtoken456");

        Assert.Equal("newtoken456", device.DeviceToken);
        Assert.True(device.LastUsedAtUtc > beforeUpdate);
    }

    [Fact]
    public void UserDevice_UpdateDeviceName_UpdatesNameAndLastUsedAt()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone",
            DateTime.UtcNow);

        var beforeUpdate = device.LastUsedAtUtc;
        Thread.Sleep(100); // Ensure time difference

        device.UpdateDeviceName("iPhone 15");

        Assert.Equal("iPhone 15", device.DeviceName);
        Assert.True(device.LastUsedAtUtc > beforeUpdate);
    }

    [Fact]
    public void UserDevice_UpdateDeviceName_WithNull_SetsToNull()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone",
            DateTime.UtcNow);

        device.UpdateDeviceName(null);

        Assert.Null(device.DeviceName);
    }

    [Fact]
    public void UserDevice_MarkAsActive_SetsIsActiveAndUpdatesLastUsedAt()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone",
            DateTime.UtcNow);

        device.MarkAsInactive();
        Assert.False(device.IsActive);

        var beforeMark = device.LastUsedAtUtc;
        Thread.Sleep(100); // Ensure time difference

        device.MarkAsActive();

        Assert.True(device.IsActive);
        Assert.True(device.LastUsedAtUtc > beforeMark);
    }

    [Fact]
    public void UserDevice_MarkAsInactive_SetsIsActiveToFalse()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone",
            DateTime.UtcNow);

        device.MarkAsInactive();

        Assert.False(device.IsActive);
    }

    [Fact]
    public void UserDevice_WithUnicodeInDeviceName_AcceptsUnicode()
    {
        var device = new UserDevice(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token123",
            "ios",
            "iPhone com acentuação",
            DateTime.UtcNow);

        Assert.Equal("iPhone com acentuação", device.DeviceName);
    }
}
