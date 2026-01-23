using Araponga.Api.Contracts.Auth;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class PasswordResetRequestValidator : AbstractValidator<PasswordResetRequest>
{
    public PasswordResetRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.")
            .MaximumLength(255).WithMessage("Email deve ter no máximo 255 caracteres.");
    }
}
