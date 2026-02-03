using Araponga.Domain.Configuration;
using Araponga.Domain.Governance;
using Araponga.Domain.Membership;
using Araponga.Domain.Policies;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>
/// Store em memória com os mesmos agregados que SharedDbContext (fonte da verdade shared em modo InMemory).
/// Usado por repositórios InMemory core em testes e quando Persistence:Provider = InMemory.
/// </summary>
public sealed class InMemorySharedStore
{
    public InMemorySharedStore()
    {
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
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
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
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);

        var adminUser = new User(
            Guid.Parse("ffffffff-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "System Admin",
            "admin@araponga.com",
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
            DateTime.UtcNow);

        Users = new List<User> { residentUser, curatorUser, adminUser };

        var membershipId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        Memberships = new List<TerritoryMembership>
        {
            new(
                membershipId,
                residentUser.Id,
                territoryB.Id,
                MembershipRole.Resident,
                ResidencyVerification.GeoVerified,
                DateTime.UtcNow,
                null,
                DateTime.UtcNow)
        };

        MembershipSettings = new List<MembershipSettings>
        {
            new(membershipId, marketplaceOptIn: true, DateTime.UtcNow, DateTime.UtcNow)
        };

        var curatorMembershipIdTerritoryB = Guid.Parse("dddddddd-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        Memberships.Add(new TerritoryMembership(
            curatorMembershipIdTerritoryB,
            curatorUser.Id,
            territoryB.Id,
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow));

        MembershipSettings.Add(new MembershipSettings(
            curatorMembershipIdTerritoryB,
            marketplaceOptIn: false,
            DateTime.UtcNow,
            DateTime.UtcNow));

        var curatorMembershipIdTerritoryA = Guid.Parse("dddddddd-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        Memberships.Add(new TerritoryMembership(
            curatorMembershipIdTerritoryA,
            curatorUser.Id,
            territoryA.Id,
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow));

        MembershipSettings.Add(new MembershipSettings(
            curatorMembershipIdTerritoryA,
            marketplaceOptIn: false,
            DateTime.UtcNow,
            DateTime.UtcNow));

        MembershipCapabilities = new List<MembershipCapability>
        {
            new(
                Guid.Parse("eeeeeeee-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                curatorMembershipIdTerritoryB,
                MembershipCapabilityType.Curator,
                DateTime.UtcNow,
                curatorUser.Id,
                curatorMembershipIdTerritoryB,
                "Seeded curator capability for in-memory tests"),
            new(
                Guid.Parse("eeeeeeee-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                curatorMembershipIdTerritoryA,
                MembershipCapabilityType.Curator,
                DateTime.UtcNow,
                curatorUser.Id,
                curatorMembershipIdTerritoryA,
                "Seeded curator capability for in-memory tests")
        };

        SystemPermissions = new List<SystemPermission>
        {
            new(
                Guid.Parse("aaaaaaaa-ffff-ffff-ffff-ffffffffffff"),
                adminUser.Id,
                SystemPermissionType.SystemAdmin,
                DateTime.UtcNow,
                null,
                null,
                null)
        };

        TerritoryJoinRequests = new List<TerritoryJoinRequest>();
        TerritoryJoinRequestRecipients = new List<TerritoryJoinRequestRecipient>();
        UserPreferences = new List<UserPreferences>();
        UserInterests = new List<UserInterest>();
        Votings = new List<Voting>();
        Votes = new List<Vote>();
        TerritoryCharacterizations = new List<TerritoryCharacterization>();
        TermsOfServices = new List<TermsOfService>();
        TermsAcceptances = new List<TermsAcceptance>();
        PrivacyPolicies = new List<PrivacyPolicy>();
        PrivacyPolicyAcceptances = new List<PrivacyPolicyAcceptance>();
        SystemConfigs = new List<SystemConfig>();
        UserDevices = new List<UserDevice>();
    }

    public List<Territory> Territories { get; }
    public List<User> Users { get; }
    public List<TerritoryMembership> Memberships { get; }
    public List<MembershipSettings> MembershipSettings { get; }
    public List<MembershipCapability> MembershipCapabilities { get; }
    public List<SystemPermission> SystemPermissions { get; }
    public List<TerritoryJoinRequest> TerritoryJoinRequests { get; }
    public List<TerritoryJoinRequestRecipient> TerritoryJoinRequestRecipients { get; }
    public List<UserPreferences> UserPreferences { get; }
    public List<UserInterest> UserInterests { get; }
    public List<Voting> Votings { get; }
    public List<Vote> Votes { get; }
    public List<TerritoryCharacterization> TerritoryCharacterizations { get; }
    public List<TermsOfService> TermsOfServices { get; }
    public List<TermsAcceptance> TermsAcceptances { get; }
    public List<PrivacyPolicy> PrivacyPolicies { get; }
    public List<PrivacyPolicyAcceptance> PrivacyPolicyAcceptances { get; }
    public List<SystemConfig> SystemConfigs { get; }
    public List<UserDevice> UserDevices { get; }
}
