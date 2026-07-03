using Arah.Domain.Membership;
using Arah.Domain.Users;
using Arah.Infrastructure.Postgres.Entities;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
    public static TerritoryMembershipRecord ToRecord(this TerritoryMembership membership)
    {
        return new TerritoryMembershipRecord
        {
            Id = membership.Id,
            UserId = membership.UserId,
            TerritoryId = membership.TerritoryId,
            Role = membership.Role,
            ResidencyVerification = membership.ResidencyVerification,
            LastGeoVerifiedAtUtc = membership.LastGeoVerifiedAtUtc,
            LastDocumentVerifiedAtUtc = membership.LastDocumentVerifiedAtUtc,
            CreatedAtUtc = membership.CreatedAtUtc,
            // Garante valor não nulo no INSERT; coluna NOT NULL e IsRowVersion() pode fazer o EF omitir no insert.
            RowVersion = new byte[8]
        };
    }

    public static TerritoryMembership ToDomain(this TerritoryMembershipRecord record)
    {
        return new TerritoryMembership(
            record.Id,
            record.UserId,
            record.TerritoryId,
            record.Role,
            record.ResidencyVerification,
            record.LastGeoVerifiedAtUtc,
            record.LastDocumentVerifiedAtUtc,
            record.CreatedAtUtc);
    }

    public static MembershipSettingsRecord ToRecord(this MembershipSettings settings)
    {
        return new MembershipSettingsRecord
        {
            MembershipId = settings.MembershipId,
            MarketplaceOptIn = settings.MarketplaceOptIn,
            CreatedAtUtc = settings.CreatedAtUtc,
            UpdatedAtUtc = settings.UpdatedAtUtc
        };
    }

    public static MembershipSettings ToDomain(this MembershipSettingsRecord record)
    {
        return new MembershipSettings(
            record.MembershipId,
            record.MarketplaceOptIn,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static MembershipCapabilityRecord ToRecord(this MembershipCapability capability)
    {
        return new MembershipCapabilityRecord
        {
            Id = capability.Id,
            MembershipId = capability.MembershipId,
            CapabilityType = capability.CapabilityType,
            GrantedAtUtc = capability.GrantedAtUtc,
            RevokedAtUtc = capability.RevokedAtUtc,
            GrantedByUserId = capability.GrantedByUserId,
            GrantedByMembershipId = capability.GrantedByMembershipId,
            Reason = capability.Reason
        };
    }

    public static MembershipCapability ToDomain(this MembershipCapabilityRecord record)
    {
        return new MembershipCapability(
            record.Id,
            record.MembershipId,
            record.CapabilityType,
            record.GrantedAtUtc,
            record.GrantedByUserId,
            record.GrantedByMembershipId,
            record.Reason);
    }

    public static SystemPermissionRecord ToRecord(this SystemPermission permission)
    {
        return new SystemPermissionRecord
        {
            Id = permission.Id,
            UserId = permission.UserId,
            PermissionType = permission.PermissionType,
            GrantedAtUtc = permission.GrantedAtUtc,
            GrantedByUserId = permission.GrantedByUserId,
            RevokedAtUtc = permission.RevokedAtUtc,
            RevokedByUserId = permission.RevokedByUserId
        };
    }

    public static SystemPermission ToDomain(this SystemPermissionRecord record)
    {
        return new SystemPermission(
            record.Id,
            record.UserId,
            record.PermissionType,
            record.GrantedAtUtc,
            record.GrantedByUserId,
            record.RevokedAtUtc,
            record.RevokedByUserId);
    }
}
