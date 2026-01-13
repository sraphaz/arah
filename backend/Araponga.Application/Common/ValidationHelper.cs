namespace Araponga.Application.Common;

public static class ValidationHelper
{
    public static bool IsValidGuid(Guid id) => id != Guid.Empty;

    public static bool IsValidTerritoryId(Guid territoryId) => IsValidGuid(territoryId);

    public static bool IsValidUserId(Guid userId) => IsValidGuid(userId);

    public static string? ValidateRequired(string? value, string fieldName)
    {
        return string.IsNullOrWhiteSpace(value) ? $"{fieldName} is required." : null;
    }
}
