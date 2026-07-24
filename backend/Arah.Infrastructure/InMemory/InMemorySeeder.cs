using Arah.Domain.Events;
using Arah.Domain.Feed;
using Arah.Domain.Financial;
using Arah.Modules.Map.Domain;
using Arah.Domain.Membership;
using Arah.Domain.Subscriptions;
using Arah.Domain.Territories;
using Arah.Domain.Users;

namespace Arah.Infrastructure.InMemory;

/// <summary>
/// Popula um <see cref="InMemoryDataStore"/> com os dados de demonstração/padrão
/// usados nos cenários in-memory e de teste. Separado do store para isolar a
/// responsabilidade de "semear" da responsabilidade de "guardar coleções".
/// </summary>
internal static class InMemorySeeder
{
    public static void Seed(InMemoryDataStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        var territoryA = new Territory(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            null,
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
            null,
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
            null,
            "Reserva do Silêncio",
            "Território inativo para testes.",
            TerritoryStatus.Inactive,
            "Paraty",
            "RJ",
            -23.2190,
            -44.7170,
            DateTime.UtcNow);

        store.Territories.Add(territoryA);
        store.Territories.Add(territoryB);
        store.Territories.Add(territoryC);

        var residentUser = new User(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Morador Teste",
            "morador@Arah.com",
            "123.456.789-00",
            null,
            "(11) 99999-0000",
            "Rua das Flores, 100",
            "google",
            "resident-external",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            null,
            DateTime.UtcNow);

        var curatorUser = new User(
            Guid.Parse("cccccccc-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Curador",
            "curador@Arah.com",
            null,
            "PASS-987654",
            "(21) 98888-0000",
            "Avenida Central, 200",
            "google",
            "curator-external",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            null,
            DateTime.UtcNow);

        var adminUser = new User(
            Guid.Parse("ffffffff-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "System Admin",
            "admin@Arah.com",
            null,
            "PASS-ADMIN",
            "(11) 97777-0000",
            "Admin Address",
            "google",
            "admin-external",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            null,
            DateTime.UtcNow);

        store.Users.Add(residentUser);
        store.Users.Add(curatorUser);
        store.Users.Add(adminUser);

        var membershipId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        store.Memberships.Add(new TerritoryMembership(
            membershipId,
            residentUser.Id,
            territoryB.Id,
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow));

        // No ambiente de testes/in-memory, habilitar opt-in por padrão
        // para manter cenários de marketplace funcionais.
        store.MembershipSettings.Add(new MembershipSettings(
            membershipId,
            marketplaceOptIn: true,
            DateTime.UtcNow,
            DateTime.UtcNow));

        // Curador precisa ter Membership no território para checagem de capability
        var curatorMembershipIdTerritoryB = Guid.Parse("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        store.Memberships.Add(new TerritoryMembership(
            curatorMembershipIdTerritoryB,
            curatorUser.Id,
            territoryB.Id,
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow));

        store.MembershipSettings.Add(new MembershipSettings(
            curatorMembershipIdTerritoryB,
            marketplaceOptIn: false,
            DateTime.UtcNow,
            DateTime.UtcNow));

        var curatorMembershipIdTerritoryA = Guid.Parse("dddddddd-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        store.Memberships.Add(new TerritoryMembership(
            curatorMembershipIdTerritoryA,
            curatorUser.Id,
            territoryA.Id,
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow));

        store.MembershipSettings.Add(new MembershipSettings(
            curatorMembershipIdTerritoryA,
            marketplaceOptIn: false,
            DateTime.UtcNow,
            DateTime.UtcNow));

        store.MembershipCapabilities.Add(new MembershipCapability(
            Guid.Parse("eeeeeeee-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            curatorMembershipIdTerritoryB,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            curatorUser.Id,
            curatorMembershipIdTerritoryB,
            "Seeded curator capability for in-memory tests"));
        store.MembershipCapabilities.Add(new MembershipCapability(
            Guid.Parse("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            curatorMembershipIdTerritoryA,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            curatorUser.Id,
            curatorMembershipIdTerritoryA,
            "Seeded curator capability for in-memory tests"));

        // Adicionar SystemPermission para adminUser
        store.SystemPermissions.Add(new SystemPermission(
            Guid.Parse("aaaaaaaa-ffff-ffff-ffff-ffffffffffff"),
            adminUser.Id,
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            null,
            null,
            null));

        // Inicializar plano FREE padrão para testes
        var freePlanId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        store.SubscriptionPlans.Add(new SubscriptionPlan(
            freePlanId,
            "FREE",
            "Plano gratuito com funcionalidades básicas sempre disponíveis",
            SubscriptionPlanTier.FREE,
            PlanScope.Global,
            null, // TerritoryId = null para plano global
            0m, // Preço zero
            null, // BillingCycle = null para FREE
            new List<FeatureCapability>
            {
                FeatureCapability.FeedBasic,
                FeatureCapability.PostsBasic,
                FeatureCapability.EventsBasic,
                FeatureCapability.MarketplaceBasic,
                FeatureCapability.ChatBasic
            },
            new Dictionary<string, object>
            {
                ["maxPosts"] = 10,
                ["maxEvents"] = 3,
                ["maxMarketplaceItems"] = 5,
                ["maxStorageMB"] = 100
            },
            isDefault: true,
            trialDays: null,
            createdByUserId: Guid.Empty, // Sistema
            stripePriceId: null,
            stripeProductId: null));

        var commercialLojaPlanId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        store.SubscriptionPlans.Add(new SubscriptionPlan(
            commercialLojaPlanId,
            "LOJA",
            "Plano comercial para vender no marketplace (FASE55)",
            SubscriptionPlanTier.BASIC,
            PlanScope.Territory,
            territoryB.Id,
            49.90m,
            SubscriptionBillingCycle.MONTHLY,
            new List<FeatureCapability>
            {
                FeatureCapability.MarketplaceBasic,
                FeatureCapability.MarketplaceAdvanced,
            },
            new Dictionary<string, object> { ["maxMarketplaceItems"] = 100 },
            isDefault: false,
            trialDays: 14,
            createdByUserId: Guid.Empty,
            stripePriceId: null,
            stripeProductId: null));

        foreach (var territory in store.Territories.Where(t => t.Status == TerritoryStatus.Active))
        {
            store.FeeSplitRules.Add(new FeeSplitRule(
                Guid.NewGuid(),
                territory.Id,
                "marketplace",
                implementerSharePercent: 40m,
                territoryFundSharePercent: 30m,
                platformSharePercent: 30m,
                effectiveFromUtc: DateTimeOffset.UtcNow.AddYears(-1)));
        }

        store.TerritoryEvents.Add(new TerritoryEvent(
            Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            territoryB.Id,
            "Reunião de moradores",
            "Encontro exclusivo para moradores.",
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(2).AddHours(2),
            -23.3732,
            -45.0184,
            "Praça do Vale",
            residentUser.Id,
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow));

        store.Posts.Add(new CommunityPost(
            Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            territoryB.Id,
            residentUser.Id,
            "Bem-vindos ao Vale!",
            "Post público de boas-vindas.",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow));
        store.Posts.Add(new CommunityPost(
            Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            territoryB.Id,
            residentUser.Id,
            "Reunião de moradores",
            "Encontro exclusivo para moradores.",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            "EVENT",
            store.TerritoryEvents[0].Id));

        store.PostGeoAnchors.Add(new PostGeoAnchor(
            Guid.Parse("abababab-abab-abab-abab-abababababab"),
            store.Posts[0].Id,
            -23.3748,
            -45.0209,
            "POST",
            DateTime.UtcNow));
        store.PostGeoAnchors.Add(new PostGeoAnchor(
            Guid.Parse("cdcdcdcd-cdcd-cdcd-cdcd-cdcdcdcdcdcd"),
            store.Posts[1].Id,
            -23.3732,
            -45.0184,
            "POST",
            DateTime.UtcNow));

        store.MapEntities.Add(new MapEntity(
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
            DateTime.UtcNow));
        store.MapEntities.Add(new MapEntity(
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
            DateTime.UtcNow));
    }
}
