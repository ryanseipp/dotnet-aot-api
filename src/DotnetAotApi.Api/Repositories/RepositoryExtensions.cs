using DotnetAotApi.Api.Repositories.Implementations;
using DotnetAotApi.Api.Repositories.Interfaces;

namespace DotnetAotApi.Api.Repositories;

public static class RepositoryExtensions
{
    public static IServiceCollection WithRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }
}
