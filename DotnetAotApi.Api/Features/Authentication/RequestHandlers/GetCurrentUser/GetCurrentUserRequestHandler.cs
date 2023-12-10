using System.Security.Claims;
using DotnetAotApi.Api.Domain;
using DotnetAotApi.Api.Features.Authentication.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.GetCurrentUser;

public static partial class GetCurrentUserRequestHandler
{
    public static async Task<Results<Ok<GetCurrentUserResponse>, RedirectHttpResult>> Handle(
        ClaimsPrincipal claimsPrincipal,
        IUserService userService,
        CancellationToken ct
    )
    {
        var user = await userService.GetAuthenticatedUser(claimsPrincipal, ct);
        if (user is null || user.Status == UserStatus.Deleted)
        {
            return TypedResults.Redirect("/login");
        }

        var response = new GetCurrentUserResponse(
            user.Id,
            user.Username,
            user.CreatedAtTimestamp,
            user.UpdatedAtTimestamp
        );

        return TypedResults.Ok(response);
    }
}
