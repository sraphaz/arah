using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using System.Text.Json;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Testes de segurança para o sistema de políticas.
/// </summary>
public sealed class PolicySecurityTests
{
    private static readonly Guid UserId1 = Guid.NewGuid();
    private static readonly Guid UserId2 = Guid.NewGuid();
    private static readonly Guid TermsId = Guid.NewGuid();

    [Fact]
    public async Task AcceptTermsAsync_UserCannotAcceptTermsForAnotherUser()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var termsRepository = new InMemoryTermsOfServiceRepository(dataStore);
        var acceptanceRepository = new InMemoryTermsAcceptanceRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new TermsAcceptanceService(acceptanceRepository, termsRepository, unitOfWork);

        var terms = new TermsOfService(
            TermsId,
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
        await termsRepository.AddAsync(terms, CancellationToken.None);

        // Act - User1 aceita termos
        var result1 = await service.AcceptTermsAsync(UserId1, TermsId, null, null, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Assert - User1 aceitou com sucesso
        Assert.True(result1.IsSuccess);
        Assert.Equal(UserId1, result1.Value?.UserId);

        // Act - User2 tenta aceitar termos (deve criar seu próprio aceite)
        var result2 = await service.AcceptTermsAsync(UserId2, TermsId, null, null, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Assert - User2 também pode aceitar (cada usuário tem seu próprio aceite)
        Assert.True(result2.IsSuccess);
        Assert.Equal(UserId2, result2.Value?.UserId);
        Assert.NotEqual(result1.Value?.Id, result2.Value?.Id);
    }

    [Fact]
    public async Task AcceptTermsAsync_CannotAcceptExpiredTerms()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var termsRepository = new InMemoryTermsOfServiceRepository(dataStore);
        var acceptanceRepository = new InMemoryTermsAcceptanceRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new TermsAcceptanceService(acceptanceRepository, termsRepository, unitOfWork);

        var terms = new TermsOfService(
            TermsId,
            "1.0",
            "Test Terms",
            "Content",
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(-1), // Expired
            true,
            null,
            null,
            null,
            DateTime.UtcNow);
        await termsRepository.AddAsync(terms, CancellationToken.None);

        // Act
        var result = await service.AcceptTermsAsync(UserId1, TermsId, null, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("expired", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RevokeAcceptanceAsync_UserCannotRevokeAnotherUserAcceptance()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var termsRepository = new InMemoryTermsOfServiceRepository(dataStore);
        var acceptanceRepository = new InMemoryTermsAcceptanceRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new TermsAcceptanceService(acceptanceRepository, termsRepository, unitOfWork);

        var terms = new TermsOfService(
            TermsId,
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
        await termsRepository.AddAsync(terms, CancellationToken.None);

        // User1 aceita termos
        var acceptResult = await service.AcceptTermsAsync(UserId1, TermsId, null, null, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);
        Assert.True(acceptResult.IsSuccess);

        // Act - User2 tenta revogar aceite de User1
        var revokeResult = await service.RevokeAcceptanceAsync(UserId2, TermsId, CancellationToken.None);

        // Assert - Deve falhar porque User2 não tem aceite para revogar
        Assert.True(revokeResult.IsFailure);
        Assert.Contains("not found", revokeResult.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAcceptanceHistoryAsync_UserCanOnlySeeOwnHistory()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var termsRepository = new InMemoryTermsOfServiceRepository(dataStore);
        var acceptanceRepository = new InMemoryTermsAcceptanceRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new TermsAcceptanceService(acceptanceRepository, termsRepository, unitOfWork);

        var terms = new TermsOfService(
            TermsId,
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
        await termsRepository.AddAsync(terms, CancellationToken.None);

        // User1 aceita
        await service.AcceptTermsAsync(UserId1, TermsId, null, null, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // User2 aceita
        await service.AcceptTermsAsync(UserId2, TermsId, null, null, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Act - User1 obtém seu histórico
        var history1 = await service.GetAcceptanceHistoryAsync(UserId1, CancellationToken.None);

        // Assert - Apenas aceites de User1
        Assert.NotNull(history1);
        Assert.All(history1, acceptance => Assert.Equal(UserId1, acceptance.UserId));
    }

    [Fact]
    public async Task AcceptTermsAsync_ValidatesTermsVersionMismatch()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var termsRepository = new InMemoryTermsOfServiceRepository(dataStore);
        var acceptanceRepository = new InMemoryTermsAcceptanceRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new TermsAcceptanceService(acceptanceRepository, termsRepository, unitOfWork);

        var termsV1 = new TermsOfService(
            TermsId,
            "1.0",
            "Test Terms",
            "Content",
            DateTime.UtcNow.AddDays(-10),
            null,
            true,
            null,
            null,
            null,
            DateTime.UtcNow);
        await termsRepository.AddAsync(termsV1, CancellationToken.None);

        // User1 aceita versão 1.0
        var result1 = await service.AcceptTermsAsync(UserId1, TermsId, null, null, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);
        Assert.True(result1.IsSuccess);
        Assert.Equal("1.0", result1.Value?.AcceptedVersion);

        // Atualizar termos para versão 2.0
        termsV1.Update(
            "Test Terms",
            "Content Updated",
            DateTime.UtcNow.AddDays(-5),
            null,
            true,
            null,
            null,
            null,
            DateTime.UtcNow);
        // Note: Não podemos mudar a versão diretamente, então vamos criar novos termos
        var termsV2 = new TermsOfService(
            Guid.NewGuid(),
            "2.0",
            "Test Terms V2",
            "Content V2",
            DateTime.UtcNow.AddDays(-1),
            null,
            true,
            null,
            null,
            null,
            DateTime.UtcNow);
        await termsRepository.AddAsync(termsV2, CancellationToken.None);

        // Act - User1 aceita versão 2.0
        var result2 = await service.AcceptTermsAsync(UserId1, termsV2.Id, null, null, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Assert - Deve criar novo aceite para versão 2.0
        Assert.True(result2.IsSuccess);
        Assert.Equal("2.0", result2.Value?.AcceptedVersion);
        Assert.NotEqual(result1.Value?.Id, result2.Value?.Id);
    }

    [Fact]
    public async Task PolicyRequirementService_OnlyReturnsActivePolicies()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var termsRepository = new InMemoryTermsOfServiceRepository(dataStore);
        var privacyRepository = new InMemoryPrivacyPolicyRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(dataStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(dataStore);

        var termsService = new TermsOfServiceService(termsRepository);
        var privacyService = new PrivacyPolicyService(privacyRepository);
        var service = new PolicyRequirementService(
            termsService,
            privacyService,
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository);

        var activeTerms = new TermsOfService(
            Guid.NewGuid(),
            "1.0",
            "Active Terms",
            "Content",
            DateTime.UtcNow.AddDays(-1),
            null,
            true, // Active
            JsonSerializer.Serialize(new List<int> { (int)MembershipRole.Resident }),
            null,
            null,
            DateTime.UtcNow);
        await termsRepository.AddAsync(activeTerms, CancellationToken.None);

        var inactiveTerms = new TermsOfService(
            Guid.NewGuid(),
            "2.0",
            "Inactive Terms",
            "Content",
            DateTime.UtcNow.AddDays(-1),
            null,
            false, // Inactive
            JsonSerializer.Serialize(new List<int> { (int)MembershipRole.Resident }),
            null,
            null,
            DateTime.UtcNow);
        await termsRepository.AddAsync(inactiveTerms, CancellationToken.None);

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            UserId1,
            Guid.NewGuid(),
            MembershipRole.Resident,
            ResidencyVerification.None,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);
        await membershipRepository.AddAsync(membership, CancellationToken.None);

        // Act
        var result = await service.GetRequiredPoliciesForUserAsync(UserId1, CancellationToken.None);

        // Assert - Apenas termos ativos devem ser retornados
        Assert.NotNull(result);
        Assert.Single(result.RequiredTerms);
        Assert.Equal(activeTerms.Id, result.RequiredTerms[0].Id);
        Assert.DoesNotContain(result.RequiredTerms, t => t.Id == inactiveTerms.Id);
    }

    [Fact]
    public async Task AcceptTermsAsync_ValidatesEffectiveDate()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var termsRepository = new InMemoryTermsOfServiceRepository(dataStore);
        var acceptanceRepository = new InMemoryTermsAcceptanceRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new TermsAcceptanceService(acceptanceRepository, termsRepository, unitOfWork);

        var futureTerms = new TermsOfService(
            TermsId,
            "1.0",
            "Future Terms",
            "Content",
            DateTime.UtcNow.AddDays(1), // Future effective date
            null,
            true,
            null,
            null,
            null,
            DateTime.UtcNow);
        await termsRepository.AddAsync(futureTerms, CancellationToken.None);

        // Act
        var result = await service.AcceptTermsAsync(UserId1, TermsId, null, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not yet effective", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AcceptTermsAsync_StoresIpAddressAndUserAgent()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var termsRepository = new InMemoryTermsOfServiceRepository(dataStore);
        var acceptanceRepository = new InMemoryTermsAcceptanceRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new TermsAcceptanceService(acceptanceRepository, termsRepository, unitOfWork);

        var terms = new TermsOfService(
            TermsId,
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
        await termsRepository.AddAsync(terms, CancellationToken.None);

        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0 Test Browser";

        // Act
        var result = await service.AcceptTermsAsync(UserId1, TermsId, ipAddress, userAgent, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ipAddress, result.Value?.IpAddress);
        Assert.Equal(userAgent, result.Value?.UserAgent);
    }
}
