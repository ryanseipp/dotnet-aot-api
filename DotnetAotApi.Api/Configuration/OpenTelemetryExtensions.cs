using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.ResourceDetectors.Container;
using OpenTelemetry.ResourceDetectors.ProcessRuntime;
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
            .ConfigureResource(
                builder =>
                    builder
                        .AddEnvironmentVariableDetector()
                        .AddService(OtelConfig.ServiceName)
                        .AddTelemetrySdk()
                        .AddDetector(new ContainerResourceDetector())
                        .AddDetector(new ProcessRuntimeDetector())
            )
            .WithMetrics(builder =>
            {
                builder
                    .AddMeter(OtelConfig.ServiceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(otlp =>
                    {
                        otlp.ExportProcessorType = OpenTelemetry.ExportProcessorType.Batch;
                        otlp.Endpoint = new Uri("http://localhost:4317");
                        otlp.Protocol = OtlpExportProtocol.Grpc;
                    });
            })
            .WithTracing(
                builder =>
                    builder
                        .AddSource(OtelConfig.ServiceName)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddNpgsql()
                        .AddOtlpExporter(otlp =>
                        {
                            otlp.ExportProcessorType = OpenTelemetry.ExportProcessorType.Batch;
                            otlp.Endpoint = new Uri("http://localhost:4317");
                            otlp.Protocol = OtlpExportProtocol.Grpc;
                        })
            );

        return services;
    }
}
