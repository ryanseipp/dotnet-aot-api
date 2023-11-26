namespace DotnetAotApi.Api.Domain;

public sealed class Todo
{
    public Todo(long userId, string content)
    {
        UserId = userId;
        Content = content;
        Status = TodoStatus.Active;
        CreatedAtTimestamp = DateTimeOffset.UtcNow;
    }

    public Todo(
        long id,
        long userId,
        string content,
        TodoStatus status,
        DateTimeOffset createdAtTimestamp,
        DateTimeOffset? updatedAtTimestamp,
        DateTimeOffset? finishedAtTimestamp
    )
    {
        Id = id;
        UserId = userId;
        Content = content;
        Status = status;
        CreatedAtTimestamp = createdAtTimestamp;
        UpdatedAtTimestamp = updatedAtTimestamp;
        FinishedAtTimestamp = finishedAtTimestamp;
    }

    public long Id { get; private set; }
    public long UserId { get; private set; }

    public string Content { get; private set; }
    public TodoStatus Status { get; private set; }

    public DateTimeOffset CreatedAtTimestamp { get; private set; }
    public DateTimeOffset? UpdatedAtTimestamp { get; private set; }
    public DateTimeOffset? FinishedAtTimestamp { get; private set; }
}
