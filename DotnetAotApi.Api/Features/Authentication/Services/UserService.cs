using System.Security.Claims;
using DotnetAotApi.Api.Domain;
using DotnetAotApi.Api.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DotnetAotApi.Api.Features.Authentication.Services;

public sealed class UserService : IUserService
{
    private readonly IMemoryCache _cache;
    private readonly IUserRepository _userRepository;

    public UserService(IMemoryCache cache, IUserRepository userRepository)
    {
        _cache = cache;
        _userRepository = userRepository;
    }

    public async Task<User?> GetAuthenticatedUser(
        ClaimsPrincipal claimsPrincipal,
        CancellationToken ct = default
    )
    {
        var identity = claimsPrincipal.Identity;
        if (identity is null || identity.Name is null)
        {
            return null;
        }

        return await GetUser(identity.Name, ct);
    }

    public async Task<User?> GetUser(string username, CancellationToken ct = default)
    {
        return await _cache.GetOrCreateAsync(
            GetCacheKey(username),
            async (entry) =>
            {
                var email = (entry.Key as string)!.Split(':').Last();
                var user = await _userRepository.GetUserByUsername(username, null, ct);

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                return user;
            }
        );
    }

    private string GetCacheKey(string username) => $"DotnetAotApi.User:{username}";
}
