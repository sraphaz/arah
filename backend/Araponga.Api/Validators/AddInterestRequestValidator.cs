using Araponga.Api.Contracts.Users;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class AddInterestRequestValidator : AbstractValidator<AddInterestRequest>
{
    public AddInterestRequestValidator()
    {
        RuleFor(x => x.InterestTag)
            .NotEmpty()
            .WithMessage("Interest tag is required.")
            .MaximumLength(50)
            .WithMessage("Interest tag must not exceed 50 characters.")
            .Matches(@"^[a-z0-9\s\-]+$")
            .WithMessage("Interest tag can only contain lowercase letters, numbers, spaces, and hyphens.");
    }
}
