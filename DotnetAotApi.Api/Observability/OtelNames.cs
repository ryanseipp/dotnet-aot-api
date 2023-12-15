namespace DotnetAotApi.Api.Observability;

public static class OtelNames
{
    public const string TodoId = "todo.id";
    public const string TodoUserId = "todo.user.id";
    public const string TodoSearchLastSeen = "todo.search.last_seen";
    public const string TodoSearchPageSize = "todo.search.page_size";

    public const string UserId = "user.id";
    public const string Username = "user.username";

    public const string CreateTodo = "Create Todo";
    public const string SearchTodos = "Search Todos";
    public const string GetTodoById = "Get Todo By Id";
    public const string UpdateTodo = "Update Todo";

    public const string IsUniqueUser = "Is Unique User";
    public const string CreateUser = "Create User";
    public const string SearchUsers = "Search Users";
    public const string GetUserById = "Get User By Id";
    public const string GetUserByUsername = "Get User By Username";
    public const string UpdateUser = "Update User";

    public const string PwnedPasswordCheck = "Pwned Password Check";
    public const string HashingPassword = "Hashing Password";
    public const string SigningInUser = "Signing In User";
}
