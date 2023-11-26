using DotnetAotApi.Api.Features.Authentication.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SignInResult = DotnetAotApi.Api.Features.Authentication.Services.SignInResult;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Login;

public static class LoginRequestHandler
{
    public static async Task<Results<EmptyHttpResult, UnauthorizedHttpResult, NotFound>> Handle(
        [FromServices] ISignInManager signInManager,
        [FromBody] LoginRequestModel request,
        CancellationToken ct
    )
    {
        return await signInManager.PasswordSignInAsync(request.Email, request.Password, ct) switch
        {
            // ISignInManager produces response on successful login
            SignInResult.Success
                => TypedResults.Empty,
            _ => TypedResults.Unauthorized()
        };
    }
}
