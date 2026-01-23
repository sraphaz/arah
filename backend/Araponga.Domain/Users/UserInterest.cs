namespace Araponga.Domain.Users;

/// <summary>
/// Representa um interesse de um usuário. Usado para personalizar o feed e caracterizar o perfil.
/// </summary>
public sealed class UserInterest
{
    public UserInterest(
        Guid id,
        Guid userId,
        string interestTag,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(interestTag))
        {
            throw new ArgumentException("Interest tag is required.", nameof(interestTag));
        }

        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        if (normalizedTag.Length > 50)
        {
            throw new ArgumentException("Interest tag must not exceed 50 characters.", nameof(interestTag));
        }

        Id = id;
        UserId = userId;
        InterestTag = normalizedTag;
        CreatedAtUtc = createdAtUtc;
    }

    /// <summary>
    /// Identificador único do interesse.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do usuário que possui este interesse.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Tag do interesse (normalizada em lowercase, máx. 50 caracteres).
    /// Exemplos: "meio ambiente", "eventos", "marketplace", "saúde", "educação", "cultura".
    /// </summary>
    public string InterestTag { get; }

    /// <summary>
    /// Data/hora UTC de criação do interesse.
    /// </summary>
    public DateTime CreatedAtUtc { get; }
}
