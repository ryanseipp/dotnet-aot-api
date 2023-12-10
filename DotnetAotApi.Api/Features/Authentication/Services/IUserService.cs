using System.Security.Claims;
using DotnetAotApi.Api.Domain;

namespace DotnetAotApi.Api.Features.Authentication.Services;

public interface IUserService
{
    Task<User?> GetAuthenticatedUser(
        ClaimsPrincipal claimsPrincipal,
        CancellationToken ct = default
    );

    Task<User?> GetUser(long id, CancellationToken ct = default);
}
