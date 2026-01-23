using Araponga.Api.Contracts.Auth;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class PasswordResetConfirmRequestValidator : AbstractValidator<PasswordResetConfirmRequest>
{
    public PasswordResetConfirmRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token é obrigatório.")
            .MaximumLength(512).WithMessage("Token deve ter no máximo 512 caracteres.");
    }
}
