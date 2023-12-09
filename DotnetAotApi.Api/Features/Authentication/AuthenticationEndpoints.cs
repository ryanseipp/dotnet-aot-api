using System.Text.Json.Serialization;
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
            .WithName(AuthenticationRouteNames.RegisterUser);

        authV1Routes
            .MapPost("/login", LoginRequestHandler.Handle)
            .WithName(AuthenticationRouteNames.LoginUser);
    }
}

[JsonSerializable(typeof(LoginRequestModel))]
[JsonSerializable(typeof(RegisterRequestModel))]
internal partial class AuthenticationJsonSerializerContext : JsonSerializerContext { }
