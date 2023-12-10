namespace DotnetAotApi.Api.Configuration;

public static class DatabaseExtensions
{
    public static IServiceCollection WithDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        Console.WriteLine(
            $"Connection string: {configuration.GetRequiredSection("ConnectionStrings")["Postgres"]!}"
        );
        services.AddNpgsqlSlimDataSource(
            configuration.GetRequiredSection("ConnectionStrings")["Postgres"]!
        );

        return services;
    }
}
