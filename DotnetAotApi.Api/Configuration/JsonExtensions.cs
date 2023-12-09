using DotnetAotApi.Api.Features.Authentication;
using DotnetAotApi.Api.Features.Todos;

namespace DotnetAotApi.Api.Configuration;

public static class JsonExtensions
{
    public static IServiceCollection WithJsonConfiguration(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            var resolverChain = options.SerializerOptions.TypeInfoResolverChain;
            resolverChain.Insert(0, AuthenticationJsonSerializerContext.Default);
            resolverChain.Insert(1, TodoJsonSerializerContext.Default);
        });

        return services;
    }
}
