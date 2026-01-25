using Araponga.Domain.Feed;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Postgres;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Araponga.Tests.Infrastructure;

/// <summary>
/// Edge case tests for Postgres Repository Integration,
/// focusing on transactions, concurrency, rollback, and database edge cases.
/// 
/// IMPORTANTE: Estes testes requerem PostgreSQL rodando.
/// Para executar: export TEST_DATABASE_CONNECTION_STRING="Host=localhost;Database=araponga_test;Username=postgres;Password=postgres"
/// </summary>
public sealed class PostgresRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public PostgresRepositoryIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private ArapongaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ArapongaDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;
        return new ArapongaDbContext(options);
    }

    #region Transaction Tests

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task BeginTransactionAsync_WithNoActiveTransaction_CreatesTransaction()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        using var context = CreateContext();
        
        await context.BeginTransactionAsync(CancellationToken.None);
        
        // Verificar que transação foi criada
        Assert.NotNull(context.Database.CurrentTransaction);
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task BeginTransactionAsync_WithActiveTransaction_ThrowsInvalidOperationException()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        using var context = CreateContext();
        
        await context.BeginTransactionAsync(CancellationToken.None);
        
        // Tentar iniciar segunda transação
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await context.BeginTransactionAsync(CancellationToken.None));
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task CommitAsync_WithTransaction_CommitsChanges()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var userId = Guid.NewGuid();
        var user = new UserRecord
        {
            Id = userId,
            DisplayName = "Test User",
            Email = "test@example.com",
            Cpf = "123.456.789-00",
            CreatedAtUtc = DateTime.UtcNow
        };

        using (var context = CreateContext())
        {
            await context.BeginTransactionAsync(CancellationToken.None);
            context.Users.Add(user);
            await context.CommitAsync(CancellationToken.None);
        }

        // Verificar que foi salvo
        using (var context = CreateContext())
        {
            var savedUser = await context.Users.FindAsync(userId);
            Assert.NotNull(savedUser);
            Assert.Equal("Test User", savedUser.DisplayName);
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task CommitAsync_WithoutTransaction_StillSavesChanges()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var userId = Guid.NewGuid();
        var user = new UserRecord
        {
            Id = userId,
            DisplayName = "Test User No Transaction",
            Email = "test2@example.com",
            Cpf = "123.456.789-01",
            AuthProvider = "test",
            ExternalId = $"test-{userId}",
            CreatedAtUtc = DateTime.UtcNow
        };

        using (var context = CreateContext())
        {
            context.Users.Add(user);
            await context.CommitAsync(CancellationToken.None);
        }

        // Verificar que foi salvo
        using (var context = CreateContext())
        {
            var savedUser = await context.Users.FindAsync(userId);
            Assert.NotNull(savedUser);
            Assert.Equal("Test User No Transaction", savedUser.DisplayName);
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task RollbackAsync_WithTransaction_RollsBackChanges()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var userId = Guid.NewGuid();
        var user = new UserRecord
        {
            Id = userId,
            DisplayName = "Test User Rollback",
            AuthProvider = "test",
            ExternalId = $"test-{userId}",
            Email = "rollback@example.com",
            Cpf = "123.456.789-02",
            CreatedAtUtc = DateTime.UtcNow
        };

        using (var context = CreateContext())
        {
            await context.BeginTransactionAsync(CancellationToken.None);
            context.Users.Add(user);
            await context.RollbackAsync(CancellationToken.None);
        }

        // Verificar que NÃO foi salvo
        using (var context = CreateContext())
        {
            var savedUser = await context.Users.FindAsync(userId);
            Assert.Null(savedUser);
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task RollbackAsync_WithoutTransaction_DoesNotThrow()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        using var context = CreateContext();
        
        // Rollback sem transação ativa não deve lançar exceção
        await context.RollbackAsync(CancellationToken.None);
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task CommitAsync_WithMultipleEntities_CommitsAll()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var user1 = new UserRecord
        {
            Id = userId1,
            DisplayName = "User 1",
            AuthProvider = "test",
            ExternalId = $"test-{userId1}",
            Email = "user1@example.com",
            Cpf = "123.456.789-03",
            CreatedAtUtc = DateTime.UtcNow
        };
        var user2 = new UserRecord
        {
            Id = userId2,
            DisplayName = "User 2",
            AuthProvider = "test",
            ExternalId = $"test-{userId2}",
            Email = "user2@example.com",
            Cpf = "123.456.789-04",
            CreatedAtUtc = DateTime.UtcNow
        };

        using (var context = CreateContext())
        {
            await context.BeginTransactionAsync(CancellationToken.None);
            context.Users.Add(user1);
            context.Users.Add(user2);
            await context.CommitAsync(CancellationToken.None);
        }

        // Verificar que ambos foram salvos
        using (var context = CreateContext())
        {
            var savedUser1 = await context.Users.FindAsync(userId1);
            var savedUser2 = await context.Users.FindAsync(userId2);
            Assert.NotNull(savedUser1);
            Assert.NotNull(savedUser2);
        }
    }

    #endregion

    #region Concurrency Tests

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task CommitAsync_WithConcurrencyConflict_ThrowsInvalidOperationException()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var postId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        // Criar post inicial
        using (var context = CreateContext())
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
            context.CommunityPosts.Add(post);
            await context.SaveChangesAsync();
        }

        // Modificar em contexto 1
        byte[]? originalRowVersion;
        using (var context1 = CreateContext())
        {
            var post = await context1.CommunityPosts.FindAsync(postId);
            Assert.NotNull(post);
            originalRowVersion = post.RowVersion;
            post.Status = PostStatus.Hidden;
            await context1.SaveChangesAsync();
        }

        // Tentar modificar com RowVersion antigo em contexto 2
        using (var context2 = CreateContext())
        {
            var post = await context2.CommunityPosts.FindAsync(postId);
            Assert.NotNull(post);
            post.RowVersion = originalRowVersion!;
            post.Status = PostStatus.Rejected;

            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await context2.CommitAsync(CancellationToken.None));
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task SaveChangesAsync_WithConcurrentUpdates_ThrowsDbUpdateConcurrencyException()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var membershipId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Criar membership inicial
        using (var context = CreateContext())
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
            context.TerritoryMemberships.Add(membership);
            await context.SaveChangesAsync();
        }

        // Modificar em contexto 1
        byte[]? originalRowVersion;
        using (var context1 = CreateContext())
        {
            var membership = await context1.TerritoryMemberships.FindAsync(membershipId);
            Assert.NotNull(membership);
            originalRowVersion = membership.RowVersion;
            membership.Role = MembershipRole.Resident;
            await context1.SaveChangesAsync();
        }

        // Tentar modificar com RowVersion antigo em contexto 2
        using (var context2 = CreateContext())
        {
            var membership = await context2.TerritoryMemberships.FindAsync(membershipId);
            Assert.NotNull(membership);
            membership.RowVersion = originalRowVersion!;
            membership.Role = MembershipRole.Resident;

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                async () => await context2.SaveChangesAsync());
        }
    }

    #endregion

    #region Integration Tests

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task AddAndRetrieve_WithPostgresRepository_WorksCorrectly()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var postId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        // Adicionar
        using (var context = CreateContext())
        {
            var post = new CommunityPostRecord
            {
                Id = postId,
                TerritoryId = territoryId,
                AuthorUserId = authorId,
                Title = "Integration Test Post",
                Content = "Content",
                Type = PostType.General,
                Visibility = PostVisibility.Public,
                Status = PostStatus.Published,
                CreatedAtUtc = DateTime.UtcNow,
                RowVersion = Array.Empty<byte>()
            };
            context.CommunityPosts.Add(post);
            await context.SaveChangesAsync();
        }

        // Recuperar
        using (var context = CreateContext())
        {
            var post = await context.CommunityPosts.FindAsync(postId);
            Assert.NotNull(post);
            Assert.Equal("Integration Test Post", post.Title);
            Assert.Equal(PostStatus.Published, post.Status);
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task Update_WithPostgresRepository_PersistsChanges()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var postId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        // Criar
        using (var context = CreateContext())
        {
            var post = new CommunityPostRecord
            {
                Id = postId,
                TerritoryId = territoryId,
                AuthorUserId = authorId,
                Title = "Original Title",
                Content = "Content",
                Type = PostType.General,
                Visibility = PostVisibility.Public,
                Status = PostStatus.Published,
                CreatedAtUtc = DateTime.UtcNow,
                RowVersion = Array.Empty<byte>()
            };
            context.CommunityPosts.Add(post);
            await context.SaveChangesAsync();
        }

        // Atualizar
        using (var context = CreateContext())
        {
            var post = await context.CommunityPosts.FindAsync(postId);
            Assert.NotNull(post);
            post.Title = "Updated Title";
            post.Status = PostStatus.Hidden;
            await context.SaveChangesAsync();
        }

        // Verificar atualização
        using (var context = CreateContext())
        {
            var post = await context.CommunityPosts.FindAsync(postId);
            Assert.NotNull(post);
            Assert.Equal("Updated Title", post.Title);
            Assert.Equal(PostStatus.Hidden, post.Status);
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task Query_WithFilters_ReturnsCorrectResults()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var territoryId = Guid.NewGuid();
        var authorId1 = Guid.NewGuid();
        var authorId2 = Guid.NewGuid();

        // Criar posts
        using (var context = CreateContext())
        {
            var post1 = new CommunityPostRecord
            {
                Id = Guid.NewGuid(),
                TerritoryId = territoryId,
                AuthorUserId = authorId1,
                Title = "Post 1",
                Content = "Content 1",
                Type = PostType.General,
                Visibility = PostVisibility.Public,
                Status = PostStatus.Published,
                CreatedAtUtc = DateTime.UtcNow,
                RowVersion = Array.Empty<byte>()
            };
            var post2 = new CommunityPostRecord
            {
                Id = Guid.NewGuid(),
                TerritoryId = territoryId,
                AuthorUserId = authorId2,
                Title = "Post 2",
                Content = "Content 2",
                Type = PostType.General,
                Visibility = PostVisibility.Public,
                Status = PostStatus.Published,
                CreatedAtUtc = DateTime.UtcNow,
                RowVersion = Array.Empty<byte>()
            };
            context.CommunityPosts.Add(post1);
            context.CommunityPosts.Add(post2);
            await context.SaveChangesAsync();
        }

        // Query com filtro
        using (var context = CreateContext())
        {
            var posts = await context.CommunityPosts
                .Where(p => p.TerritoryId == territoryId && p.AuthorUserId == authorId1)
                .ToListAsync();
            
            Assert.Single(posts);
            Assert.Equal("Post 1", posts[0].Title);
        }
    }

    #endregion

    #region Edge Cases

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task CommitAsync_AfterDispose_ThrowsException()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        ArapongaDbContext? context = null;
        try
        {
            context = CreateContext();
            await context.BeginTransactionAsync(CancellationToken.None);
            context.Dispose();
            
            // Tentar commit após dispose deve lançar exceção
            await Assert.ThrowsAnyAsync<ObjectDisposedException>(
                async () => await context.CommitAsync(CancellationToken.None));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task MultipleTransactions_Sequentially_WorksCorrectly()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Primeira transação
        using (var context = CreateContext())
        {
            await context.BeginTransactionAsync(CancellationToken.None);
            context.Users.Add(new UserRecord
            {
                Id = userId1,
                DisplayName = "User 1",
                Email = "user1@test.com",
                Cpf = "123.456.789-05",
                CreatedAtUtc = DateTime.UtcNow
            });
            await context.CommitAsync(CancellationToken.None);
        }

        // Segunda transação
        using (var context = CreateContext())
        {
            await context.BeginTransactionAsync(CancellationToken.None);
            context.Users.Add(new UserRecord
            {
                Id = userId2,
                DisplayName = "User 2",
                Email = "user2@test.com",
                Cpf = "123.456.789-06",
                CreatedAtUtc = DateTime.UtcNow
            });
            await context.CommitAsync(CancellationToken.None);
        }

        // Verificar ambos
        using (var context = CreateContext())
        {
            var user1 = await context.Users.FindAsync(userId1);
            var user2 = await context.Users.FindAsync(userId2);
            Assert.NotNull(user1);
            Assert.NotNull(user2);
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task Transaction_WithException_RollsBackAutomatically()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var userId = Guid.NewGuid();

        try
        {
            using (var context = CreateContext())
            {
                await context.BeginTransactionAsync(CancellationToken.None);
                context.Users.Add(new UserRecord
                {
                    Id = userId,
                    DisplayName = "User Exception",
                    Email = "exception@test.com",
                    Cpf = "123.456.789-07",
                    CreatedAtUtc = DateTime.UtcNow
                });
                
                // Simular exceção antes do commit
                throw new InvalidOperationException("Test exception");
            }
        }
        catch (InvalidOperationException)
        {
            // Esperado
        }

        // Verificar que NÃO foi salvo (rollback automático)
        using (var context = CreateContext())
        {
            var user = await context.Users.FindAsync(userId);
            Assert.Null(user);
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task SaveChangesAsync_WithUnicode_HandlesCorrectly()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var postId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        using (var context = CreateContext())
        {
            var post = new CommunityPostRecord
            {
                Id = postId,
                TerritoryId = territoryId,
                AuthorUserId = authorId,
                Title = "Post com café, naïve, résumé, 文字 e emoji 🎉",
                Content = "Conteúdo com Unicode: café, naïve, résumé, 文字 e emoji 🎉",
                Type = PostType.General,
                Visibility = PostVisibility.Public,
                Status = PostStatus.Published,
                CreatedAtUtc = DateTime.UtcNow,
                RowVersion = Array.Empty<byte>()
            };
            context.CommunityPosts.Add(post);
            await context.SaveChangesAsync();
        }

        // Verificar que Unicode foi preservado
        using (var context = CreateContext())
        {
            var post = await context.CommunityPosts.FindAsync(postId);
            Assert.NotNull(post);
            Assert.Contains("café", post.Title);
            Assert.Contains("文字", post.Title);
            Assert.Contains("🎉", post.Title);
        }
    }

    [Fact(Skip = "Requires PostgreSQL database. Set TEST_DATABASE_CONNECTION_STRING environment variable to run.")]
    public async Task Query_WithLargeResultSet_HandlesCorrectly()
    {
        if (!await _fixture.IsDatabaseAvailableAsync())
        {
            return;
        }

        var territoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        // Criar múltiplos posts
        using (var context = CreateContext())
        {
            for (int i = 0; i < 100; i++)
            {
                context.CommunityPosts.Add(new CommunityPostRecord
                {
                    Id = Guid.NewGuid(),
                    TerritoryId = territoryId,
                    AuthorUserId = authorId,
                    Title = $"Post {i}",
                    Content = $"Content {i}",
                    Type = PostType.General,
                    Visibility = PostVisibility.Public,
                    Status = PostStatus.Published,
                    CreatedAtUtc = DateTime.UtcNow,
                    RowVersion = Array.Empty<byte>()
                });
            }
            await context.SaveChangesAsync();
        }

        // Query todos
        using (var context = CreateContext())
        {
            var posts = await context.CommunityPosts
                .Where(p => p.TerritoryId == territoryId)
                .ToListAsync();
            
            Assert.Equal(100, posts.Count);
        }
    }

    #endregion
}
