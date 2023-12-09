using DotnetAotApi.Api.Domain;
using DotnetAotApi.Api.Repositories.Interfaces;
using Npgsql;

namespace DotnetAotApi.Api.Repositories.Implementations;

public sealed class UserRepository : IUserRepository
{
    private readonly NpgsqlConnection _dbConnection;

    public UserRepository(NpgsqlConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<NpgsqlTransaction> BeginTransaction(CancellationToken ct = default)
    {
        return await _dbConnection.BeginTransactionAsync(ct);
    }

    public async Task<bool> IsUniqueUser(
        User user,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        await using var sqlCmd = new NpgsqlCommand(
            "SELECT 1 FROM users WHERE username = $1 LIMIT 1",
            _dbConnection,
            transaction
        )
        {
            Parameters = { new() { Value = user.Username } }
        };

        await sqlCmd.PrepareAsync(ct);
        var exists = await sqlCmd.ExecuteScalarAsync(ct);
        return exists is null;
    }

    public async Task<long> CreateUser(
        User user,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        await using var sqlCmd = new NpgsqlCommand(
            """
            INSERT INTO users (usernamename, status, password_hash, created_at)
            VALUES ($1, $2, $3, $4)
            RETURNING id
            """,
            _dbConnection,
            transaction
        )
        {
            Parameters =
            {
                new() { Value = user.Username },
                new() { Value = user.Status.ToString() },
                new() { Value = user.PasswordHash },
                new() { Value = user.CreatedAtTimestamp },
            }
        };

        await sqlCmd.PrepareAsync(ct);
        await using var reader = await sqlCmd.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);

        return reader.GetInt64(0);
    }

    public async Task<User?> GetUserByUsername(
        string username,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        await using var sqlCmd = new NpgsqlCommand(
            """
            SELECT id, username, status, password_hash, created_at, updated_at, deleted_at
            FROM users
            WHERE username = $1 LIMIT 1
            """,
            _dbConnection,
            transaction
        )
        {
            Parameters = { new() { Value = username } }
        };

        await sqlCmd.PrepareAsync(ct);
        await using var reader = await sqlCmd.ExecuteReaderAsync(ct);

        return await GetUserFromReader(reader);
    }

    public async Task<bool> UpdateUser(
        User user,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        await using var sqlCmd = new NpgsqlCommand(
            """
                UPDATE users
                SET username = $1, status = $2, password_hash = $3, created_at = $4 updated_at = $5, deleted_at = $6
                WHERE id = $7
                """,
            _dbConnection,
            transaction
        )
        {
            Parameters =
            {
                new() { Value = user.Username },
                new() { Value = user.Status.ToString() },
                new() { Value = user.PasswordHash },
                new() { Value = user.CreatedAtTimestamp },
                new() { Value = user.UpdatedAtTimestamp },
                new() { Value = user.DeletedAtTimestamp },
                new() { Value = user.Id },
            }
        };

        await sqlCmd.PrepareAsync(ct);
        var rowsUpdated = await sqlCmd.ExecuteNonQueryAsync(ct);
        return rowsUpdated == 1;
    }

    private async Task<User?> GetUserFromReader(NpgsqlDataReader reader)
    {
        var hasResult = await reader.ReadAsync();
        if (!hasResult)
        {
            return null;
        }

        var id = reader.GetInt64(0);
        var username = reader.GetString(1);
        var status = Enum.Parse<UserStatus>(reader.GetString(3));
        var passwordHash = reader.GetString(4);
        var createdAt = reader.GetFieldValue<DateTimeOffset>(5);
        var updatedAt = reader.GetFieldValue<DateTimeOffset?>(6);
        var deletedAt = reader.GetFieldValue<DateTimeOffset?>(7);

        return new User(id, username, status, passwordHash, createdAt, updatedAt, deletedAt);
    }
}
