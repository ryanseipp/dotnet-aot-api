using System.Security.Cryptography;
using System.Text;

namespace DotnetAotApi.Api.Features.Authentication.Services;

public sealed class HaveIBeenPwnedClient
{
    private readonly HttpClient _httpClient;

    public HaveIBeenPwnedClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> PasswordIsPwned(string password, CancellationToken ct = default)
    {
        using var activity = OtelConfig.Source.StartActivity("PasswordIsPwned");
        var sha1Hash = ComputeHash(password);
        var hashPrefix = sha1Hash.AsSpan().Slice(0, 5).ToString();
        var hashSuffix = sha1Hash.AsSpan().Slice(5).ToString();

        // Retry is implemented via HttpClient, and this is nice-to-have.
        // Do not prevent upstream actions if the external service is down.
        var resultIfRequestFailed = false;

        try
        {
            using var response = await _httpClient.GetAsync($"range/{hashPrefix}", ct);

            if (!response.IsSuccessStatusCode)
            {
                return resultIfRequestFailed;
            }

            using var responseStream = await response.Content.ReadAsStreamAsync(ct);
            using var streamReader = new StreamReader(responseStream);

            while (!streamReader.EndOfStream)
            {
                var lineParts = streamReader.ReadLine()!.Split(':');
                if (lineParts[0].Equals(hashSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            return resultIfRequestFailed;
        }
    }

    private string ComputeHash(string password)
    {
        var utf8Password = Encoding.UTF8.GetBytes(password);
        using var sha1Hasher = SHA1.Create();
        return Convert.ToHexString(sha1Hasher.ComputeHash(utf8Password));
    }
}
