using System.Net.Http.Json;

namespace Speck.HttpExtensions;

internal class TokenService(HttpClient http, AuthorizationConfiguration configuration) : ITokenService
{
    public async Task<Token> GetTokenAsync()
    {
        using var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("audience", configuration.Audience),
            new KeyValuePair<string, string>("client_id", configuration.ClientId),
            new KeyValuePair<string, string>("client_secret", configuration.ClientSecret),
            new KeyValuePair<string, string>("scope", configuration.Scope)
        ]);

        using var response = await http.PostAsync(configuration.TokenUri, content);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<Token>()
            .ThrowIfNull();
    }
}
