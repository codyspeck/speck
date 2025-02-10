using System.Net;
using System.Text.Json;

using RichardSzalay.MockHttp;

namespace Speck.HttpExtensions.UnitTests;

public static class MockHttpMessageHandlerExtensions
{
    public static MockHttpMessageHandler ConfigureOkResponse(
        this MockHttpMessageHandler mockHttpMessageHandler,
        Uri uri,
        string accessToken)
    {
        mockHttpMessageHandler
            .Expect(HttpMethod.Get, uri.ToString())
            .WithHeaders("Authorization", $"Bearer {accessToken}")
            .Respond(HttpStatusCode.OK);
        
        return mockHttpMessageHandler;
    }
    
    public static MockHttpMessageHandler ConfigureTokenResponse(
        this MockHttpMessageHandler mockHttpMessageHandler,
        AuthorizationConfiguration configuration,
        string accessToken)
    {
        return ConfigureTokenResponse(mockHttpMessageHandler, configuration, accessToken, 3600);
    }
    
    public static MockHttpMessageHandler ConfigureTokenResponse(
        this MockHttpMessageHandler mockHttpMessageHandler,
        AuthorizationConfiguration configuration,
        string accessToken,
        int expiresIn)
    {
        mockHttpMessageHandler
            .Expect(HttpMethod.Post, configuration.TokenUri!.ToString())
            .WithFormData("audience", configuration.Audience)
            .WithFormData("client_id", configuration.ClientId)
            .WithFormData("client_secret", configuration.ClientSecret)
            .WithFormData("scope", configuration.Scope)
            .Respond("application/json", JsonSerializer.Serialize(new
            {
                access_token = accessToken,
                expires_in = expiresIn
            }));

        return mockHttpMessageHandler;
    }
}
