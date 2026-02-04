using Araponga.Domain.Feed;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class ConcurrencyTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ConcurrencyTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task UpdateCommunityPost_ThrowsConcurrencyException_WhenRowVersionMismatch()
    {
        // Este teste requer PostgreSQL rodando
        // Para executar: export TEST_DATABASE_CONNECTION_STRING="Host=localhost;Database=araponga_test;Username=postgres;Password=postgres"
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return; // Skip se banco não disponível
        }

        // Arrange
        var options = new DbContextOptionsBuilder<ArapongaDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;

        var postId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        // Criar post inicial
        using (var context1 = new ArapongaDbContext(options))
        {
            var post = new CommunityPostRecord
            {
                Id = postId,
                TerritoryId = territoryId,
                AuthorUserId = authorId,
                Title = "Test Post",
                Content = "Content",
                Type = PostType.General,
                Visibility = PostVisibility.Public,
                Status = PostStatus.Published,
                CreatedAtUtc = DateTime.UtcNow,
                RowVersion = Array.Empty<byte>()
            };
            context1.CommunityPosts.Add(post);
            await context1.SaveChangesAsync();
        }

        // Simular dois contextos diferentes tentando atualizar
        byte[]? originalRowVersion;
        using (var context2 = new ArapongaDbContext(options))
        {
            var post = await context2.CommunityPosts.FindAsync(postId);
            Assert.NotNull(post);
            originalRowVersion = post.RowVersion;
            post.Status = PostStatus.Hidden;
            await context2.SaveChangesAsync();
        }

        // Tentar atualizar com RowVersion antigo
        using (var context3 = new ArapongaDbContext(options))
        {
            var post = await context3.CommunityPosts.FindAsync(postId);
            Assert.NotNull(post);
            // Simular RowVersion antigo
            post.RowVersion = originalRowVersion!;
            post.Status = PostStatus.Rejected;

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                async () => await context3.SaveChangesAsync());
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task UpdateTerritoryMembership_ThrowsConcurrencyException_WhenRowVersionMismatch()
    {
        // Este teste requer PostgreSQL rodando
        // Para executar: export TEST_DATABASE_CONNECTION_STRING="Host=localhost;Database=araponga_test;Username=postgres;Password=postgres"
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return; // Skip se banco não disponível
        }

        // Arrange
        var options = new DbContextOptionsBuilder<ArapongaDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;

        var membershipId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Criar membership inicial
        using (var context1 = new ArapongaDbContext(options))
        {
            var membership = new TerritoryMembershipRecord
            {
                Id = membershipId,
                UserId = userId,
                TerritoryId = territoryId,
                Role = MembershipRole.Visitor,
                ResidencyVerification = ResidencyVerification.None,
                CreatedAtUtc = DateTime.UtcNow,
                RowVersion = Array.Empty<byte>()
            };
            context1.TerritoryMemberships.Add(membership);
            await context1.SaveChangesAsync();
        }

        // Simular dois contextos diferentes tentando atualizar
        byte[]? originalRowVersion;
        using (var context2 = new ArapongaDbContext(options))
        {
            var membership = await context2.TerritoryMemberships.FindAsync(membershipId);
            Assert.NotNull(membership);
            originalRowVersion = membership.RowVersion;
            membership.Role = MembershipRole.Resident;
            await context2.SaveChangesAsync();
        }

        // Tentar atualizar com RowVersion antigo
        using (var context3 = new ArapongaDbContext(options))
        {
            var membership = await context3.TerritoryMemberships.FindAsync(membershipId);
            Assert.NotNull(membership);
            // Simular RowVersion antigo
            membership.RowVersion = originalRowVersion!;
            membership.Role = MembershipRole.Visitor;

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                async () => await context3.SaveChangesAsync());
        }
    }
}

/// <summary>
/// Fixture para testes de concorrência que requerem banco de dados real.
/// </summary>
public sealed class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }

    public DatabaseFixture()
    {
        // Usar connection string de teste ou criar banco em memória
        ConnectionString = Environment.GetEnvironmentVariable("TEST_DATABASE_CONNECTION_STRING")
            ?? "Host=localhost;Database=araponga_test;Username=postgres;Password=postgres";
    }

    public async Task<bool> IsDatabaseAvailableAsync()
    {
        try
        {
            var options = new DbContextOptionsBuilder<ArapongaDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            using var context = new ArapongaDbContext(options);
            await context.Database.CanConnectAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        // Cleanup se necessário
    }
}
