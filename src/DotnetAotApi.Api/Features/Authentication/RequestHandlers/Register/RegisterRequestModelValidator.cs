using FluentValidation;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Register;

public sealed class RegisterRequestModelValidator : AbstractValidator<RegisterRequestModel>
{
    public RegisterRequestModelValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MaximumLength(256);

        RuleFor(p => p.Email).NotEmpty().EmailAddress().MaximumLength(512);

        RuleFor(p => p.Password).NotEmpty().MinimumLength(16).MaximumLength(64);
    }
}
