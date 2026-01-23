using Araponga.Domain.Membership;
using Araponga.Domain.Users;

namespace Araponga.Domain.Policies;

/// <summary>
/// Representa uma versão da Política de Privacidade da plataforma.
/// </summary>
public sealed class PrivacyPolicy
{
    public const int MaxTitleLength = 200;
    public const int MaxContentLength = 50_000;

    public Guid Id { get; private set; }
    
    /// <summary>
    /// Versão da política (ex: "1.0", "2.0", "2.1").
    /// </summary>
    public string Version { get; private set; }
    
    /// <summary>
    /// Título da política.
    /// </summary>
    public string Title { get; private set; }
    
    /// <summary>
    /// Conteúdo da política (Markdown ou HTML).
    /// </summary>
    public string Content { get; private set; }
    
    /// <summary>
    /// Data de vigência da política.
    /// </summary>
    public DateTime EffectiveDate { get; private set; }
    
    /// <summary>
    /// Data de expiração da política (nullable).
    /// </summary>
    public DateTime? ExpirationDate { get; private set; }
    
    /// <summary>
    /// Indica se a política está ativa.
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// Papéis que devem aceitar esta política (JSON array).
    /// </summary>
    public string? RequiredRoles { get; private set; }
    
    /// <summary>
    /// Capabilities que devem aceitar esta política (JSON array).
    /// </summary>
    public string? RequiredCapabilities { get; private set; }
    
    /// <summary>
    /// System Permissions que devem aceitar esta política (JSON array).
    /// </summary>
    public string? RequiredSystemPermissions { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private PrivacyPolicy()
    {
        Version = string.Empty;
        Title = string.Empty;
        Content = string.Empty;
    }

    public PrivacyPolicy(
        Guid id,
        string version,
        string title,
        string content,
        DateTime effectiveDate,
        DateTime? expirationDate,
        bool isActive,
        string? requiredRoles,
        string? requiredCapabilities,
        string? requiredSystemPermissions,
        DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(version))
        {
            throw new ArgumentException("Version is required.", nameof(version));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (title.Length > MaxTitleLength)
        {
            throw new ArgumentException($"Title must not exceed {MaxTitleLength} characters.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content is required.", nameof(content));
        }

        if (content.Length > MaxContentLength)
        {
            throw new ArgumentException($"Content must not exceed {MaxContentLength} characters.", nameof(content));
        }

        if (expirationDate.HasValue && expirationDate.Value <= effectiveDate)
        {
            throw new ArgumentException("ExpirationDate must be after EffectiveDate.", nameof(expirationDate));
        }

        Id = id;
        Version = version.Trim();
        Title = title.Trim();
        Content = content.Trim();
        EffectiveDate = effectiveDate;
        ExpirationDate = expirationDate;
        IsActive = isActive;
        RequiredRoles = requiredRoles;
        RequiredCapabilities = requiredCapabilities;
        RequiredSystemPermissions = requiredSystemPermissions;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public void Update(
        string title,
        string content,
        DateTime effectiveDate,
        DateTime? expirationDate,
        bool isActive,
        string? requiredRoles,
        string? requiredCapabilities,
        string? requiredSystemPermissions,
        DateTime updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (title.Length > MaxTitleLength)
        {
            throw new ArgumentException($"Title must not exceed {MaxTitleLength} characters.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content is required.", nameof(content));
        }

        if (content.Length > MaxContentLength)
        {
            throw new ArgumentException($"Content must not exceed {MaxContentLength} characters.", nameof(content));
        }

        if (expirationDate.HasValue && expirationDate.Value <= effectiveDate)
        {
            throw new ArgumentException("ExpirationDate must be after EffectiveDate.", nameof(expirationDate));
        }

        Title = title.Trim();
        Content = content.Trim();
        EffectiveDate = effectiveDate;
        ExpirationDate = expirationDate;
        IsActive = isActive;
        RequiredRoles = requiredRoles;
        RequiredCapabilities = requiredCapabilities;
        RequiredSystemPermissions = requiredSystemPermissions;
        UpdatedAtUtc = updatedAtUtc;
    }

    public void Deactivate(DateTime updatedAtUtc)
    {
        IsActive = false;
        UpdatedAtUtc = updatedAtUtc;
    }
}
