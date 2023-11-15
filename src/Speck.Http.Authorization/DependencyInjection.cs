using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Speck.Http.Authorization;

public static class DependencyInjection
{
    public static IHttpClientBuilder AddAuthorizationHandler(this IHttpClientBuilder builder, TokenServiceOptions options)
    {
        builder.Services.AddMemoryCache();
        
        return builder.AddHttpMessageHandler(provider =>
        {
            var service = new TokenService(
                provider.GetRequiredService<HttpClient>(),
                options);
            
            var decorator = new TokenServiceCachingDecorator(
                service,
                provider.GetRequiredService<IMemoryCache>(),
                builder.Name);
            
            return new AuthorizationHandler(decorator);
        });
    }
}
