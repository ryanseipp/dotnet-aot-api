using System.Security.Claims;
using DotnetAotApi.Api.Features.Authentication.Services;
using DotnetAotApi.Api.Features.Todos.RequestHandlers.Shared;
using DotnetAotApi.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAotApi.Api.Features.Todos.RequestHandlers.GetTodo;

public static class GetTodoRequestHandler
{
    public const string Route = "/{id:long}";
    public const string Name = "GetTodo";

    public static async Task<Results<Ok<TodoViewModel>, NotFound, UnauthorizedHttpResult>> Handle(
        [FromServices] IUserService userService,
        [FromServices] ClaimsPrincipal claimsPrincipal,
        [FromServices] ITodoRepository todoRepository,
        [FromRoute] long id,
        CancellationToken ct
    )
    {
        var user = await userService.GetAuthenticatedUser(claimsPrincipal, ct);
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var todo = await todoRepository.GetTodoById(user.Id, id, null, ct);
        if (todo is null)
        {
            return TypedResults.NotFound();
        }

        var result = new TodoViewModel(
            id,
            todo.Content,
            todo.Status.ToString(),
            todo.CreatedAtTimestamp,
            todo.UpdatedAtTimestamp,
            todo.FinishedAtTimestamp
        );

        return TypedResults.Ok(result);
    }
}
