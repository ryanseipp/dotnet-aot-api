using DotnetAotApi.Api.Domain;
using Npgsql;

namespace DotnetAotApi.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<NpgsqlTransaction> BeginTransaction(CancellationToken ct = default);

    Task<bool> IsUniqueUser(
        User user,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    );

    Task<long> CreateUser(
        User user,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    );

    Task<User?> GetUserByEmail(
        string email,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    );

    Task<bool> UpdateUser(
        User user,
        NpgsqlTransaction? transaction = null,
        CancellationToken ct = default
    );
}
