using System.Text.Json.Serialization;
using DotnetAotApi.Api.Features.Todos.RequestHandlers.CreateTodo;
using DotnetAotApi.Api.Features.Todos.RequestHandlers.GetTodo;
using DotnetAotApi.Api.Features.Todos.RequestHandlers.SearchTodos;
using DotnetAotApi.Api.Features.Todos.RequestHandlers.Shared;

namespace DotnetAotApi.Api.Features.Todos;

public sealed class TodosEndpoints : IEndpoint
{
    public static void MapEndpoints(IEndpointRouteBuilder builder)
    {
        var todoV1Routes = builder.MapGroup("/v1.0/todos").WithTags("todos");

        todoV1Routes
            .MapGet(SearchTodosRequestHandler.Route, SearchTodosRequestHandler.Handle)
            .WithName(SearchTodosRequestHandler.Name);

        todoV1Routes
            .MapPost(CreateTodoRequestHandler.Route, CreateTodoRequestHandler.Handle)
            .WithName(CreateTodoRequestHandler.Name);

        todoV1Routes
            .MapGet(GetTodoRequestHandler.Route, GetTodoRequestHandler.Handle)
            .WithName(GetTodoRequestHandler.Name);
    }
}

[JsonSerializable(typeof(CreateTodoRequestModel))]
[JsonSerializable(typeof(SearchTodosResultModel))]
[JsonSerializable(typeof(TodoViewModel))]
internal partial class TodoJsonSerializerContext : JsonSerializerContext { }
