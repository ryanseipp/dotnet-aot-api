using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DotnetAotApi.Api.Observability;

public static class OtelConfig
{
    public const string ServiceName = "DotnetAotApi.Api";

    public static Meter Meter = new(ServiceName);
    public static Counter<long> RegisteredUsers = Meter.CreateCounter<long>("users.registered");
    public static UpDownCounter<long> ActiveTodos = Meter.CreateUpDownCounter<long>("todos.active");

    public static ActivitySource Source = new ActivitySource(ServiceName);
}
