using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace Speck.HttpExtensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a <see cref="DelegatingHandler"/> to the <see cref="IHttpClientBuilder"/> responsible for managing the
    /// lifecycle of machine-to-machine access tokens and injecting them into the authorization header on outgoing http
    /// requests.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="configuration">The message handler's configuration.</param>
    /// <returns>This.</returns>
    /// <remarks>The default <see cref="HttpClient"/> will be used to make access token requests.</remarks>
    public static IHttpClientBuilder AddAuthorization(
        this IHttpClientBuilder builder,
        AuthorizationConfiguration configuration)
    {
        return AddAuthorization(builder, string.Empty, configuration);
    }
    
    /// <summary>
    /// Adds a <see cref="DelegatingHandler"/> to the <see cref="IHttpClientBuilder"/> responsible for managing the
    /// lifecycle of machine-to-machine access tokens and injecting them into the authorization header on outgoing http
    /// requests.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="httpClientName">The named <see cref="HttpClient"/> used to make access token requests.</param>
    /// <param name="configuration">The message handler's configuration.</param>
    /// <returns>This.</returns>
    public static IHttpClientBuilder AddAuthorization(
        this IHttpClientBuilder builder,
        string httpClientName,
        AuthorizationConfiguration configuration)
    {
        builder.Services.AddKeyedSingleton<ITokenService>(builder.Name, (services, _) =>
            new TokenServiceCachingDecorator(
                new MemoryCache(new MemoryCacheOptions { Clock = services.GetService<ISystemClock>() }),
                new TokenService(
                    services.GetRequiredService<IHttpClientFactory>()
                        .CreateClient(httpClientName),
                    configuration)));
        
        return builder.AddHttpMessageHandler(services => new AuthorizationDelegatingHandler(
            services.GetRequiredKeyedService<ITokenService>(builder.Name)));
    }
}
