using Araponga.Domain.Feed;
using Araponga.Domain.Health;
using Araponga.Domain.Map;
using Araponga.Domain.Moderation;
using Araponga.Domain.Social;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryDataStore
{
    public InMemoryDataStore()
    {
        var territoryA = new Territory(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Sertão do Camburi",
            "Território canônico para comunidade local.",
            TerritoryStatus.Active,
            "Ubatuba",
            "SP",
            -23.3501,
            -44.8912,
            DateTime.UtcNow);

        var territoryB = new Territory(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Vale do Itamambuca",
            "Território canônico para operações comunitárias.",
            TerritoryStatus.Active,
            "Ubatuba",
            "SP",
            -23.3744,
            -45.0205,
            DateTime.UtcNow);

        var territoryC = new Territory(
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            "Reserva do Silêncio",
            "Território inativo para testes.",
            TerritoryStatus.Inactive,
            "Paraty",
            "RJ",
            -23.2190,
            -44.7170,
            DateTime.UtcNow);

        Territories = new List<Territory> { territoryA, territoryB, territoryC };

        var residentUser = new User(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Morador Teste",
            "morador@araponga.com",
            "123.456.789-00",
            null,
            "(11) 99999-0000",
            "Rua das Flores, 100",
            "google",
            "resident-external",
            UserRole.Resident,
            DateTime.UtcNow);

        var curatorUser = new User(
            Guid.Parse("cccccccc-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Curador",
            "curador@araponga.com",
            null,
            "PASS-987654",
            "(21) 98888-0000",
            "Avenida Central, 200",
            "google",
            "curator-external",
            UserRole.Curator,
            DateTime.UtcNow);

        Users = new List<User> { residentUser, curatorUser };

        Memberships = new List<TerritoryMembership>
        {
            new(
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                residentUser.Id,
                territoryB.Id,
                MembershipRole.Resident,
                VerificationStatus.Validated,
                DateTime.UtcNow)
        };

        Posts = new List<CommunityPost>
        {
            new(
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                territoryB.Id,
                residentUser.Id,
                "Bem-vindos ao Vale!",
                "Post público de boas-vindas.",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow),
            new(
                Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                territoryB.Id,
                residentUser.Id,
                "Reunião de moradores",
                "Encontro exclusivo para moradores.",
                PostType.Event,
                PostVisibility.ResidentsOnly,
                PostStatus.Published,
                null,
                DateTime.UtcNow)
        };

        PostGeoAnchors = new List<PostGeoAnchor>
        {
            new(
                Guid.Parse("abababab-abab-abab-abab-abababababab"),
                Posts[0].Id,
                -23.3748,
                -45.0209,
                "POST",
                DateTime.UtcNow),
            new(
                Guid.Parse("cdcdcdcd-cdcd-cdcd-cdcd-cdcdcdcdcdcd"),
                Posts[1].Id,
                -23.3732,
                -45.0184,
                "EVENT",
                DateTime.UtcNow)
        };

        MapEntities = new List<MapEntity>
        {
            new(
                Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                territoryB.Id,
                residentUser.Id,
                "Cachoeira do Vale",
                "espaço natural",
                -23.3723,
                -45.0193,
                MapEntityStatus.Validated,
                MapEntityVisibility.Public,
                5,
                DateTime.UtcNow),
            new(
                Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                territoryB.Id,
                residentUser.Id,
                "Nascente Secreta",
                "espaço natural",
                -23.3751,
                -45.0179,
                MapEntityStatus.Validated,
                MapEntityVisibility.ResidentsOnly,
                2,
                DateTime.UtcNow)
        };

        HealthAlerts = new List<HealthAlert>();
    }

    public List<Territory> Territories { get; }
    public List<User> Users { get; }
    public List<TerritoryMembership> Memberships { get; }
    public List<UserTerritory> UserTerritories { get; } = new();
    public List<CommunityPost> Posts { get; }
    public List<MapEntity> MapEntities { get; }
    public List<MapEntityRelation> MapEntityRelations { get; } = new();
    public List<PostGeoAnchor> PostGeoAnchors { get; }
    public List<HealthAlert> HealthAlerts { get; }
    public Dictionary<string, Guid> ActiveTerritories { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<Guid, HashSet<string>> PostLikes { get; } = new();
    public Dictionary<Guid, List<PostComment>> PostComments { get; } = new();
    public Dictionary<Guid, HashSet<Guid>> PostShares { get; } = new();
    public List<Application.Models.AuditEntry> AuditEntries { get; } = new();
    public List<ModerationReport> ModerationReports { get; } = new();
    public List<UserBlock> UserBlocks { get; } = new();
    public List<Sanction> Sanctions { get; } = new();
}
