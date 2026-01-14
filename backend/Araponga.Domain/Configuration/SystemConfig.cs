namespace Araponga.Domain.Configuration;

/// <summary>
/// Configuração global do sistema (calibrável) gerenciada por SystemAdmin.
/// Importante: não deve armazenar segredos (tokens, senhas). Apenas valores calibráveis e seleção de provider.
/// </summary>
public sealed class SystemConfig
{
    public const int MaxKeyLength = 200;
    public const int MaxValueLength = 10_000;
    public const int MaxDescriptionLength = 1_000;

    public SystemConfig(
        Guid id,
        string key,
        string value,
        SystemConfigCategory category,
        string? description,
        DateTime createdAtUtc,
        Guid createdByUserId,
        DateTime? updatedAtUtc,
        Guid? updatedByUserId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new ArgumentException("CreatedByUserId is required.", nameof(createdByUserId));
        }

        var normalizedKey = NormalizeKey(key);
        if (string.IsNullOrWhiteSpace(normalizedKey))
        {
            throw new ArgumentException("Key is required.", nameof(key));
        }

        if (normalizedKey.Length > MaxKeyLength)
        {
            throw new ArgumentException($"Key must not exceed {MaxKeyLength} characters.", nameof(key));
        }

        if (value is null)
        {
            throw new ArgumentException("Value is required.", nameof(value));
        }

        var normalizedValue = value.Trim();
        if (normalizedValue.Length == 0)
        {
            throw new ArgumentException("Value is required.", nameof(value));
        }

        if (normalizedValue.Length > MaxValueLength)
        {
            throw new ArgumentException($"Value must not exceed {MaxValueLength} characters.", nameof(value));
        }

        var normalizedDescription = NormalizeOptional(description);
        if (normalizedDescription is not null && normalizedDescription.Length > MaxDescriptionLength)
        {
            throw new ArgumentException($"Description must not exceed {MaxDescriptionLength} characters.", nameof(description));
        }

        Id = id;
        Key = normalizedKey;
        Value = normalizedValue;
        Category = category;
        Description = normalizedDescription;
        CreatedAtUtc = createdAtUtc;
        CreatedByUserId = createdByUserId;
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }

    public Guid Id { get; }
    public string Key { get; }
    public string Value { get; private set; }
    public SystemConfigCategory Category { get; private set; }
    public string? Description { get; private set; }

    public DateTime CreatedAtUtc { get; }
    public Guid CreatedByUserId { get; }

    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }

    public void Update(
        string value,
        SystemConfigCategory category,
        string? description,
        Guid updatedByUserId,
        DateTime updatedAtUtc)
    {
        if (updatedByUserId == Guid.Empty)
        {
            throw new ArgumentException("UpdatedByUserId is required.", nameof(updatedByUserId));
        }

        if (value is null)
        {
            throw new ArgumentException("Value is required.", nameof(value));
        }

        var normalizedValue = value.Trim();
        if (normalizedValue.Length == 0)
        {
            throw new ArgumentException("Value is required.", nameof(value));
        }

        if (normalizedValue.Length > MaxValueLength)
        {
            throw new ArgumentException($"Value must not exceed {MaxValueLength} characters.", nameof(value));
        }

        var normalizedDescription = NormalizeOptional(description);
        if (normalizedDescription is not null && normalizedDescription.Length > MaxDescriptionLength)
        {
            throw new ArgumentException($"Description must not exceed {MaxDescriptionLength} characters.", nameof(description));
        }

        Value = normalizedValue;
        Category = category;
        Description = normalizedDescription;
        UpdatedAtUtc = updatedAtUtc;
        UpdatedByUserId = updatedByUserId;
    }

    private static string NormalizeKey(string key)
        => string.IsNullOrWhiteSpace(key) ? string.Empty : key.Trim().ToLowerInvariant();

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

