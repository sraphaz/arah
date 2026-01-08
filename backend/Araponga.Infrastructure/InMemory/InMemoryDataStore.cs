using Araponga.Domain.Feed;
using Araponga.Domain.Map;
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
            "Território piloto para comunidade local, entidades do mapa e saúde do território.",
            SensitivityLevel.High,
            TerritoryStatus.Pilot,
            DateTime.UtcNow);

        var territoryB = new Territory(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Vale do Itamambuca",
            "Território ativo para operações comunitárias.",
            SensitivityLevel.Medium,
            TerritoryStatus.Active,
            DateTime.UtcNow);

        var territoryC = new Territory(
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            "Reserva do Silêncio",
            "Território desativado para testes de visibilidade.",
            SensitivityLevel.High,
            TerritoryStatus.Inactive,
            DateTime.UtcNow);

        Territories = new List<Territory> { territoryA, territoryB, territoryC };

        var residentUser = new User(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Morador Teste",
            "morador@araponga.com",
            "google",
            "resident-external",
            DateTime.UtcNow);

        Users = new List<User> { residentUser };

        Memberships = new List<UserTerritory>
        {
            new(
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                residentUser.Id,
                territoryB.Id,
                MembershipStatus.Validated,
                DateTime.UtcNow)
        };

        Posts = new List<CommunityPost>
        {
            new(
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                territoryB.Id,
                "Bem-vindos ao Vale!",
                "Post público de boas-vindas.",
                PostVisibility.Public,
                DateTime.UtcNow),
            new(
                Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                territoryB.Id,
                "Reunião de moradores",
                "Encontro exclusivo para moradores.",
                PostVisibility.ResidentsOnly,
                DateTime.UtcNow)
        };

        MapEntities = new List<MapEntity>
        {
            new(
                Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                territoryB.Id,
                "Cachoeira do Vale",
                "Cachoeira",
                MapEntityVisibility.Public,
                DateTime.UtcNow),
            new(
                Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                territoryB.Id,
                "Nascente Secreta",
                "Nascente",
                MapEntityVisibility.ResidentsOnly,
                DateTime.UtcNow)
        };
    }

    public List<Territory> Territories { get; }
    public List<User> Users { get; }
    public List<UserTerritory> Memberships { get; }
    public List<CommunityPost> Posts { get; }
    public List<MapEntity> MapEntities { get; }
    public Dictionary<string, Guid> ActiveTerritories { get; } = new(StringComparer.OrdinalIgnoreCase);
}
