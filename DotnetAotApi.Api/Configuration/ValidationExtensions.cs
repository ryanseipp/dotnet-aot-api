using DotnetAotApi.Api.Features.Authentication.RequestHandlers.Login;
using DotnetAotApi.Api.Features.Authentication.RequestHandlers.Register;
using FluentValidation;

namespace DotnetAotApi.Api.Configuration;

public static class ValidationExtensions
{
    public static IServiceCollection WithValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<RegisterRequestModel>, RegisterRequestModelValidator>();
        services.AddScoped<IValidator<LoginRequestModel>, LoginRequestModelValidator>();

        return services;
    }
}
