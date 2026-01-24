using Araponga.Api.Contracts.Users;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class UpdateBioRequestValidator : AbstractValidator<UpdateBioRequest>
{
    public UpdateBioRequestValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(500)
            .WithMessage("Bio deve ter no máximo 500 caracteres.");
    }
}
