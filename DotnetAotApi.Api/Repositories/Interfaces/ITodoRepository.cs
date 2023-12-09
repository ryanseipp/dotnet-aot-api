using DotnetAotApi.Api.Domain;
using Npgsql;

namespace DotnetAotApi.Api.Repositories.Interfaces;

public interface ITodoRepository
{
    Task<NpgsqlTransaction> BeginTransaction(CancellationToken ct = default);

    Task<long> CreateTodo(
        Todo todo,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    );

    Task<(long, IReadOnlyCollection<Todo>)> GetTodos(
        long userId,
        long lastSeen,
        int pageSize,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    );

    Task<Todo?> GetTodoById(
        long userId,
        long todoId,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    );
}
