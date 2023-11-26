using System.Diagnostics;

namespace DotnetAotApi.Api.Observability;

public static class ActivitySourceExtensions
{
    public static Activity? StartActivityWithTags(
        this ActivitySource source,
        string name,
        List<KeyValuePair<string, object?>> tags
    )
    {
        return source.StartActivity(
            name,
            ActivityKind.Internal,
            Activity.Current?.Context ?? new ActivityContext(),
            tags
        );
    }
}
