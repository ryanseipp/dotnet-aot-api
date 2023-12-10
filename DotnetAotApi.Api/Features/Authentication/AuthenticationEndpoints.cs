using System.Text.Json.Serialization;
using DotnetAotApi.Api.Features.Authentication.RequestHandlers.GetCurrentUser;
using DotnetAotApi.Api.Features.Authentication.RequestHandlers.Login;
using DotnetAotApi.Api.Features.Authentication.RequestHandlers.Register;

namespace DotnetAotApi.Api.Features.Authentication;

public sealed class AuthenticationEndpoints : IEndpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var authV1Routes = builder.MapGroup("/v1.0/auth").WithTags("auth");

        authV1Routes
            .MapPost("/register", RegisterRequestHandler.Handle)
            .DisableAntiforgery()
            .WithName(AuthenticationRouteNames.RegisterUser);

        authV1Routes
            .MapPost("/login", LoginRequestHandler.Handle)
            .DisableAntiforgery()
            .WithName(AuthenticationRouteNames.LoginUser);

        authV1Routes
            .MapGet("/current", GetCurrentUserRequestHandler.Handle)
            .WithName(AuthenticationRouteNames.GetCurrentUser);
    }
}

[JsonSerializable(typeof(LoginRequestModel))]
[JsonSerializable(typeof(RegisterRequestModel))]
[JsonSerializable(typeof(GetCurrentUserResponse))]
internal partial class AuthenticationJsonSerializerContext : JsonSerializerContext { }
