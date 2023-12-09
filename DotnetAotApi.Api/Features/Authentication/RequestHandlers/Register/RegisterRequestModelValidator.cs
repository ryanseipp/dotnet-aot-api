using FluentValidation;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Register;

public sealed class RegisterRequestModelValidator : AbstractValidator<RegisterRequestModel>
{
    public RegisterRequestModelValidator()
    {
        RuleFor(p => p.Username).NotEmpty().MaximumLength(256);
        RuleFor(p => p.Password).NotEmpty().MinimumLength(16).MaximumLength(64);
    }
}
