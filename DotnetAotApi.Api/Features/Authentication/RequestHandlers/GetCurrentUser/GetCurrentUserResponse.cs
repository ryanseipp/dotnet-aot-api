namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.GetCurrentUser;

public sealed record GetCurrentUserResponse(
    long Id,
    string Username,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
