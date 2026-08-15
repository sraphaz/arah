using Arah.Api.Contracts.Assets;
using Arah.Modules.Assets.Domain;
using FluentValidation;

namespace Arah.Api.Validators;

public sealed class CreateNaturalAssetRequestValidator : AbstractValidator<CreateNaturalAssetRequest>
{
    public CreateNaturalAssetRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .MaximumLength(40)
            .Must(type => NaturalAssetType.TryValidatePointType(type, out _, out _))
            .WithMessage(request =>
            {
                NaturalAssetType.TryValidatePointType(request.Type, out _, out var error);
                return error ?? "Invalid type.";
            });

        RuleFor(x => x.Name)
            .NotEmptyWithMaxLength(200);

        RuleFor(x => x.Description)
            .MaxLengthWhenNotEmpty(1000);

        RuleFor(x => x.Latitude)
            .Latitude();

        RuleFor(x => x.Longitude)
            .Longitude();

        RuleFor(x => x.WaterType)
            .MaximumLength(40)
            .Must(waterType => WaterPointWaterType.TryValidate(waterType, out _, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.WaterType))
            .WithMessage(request =>
            {
                WaterPointWaterType.TryValidate(request.WaterType, out _, out var error);
                return error ?? "Invalid water_type.";
            });

        RuleFor(x => x.PotabilityNotes)
            .MaxLengthWhenNotEmpty(1000);
    }
}
