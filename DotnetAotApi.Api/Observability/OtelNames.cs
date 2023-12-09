namespace DotnetAotApi.Api.Observability;

public static class OtelNames
{
    public const string TodoId = "todo.id";
    public const string TodoUserId = "todo.user.id";
    public const string TodoSearchLastSeen = "todo.search.last_seen";
    public const string TodoSearchPageSize = "todo.search.page_size";

    public const string CreateTodo = "Create Todo";
    public const string SearchTodos = "Search Todos";
    public const string GetTodoById = "Get Todo By Id";
    public const string UpdateTodo = "Update Todo";
}
