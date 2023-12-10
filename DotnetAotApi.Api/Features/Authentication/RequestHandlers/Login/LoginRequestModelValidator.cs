using FluentValidation;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Login;

public sealed class LoginRequestModelValidator : AbstractValidator<LoginRequestModel>
{
    public LoginRequestModelValidator()
    {
        RuleFor(p => p.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(256)
            .WithMessage("Username cannot be more than 256 characters.");

        RuleFor(p => p.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(16)
            .WithMessage("Password must be at least 16 characters.")
            .MaximumLength(64)
            .WithMessage("Password cannot be more than 64 characters.");
    }
}
