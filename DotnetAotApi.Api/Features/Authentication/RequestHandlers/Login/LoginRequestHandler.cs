using DotnetAotApi.Api.Features.Authentication.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SignInResult = DotnetAotApi.Api.Features.Authentication.Services.SignInResult;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Login;

public static class LoginRequestHandler
{
    public static async Task<Results<RedirectHttpResult, UnauthorizedHttpResult, NotFound>> Handle(
        [FromServices] ISignInManager signInManager,
        [FromForm] string username,
        [FromForm] string password,
        CancellationToken ct
    )
    {
        var request = new LoginRequestModel(username, password);
        return await signInManager.PasswordSignInAsync(
            request.Username,
            request.Password,
            ct
        ) switch
        {
            // ISignInManager produces response on successful login
            SignInResult.Success
                => TypedResults.Redirect("/v1.0/auth/current"),
            _ => TypedResults.Unauthorized()
        };
    }
}
