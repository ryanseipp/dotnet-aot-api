using System.Security.Claims;
using DotnetAotApi.Api.Features.Authentication.Services;
using DotnetAotApi.Api.Features.Todos.RequestHandlers.Shared;
using DotnetAotApi.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAotApi.Api.Features.Todos.RequestHandlers.SearchTodos;

public static class SearchTodosRequestHandler
{
    public const string Route = "/";
    public const string Name = "SearchTodos";

    public static async Task<Results<Ok<SearchTodosResultModel>, UnauthorizedHttpResult>> Handle(
        [FromServices] IUserService userService,
        [FromServices] ClaimsPrincipal claimsPrincipal,
        [FromServices] ITodoRepository todoRepository,
        CancellationToken ct,
        long lastSeen = 0,
        int pageSize = 10
    )
    {
        var user = await userService.GetAuthenticatedUser(claimsPrincipal, ct);
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var (count, todos) = await todoRepository.GetTodos(user.Id, lastSeen!, pageSize!);
        var todoModels = todos
            .Select(
                t =>
                    new TodoViewModel(
                        t.Id,
                        t.Content,
                        t.Status.ToString(),
                        t.CreatedAtTimestamp,
                        t.UpdatedAtTimestamp,
                        t.FinishedAtTimestamp
                    )
            )
            .ToArray();

        var result = new SearchTodosResultModel(
            count,
            todoModels,
            todoModels.LastOrDefault()?.Id ?? 0
        );

        return TypedResults.Ok(result);
    }
}
