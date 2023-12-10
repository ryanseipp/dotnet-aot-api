using DotnetAotApi.Api.Domain;
using DotnetAotApi.Api.Features.Authentication.Services;
using DotnetAotApi.Api.Repositories.Interfaces;
using FluentValidation;
// using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Register;

public static class RegisterRequestHandler
{
    public static async Task<Results<Created<long>, BadRequest, ValidationProblem>> Handle(
        [FromServices] IValidator<RegisterRequestModel> validator,
        [FromServices] HaveIBeenPwnedClient haveIBeenPwned,
        [FromServices] IUserRepository userRepository,
        [FromServices] IPasswordHasher passwordHasher,
        [FromForm] string username,
        [FromForm] string password,
        CancellationToken ct
    )
    {
        var request = new RegisterRequestModel(username, password);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        if (await haveIBeenPwned.PasswordIsPwned(request.Password))
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    {
                        nameof(request.Password),
                        new string[]
                        {
                            "Password has appeared in data breaches. Please use another"
                        }
                    }
                }
            );
        }

        var hashedPassword = await passwordHasher.HashPassword(request.Password);
        var user = new User(request.Username, hashedPassword);

        await using var transaction = await userRepository.BeginTransaction(ct);
        if (!await userRepository.IsUniqueUser(user, transaction))
        {
            await transaction.RollbackAsync();
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    {
                        nameof(request.Username),
                        new string[] { "Email cannot be used to register a new user" }
                    }
                }
            );
        }

        await userRepository.CreateUser(user, transaction, ct);
        await transaction.CommitAsync();
        OtelConfig.RegisteredUsers.Add(1);

        return TypedResults.Created("/user/{id}", user.Id);
    }
}
