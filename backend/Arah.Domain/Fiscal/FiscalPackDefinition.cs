namespace Arah.Domain.Fiscal;

/// <summary>
/// Definição de pacote fiscal no catálogo da instância (pluggável por país/regime).
/// </summary>
public sealed class FiscalPackDefinition
{
    public FiscalPackDefinition(
        string id,
        string countryCode,
        string displayName,
        IReadOnlyList<string> capabilities)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Pack id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(countryCode) || countryCode.Trim().Length != 2)
        {
            throw new ArgumentException("Country code must be ISO-3166 alpha-2.", nameof(countryCode));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        ArgumentNullException.ThrowIfNull(capabilities);

        Id = id.Trim().ToLowerInvariant();
        CountryCode = countryCode.Trim().ToUpperInvariant();
        DisplayName = displayName.Trim();
        Capabilities = capabilities.ToArray();
    }

    public string Id { get; }
    public string CountryCode { get; }
    public string DisplayName { get; }
    public IReadOnlyList<string> Capabilities { get; }

    public static FiscalPackDefinition BrazilV1 { get; } = new(
        "brazil.v1",
        "BR",
        "Brasil v1 — KYC comercial + meios locais",
        new[] { "kyc", "pix", "nfse" });
}
