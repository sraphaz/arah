using Araponga.Api.Contracts.Governance;
using Araponga.Domain.Governance;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class CreateVotingRequestValidator : AbstractValidator<CreateVotingRequest>
{
    public CreateVotingRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Voting type is required.")
            .Must(BeValidVotingType)
            .WithMessage("Invalid voting type. Valid types: ThemePrioritization, ModerationRule, TerritoryCharacterization, FeatureFlag, CommunityPolicy.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.Options)
            .NotEmpty()
            .WithMessage("At least one option is required.")
            .Must(options => options.Count >= 2)
            .WithMessage("At least 2 options are required.")
            .Must(options => options.Count <= 10)
            .WithMessage("Maximum 10 options allowed.");

        RuleForEach(x => x.Options)
            .NotEmpty()
            .WithMessage("Option cannot be empty.")
            .MaximumLength(100)
            .WithMessage("Option must not exceed 100 characters.");

        RuleFor(x => x.Visibility)
            .NotEmpty()
            .WithMessage("Visibility is required.")
            .Must(BeValidVisibility)
            .WithMessage("Invalid visibility. Valid values: AllMembers, ResidentsOnly, CuratorsOnly.");

        RuleFor(x => x.EndsAtUtc)
            .GreaterThan(x => x.StartsAtUtc)
            .When(x => x.StartsAtUtc.HasValue && x.EndsAtUtc.HasValue)
            .WithMessage("End date must be after start date.");
    }

    private static bool BeValidVotingType(string type)
    {
        return Enum.TryParse<VotingType>(type, ignoreCase: true, out _);
    }

    private static bool BeValidVisibility(string visibility)
    {
        return Enum.TryParse<VotingVisibility>(visibility, ignoreCase: true, out _);
    }
}
