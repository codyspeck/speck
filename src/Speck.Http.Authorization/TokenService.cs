using System.Net.Http.Json;
using System.Text.Json;

namespace Speck.Http.Authorization;

internal class TokenService : ITokenService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    
    private readonly HttpClient _http;
    private readonly TokenServiceOptions _options;

    public TokenService(HttpClient http, TokenServiceOptions options)
    {
        _http = http;
        _options = options;
    }

    public async Task<Token> GetToken()
    {
        using var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new("grant_type", "client_credentials"),
            new("client_id", _options.ClientId),
            new("client_secret", _options.ClientSecret),
            new("audience", _options.Audience),
            new("scope", _options.Scope)
        });

        var response = await _http.PostAsync(_options.TokenUrl, content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Token>(JsonSerializerOptions);
    }
}
