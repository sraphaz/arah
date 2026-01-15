namespace Araponga.Application.Common;

/// <summary>
/// Helpers para validação comum, reduzindo duplicação de código.
/// </summary>
public static class ValidationHelpers
{
    /// <summary>
    /// Valida se uma string não está vazia e tem tamanho máximo.
    /// </summary>
    public static bool IsValidString(string? value, int maxLength, bool required = true)
    {
        if (required && string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value == null || value.Length <= maxLength;
    }

    /// <summary>
    /// Valida se uma coordenada de latitude é válida.
    /// </summary>
    public static bool IsValidLatitude(double latitude)
    {
        return latitude >= Constants.Geo.MinLatitude && latitude <= Constants.Geo.MaxLatitude;
    }

    /// <summary>
    /// Valida se uma coordenada de longitude é válida.
    /// </summary>
    public static bool IsValidLongitude(double longitude)
    {
        return longitude >= Constants.Geo.MinLongitude && longitude <= Constants.Geo.MaxLongitude;
    }

    /// <summary>
    /// Valida se um par de coordenadas (latitude, longitude) é válido.
    /// </summary>
    public static bool IsValidCoordinate(double latitude, double longitude)
    {
        return IsValidLatitude(latitude) && IsValidLongitude(longitude);
    }

    /// <summary>
    /// Valida se um número de página é válido.
    /// </summary>
    public static bool IsValidPageNumber(int pageNumber)
    {
        return pageNumber >= Constants.Pagination.DefaultPageNumber;
    }

    /// <summary>
    /// Valida e normaliza o tamanho de página.
    /// </summary>
    public static int NormalizePageSize(int pageSize)
    {
        if (pageSize < Constants.Pagination.MinPageSize)
        {
            return Constants.Pagination.DefaultPageSize;
        }

        if (pageSize > Constants.Pagination.MaxPageSize)
        {
            return Constants.Pagination.MaxPageSize;
        }

        return pageSize;
    }

    /// <summary>
    /// Valida se um email tem formato válido (básico).
    /// </summary>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Valida se uma URL é válida.
    /// </summary>
    public static bool IsValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    /// <summary>
    /// Valida se um Guid não é vazio.
    /// </summary>
    public static bool IsValidGuid(Guid guid)
    {
        return guid != Guid.Empty;
    }
}
