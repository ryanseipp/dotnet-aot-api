using DotnetAotApi.Api.Features.Authentication.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SignInResult = DotnetAotApi.Api.Features.Authentication.Services.SignInResult;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Login;

public static class LoginRequestHandler
{
    public static async Task<
        Results<EmptyHttpResult, UnauthorizedHttpResult, ValidationProblem>
    > Handle(
        [FromServices] ISignInManager signInManager,
        [FromServices] IValidator<LoginRequestModel> validator,
        [FromBody] LoginRequestModel request,
        CancellationToken ct
    )
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await signInManager.PasswordSignInAsync(
            request.Username,
            request.Password,
            ct
        );

        return result switch
        {
            // ISignInManager produces response on successful login
            SignInResult.Success
                => TypedResults.Empty,
            _ => TypedResults.Unauthorized()
        };
    }
}
