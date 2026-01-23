using FluentValidation;

namespace Araponga.Api.Validators;

public static class CommonValidators
{
    public static IRuleBuilderOptions<T, string> NotEmptyWithMaxLength<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int maxLength)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .MaximumLength(maxLength).WithMessage("{PropertyName} deve ter no máximo " + maxLength + " caracteres.");
    }

    public static IRuleBuilderOptions<T, string?> MaxLengthWhenNotEmpty<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        int maxLength)
    {
        return ruleBuilder
            .Must(value => string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
            .WithMessage("{PropertyName} deve ter no máximo " + maxLength + " caracteres.");
    }

    public static IRuleBuilderOptions<T, Guid> ValidGuid<T>(
        this IRuleBuilder<T, Guid> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("{PropertyName} não pode ser vazio.");
    }

    public static IRuleBuilderOptions<T, Guid?> ValidOptionalGuid<T>(
        this IRuleBuilder<T, Guid?> ruleBuilder)
    {
        return ruleBuilder
            .Must(value => value is null || value.Value != Guid.Empty)
            .WithMessage("{PropertyName} não pode ser vazio.");
    }
}
