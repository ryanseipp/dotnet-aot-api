namespace DotnetAotApi.Api.Features;

public interface IEndpoint
{
    static abstract void MapEndpoints(IEndpointRouteBuilder builder);
};
