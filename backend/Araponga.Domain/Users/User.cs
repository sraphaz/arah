namespace Araponga.Domain.Users;

public sealed class User
{
    public User(
        Guid id,
        string displayName,
        string email,
        string provider,
        string externalId,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new ArgumentException("Provider is required.", nameof(provider));
        }

        if (string.IsNullOrWhiteSpace(externalId))
        {
            throw new ArgumentException("External ID is required.", nameof(externalId));
        }

        Id = id;
        DisplayName = displayName.Trim();
        Email = email.Trim();
        Provider = provider.Trim();
        ExternalId = externalId.Trim();
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public string DisplayName { get; }
    public string Email { get; }
    public string Provider { get; }
    public string ExternalId { get; }
    public DateTime CreatedAtUtc { get; }
}
