using System.Security.Cryptography;
using System.Text;
using DotnetAotApi.Api.Configuration.Authentication;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;

namespace DotnetAotApi.Api.Features.Authentication.Services;

public sealed class Argon2idPasswordHasher : IPasswordHasher
{
    public async Task<string> HashPassword(string password)
    {
        return await Task.Run(() => HashPasswordInternal(password)).ConfigureAwait(false);
    }

    public async Task<HashResult> ValidatePassword(string? hash, string password)
    {
        return await Task.Run(() => ValidatePasswordInternal(hash, password)).ConfigureAwait(false);
    }

    private static string HashPasswordInternal(string password)
    {
        var salt = new byte[16];
        RandomNumberGenerator.Create().GetBytes(salt);

        var config = new Argon2Config
        {
            Password = Encoding.UTF8.GetBytes(password),
            Salt = salt,
            Type = AuthenticationConfiguration.Algorithm,
            Version = AuthenticationConfiguration.Version,
            MemoryCost = (int)AuthenticationConfiguration.MemoryCost,
            TimeCost = (int)AuthenticationConfiguration.Iterations,
            Threads = (int)AuthenticationConfiguration.Paralellism
        };

        return Argon2.Hash(config);
    }

    private static HashResult ValidatePasswordInternal(string? hash, string password)
    {
        SecureArray<byte>? decodedHash = null;
        try
        {
            var expectedHash =
                hash
                ?? "$argon2id$v=19$m=12288,t=3,p=1$RUhxczVSVE5SQV4z$T0blM/Jzk2V6LQ/TRNqfm5Mine3F6wP2564aq7Uxr+o";

            var config = new Argon2Config();
            var decoded = config.DecodeString(expectedHash, out decodedHash);
            config.Password = Encoding.UTF8.GetBytes(password);

            if (!decoded || decodedHash == null)
            {
                throw new ArgumentException($"{nameof(hash)} is not a valid Argon2 hash");
            }

            var hashConfig = new Argon2(config);

            var matches =
                config.Type == AuthenticationConfiguration.Algorithm
                && config.Version == AuthenticationConfiguration.Version
                && config.MemoryCost == AuthenticationConfiguration.MemoryCost
                && config.TimeCost == AuthenticationConfiguration.Iterations
                && config.Threads == AuthenticationConfiguration.Paralellism;

            using var hashToVerify = hashConfig.Hash();
            var result = Argon2.FixedTimeEquals(decodedHash, hashToVerify);

            return (result, matches) switch
            {
                (true, false) => HashResult.ValidHash,
                (true, true) => HashResult.ValidWithRehashNeeded,
                (_, _) => HashResult.InvalidHash,
            };
        }
        finally
        {
            decodedHash?.Dispose();
        }
    }
}
