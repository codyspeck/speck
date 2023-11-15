using Microsoft.Extensions.DependencyInjection;

namespace Speck.Http.Authorization.Tests;

public class Tests
{
    [Fact]
    public async Task Test()
    {
        var services = new ServiceCollection();

        services
            .AddHttpClient<Service>()
            .AddAuthorizationHandler(new TokenServiceOptions
            {
                Audience = "https://test.com",
                ClientId = "hKtc3cmjmA7w2ebrlKLA1lP6B5INeBaW",
                ClientSecret = "YeSomxI7gRHLAVwtJY4UA-guSYa59Ko_BDPSdsIP3h42mvIA0VbldavmTdhuT8Ci",
                TokenUrl = "https://codyspeck.auth0.com/oauth/token"
            });

        await using var provider = services.BuildServiceProvider();

        await provider.GetRequiredService<Service>().Do();
    }

    private class Service
    {
        private readonly HttpClient _http;

        public Service(HttpClient http)
        {
            _http = http;
        }

        public async Task Do()
        {
            _ = await _http.GetAsync("https://google.com");
        }
    }
}