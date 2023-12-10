using Microsoft.AspNetCore.Mvc;

namespace DotnetAotApi.Api.Features.Authentication.RequestHandlers.Register;

public sealed record RegisterRequestModel([FromForm] string Username, [FromForm] string Password);
