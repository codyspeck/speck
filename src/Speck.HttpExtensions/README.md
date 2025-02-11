# Speck.HttpExtensions

A collection of extension methods for [Microsoft.Extensions.Http](https://www.nuget.org/packages/microsoft.extensions.http).

---

## Authorization

Add a message handler to an **IHttpClientBuilder** responsible for managing machine-to-machine access tokens using the
OAuth 2.0 Client Credentials flow:

```c#
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpClient("named-http-client")
    .AddAuthorization(new AuthorizationConfiguration
    {
        Audience = "audience",
        ClientId = "client_id",
        ClientSecret = "client_secret",
        Scope = "scope",
        TokenUri = new Uri("https://tokenendpoint.com/oauth/token")
    });
```
