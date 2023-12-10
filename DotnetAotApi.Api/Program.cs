using DotnetAotApi.Api.Configuration;
using DotnetAotApi.Api.Configuration.Authentication;
using DotnetAotApi.Api.Features;
using DotnetAotApi.Api.Repositories;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Logging.WithLoggingConfiguration(builder.Environment);

builder
    .Services
    .AddResponseCompression()
    .AddAntiforgery()
    .AddProblemDetails()
    .WithApplicationOptions()
    .WithValidation()
    .WithJsonConfiguration()
    .WithOpenTelemetry(builder.Environment)
    .WithHealthChecks(builder.Configuration)
    .WithDatabase(builder.Configuration)
    .WithAuthentication()
    .WithRepositories();

var app = builder.Build();
app.UseResponseCaching()
    .UseDefaultFiles(new DefaultFilesOptions { RedirectToAppendTrailingSlash = false })
    .UseStaticFiles(new StaticFileOptions { RedirectToAppendTrailingSlash = false });

app.WithAuthentication();
app.UseAntiforgery();
app.MapFeatureEndpoints();

ProgramLogs.StartingApplication(app.Logger);

app.Run();

internal static partial class ProgramLogs
{
    [LoggerMessage(LogLevel.Information, "Starting API...")]
    public static partial void StartingApplication(ILogger logger);
}
