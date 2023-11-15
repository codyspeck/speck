namespace Speck.Http.Authorization;

public class TokenServiceOptions
{
    public string Audience { get; set; }
    
    public string ClientId { get; set; }
    
    public string ClientSecret { get; set; }

    public string Scope { get; set; }

    public string TokenUrl { get; set; }
}
    