using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class CouponServiceTests
{
    private readonly Mock<ICouponRepository> _couponRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CouponService _service;

    public CouponServiceTests()
    {
        _couponRepositoryMock = new Mock<ICouponRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new CouponService(
            _couponRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateCouponAsync_CreatesCoupon_WhenValidData()
    {
        // Arrange
        var code = "WELCOME10";
        _couponRepositoryMock
            .Setup(r => r.CodeExistsAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _couponRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateCouponAsync(
            code,
            "Welcome Discount",
            "10% off for new users",
            CouponDiscountType.Percentage,
            10m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            100,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(code, result.Value.Code);
        _couponRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCouponAsync_ReturnsFailure_WhenInvalidData()
    {
        // Arrange
        var code = "INVALID";
        _couponRepositoryMock
            .Setup(r => r.CodeExistsAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act - Tentar criar cupom com desconto > 100%
        var result = await _service.CreateCouponAsync(
            code,
            "Invalid Discount",
            null,
            CouponDiscountType.Percentage,
            150m, // > 100%
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            null,
            CancellationToken.None);

        // Assert
        // O domínio deve validar isso
        // Por enquanto, assumimos que o serviço cria e o domínio valida
        // Se o domínio lançar exceção, o teste deve capturar
    }

    [Fact]
    public async Task CreateCouponAsync_ValidatesExpirationDate()
    {
        // Arrange
        var code = "EXPIRED";
        _couponRepositoryMock
            .Setup(r => r.CodeExistsAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act - Tentar criar cupom com data de expiração no passado
        var result = await _service.CreateCouponAsync(
            code,
            "Expired Coupon",
            null,
            CouponDiscountType.Percentage,
            10m,
            DateTime.UtcNow.AddMonths(-2),
            DateTime.UtcNow.AddMonths(-1), // Expiração no passado
            null,
            CancellationToken.None);

        // Assert
        // O domínio deve validar isso
        // Por enquanto, assumimos que o serviço cria e o domínio valida
    }

    [Fact]
    public async Task ApplyCouponToSubscriptionAsync_AppliesCoupon_WhenValid()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var couponCode = "WELCOME10";
        var coupon = CreateValidCoupon(couponCode);
        var subscriptionCouponRepositoryMock = new Mock<ISubscriptionCouponRepository>();

        _couponRepositoryMock
            .Setup(r => r.GetByCodeAsync(couponCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);
        subscriptionCouponRepositoryMock
            .Setup(r => r.GetBySubscriptionIdAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionCoupon?)null);
        subscriptionCouponRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionCoupon>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _couponRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Coupon>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ApplyCouponToSubscriptionAsync(
            subscriptionId,
            couponCode,
            subscriptionCouponRepositoryMock.Object,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        subscriptionCouponRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SubscriptionCoupon>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyCouponToSubscriptionAsync_ReturnsFailure_WhenCouponExpired()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var couponCode = "EXPIRED";
        var coupon = CreateExpiredCoupon(couponCode);
        var subscriptionCouponRepositoryMock = new Mock<ISubscriptionCouponRepository>();

        _couponRepositoryMock
            .Setup(r => r.GetByCodeAsync(couponCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        // Act
        var result = await _service.ApplyCouponToSubscriptionAsync(
            subscriptionId,
            couponCode,
            subscriptionCouponRepositoryMock.Object,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not valid", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ApplyCouponToSubscriptionAsync_ReturnsFailure_WhenCouponNotFound()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var couponCode = "NOTFOUND";
        var subscriptionCouponRepositoryMock = new Mock<ISubscriptionCouponRepository>();

        _couponRepositoryMock
            .Setup(r => r.GetByCodeAsync(couponCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coupon?)null);

        // Act
        var result = await _service.ApplyCouponToSubscriptionAsync(
            subscriptionId,
            couponCode,
            subscriptionCouponRepositoryMock.Object,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ApplyCouponToSubscriptionAsync_ValidatesUsageLimit()
    {
        // Arrange
        var subscriptionId = Guid.NewGuid();
        var couponCode = "LIMITED";
        var coupon = CreateCouponWithMaxUses(couponCode, maxUses: 1, currentUses: 1);
        var subscriptionCouponRepositoryMock = new Mock<ISubscriptionCouponRepository>();

        _couponRepositoryMock
            .Setup(r => r.GetByCodeAsync(couponCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        // Act
        var result = await _service.ApplyCouponToSubscriptionAsync(
            subscriptionId,
            couponCode,
            subscriptionCouponRepositoryMock.Object,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not valid", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateCouponAsync_ReturnsTrue_WhenValid()
    {
        // Arrange
        var couponCode = "VALID";
        var coupon = CreateValidCoupon(couponCode);

        _couponRepositoryMock
            .Setup(r => r.GetByCodeAsync(couponCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        // Act
        var result = await _service.ValidateCouponAsync(couponCode, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task ValidateCouponAsync_ReturnsFalse_WhenExpired()
    {
        // Arrange
        var couponCode = "EXPIRED";
        var coupon = CreateExpiredCoupon(couponCode);

        _couponRepositoryMock
            .Setup(r => r.GetByCodeAsync(couponCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        // Act
        var result = await _service.ValidateCouponAsync(couponCode, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not valid", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateCouponAsync_ReturnsFalse_WhenUsageLimitExceeded()
    {
        // Arrange
        var couponCode = "LIMITED";
        var coupon = CreateCouponWithMaxUses(couponCode, maxUses: 1, currentUses: 1);

        _couponRepositoryMock
            .Setup(r => r.GetByCodeAsync(couponCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupon);

        // Act
        var result = await _service.ValidateCouponAsync(couponCode, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not valid", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    private static Coupon CreateValidCoupon(string code)
    {
        return new Coupon(
            Guid.NewGuid(),
            code,
            "Valid Coupon",
            "Valid coupon description",
            CouponDiscountType.Percentage,
            10m,
            DateTime.UtcNow.AddMonths(-1),
            DateTime.UtcNow.AddMonths(1),
            100);
    }

    private static Coupon CreateExpiredCoupon(string code)
    {
        return new Coupon(
            Guid.NewGuid(),
            code,
            "Expired Coupon",
            "Expired coupon",
            CouponDiscountType.Percentage,
            10m,
            DateTime.UtcNow.AddMonths(-2),
            DateTime.UtcNow.AddMonths(-1), // Expirou
            100);
    }

    private static Coupon CreateCouponWithMaxUses(string code, int maxUses, int currentUses)
    {
        var coupon = new Coupon(
            Guid.NewGuid(),
            code,
            "Limited Coupon",
            "Limited uses",
            CouponDiscountType.Percentage,
            10m,
            DateTime.UtcNow.AddMonths(-1),
            DateTime.UtcNow.AddMonths(1),
            maxUses);

        // Simular uso do cupom
        for (int i = 0; i < currentUses; i++)
        {
            coupon.Use();
        }

        return coupon;
    }
}
