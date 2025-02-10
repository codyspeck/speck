using System.Net.Http.Headers;

namespace Speck.HttpExtensions;

internal class AuthorizationDelegatingHandler(ITokenService tokenService) : DelegatingHandler
{
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).GetAwaiter().GetResult();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenService.GetTokenAsync();
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        
        return await base.SendAsync(request, cancellationToken);
    }
}
