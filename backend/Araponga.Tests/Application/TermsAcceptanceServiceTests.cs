using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Policies;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class TermsAcceptanceServiceTests
{
    private readonly Mock<ITermsAcceptanceRepository> _acceptanceRepositoryMock;
    private readonly Mock<ITermsOfServiceRepository> _termsRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TermsAcceptanceService _service;

    public TermsAcceptanceServiceTests()
    {
        _acceptanceRepositoryMock = new Mock<ITermsAcceptanceRepository>();
        _termsRepositoryMock = new Mock<ITermsOfServiceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new TermsAcceptanceService(
            _acceptanceRepositoryMock.Object,
            _termsRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AcceptTermsAsync_WhenTermsNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var termsId = Guid.NewGuid();
        _termsRepositoryMock.Setup(r => r.GetByIdAsync(termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TermsOfService?)null);

        // Act
        var result = await _service.AcceptTermsAsync(userId, termsId, null, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AcceptTermsAsync_WhenTermsNotActive_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var termsId = Guid.NewGuid();
        var terms = new TermsOfService(
            termsId,
            "1.0",
            "Test Terms",
            "Content",
            DateTime.UtcNow.AddDays(-10),
            null,
            false, // Not active
            null,
            null,
            null,
            DateTime.UtcNow);
        _termsRepositoryMock.Setup(r => r.GetByIdAsync(termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(terms);

        // Act
        var result = await _service.AcceptTermsAsync(userId, termsId, null, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not active", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AcceptTermsAsync_WhenTermsNotYetEffective_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var termsId = Guid.NewGuid();
        var terms = new TermsOfService(
            termsId,
            "1.0",
            "Test Terms",
            "Content",
            DateTime.UtcNow.AddDays(1), // Future date
            null,
            true,
            null,
            null,
            null,
            DateTime.UtcNow);
        _termsRepositoryMock.Setup(r => r.GetByIdAsync(termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(terms);

        // Act
        var result = await _service.AcceptTermsAsync(userId, termsId, null, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not yet effective", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AcceptTermsAsync_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var termsId = Guid.NewGuid();
        var terms = new TermsOfService(
            termsId,
            "1.0",
            "Test Terms",
            "Content",
            DateTime.UtcNow.AddDays(-1),
            null,
            true,
            null,
            null,
            null,
            DateTime.UtcNow);
        _termsRepositoryMock.Setup(r => r.GetByIdAsync(termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(terms);
        _acceptanceRepositoryMock.Setup(r => r.GetByUserAndTermsAsync(userId, termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TermsAcceptance?)null);

        // Act
        var result = await _service.AcceptTermsAsync(userId, termsId, "127.0.0.1", "TestAgent", CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(termsId, result.Value.TermsOfServiceId);
        Assert.Equal("1.0", result.Value.AcceptedVersion);
        _acceptanceRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TermsAcceptance>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcceptTermsAsync_WhenAlreadyAcceptedSameVersion_ReturnsExisting()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var termsId = Guid.NewGuid();
        var terms = new TermsOfService(
            termsId,
            "1.0",
            "Test Terms",
            "Content",
            DateTime.UtcNow.AddDays(-1),
            null,
            true,
            null,
            null,
            null,
            DateTime.UtcNow);
        var existingAcceptance = new TermsAcceptance(
            Guid.NewGuid(),
            userId,
            termsId,
            DateTime.UtcNow.AddDays(-5),
            "1.0",
            null,
            null);
        _termsRepositoryMock.Setup(r => r.GetByIdAsync(termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(terms);
        _acceptanceRepositoryMock.Setup(r => r.GetByUserAndTermsAsync(userId, termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAcceptance);

        // Act
        var result = await _service.AcceptTermsAsync(userId, termsId, null, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(existingAcceptance.Id, result.Value?.Id);
        _acceptanceRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TermsAcceptance>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HasAcceptedTermsAsync_WhenAccepted_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var termsId = Guid.NewGuid();
        _acceptanceRepositoryMock.Setup(r => r.HasAcceptedAsync(userId, termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.HasAcceptedTermsAsync(userId, termsId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RevokeAcceptanceAsync_WhenAcceptanceNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var termsId = Guid.NewGuid();
        _acceptanceRepositoryMock.Setup(r => r.GetByUserAndTermsAsync(userId, termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TermsAcceptance?)null);

        // Act
        var result = await _service.RevokeAcceptanceAsync(userId, termsId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RevokeAcceptanceAsync_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var termsId = Guid.NewGuid();
        var acceptance = new TermsAcceptance(
            Guid.NewGuid(),
            userId,
            termsId,
            DateTime.UtcNow.AddDays(-5),
            "1.0",
            null,
            null);
        _acceptanceRepositoryMock.Setup(r => r.GetByUserAndTermsAsync(userId, termsId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(acceptance);

        // Act
        var result = await _service.RevokeAcceptanceAsync(userId, termsId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value?.IsRevoked);
        _acceptanceRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TermsAcceptance>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
