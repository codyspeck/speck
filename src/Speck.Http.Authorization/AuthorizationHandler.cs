using System.Net.Http.Headers;

namespace Speck.Http.Authorization;

internal class AuthorizationHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;

    public AuthorizationHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetToken();
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}
