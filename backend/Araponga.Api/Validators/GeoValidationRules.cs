using FluentValidation;

namespace Araponga.Api.Validators;

public static class GeoValidationRules
{
    public const double MinLatitude = -90;
    public const double MaxLatitude = 90;
    public const double MinLongitude = -180;
    public const double MaxLongitude = 180;

    public static bool IsValidLatitude(double latitude)
    {
        return latitude >= MinLatitude && latitude <= MaxLatitude;
    }

    public static bool IsValidLongitude(double longitude)
    {
        return longitude >= MinLongitude && longitude <= MaxLongitude;
    }

    public static IRuleBuilderOptions<T, double> Latitude<T>(this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder
            .Must(IsValidLatitude)
            .WithMessage($"Latitude deve estar entre {MinLatitude} e {MaxLatitude}.");
    }

    public static IRuleBuilderOptions<T, double> Longitude<T>(this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder
            .Must(IsValidLongitude)
            .WithMessage($"Longitude deve estar entre {MinLongitude} e {MaxLongitude}.");
    }

    public static IRuleBuilderOptions<T, double?> OptionalLatitude<T>(this IRuleBuilder<T, double?> ruleBuilder)
    {
        return ruleBuilder
            .Must(value => value is null || IsValidLatitude(value.Value))
            .WithMessage($"Latitude deve estar entre {MinLatitude} e {MaxLatitude}.");
    }

    public static IRuleBuilderOptions<T, double?> OptionalLongitude<T>(this IRuleBuilder<T, double?> ruleBuilder)
    {
        return ruleBuilder
            .Must(value => value is null || IsValidLongitude(value.Value))
            .WithMessage($"Longitude deve estar entre {MinLongitude} e {MaxLongitude}.");
    }
}
