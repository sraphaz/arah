using Araponga.Application.Events;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Events;
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
    private static readonly IEventBus EventBus = new NoOpEventBus();

    [Fact]
    public async Task FeedService_ValidatesInputsAndFlags()
    {
        var dataStore = new InMemoryDataStore();
        var service = FeedServiceTestHelper.CreateFeedService(dataStore, EventBus);

        var invalid = await service.CreatePostAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "",
            "",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            new List<GeoAnchorInput>
            {
                new(-23.0, -45.0, "POST")
            },
            null,
            CancellationToken.None);

        Assert.False(invalid.IsSuccess);

        var disabledAlert = await service.CreatePostAsync(
            PilotTerritoryId,
            Guid.NewGuid(),
            "Titulo",
            "Conteudo",
            PostType.Alert,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            new List<GeoAnchorInput>
            {
                new(-23.0, -45.0, "POST")
            },
            null,
            CancellationToken.None);

        Assert.False(disabledAlert.IsSuccess);

        var likeMissing = await service.LikeAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "session:1",
            null,
            CancellationToken.None);

        Assert.False(likeMissing);

        var feedRepository = new InMemoryFeedRepository(dataStore);
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
        var service = FeedServiceTestHelper.CreateFeedService(dataStore, EventBus);
        var feedRepository = new InMemoryFeedRepository(dataStore);

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

        Assert.False(comment.IsSuccess);

        var share = await service.ShareAsync(
            ActiveTerritoryId,
            post.Id,
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.False(share.IsSuccess);
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
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MapService(repository, accessEvaluator, auditLogger, blockRepository, relationRepository, unitOfWork);

        var suggestion = await service.SuggestAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "Lugar",
            "espaço natural",
            -23.0,
            -45.0,
            CancellationToken.None);

        Assert.True(suggestion.IsSuccess);

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

        Assert.False(confirm.IsSuccess);
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
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MapService(repository, accessEvaluator, auditLogger, blockRepository, relationRepository, unitOfWork);

        var residentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var entityId = dataStore.MapEntities[0].Id;

        var relation = await service.RelateAsync(
            ActiveTerritoryId,
            entityId,
            residentId,
            CancellationToken.None);

        Assert.True(relation.IsSuccess);
        Assert.NotNull(relation.Value);

        var duplicate = await service.RelateAsync(
            ActiveTerritoryId,
            entityId,
            residentId,
            CancellationToken.None);

        Assert.False(duplicate.IsSuccess);
        Assert.Equal("Relation already exists.", duplicate.Error);
    }

    [Fact]
    public async Task HealthService_ValidatesAlerts()
    {
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var alertRepository = new InMemoryHealthAlertRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new HealthService(alertRepository, feedRepository, auditLogger, unitOfWork);

        var report = await service.ReportAlertAsync(
            ActiveTerritoryId,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Titulo",
            "Desc",
            CancellationToken.None);

        Assert.True(report.IsSuccess);

        var validated = await service.ValidateAlertAsync(
            ActiveTerritoryId,
            report.Value!.Id,
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
        var sanctionRepository = new InMemorySanctionRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new ReportService(reportRepository, feedRepository, userRepository, sanctionRepository, auditLogger, EventBus, unitOfWork);

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
        var sanctionRepository = new InMemorySanctionRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new ReportService(reportRepository, feedRepository, userRepository, sanctionRepository, auditLogger, EventBus, unitOfWork);

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
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new UserBlockService(blockRepository, userRepository, auditLogger, unitOfWork);

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
        var service = FeedServiceTestHelper.CreateFeedService(dataStore, EventBus);

        var blockerId = dataStore.Users[1].Id;
        var blockedId = dataStore.Users[0].Id;
        var blockRepository = new InMemoryUserBlockRepository(dataStore);

        await blockRepository.AddAsync(
            new Araponga.Domain.Moderation.UserBlock(blockerId, blockedId, DateTime.UtcNow),
            CancellationToken.None);

        var posts = await service.ListForTerritoryAsync(
            ActiveTerritoryId,
            blockerId,
            null,
            null,
            CancellationToken.None);

        Assert.Empty(posts);
    }

    [Fact]
    public async Task EventsService_SetsCreatedByMembershipForVisitor()
    {
        var dataStore = new InMemoryDataStore();
        var eventRepository = new InMemoryTerritoryEventRepository(dataStore);
        var participationRepository = new InMemoryEventParticipationRepository(dataStore);
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new EventsService(
            eventRepository,
            participationRepository,
            feedRepository,
            accessEvaluator,
            userRepository,
            unitOfWork);

        var visitorId = Guid.NewGuid();
        await userRepository.AddAsync(
            new User(
                visitorId,
                "Visitante",
                "visitante@araponga.com",
                null,
                "DOC-1",
                "(00) 90000-0000",
                "Rua 1",
                "google",
                "visitor-external",
                UserRole.Visitor,
                DateTime.UtcNow),
            CancellationToken.None);

        var create = await service.CreateEventAsync(
            ActiveTerritoryId,
            visitorId,
            "Evento visitante",
            "Detalhes",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(2),
            -23.0,
            -45.0,
            "Praça central",
            CancellationToken.None);

        Assert.True(create.IsSuccess);
        Assert.NotNull(create.Value);
        Assert.Equal(MembershipRole.Visitor, create.Value!.Event.CreatedByMembership);
    }

    [Fact]
    public async Task EventsService_SetsCreatedByMembershipForResident()
    {
        var dataStore = new InMemoryDataStore();
        var eventRepository = new InMemoryTerritoryEventRepository(dataStore);
        var participationRepository = new InMemoryEventParticipationRepository(dataStore);
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new EventsService(
            eventRepository,
            participationRepository,
            feedRepository,
            accessEvaluator,
            userRepository,
            unitOfWork);

        var residentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var create = await service.CreateEventAsync(
            ActiveTerritoryId,
            residentId,
            "Evento morador",
            "Detalhes",
            DateTime.UtcNow.AddDays(3),
            null,
            -23.1,
            -45.1,
            null,
            CancellationToken.None);

        Assert.True(create.IsSuccess);
        Assert.NotNull(create.Value);
        Assert.Equal(MembershipRole.Resident, create.Value!.Event.CreatedByMembership);
    }

    [Fact]
    public async Task EventsService_UpsertsParticipation()
    {
        var dataStore = new InMemoryDataStore();
        var eventRepository = new InMemoryTerritoryEventRepository(dataStore);
        var participationRepository = new InMemoryEventParticipationRepository(dataStore);
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new EventsService(
            eventRepository,
            participationRepository,
            feedRepository,
            accessEvaluator,
            userRepository,
            unitOfWork);

        var eventId = Guid.NewGuid();
        var territoryEvent = new TerritoryEvent(
            eventId,
            ActiveTerritoryId,
            "Mutirão",
            "Detalhes",
            DateTime.UtcNow.AddDays(5),
            null,
            -23.2,
            -45.2,
            null,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await eventRepository.AddAsync(territoryEvent, CancellationToken.None);

        var userId = Guid.NewGuid();
        await service.SetParticipationAsync(eventId, userId, EventParticipationStatus.Interested, CancellationToken.None);
        await service.SetParticipationAsync(eventId, userId, EventParticipationStatus.Confirmed, CancellationToken.None);

        Assert.Single(dataStore.EventParticipations);
        Assert.Equal(EventParticipationStatus.Confirmed, dataStore.EventParticipations[0].Status);
    }

    [Fact]
    public async Task EventsService_ListsByTerritoryAndCounts()
    {
        var dataStore = new InMemoryDataStore();
        dataStore.TerritoryEvents.Clear();
        var eventRepository = new InMemoryTerritoryEventRepository(dataStore);
        var participationRepository = new InMemoryEventParticipationRepository(dataStore);
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new EventsService(
            eventRepository,
            participationRepository,
            feedRepository,
            accessEvaluator,
            userRepository,
            unitOfWork);

        var eventA = new TerritoryEvent(
            Guid.NewGuid(),
            ActiveTerritoryId,
            "Evento A",
            null,
            DateTime.UtcNow.AddDays(1),
            null,
            -23.2,
            -45.2,
            null,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        var eventB = new TerritoryEvent(
            Guid.NewGuid(),
            ActiveTerritoryId,
            "Evento B",
            null,
            DateTime.UtcNow.AddDays(10),
            null,
            -23.25,
            -45.25,
            null,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await eventRepository.AddAsync(eventA, CancellationToken.None);
        await eventRepository.AddAsync(eventB, CancellationToken.None);

        await participationRepository.UpsertAsync(
            new EventParticipation(eventA.Id, Guid.NewGuid(), EventParticipationStatus.Confirmed, DateTime.UtcNow, DateTime.UtcNow),
            CancellationToken.None);
        await participationRepository.UpsertAsync(
            new EventParticipation(eventA.Id, Guid.NewGuid(), EventParticipationStatus.Interested, DateTime.UtcNow, DateTime.UtcNow),
            CancellationToken.None);

        var results = await service.ListEventsAsync(
            ActiveTerritoryId,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(2),
            null,
            CancellationToken.None);

        Assert.Single(results);
        var summary = results[0];
        Assert.Equal(eventA.Id, summary.Event.Id);
        Assert.Equal(1, summary.ConfirmedCount);
        Assert.Equal(1, summary.InterestedCount);
    }

    [Fact]
    public async Task EventsService_FiltersNearbyEvents()
    {
        var dataStore = new InMemoryDataStore();
        var eventRepository = new InMemoryTerritoryEventRepository(dataStore);
        var participationRepository = new InMemoryEventParticipationRepository(dataStore);
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new EventsService(
            eventRepository,
            participationRepository,
            feedRepository,
            accessEvaluator,
            userRepository,
            unitOfWork);

        var nearbyEvent = new TerritoryEvent(
            Guid.NewGuid(),
            ActiveTerritoryId,
            "Evento perto",
            null,
            DateTime.UtcNow.AddDays(1),
            null,
            -23.3501,
            -44.8912,
            null,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        var farEvent = new TerritoryEvent(
            Guid.NewGuid(),
            ActiveTerritoryId,
            "Evento longe",
            null,
            DateTime.UtcNow.AddDays(1),
            null,
            -22.0,
            -44.0,
            null,
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await eventRepository.AddAsync(nearbyEvent, CancellationToken.None);
        await eventRepository.AddAsync(farEvent, CancellationToken.None);

        var results = await service.GetEventsNearbyAsync(
            -23.35,
            -44.89,
            5,
            null,
            null,
            ActiveTerritoryId,
            CancellationToken.None);

        Assert.Single(results);
        Assert.Equal(nearbyEvent.Id, results[0].Event.Id);
    }

    [Fact]
    public async Task FeedService_AllowsPostsWithoutGeoAnchors()
    {
        var dataStore = new InMemoryDataStore();
        var service = FeedServiceTestHelper.CreateFeedService(dataStore, EventBus);

        var created = await service.CreatePostAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "Post sem geo",
            "Conteudo",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Value);
        Assert.DoesNotContain(dataStore.PostGeoAnchors, anchor => anchor.PostId == created.Value!.Id);
    }

    [Fact]
    public async Task FeedService_PersistsGeoAnchorsFromMediaMetadata()
    {
        var dataStore = new InMemoryDataStore();
        var service = FeedServiceTestHelper.CreateFeedService(dataStore, EventBus);

        var created = await service.CreatePostAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "Post com midia",
            "Conteudo",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            new List<GeoAnchorInput>
            {
                new(-23.3701, -45.0201, "POST")
            },
            null,
            CancellationToken.None);

        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Value);

        var anchors = dataStore.PostGeoAnchors.Where(anchor => anchor.PostId == created.Value!.Id).ToList();
        Assert.Single(anchors);
        Assert.Equal("POST", anchors[0].Type);
    }

    [Fact]
    public async Task FeedService_DeduplicatesAndLimitsGeoAnchors()
    {
        var dataStore = new InMemoryDataStore();
        var service = FeedServiceTestHelper.CreateFeedService(dataStore, EventBus);

        var anchors = new List<GeoAnchorInput>
        {
            new(-23.123456, -45.123456, "POST"),
            new(-23.123459, -45.123459, "POST")
        };

        for (var i = 0; i < 60; i += 1)
        {
            anchors.Add(new GeoAnchorInput(-23.0 - i * 0.01, -45.0 - i * 0.01, "POST"));
        }

        var created = await service.CreatePostAsync(
            ActiveTerritoryId,
            Guid.NewGuid(),
            "Post com muitas midias",
            "Conteudo",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            anchors,
            null,
            CancellationToken.None);

        Assert.True(created.IsSuccess);
        Assert.NotNull(created.Value);

        var savedAnchors = dataStore.PostGeoAnchors.Where(anchor => anchor.PostId == created.Value!.Id).ToList();
        Assert.Equal(50, savedAnchors.Count);
    }

    [Fact]
    public async Task FeedService_FiltersByMapEntity()
    {
        var dataStore = new InMemoryDataStore();
        var service = FeedServiceTestHelper.CreateFeedService(dataStore, EventBus);

        var entityId = dataStore.MapEntities[0].Id;
        var feedRepository = new InMemoryFeedRepository(dataStore);
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
            null,
            CancellationToken.None);

        Assert.Contains(filtered, item => item.MapEntityId == entityId);
    }

    [Fact]
    public async Task MembershipService_ReturnsStatusAndValidates()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

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

    [Fact]
    public async Task MembershipService_AllowsVisitorUpgradeToResident()
    {
        var dataStore = new InMemoryDataStore();
        var repository = new InMemoryTerritoryMembershipRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new MembershipService(repository, auditLogger, unitOfWork);

        var userId = Guid.NewGuid();

        var visitor = await service.DeclareMembershipAsync(
            userId,
            ActiveTerritoryId,
            MembershipRole.Visitor,
            CancellationToken.None);

        var upgraded = await service.DeclareMembershipAsync(
            userId,
            ActiveTerritoryId,
            MembershipRole.Resident,
            CancellationToken.None);

        Assert.Equal(visitor.Id, upgraded.Id);
        Assert.Equal(MembershipRole.Resident, upgraded.Role);
        Assert.Equal(VerificationStatus.Pending, upgraded.VerificationStatus);
    }

    [Fact]
    public async Task ReportService_AppliesThresholdsForPostAndUser()
    {
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var reportRepository = new InMemoryReportRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var sanctionRepository = new InMemorySanctionRepository(dataStore);
        var service = new ReportService(
            reportRepository,
            feedRepository,
            userRepository,
            sanctionRepository,
            auditLogger,
            EventBus,
            new InMemoryUnitOfWork());

        var postId = dataStore.Posts[0].Id;

        for (var i = 0; i < 3; i += 1)
        {
            var result = await service.ReportPostAsync(
                Guid.NewGuid(),
                postId,
                "SPAM",
                null,
                CancellationToken.None);
            Assert.True(result.created);
        }

        var post = await feedRepository.GetPostAsync(postId, CancellationToken.None);
        Assert.NotNull(post);
        Assert.Equal(PostStatus.Hidden, post!.Status);

        var userId = dataStore.Users[0].Id;
        for (var i = 0; i < 3; i += 1)
        {
            var result = await service.ReportUserAsync(
                Guid.NewGuid(),
                ActiveTerritoryId,
                userId,
                "SPAM",
                null,
                CancellationToken.None);
            Assert.True(result.created);
        }

        Assert.Contains(dataStore.Sanctions, sanction =>
            sanction.TargetId == userId &&
            sanction.Type == Araponga.Domain.Moderation.SanctionType.PostingRestriction);
    }

    [Fact]
    public async Task FeedService_BlocksPostingWhenSanctioned()
    {
        var dataStore = new InMemoryDataStore();
        var feedRepository = new InMemoryFeedRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(new InMemoryTerritoryMembershipRepository(dataStore));
        var featureFlags = new InMemoryFeatureFlagService();
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var blockRepository = new InMemoryUserBlockRepository(dataStore);
        var mapRepository = new InMemoryMapRepository(dataStore);
        var geoAnchorRepository = new InMemoryPostGeoAnchorRepository(dataStore);
        var postAssetRepository = new InMemoryPostAssetRepository(dataStore);
        var assetRepository = new InMemoryAssetRepository(dataStore);
        var sanctionRepository = new InMemorySanctionRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var restrictedUserId = dataStore.Users[0].Id;
        await sanctionRepository.AddAsync(
            new Araponga.Domain.Moderation.Sanction(
                Guid.NewGuid(),
                ActiveTerritoryId,
                Araponga.Domain.Moderation.SanctionScope.Territory,
                Araponga.Domain.Moderation.SanctionTargetType.User,
                restrictedUserId,
                Araponga.Domain.Moderation.SanctionType.PostingRestriction,
                "Threshold",
                Araponga.Domain.Moderation.SanctionStatus.Active,
                DateTime.UtcNow.AddMinutes(-5),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow),
            CancellationToken.None);

        var service = FeedServiceTestHelper.CreateFeedService(dataStore, EventBus);

        var result = await service.CreatePostAsync(
            ActiveTerritoryId,
            restrictedUserId,
            "Post bloqueado",
            "Conteudo",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            new List<GeoAnchorInput>
            {
                new(-23.0, -45.0, "POST")
            },
            null,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}
