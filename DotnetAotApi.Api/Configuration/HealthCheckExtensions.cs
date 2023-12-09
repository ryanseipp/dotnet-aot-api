namespace DotnetAotApi.Api.Configuration;

public static class HeathCheckExtensions
{
    public static IServiceCollection WithHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetRequiredSection("ConnectionStrings")["Postgres"]!);

        return services;
    }
}
