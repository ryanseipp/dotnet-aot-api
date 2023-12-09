using System.Security.Claims;
using DotnetAotApi.Api.Domain;
using DotnetAotApi.Api.Features.Authentication.Services;
using DotnetAotApi.Api.Features.Todos.RequestHandlers.GetTodo;
using DotnetAotApi.Api.Features.Todos.RequestHandlers.Shared;
using DotnetAotApi.Api.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAotApi.Api.Features.Todos.RequestHandlers.CreateTodo;

public static class CreateTodoRequestHandler
{
    public const string Route = "/";
    public const string Name = "CreateTodo";

    public static async Task<
        Results<Created<TodoViewModel>, ValidationProblem, UnauthorizedHttpResult>
    > Handle(
        [FromServices] IUserService userService,
        [FromServices] ClaimsPrincipal claimsPrincipal,
        [FromServices] ITodoRepository todoRepository,
        [FromServices] IValidator<CreateTodoRequestModel> validator,
        [FromBody] CreateTodoRequestModel request,
        CancellationToken ct
    )
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await userService.GetAuthenticatedUser(claimsPrincipal, ct);
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var todo = new Todo(user.Id, request.Content);
        var id = await todoRepository.CreateTodo(todo, null, ct);

        OtelConfig.ActiveTodos.Add(1);

        var result = new TodoViewModel(
            id,
            todo.Content,
            todo.Status.ToString(),
            todo.CreatedAtTimestamp,
            todo.UpdatedAtTimestamp,
            todo.FinishedAtTimestamp
        );

        return TypedResults.Created(string.Format(GetTodoRequestHandler.Route, id), result);
    }
}
