using System.Data;
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
        await EnsureConnectionOpen(ct);
        return await _dbConnection.BeginTransactionAsync(ct);
    }

    public async Task<bool> IsUniqueUser(
        User user,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        await EnsureConnectionOpen(ct);
        await using var sqlCmd = new NpgsqlCommand(
            "SELECT 1 FROM dotnet_aot_api.users WHERE username = $1 LIMIT 1",
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
        await EnsureConnectionOpen(ct);
        await using var sqlCmd = new NpgsqlCommand(
            """
            INSERT INTO dotnet_aot_api.users (username, status, password_hash, created_at)
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

    public async Task<User?> GetUserById(
        long id,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        await EnsureConnectionOpen(ct);
        await using var sqlCmd = new NpgsqlCommand(
            """
            SELECT id, username, status, password_hash, created_at, updated_at, deleted_at
            FROM dotnet_aot_api.users
            WHERE id = $1 LIMIT 1
            """,
            _dbConnection,
            transaction
        )
        {
            Parameters = { new() { Value = id } }
        };

        await sqlCmd.PrepareAsync(ct);
        await using var reader = await sqlCmd.ExecuteReaderAsync(ct);

        return await GetUserFromReader(reader);
    }

    public async Task<User?> GetUserByUsername(
        string username,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    )
    {
        await EnsureConnectionOpen(ct);
        await using var sqlCmd = new NpgsqlCommand(
            """
            SELECT id, username, status, password_hash, created_at, updated_at, deleted_at
            FROM dotnet_aot_api.users
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
        await EnsureConnectionOpen(ct);
        await using var sqlCmd = new NpgsqlCommand(
            """
                UPDATE dotnet_aot_api.users
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

    private async ValueTask EnsureConnectionOpen(CancellationToken ct = default)
    {
        if (_dbConnection.State != ConnectionState.Open)
        {
            await _dbConnection.OpenAsync(ct);
        }
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
        var status = Enum.Parse<UserStatus>(reader.GetString(2));
        var passwordHash = reader.GetString(3);
        var createdAt = reader.GetFieldValue<DateTimeOffset>(4);
        var updatedAt = reader.GetFieldValue<DateTimeOffset?>(5);
        var deletedAt = reader.GetFieldValue<DateTimeOffset?>(6);

        return new User(id, username, status, passwordHash, createdAt, updatedAt, deletedAt);
    }
}
