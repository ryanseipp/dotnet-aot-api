namespace DotnetAotApi.Api.Features.Authentication.Services;

public enum HashResult
{
    ValidHash,
    InvalidHash,
    ValidWithRehashNeeded,
}
