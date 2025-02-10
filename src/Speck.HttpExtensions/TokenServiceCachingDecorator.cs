using Microsoft.Extensions.Caching.Memory;

namespace Speck.HttpExtensions;

internal class TokenServiceCachingDecorator(IMemoryCache cache, ITokenService tokenService) : ITokenService
{
    private readonly object _key = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<Token> GetTokenAsync()
    {
        await _semaphore.WaitAsync();

        try
        {
            return await cache
                .GetOrCreateAsync<Token>(_key, async entry =>
                {
                    var token = await tokenService.GetTokenAsync();

                    // Shave an arbitrary 15 seconds off the "expires_in" value in attempt to prevent the race
                    // condition where a token can be used after it expires.
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.ExpiresIn - 15);

                    return token;
                })
                .ThrowIfNull();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
