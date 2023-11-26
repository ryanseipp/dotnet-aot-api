namespace DotnetAotApi.Api.Configuration;

public static class ApplicationExtensions
{
    public static IServiceCollection WithApplicationOptions(this IServiceCollection services)
    {
        services
            .AddOptions<ApplicationConnectionStrings>()
            .BindConfiguration(ApplicationConnectionStrings.ConnectionStrings)
            .Validate(
                config =>
                {
                    return !string.IsNullOrEmpty(config.Postgres);
                },
                "Postgres connection string cannot be empty"
            )
            .ValidateOnStart();

        return services;
    }
}

public class ApplicationConnectionStrings
{
    public const string ConnectionStrings = "ConnectionStrings";

    public string? Postgres { get; set; } = null;
}
