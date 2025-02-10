using System.Text.Json.Serialization;

namespace Speck.HttpExtensions;

internal class Token
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
}
