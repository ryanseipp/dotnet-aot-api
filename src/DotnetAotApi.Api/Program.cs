using DotnetAotApi.Api.Configuration;
using DotnetAotApi.Api.Features;
using DotnetAotApi.Api.Repositories;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Logging.WithLoggingConfiguration(builder.Environment);

builder
    .Services
    .AddProblemDetails()
    .WithApplicationOptions()
    .WithJsonConfiguration()
    .WithOpenTelemetry(builder.Environment)
    .WithHealthChecks(builder.Configuration)
    .WithDatabase(builder.Configuration)
    .WithRepositories();

var app = builder.Build();
app.MapHealthChecks("/healthz");
app.MapGet(
    "/hello",
    async () =>
    {
        await Task.Delay(250);
        return TypedResults.Ok("Hello, World!");
    }
);

app.MapFeatureEndpoints();

ProgramLogs.StartingApplication(app.Logger);

app.Run();

internal static partial class ProgramLogs
{
    [LoggerMessage(LogLevel.Information, "Starting API...")]
    public static partial void StartingApplication(ILogger logger);
}
