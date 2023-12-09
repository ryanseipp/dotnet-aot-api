using FluentValidation;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Login;

public sealed class LoginRequestModelValidator : AbstractValidator<LoginRequestModel>
{
    public LoginRequestModelValidator()
    {
        RuleFor(p => p.Username).NotEmpty().MaximumLength(256);
        RuleFor(p => p.Password).NotEmpty().MinimumLength(16).MinimumLength(64);
    }
}
