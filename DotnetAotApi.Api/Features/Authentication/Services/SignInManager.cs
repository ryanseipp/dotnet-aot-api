using System.Security.Claims;
using DotnetAotApi.Api.Domain;
using DotnetAotApi.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DotnetAotApi.Api.Features.Authentication.Services;

public sealed class SignInManager : ISignInManager
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly HttpContext _httpContext;

    public SignInManager(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IHttpContextAccessor httpContext
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;

        if (httpContext.HttpContext is null)
        {
            throw new InvalidOperationException("HttpContext must not be null");
        }
        _httpContext = httpContext.HttpContext;
    }

    public async Task<SignInResult> PasswordSignInAsync(
        string username,
        string password,
        CancellationToken ct = default
    )
    {
        var user = await _userRepository.GetUserByUsername(username, null, ct);
        return await PasswordSignInAsync(user, password, ct);
    }

    public async Task<SignInResult> PasswordSignInAsync(
        User? user,
        string password,
        CancellationToken ct = default
    )
    {
        var result = await _passwordHasher.ValidatePassword(user?.PasswordHash, password);

        if (user is null || result == HashResult.InvalidHash)
        {
            return SignInResult.Fail;
        }

        if (result == HashResult.ValidWithRehashNeeded)
        {
            var newPasswordHash = await _passwordHasher.HashPassword(password);
            user.UpdatePasswordHash(newPasswordHash);
            await _userRepository.UpdateUser(user, null, ct);
        }

        var id = new ClaimsIdentity(
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.NameIdentifier,
            ClaimTypes.Role
        );

        id.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        id.AddClaim(new Claim(ClaimTypes.Name, user.Username));
        var principal = new ClaimsPrincipal(id);

        await _httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );
        _httpContext.User = principal;

        return SignInResult.Success;
    }
}
