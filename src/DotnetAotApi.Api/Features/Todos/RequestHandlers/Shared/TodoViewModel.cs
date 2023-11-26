namespace DotnetAotApi.Api.Features.Todos.RequestHandlers.Shared;

public sealed record TodoViewModel(
    long Id,
    string Content,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? FinishedAt
);
