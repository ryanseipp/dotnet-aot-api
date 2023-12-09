using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace DotnetAotApi.Api.Configuration;

public static class LoggingExtensions
{
    public static ILoggingBuilder WithLoggingConfiguration(
        this ILoggingBuilder logging,
        IWebHostEnvironment environment
    )
    {
        logging.AddOpenTelemetry(builder =>
        {
            builder.IncludeScopes = true;

            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService(environment.ApplicationName);
            builder.SetResourceBuilder(resourceBuilder);
            builder.AddOtlpExporter();
        });

        return logging;
    }
}
