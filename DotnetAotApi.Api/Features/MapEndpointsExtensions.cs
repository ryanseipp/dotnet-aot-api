namespace DotnetAotApi.Api.Features;

public static partial class MapEndpointsExtensions
{
    static partial void MapEndpoints(IEndpointRouteBuilder builder);

    public static void MapFeatureEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapHealthChecks("/healthz");
        MapEndpoints(builder);
    }
}
