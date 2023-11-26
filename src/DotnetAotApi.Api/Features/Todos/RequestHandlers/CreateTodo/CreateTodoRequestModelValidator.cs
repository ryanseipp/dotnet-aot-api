using FluentValidation;

namespace DotnetAotApi.Api.Features.Todos.RequestHandlers.CreateTodo;

public sealed class CreateTodoRequestModelValidator : AbstractValidator<CreateTodoRequestModel>
{
    public CreateTodoRequestModelValidator()
    {
        RuleFor(p => p.Content).NotEmpty();
    }
}
