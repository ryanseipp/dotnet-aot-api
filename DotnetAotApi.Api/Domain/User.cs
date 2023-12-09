namespace DotnetAotApi.Api.Domain;

public sealed class User
{
    public User(string username, string passwordHash)
    {
        Id = default;
        Username = username;
        Status = UserStatus.Active;
        PasswordHash = passwordHash;
        CreatedAtTimestamp = DateTimeOffset.UtcNow;
    }

    public User(
        long id,
        string username,
        UserStatus status,
        string passwordHash,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        DateTimeOffset? deletedAt
    )
    {
        Id = id;
        Username = username;
        Status = status;
        PasswordHash = passwordHash;
        CreatedAtTimestamp = createdAt;
        UpdatedAtTimestamp = updatedAt;
        DeletedAtTimestamp = deletedAt;
    }

    public long Id { get; private set; }
    public string Username { get; private set; }
    public UserStatus Status { get; private set; }

    public string PasswordHash { get; private set; }

    public DateTimeOffset CreatedAtTimestamp { get; private set; }
    public DateTimeOffset? UpdatedAtTimestamp { get; private set; }
    public DateTimeOffset? DeletedAtTimestamp { get; private set; }

    public void UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAtTimestamp = DateTimeOffset.UtcNow;
    }
}
