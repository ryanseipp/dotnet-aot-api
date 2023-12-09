namespace DotnetAotApi.Api.Features.Authentication.Services;

public interface IPasswordHasher
{
    Task<string> HashPassword(string password);
    Task<HashResult> ValidatePassword(string? hash, string password);
}
