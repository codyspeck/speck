using Microsoft.Extensions.Caching.Memory;

namespace Speck.Http.Authorization;

internal class TokenServiceCachingDecorator : ITokenService
{
    private readonly ITokenService _tokenService;
    private readonly IMemoryCache _cache;
    private readonly string _pipelineName;

    public TokenServiceCachingDecorator(ITokenService tokenService, IMemoryCache cache, string pipelineName)
    {
        _tokenService = tokenService;
        _cache = cache;
        _pipelineName = pipelineName;
    }

    public async Task<Token> GetToken()
    {
        return await _cache.GetOrCreateAsync(_pipelineName, async entry =>
        {
            var token = await _tokenService.GetToken();
            
            // remove an arbitrary 60 seconds to avoid token expiring before request is made
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.ExpiresIn - 60);

            return token;
        });
    }
}