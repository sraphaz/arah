using Araponga.Api.Contracts.Users;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class UpdateDisplayNameRequestValidator : AbstractValidator<UpdateDisplayNameRequest>
{
    public UpdateDisplayNameRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Nome de exibição é obrigatório.")
            .MinimumLength(1).WithMessage("Nome de exibição não pode ser vazio.")
            .MaximumLength(200).WithMessage("Nome de exibição deve ter no máximo 200 caracteres.");
    }
}
