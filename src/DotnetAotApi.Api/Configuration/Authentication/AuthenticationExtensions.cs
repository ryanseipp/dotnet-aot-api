using DotnetAotApi.Api.Features.Authentication.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace DotnetAotApi.Api.Configuration.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection WithAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(72);
            });

        services.AddHttpContextAccessor();

        services.AddScoped<IPasswordHasher, Argon2idPasswordHasher>();
        services.AddScoped<ISignInManager, SignInManager>();
        services.AddScoped<IUserService, UserService>();

        services
            .AddHttpClient<HaveIBeenPwnedClient>(httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://api.pwnedpasswords.com/");
                httpClient.Timeout = TimeSpan.FromSeconds(1);
            })
            .AddTransientHttpErrorPolicy(
                policy =>
                    policy.WaitAndRetryAsync(
                        Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMilliseconds(500), 5)
                    )
            );

        return services;
    }

    public static WebApplication WithAuthentication(this WebApplication builder)
    {
        builder.UseCookiePolicy(
            new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                Secure = CookieSecurePolicy.SameAsRequest,
                HttpOnly = HttpOnlyPolicy.Always,
            }
        );

        builder.UseAuthentication();

        return builder;
    }
}
