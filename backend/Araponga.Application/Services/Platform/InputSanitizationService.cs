using System.Text.RegularExpressions;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para sanitização avançada de inputs do usuário.
/// </summary>
public sealed class InputSanitizationService
{
    // Regex para remover tags HTML
    private static readonly Regex HtmlTagRegex = new(@"<[^>]+>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    // Regex para detectar caracteres perigosos em paths
    private static readonly Regex DangerousPathChars = new(@"[<>:""|?*\x00-\x1f]", RegexOptions.Compiled);
    
    // Regex para validar URLs
    private static readonly Regex UrlRegex = new(@"^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Sanitiza HTML removendo tags e caracteres perigosos.
    /// </summary>
    public string SanitizeHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        // Remover tags HTML
        var sanitized = HtmlTagRegex.Replace(input, string.Empty);
        
        // Escapar caracteres HTML especiais
        sanitized = sanitized
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#x27;")
            .Replace("/", "&#x2F;");

        return sanitized.Trim();
    }

    /// <summary>
    /// Sanitiza paths removendo caracteres perigosos e normalizando.
    /// </summary>
    public string SanitizePath(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var sanitized = input.Trim();
        
        // Remover caracteres perigosos
        sanitized = DangerousPathChars.Replace(sanitized, string.Empty);
        
        // Normalizar separadores de path
        sanitized = sanitized.Replace('\\', '/');
        
        // Remover referências a diretórios superiores
        while (sanitized.Contains("../"))
        {
            sanitized = sanitized.Replace("../", string.Empty);
        }
        
        while (sanitized.Contains("..\\"))
        {
            sanitized = sanitized.Replace("..\\", string.Empty);
        }

        return sanitized.Trim();
    }

    /// <summary>
    /// Sanitiza e valida URLs.
    /// </summary>
    public string? SanitizeUrl(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var sanitized = input.Trim();
        
        // Validar formato de URL
        if (!UrlRegex.IsMatch(sanitized))
        {
            return null;
        }

        // Validar que é HTTP/HTTPS (não javascript:, data:, etc)
        if (!sanitized.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !sanitized.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return sanitized;
    }

    /// <summary>
    /// Sanitiza texto genérico removendo caracteres de controle e normalizando espaços.
    /// </summary>
    public string SanitizeText(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var sanitized = input.Trim();
        
        // Remover caracteres de controle (exceto \n, \r, \t)
        sanitized = Regex.Replace(sanitized, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty);
        
        // Normalizar espaços múltiplos
        sanitized = Regex.Replace(sanitized, @"\s+", " ");

        return sanitized;
    }

    /// <summary>
    /// Sanitiza SQL removendo caracteres perigosos (proteção adicional, EF Core já protege contra SQL injection).
    /// </summary>
    public string SanitizeSql(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var sanitized = input.Trim();
        
        // Remover caracteres perigosos comuns em SQL injection
        var dangerousPatterns = new[]
        {
            @"--",           // Comentários SQL
            @";",            // Separador de comandos
            @"\/\*",         // Comentários multi-linha
            @"\*\/",         // Fim de comentários
            @"xp_",          // Extended procedures
            @"sp_",          // Stored procedures
            @"exec\s+",      // EXEC
            @"execute\s+",   // EXECUTE
            @"union\s+",     // UNION
            @"select\s+",    // SELECT
            @"insert\s+",    // INSERT
            @"update\s+",   // UPDATE
            @"delete\s+",   // DELETE
            @"drop\s+",      // DROP
        };

        foreach (var pattern in dangerousPatterns)
        {
            sanitized = Regex.Replace(sanitized, pattern, string.Empty, RegexOptions.IgnoreCase);
        }

        return sanitized.Trim();
    }
}
