namespace DotnetAotApi.Api.Domain;

public sealed class User
{
    public User(string name, string email, string passwordHash)
    {
        Id = default;
        Name = name;
        Email = email;
        Status = UserStatus.Active;
        PasswordHash = passwordHash;
        CreatedAtTimestamp = DateTimeOffset.UtcNow;
    }

    public User(
        long id,
        string name,
        string email,
        UserStatus status,
        string passwordHash,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        DateTimeOffset? deletedAt
    )
    {
        Id = id;
        Name = name;
        Email = email;
        Status = status;
        PasswordHash = passwordHash;
        CreatedAtTimestamp = createdAt;
        UpdatedAtTimestamp = updatedAt;
        DeletedAtTimestamp = deletedAt;
    }

    public long Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
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
