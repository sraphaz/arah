using Araponga.Application.Services;
using Araponga.Domain.Feed;
using Araponga.Domain.Map;
using Araponga.Domain.Social;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class ApplicationServiceTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid PilotTerritoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task FeedService_ValidatesInputsAndFlags()
    {
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var featureFlags = new InMemoryFeatureFlagService();
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var mapRepository = new InMemoryMapRepository(dataStore);
        var service = new FeedService(feedRepository, accessEvaluator, featureFlags, auditLogger, blockRepository, mapRepository);

        var invalid = await service.CreatePostAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "",
            "",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            CancellationToken.None);

        Assert.False(invalid.success);

        var disabledAlert = await service.CreatePostAsync(
            PilotTerritoryId,
            Guid.NewGuid(),
            "Titulo",
            "Conteudo",
            PostType.Alert,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            CancellationToken.None);

        Assert.False(disabledAlert.success);

        var likeMissing = await service.LikeAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "session:1",
            null,
            CancellationToken.None);

        Assert.False(likeMissing);

        var residentOnlyPost = new CommunityPost(
            Guid.NewGuid(),
            ActiveTerritoryId,
            Guid.NewGuid(),
            "Title",
            "Content",
            PostType.General,
            PostVisibility.ResidentsOnly,
            PostStatus.Published,
            null,
            DateTime.UtcNow);
        await feedRepository.AddPostAsync(residentOnlyPost, CancellationToken.None);

        var likeBlocked = await service.LikeAsync(
            ActiveTerritoryId,
            residentOnlyPost.Id,
            "session:2",
            null,
            CancellationToken.None);

        Assert.False(likeBlocked);
    }

    [Fact]
    public async Task FeedService_BlocksNonResidentCommentAndShare()
    {
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var featureFlags = new InMemoryFeatureFlagService();
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var mapRepository = new InMemoryMapRepository(dataStore);
        var service = new FeedService(feedRepository, accessEvaluator, featureFlags, auditLogger, blockRepository, mapRepository);

        var post = new CommunityPost(
            Guid.NewGuid(),
            ActiveTerritoryId,
            Guid.NewGuid(),
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);

        await feedRepository.AddPostAsync(post, CancellationToken.None);

        var comment = await service.CommentAsync(
            ActiveTerritoryId,
            post.Id,
            Guid.NewGuid(),
            "Comentario",
            CancellationToken.None);

        Assert.False(comment.success);

        var share = await service.ShareAsync(
            ActiveTerritoryId,
            post.Id,
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(share.success);
    }

    [Fact]
    public async Task MapService_AllowsSuggestionsAndHandlesMissingEntities()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryMapRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var relationRepository = new InMemoryMapEntityRelationRepository(dataStore);
        var service = new MapService(repository, accessEvaluator, auditLogger, blockRepository, relationRepository);

        var suggestion = await service.SuggestAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "Lugar",
            "estabelecimento",
            CancellationToken.None);

        Assert.True(suggestion.success);

        var validate = await service.ValidateAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            MapEntityStatus.Validated,
            CancellationToken.None);

        Assert.False(validate);

        var confirm = await service.ConfirmAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(confirm.success);
    }

    [Fact]
    public async Task MapService_RelatesResidentToEntity()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryMapRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var relationRepository = new InMemoryMapEntityRelationRepository(dataStore);
        var service = new MapService(repository, accessEvaluator, auditLogger, blockRepository, relationRepository);

        var residentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var entityId = dataStore.MapEntities[0].Id;

        var relation = await service.RelateAsync(
            ActiveTerritoryId,
            entityId,
            residentId,
            CancellationToken.None);

        Assert.True(relation.success);
        Assert.NotNull(relation.relation);

        var duplicate = await service.RelateAsync(
            ActiveTerritoryId,
            entityId,
            residentId,
            CancellationToken.None);

        Assert.False(duplicate.success);
        Assert.Null(duplicate.error);
    }

    [Fact]
    public async Task HealthService_ValidatesAlerts()
    {
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var alertRepository = new InMemoryHealthAlertRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var service = new HealthService(alertRepository, feedRepository, auditLogger);

        var report = await service.ReportAlertAsync(
            ActiveTerritoryId,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Titulo",
            "Desc",
            CancellationToken.None);

        Assert.True(report.success);

        var validated = await service.ValidateAlertAsync(
            ActiveTerritoryId,
            report.alert!.Id,
            Guid.Parse("cccccccc-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Araponga.Domain.Health.HealthAlertStatus.Validated,
            CancellationToken.None);

        Assert.True(validated);
        Assert.Contains(dataStore.Posts, post => post.Type == PostType.Alert);
    }

    [Fact]
    public async Task ReportService_DeduplicatesPostReports()
    {
        var dataStore = new InMemoryDataStore();
        var reportRepository = new InMemoryReportRepository(dataStore);
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var service = new ReportService(reportRepository, feedRepository, userRepository, auditLogger);

        var reporterId = Guid.NewGuid();
        var postId = dataStore.Posts[0].Id;

        var created = await service.ReportPostAsync(
            reporterId,
            postId,
            "SPAM",
            null,
            CancellationToken.None);

        Assert.True(created.created);
        Assert.NotNull(created.report);

        var duplicate = await service.ReportPostAsync(
            reporterId,
            postId,
            "SPAM",
            "repetido",
            CancellationToken.None);

        Assert.False(duplicate.created);
        Assert.Null(duplicate.report);
    }

    [Fact]
    public async Task ReportService_RejectsUnknownTargets()
    {
        var dataStore = new InMemoryDataStore();
        var reportRepository = new InMemoryReportRepository(dataStore);
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var service = new ReportService(reportRepository, feedRepository, userRepository, auditLogger);

        var missingPost = await service.ReportPostAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "SPAM",
            null,
            CancellationToken.None);

        Assert.Equal("Post not found.", missingPost.error);

        var missingUser = await service.ReportUserAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "SPAM",
            null,
            CancellationToken.None);

        Assert.Equal("User not found.", missingUser.error);
    }

    [Fact]
    public async Task UserBlockService_DeduplicatesBlocks()
    {
        var dataStore = new InMemoryDataStore();
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var service = new UserBlockService(blockRepository, userRepository, auditLogger);

        var blockerId = Guid.NewGuid();
        var blockedId = dataStore.Users[0].Id;

        var created = await service.BlockAsync(blockerId, blockedId, CancellationToken.None);
        Assert.True(created.created);
        Assert.NotNull(created.block);

        var duplicate = await service.BlockAsync(blockerId, blockedId, CancellationToken.None);
        Assert.False(duplicate.created);
        Assert.Null(duplicate.block);
    }

    [Fact]
    public async Task FeedService_FiltersBlockedAuthors()
    {
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var featureFlags = new InMemoryFeatureFlagService();
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var mapRepository = new InMemoryMapRepository(dataStore);
        var service = new FeedService(feedRepository, accessEvaluator, featureFlags, auditLogger, blockRepository, mapRepository);

        var blockerId = dataStore.Users[1].Id;
        var blockedId = dataStore.Users[0].Id;

        await blockRepository.AddAsync(
            new Araponga.Domain.Moderation.UserBlock(blockerId, blockedId, DateTime.UtcNow),
            CancellationToken.None);

        var posts = await service.ListForTerritoryAsync(
            ActiveTerritoryId,
            blockerId,
            null,
            CancellationToken.None);

        Assert.Empty(posts);
    }

    [Fact]
    public async Task FeedService_CreatesPendingEventForVisitorAndApproves()
    {
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var featureFlags = new InMemoryFeatureFlagService();
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var mapRepository = new InMemoryMapRepository(dataStore);
        var service = new FeedService(feedRepository, accessEvaluator, featureFlags, auditLogger, blockRepository, mapRepository);

        var visitorId = Guid.NewGuid();
        var create = await service.CreatePostAsync(
            ActiveTerritoryId,
            visitorId,
            "Evento visitante",
            "Detalhes",
            PostType.Event,
            PostVisibility.Public,
            PostStatus.PendingApproval,
            null,
            CancellationToken.None);

        Assert.True(create.success);
        Assert.Equal(PostStatus.PendingApproval, create.post!.Status);

        var approve = await service.ApproveEventAsync(
            ActiveTerritoryId,
            create.post.Id,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            PostStatus.Published,
            CancellationToken.None);

        Assert.True(approve.success);
    }

    [Fact]
    public async Task FeedService_FiltersByMapEntity()
    {
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var featureFlags = new InMemoryFeatureFlagService();
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var mapRepository = new InMemoryMapRepository(dataStore);
        var service = new FeedService(feedRepository, accessEvaluator, featureFlags, auditLogger, blockRepository, mapRepository);

        var entityId = dataStore.MapEntities[0].Id;
        var post = new CommunityPost(
            Guid.NewGuid(),
            ActiveTerritoryId,
            Guid.NewGuid(),
            "Post com entidade",
            "Conteudo",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            entityId,
            DateTime.UtcNow);

        await feedRepository.AddPostAsync(post, CancellationToken.None);

        var filtered = await service.ListForTerritoryAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            entityId,
            CancellationToken.None);

        Assert.Contains(filtered, item => item.MapEntityId == entityId);
    }

    [Fact]
    public async Task MembershipService_ReturnsStatusAndValidates()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var service = new MembershipService(repository, auditLogger);

        var status = await service.GetStatusAsync(Guid.NewGuid(), ActiveTerritoryId, CancellationToken.None);
        Assert.Null(status);

        var membership = await service.DeclareMembershipAsync(
            Guid.NewGuid(),
            ActiveTerritoryId,
            MembershipRole.Resident,
            CancellationToken.None);

        await service.ValidateAsync(
            membership.Id,
            Guid.NewGuid(),
            ActiveTerritoryId,
            VerificationStatus.Validated,
            CancellationToken.None);

        var updated = await repository.GetByUserAndTerritoryAsync(membership.UserId, ActiveTerritoryId, CancellationToken.None);
        Assert.Equal(VerificationStatus.Validated, updated!.VerificationStatus);
    }
}
