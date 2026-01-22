using Araponga.Api.Contracts.Devices;
using FluentValidation;

namespace Araponga.Api.Validators;

public sealed class RegisterDeviceRequestValidator : AbstractValidator<RegisterDeviceRequest>
{
    public RegisterDeviceRequestValidator()
    {
        RuleFor(x => x.DeviceToken)
            .NotEmpty().WithMessage("Device token is required.")
            .MaximumLength(500).WithMessage("Device token must not exceed 500 characters.");

        RuleFor(x => x.Platform)
            .NotEmpty().WithMessage("Platform is required.")
            .Must(platform => !string.IsNullOrWhiteSpace(platform) &&
                (platform.Equals("IOS", StringComparison.OrdinalIgnoreCase) ||
                 platform.Equals("ANDROID", StringComparison.OrdinalIgnoreCase) ||
                 platform.Equals("WEB", StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Platform must be one of: IOS, ANDROID, WEB.");

        When(x => !string.IsNullOrWhiteSpace(x.DeviceName), () =>
        {
            RuleFor(x => x.DeviceName)
                .MaximumLength(100).WithMessage("Device name must not exceed 100 characters.");
        });
    }
}
