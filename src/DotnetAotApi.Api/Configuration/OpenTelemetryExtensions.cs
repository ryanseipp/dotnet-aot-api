using DotnetAotApi.Api.Features;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DotnetAotApi.Api.Configuration;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection WithOpenTelemetry(
        this IServiceCollection services,
        IWebHostEnvironment environment
    )
    {
        services.AddOptions<OtlpExporterOptions>();

        services
            .AddOpenTelemetry()
            .ConfigureResource(builder => builder.AddService(OtelConfig.ServiceName))
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = new Uri("http://localhost:4317");
                        otlp.Protocol = OtlpExportProtocol.Grpc;
                    });
            })
            .WithTracing(
                builder =>
                    builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddNpgsql()
                        .AddOtlpExporter(otlp =>
                        {
                            otlp.Endpoint = new Uri("http://localhost:4317");
                            otlp.Protocol = OtlpExportProtocol.Grpc;
                        })
            );

        return services;
    }
}
