using FluentValidation;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Login;

public sealed class LoginRequestModelValidator : AbstractValidator<LoginRequestModel>
{
    public LoginRequestModelValidator()
    {
        RuleFor(p => p.Email).NotEmpty().EmailAddress();

        // Do not validate password here. That's done within the request hanldler.
    }
}
