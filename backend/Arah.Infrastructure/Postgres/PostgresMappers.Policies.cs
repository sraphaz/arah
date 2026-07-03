using Arah.Domain.Policies;
using Arah.Infrastructure.Postgres.Entities;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
    // -----------------------
    // Policies
    // -----------------------

    public static TermsOfServiceRecord ToRecord(this TermsOfService terms)
    {
        return new TermsOfServiceRecord
        {
            Id = terms.Id,
            Version = terms.Version,
            Title = terms.Title,
            Content = terms.Content,
            EffectiveDate = terms.EffectiveDate,
            ExpirationDate = terms.ExpirationDate,
            IsActive = terms.IsActive,
            RequiredRoles = terms.RequiredRoles,
            RequiredCapabilities = terms.RequiredCapabilities,
            RequiredSystemPermissions = terms.RequiredSystemPermissions,
            CreatedAtUtc = terms.CreatedAtUtc,
            UpdatedAtUtc = terms.UpdatedAtUtc
        };
    }

    public static TermsOfService ToDomain(this TermsOfServiceRecord record)
    {
        var terms = new TermsOfService(
            record.Id,
            record.Version,
            record.Title,
            record.Content,
            record.EffectiveDate,
            record.ExpirationDate,
            record.IsActive,
            record.RequiredRoles,
            record.RequiredCapabilities,
            record.RequiredSystemPermissions,
            record.CreatedAtUtc);

        // Atualizar UpdatedAtUtc usando reflection
        var updatedAtProp = typeof(TermsOfService).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (updatedAtProp?.SetMethod != null)
        {
            updatedAtProp.SetValue(terms, record.UpdatedAtUtc);
        }

        return terms;
    }

    public static TermsAcceptanceRecord ToRecord(this TermsAcceptance acceptance)
    {
        return new TermsAcceptanceRecord
        {
            Id = acceptance.Id,
            UserId = acceptance.UserId,
            TermsOfServiceId = acceptance.TermsOfServiceId,
            AcceptedAtUtc = acceptance.AcceptedAtUtc,
            IpAddress = acceptance.IpAddress,
            UserAgent = acceptance.UserAgent,
            AcceptedVersion = acceptance.AcceptedVersion,
            IsRevoked = acceptance.IsRevoked,
            RevokedAtUtc = acceptance.RevokedAtUtc
        };
    }

    public static TermsAcceptance ToDomain(this TermsAcceptanceRecord record)
    {
        var acceptance = new TermsAcceptance(
            record.Id,
            record.UserId,
            record.TermsOfServiceId,
            record.AcceptedAtUtc,
            record.AcceptedVersion,
            record.IpAddress,
            record.UserAgent);

        // Atualizar IsRevoked e RevokedAtUtc usando reflection se necessário
        if (record.IsRevoked)
        {
            var isRevokedProp = typeof(TermsAcceptance).GetProperty("IsRevoked", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var revokedAtProp = typeof(TermsAcceptance).GetProperty("RevokedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (isRevokedProp?.SetMethod != null)
            {
                isRevokedProp.SetValue(acceptance, true);
            }
            if (revokedAtProp?.SetMethod != null && record.RevokedAtUtc.HasValue)
            {
                revokedAtProp.SetValue(acceptance, record.RevokedAtUtc.Value);
            }
        }

        return acceptance;
    }

    public static PrivacyPolicyRecord ToRecord(this PrivacyPolicy policy)
    {
        return new PrivacyPolicyRecord
        {
            Id = policy.Id,
            Version = policy.Version,
            Title = policy.Title,
            Content = policy.Content,
            EffectiveDate = policy.EffectiveDate,
            ExpirationDate = policy.ExpirationDate,
            IsActive = policy.IsActive,
            RequiredRoles = policy.RequiredRoles,
            RequiredCapabilities = policy.RequiredCapabilities,
            RequiredSystemPermissions = policy.RequiredSystemPermissions,
            CreatedAtUtc = policy.CreatedAtUtc,
            UpdatedAtUtc = policy.UpdatedAtUtc
        };
    }

    public static PrivacyPolicy ToDomain(this PrivacyPolicyRecord record)
    {
        var policy = new PrivacyPolicy(
            record.Id,
            record.Version,
            record.Title,
            record.Content,
            record.EffectiveDate,
            record.ExpirationDate,
            record.IsActive,
            record.RequiredRoles,
            record.RequiredCapabilities,
            record.RequiredSystemPermissions,
            record.CreatedAtUtc);

        // Atualizar UpdatedAtUtc usando reflection
        var updatedAtProp = typeof(PrivacyPolicy).GetProperty("UpdatedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (updatedAtProp?.SetMethod != null)
        {
            updatedAtProp.SetValue(policy, record.UpdatedAtUtc);
        }

        return policy;
    }

    public static PrivacyPolicyAcceptanceRecord ToRecord(this PrivacyPolicyAcceptance acceptance)
    {
        return new PrivacyPolicyAcceptanceRecord
        {
            Id = acceptance.Id,
            UserId = acceptance.UserId,
            PrivacyPolicyId = acceptance.PrivacyPolicyId,
            AcceptedAtUtc = acceptance.AcceptedAtUtc,
            IpAddress = acceptance.IpAddress,
            UserAgent = acceptance.UserAgent,
            AcceptedVersion = acceptance.AcceptedVersion,
            IsRevoked = acceptance.IsRevoked,
            RevokedAtUtc = acceptance.RevokedAtUtc
        };
    }

    public static PrivacyPolicyAcceptance ToDomain(this PrivacyPolicyAcceptanceRecord record)
    {
        var acceptance = new PrivacyPolicyAcceptance(
            record.Id,
            record.UserId,
            record.PrivacyPolicyId,
            record.AcceptedAtUtc,
            record.AcceptedVersion,
            record.IpAddress,
            record.UserAgent);

        // Atualizar IsRevoked e RevokedAtUtc usando reflection se necessário
        if (record.IsRevoked)
        {
            var isRevokedProp = typeof(PrivacyPolicyAcceptance).GetProperty("IsRevoked", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var revokedAtProp = typeof(PrivacyPolicyAcceptance).GetProperty("RevokedAtUtc", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (isRevokedProp?.SetMethod != null)
            {
                isRevokedProp.SetValue(acceptance, true);
            }
            if (revokedAtProp?.SetMethod != null && record.RevokedAtUtc.HasValue)
            {
                revokedAtProp.SetValue(acceptance, record.RevokedAtUtc.Value);
            }
        }

        return acceptance;
    }
}
