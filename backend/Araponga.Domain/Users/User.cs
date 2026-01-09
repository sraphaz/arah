namespace Araponga.Domain.Users;

public sealed class User
{
    public User(
        Guid id,
        string displayName,
        string? email,
        string? cpf,
        string? foreignDocument,
        string? phoneNumber,
        string? address,
        string provider,
        string externalId,
        UserRole role,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentException("Provider is required.", nameof(provider));
        }

        if (string.IsNullOrWhiteSpace(externalId))
        {
            throw new ArgumentException("External ID is required.", nameof(externalId));
        }

        var normalizedCpf = NormalizeOptional(cpf);
        var normalizedForeignDocument = NormalizeOptional(foreignDocument);

        if (string.IsNullOrWhiteSpace(normalizedCpf) && string.IsNullOrWhiteSpace(normalizedForeignDocument))
        {
            throw new ArgumentException("CPF or foreign document is required.", nameof(cpf));
        }

        if (!string.IsNullOrWhiteSpace(normalizedCpf) && !string.IsNullOrWhiteSpace(normalizedForeignDocument))
        {
            throw new ArgumentException("Provide either CPF or foreign document, not both.", nameof(cpf));
        }

        Id = id;
        DisplayName = displayName.Trim();
        Email = NormalizeOptional(email);
        Cpf = normalizedCpf;
        ForeignDocument = normalizedForeignDocument;
        PhoneNumber = NormalizeOptional(phoneNumber);
        Address = NormalizeOptional(address);
        Provider = provider.Trim();
        ExternalId = externalId.Trim();
        Role = role;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public string DisplayName { get; }
    public string? Email { get; }
    public string? Cpf { get; }
    public string? ForeignDocument { get; }
    public string? PhoneNumber { get; }
    public string? Address { get; }
    public string Provider { get; }
    public string ExternalId { get; }
    public UserRole Role { get; }
    public DateTime CreatedAtUtc { get; }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
