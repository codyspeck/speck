using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using RichardSzalay.MockHttp;
using Shouldly;

namespace Speck.HttpExtensions.UnitTests;

public class AuthorizationHandlerTests
{
    class Service(HttpClient http)
    {
        public async Task GetAsync(Uri uri)
        {
            var response = await http.GetAsync(uri);

            response.EnsureSuccessStatusCode();
        }
    }

    private readonly Fixture _fixture = new();

    [Test]
    public async Task Access_token_is_fetched_and_injected_into_authorization_header()
    {
        var configuration = _fixture.Create<AuthorizationConfiguration>();
        var accessToken = _fixture.Create<string>();
        var uri = _fixture.Create<Uri>();

        var mockAccessTokenMessageHandler = new MockHttpMessageHandler()
            .ConfigureTokenResponse(configuration, accessToken);

        var mockTestServiceMessageHandler = new MockHttpMessageHandler()
            .ConfigureOkResponse(uri, accessToken);

        var services = new ServiceCollection();

        services.ConfigureHttpClientDefaults(builder => builder
            .ConfigurePrimaryHttpMessageHandler(_ => mockAccessTokenMessageHandler));

        services
            .AddHttpClient<Service>()
            .AddAuthorization(configuration)
            .ConfigurePrimaryHttpMessageHandler(_ => mockTestServiceMessageHandler);
        
        await using var provider = services.BuildServiceProvider();

        await Should.NotThrowAsync(provider
            .GetRequiredService<Service>()
            .GetAsync(uri));
        
        mockAccessTokenMessageHandler.VerifyNoOutstandingExpectation();
        mockTestServiceMessageHandler.VerifyNoOutstandingExpectation();
    }
    
    [Test]
    public async Task Access_token_is_cached_and_reused_in_authorization_header()
    {
        var configuration = _fixture.Create<AuthorizationConfiguration>();
        var accessToken = _fixture.Create<string>();
        var uri = _fixture.Create<Uri>();
        var clock = new FakeSystemClock();

        var mockAccessTokenMessageHandler = new MockHttpMessageHandler()
            .ConfigureTokenResponse(configuration, accessToken);

        var mockTestServiceMessageHandler = new MockHttpMessageHandler()
            .ConfigureOkResponse(uri, accessToken)
            .ConfigureOkResponse(uri, accessToken);

        var services = new ServiceCollection()
            .AddSingleton<ISystemClock>(clock);

        services.ConfigureHttpClientDefaults(builder => builder
            .ConfigurePrimaryHttpMessageHandler(_ => mockAccessTokenMessageHandler));

        services
            .AddHttpClient<Service>()
            .AddAuthorization(configuration)
            .ConfigurePrimaryHttpMessageHandler(_ => mockTestServiceMessageHandler);
        
        await using var provider = services.BuildServiceProvider();
        
        await Should.NotThrowAsync(provider
            .GetRequiredService<Service>()
            .GetAsync(uri));
        
        await Should.NotThrowAsync(provider
            .GetRequiredService<Service>()
            .GetAsync(uri));
        
        mockAccessTokenMessageHandler.VerifyNoOutstandingExpectation();
        mockTestServiceMessageHandler.VerifyNoOutstandingExpectation();
    }
    
    [Test]
    public async Task Access_token_expires_fifteen_seconds_early()
    {
        var configuration = _fixture.Create<AuthorizationConfiguration>();
        var firstAccessToken = _fixture.Create<string>();
        var secondAccessToken = _fixture.Create<string>();
        var uri = _fixture.Create<Uri>();
        var clock = new FakeSystemClock();
        
        const int expiresIn = 3600;

        var mockAccessTokenMessageHandler = new MockHttpMessageHandler()
            .ConfigureTokenResponse(configuration, firstAccessToken, expiresIn)
            .ConfigureTokenResponse(configuration, secondAccessToken);

        var mockTestServiceMessageHandler = new MockHttpMessageHandler()
            .ConfigureOkResponse(uri, firstAccessToken)
            .ConfigureOkResponse(uri, secondAccessToken);

        var services = new ServiceCollection()
            .AddSingleton<ISystemClock>(clock);

        services.ConfigureHttpClientDefaults(builder => builder
            .ConfigurePrimaryHttpMessageHandler(_ => mockAccessTokenMessageHandler));

        services
            .AddHttpClient<Service>()
            .AddAuthorization(configuration)
            .ConfigurePrimaryHttpMessageHandler(_ => mockTestServiceMessageHandler);
        
        await using var provider = services.BuildServiceProvider();
        
        await Should.NotThrowAsync(provider
            .GetRequiredService<Service>()
            .GetAsync(uri));
        
        clock.AddSeconds(expiresIn - 16);
        
        await Should.ThrowAsync<MockHttpMatchException>(provider
            .GetRequiredService<Service>()
            .GetAsync(uri));
        
        clock.AddSeconds(1);
        
        await Should.NotThrowAsync(provider
            .GetRequiredService<Service>()
            .GetAsync(uri));
        
        mockAccessTokenMessageHandler.VerifyNoOutstandingExpectation();
        mockTestServiceMessageHandler.VerifyNoOutstandingExpectation();
    }
}
