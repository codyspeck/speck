using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

namespace Speck.HttpExtensions;

public class AuthorizationConfiguration
{
    /// <summary>
    /// The audience used in the access token request.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// The client id used in the access token request.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The client secret used in the access token request.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// The scope used in the access token request.
    /// </summary>
    public string Scope { get; set; } = string.Empty;

    /// <summary>
    /// The url of the token endpoint.
    /// </summary>
    public Uri? TokenUri { get; set; }
}
