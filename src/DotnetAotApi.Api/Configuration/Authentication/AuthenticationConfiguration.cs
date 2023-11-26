using Isopoh.Cryptography.Argon2;

namespace DotnetAotApi.Api.Configuration.Authentication;

public sealed class AuthenticationConfiguration
{
    // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#argon2id
    public const Argon2Type Algorithm = Argon2Type.HybridAddressing;
    public const Argon2Version Version = Argon2Version.Nineteen;
    public const uint MemoryCost = 12288;
    public const uint Iterations = 3;
    public const uint Paralellism = 1;
}
