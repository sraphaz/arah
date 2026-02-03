using Araponga.Application.Services;
using Araponga.Domain.Policies;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for TermsAcceptanceService (terms not found, expired, ListByUser empty).
/// </summary>
public sealed class TermsAcceptanceServiceEdgeCasesTests
{
    [Fact]
    public async Task AcceptTermsAsync_WhenTermsNotFound_ReturnsFailure()
    {
        var sharedStore = new InMemorySharedStore();
        var acceptanceRepo = new InMemoryTermsAcceptanceRepository(sharedStore);
        var termsRepo = new InMemoryTermsOfServiceRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TermsAcceptanceService(acceptanceRepo, termsRepo, uow);
        var userId = Guid.NewGuid();

        var result = await svc.AcceptTermsAsync(userId, Guid.NewGuid(), null, null, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AcceptTermsAsync_WhenTermsExpired_ReturnsFailure()
    {
        var sharedStore = new InMemorySharedStore();
        var termsRepo = new InMemoryTermsOfServiceRepository(sharedStore);
        var termsId = Guid.NewGuid();
        var terms = new TermsOfService(
            termsId,
            "1.0",
            "Expired",
            "Content",
            DateTime.UtcNow.AddDays(-30),
            DateTime.UtcNow.AddDays(-1),
            true,
            null,
            null,
            null,
            DateTime.UtcNow.AddDays(-30));
        await termsRepo.AddAsync(terms, CancellationToken.None);
        var acceptanceRepo = new InMemoryTermsAcceptanceRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TermsAcceptanceService(acceptanceRepo, termsRepo, uow);
        var userId = Guid.NewGuid();

        var result = await svc.AcceptTermsAsync(userId, termsId, null, null, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("expired", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAcceptanceHistoryAsync_WhenUserHasNone_ReturnsEmpty()
    {
        var sharedStore = new InMemorySharedStore();
        var acceptanceRepo = new InMemoryTermsAcceptanceRepository(sharedStore);
        var termsRepo = new InMemoryTermsOfServiceRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TermsAcceptanceService(acceptanceRepo, termsRepo, uow);
        var userId = Guid.NewGuid();

        var list = await svc.GetAcceptanceHistoryAsync(userId, CancellationToken.None);

        Assert.NotNull(list);
        Assert.Empty(list);
    }
}
