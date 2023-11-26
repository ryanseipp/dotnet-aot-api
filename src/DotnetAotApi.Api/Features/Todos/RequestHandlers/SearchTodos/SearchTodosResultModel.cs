using DotnetAotApi.Api.Features.Todos.RequestHandlers.Shared;

namespace DotnetAotApi.Api.Features.Todos.RequestHandlers.SearchTodos;

public sealed record SearchTodosResultModel(long Count, TodoViewModel[] Results, long LastSeen);
