using DotnetAotApi.Api.Domain;

namespace DotnetAotApi.Api.Features.Authentication.Services;

public interface ISignInManager
{
    Task<SignInResult> PasswordSignInAsync(
        string userEmail,
        string password,
        CancellationToken ct = default
    );

    Task<SignInResult> PasswordSignInAsync(
        User? user,
        string password,
        CancellationToken ct = default
    );
}
