using DotnetAotApi.Api.Domain;
using DotnetAotApi.Api.Repositories.Interfaces;
using Npgsql;

namespace DotnetAotApi.Api.Repositories.Implementations;

public sealed class TodoRepository : ITodoRepository
{
    private readonly NpgsqlConnection _dbConnection;

    public TodoRepository(NpgsqlConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<NpgsqlTransaction> BeginTransaction(CancellationToken ct = default)
    {
        return await _dbConnection.BeginTransactionAsync(ct);
    }

    public async Task<long> CreateTodo(
        Todo todo,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        using var activity = OtelConfig
            .Source
            .StartActivityWithTags(
                OtelNames.CreateTodo,
                new() { new(OtelNames.TodoUserId, todo.UserId) }
            );

        await using var sqlCmd = new NpgsqlCommand(
            """
            INSERT INTO todos (user_id, content, status, created_at)
            VALUES ($1, $2, $3, $4)
            RETURNING id
            """,
            _dbConnection,
            transaction
        )
        {
            Parameters =
            {
                new() { Value = todo.UserId },
                new() { Value = todo.Content },
                new() { Value = todo.Status.ToString() },
                new() { Value = todo.CreatedAtTimestamp }
            }
        };

        await sqlCmd.PrepareAsync(ct);
        await using var reader = await sqlCmd.ExecuteReaderAsync(ct);
        await reader.ReadAsync();

        var id = reader.GetFieldValue<long>(0);
        activity?.SetTag(OtelNames.TodoId, id.ToString());

        return id;
    }

    public async Task<(long, IReadOnlyCollection<Todo>)> GetTodos(
        long userId,
        long lastSeen,
        int pageSize,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        using var activity = OtelConfig
            .Source
            .StartActivityWithTags(
                OtelNames.SearchTodos,
                new()
                {
                    new(OtelNames.TodoSearchLastSeen, lastSeen),
                    new(OtelNames.TodoSearchPageSize, pageSize)
                }
            );

        var batch = new NpgsqlBatch(_dbConnection, transaction)
        {
            BatchCommands =
            {
                new(
                    """
                    SELECT id, user_id, content, status, created_at, updated_at, finished_at
                    FROM todos
                    WHERE id > $1 AND user_id = $2
                    LIMIT $3
                    """
                )
                {
                    Parameters =
                    {
                        new() { Value = lastSeen },
                        new() { Value = userId },
                        new() { Value = pageSize }
                    }
                },
                new("SELECT COUNT(*) FROM todos WHERE user_id = $1")
                {
                    Parameters = { new() { Value = userId } }
                }
            }
        };

        await batch.PrepareAsync(ct);
        await using var reader = await batch.ExecuteReaderAsync(ct);

        var todos = await GetTodosFromReader(reader, ct);
        await reader.NextResultAsync(ct);
        await reader.ReadAsync(ct);
        var count = reader.GetInt64(0);

        return (count, todos);
    }

    public async Task<Todo?> GetTodoById(
        long userId,
        long todoId,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        using var activity = OtelConfig
            .Source
            .StartActivityWithTags(
                OtelNames.GetTodoById,
                new() { new(OtelNames.TodoId, todoId), new(OtelNames.TodoUserId, userId) }
            );

        await using var sqlCmd = new NpgsqlCommand(
            """
            SELECT id, user_id, content, status, created_at, updated_at, finished_at
            FROM todos
            WHERE id = $1 AND user_id = $2
            LIMIT 1
            """,
            _dbConnection,
            transaction
        )
        {
            Parameters =
            {
                new() { Value = todoId },
                new() { Value = userId }
            }
        };

        await sqlCmd.PrepareAsync(ct);
        await using var reader = await sqlCmd.ExecuteReaderAsync(ct);
        return (await GetTodosFromReader(reader, ct)).FirstOrDefault();
    }

    private async Task<List<Todo>> GetTodosFromReader(
        NpgsqlDataReader reader,
        CancellationToken ct = default
    )
    {
        var results = new List<Todo>();

        while (await reader.ReadAsync(ct))
        {
            var id = reader.GetInt64(0);
            var userId = reader.GetInt64(1);
            var content = reader.GetString(2);
            var status = Enum.Parse<TodoStatus>(reader.GetString(3));
            var createdAt = reader.GetFieldValue<DateTimeOffset>(4);
            var updatedAt = reader.GetFieldValue<DateTimeOffset?>(5);
            var finishedAt = reader.GetFieldValue<DateTimeOffset?>(6);

            results.Add(new Todo(id, userId, content, status, createdAt, updatedAt, finishedAt));
        }

        return results;
    }
}
